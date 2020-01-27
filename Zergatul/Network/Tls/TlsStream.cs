using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Certificate;
using Zergatul.IO;
using Zergatul.Network.Tls.Extensions;
using Zergatul.Network.Tls.Messages;
using Zergatul.Security;
using Zergatul.Security.Tls;

namespace Zergatul.Network.Tls
{
    // ***********************************
    // The Transport Layer Security (TLS) Protocol Version 1.2
    // https://tools.ietf.org/html/rfc5246
    // ***********************************

    // Addition of SEED Cipher Suites to Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc4162

    // Pre-Shared Key Ciphersuites for Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc4279

    // Elliptic Curve Cryptography (ECC) Cipher Suites for Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc4492

    // AES Galois Counter Mode (GCM) Cipher Suites for TLS
    // https://tools.ietf.org/html/rfc5288

    // TLS Elliptic Curve Cipher Suites with SHA-256/384 and AES Galois Counter Mode (GCM)
    // https://tools.ietf.org/html/rfc5289

    // Pre-Shared Key Cipher Suites for TLS with SHA-256/384 and AES Galois Counter Mode
    // https://tools.ietf.org/html/rfc5487

    // Camellia Cipher Suites for TLS
    // https://tools.ietf.org/html/rfc5932

    // Addition of the ARIA Cipher Suites to Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc6209

    // Addition of the Camellia Cipher Suites to Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc6367

    // AES-CCM Cipher Suites for Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc6655

    // Transport Layer Security (TLS) Session Hash and Extended Master Secret Extension
    // https://tools.ietf.org/html/rfc7627

    // ChaCha20-Poly1305 Cipher Suites for Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc7905

    // IANA Registry TLS Parameters
    // https://www.iana.org/assignments/tls-parameters/tls-parameters.xhtml

    public partial class TlsStream : Security.TlsStream
    {
        public ConnectionInfo ConnectionInfo { get; private set; }
        public bool KeepOpen { get; set; }

        private Stream _innerStream;
        private RecordMessageStream _messageStream;
        private SecureRandom _random;

        private CipherSuiteBuilder SelectedCipher;
        private Role Role;

        private CipherSuite[] _clientCipherSuites;

        private string _serverHost;

        private SecurityParameters SecurityParameters;
        private bool _isClosed;

        private X509Certificate _clientCertificate;
        private X509Certificate _serverCertificate;
        private TlsExtension[] _clientExtensions;
        private TlsExtension[] _serverExtensions;
        private byte[] _sessionId;
        private TlsStreamSession _clientSession;
        private TlsStreamSession _serverSession;
        private bool _reuseSession;

        private Flows _flows;

        public TlsStream(Stream innerStream, ProtocolVersion version = ProtocolVersion.Tls12)
        {
            switch (version)
            {
                case ProtocolVersion.Tls12:
                case ProtocolVersion.Tls13:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version));
            }

            this._innerStream = innerStream;

            this._random =
                Parameters.SecureRandom ??
                Parameters.Provider?.GetSecureRandom() ??
                SecurityProvider.GetSecureRandomInstance();
            if (_random == null)
                throw new InvalidOperationException("Cannot instantiate SecureRandom");

            this.SecurityParameters = new SecurityParameters();
            this.SecurityParameters.Version = version;
            this.SecurityParameters.HandshakeBuffer = new List<byte>();

            this._messageStream = new RecordMessageStream(innerStream, SecurityParameters.HandshakeBuffer);
            this._messageStream.Version = version;

            this.ConnectionInfo = new ConnectionInfo();

            this._flows = new Flows(this);
        }

        public override void AuthenticateAsClient()
        {
            if (Parameters.Host == null)
                throw new ArgumentNullException("Parameters.Host");

            Role = Role.Client;
            _serverHost = Parameters.Host;

            GenerateRandom();
            _messageStream.EncodingSequenceNum = 0;
            _messageStream.DecodingSequenceNum = 0;

            MessageFlow flow = null;

            switch (this.SecurityParameters.Version)
            {
                case ProtocolVersion.Tls12:
                    flow = Tls12Flow;
                    break;
                case ProtocolVersion.Tls13:
                    throw new NotImplementedException();
                default:
                    throw new InvalidOperationException();
            }

            while (true)
            {
                var possible = flow.Flows.Where(f => f.Condition(this)).ToArray();
                if (possible.Length == 0)
                    throw new InvalidOperationException("Cannot find next state");

                if (possible.Any(f => f.Next.State == ConnectionState.ApplicationData))
                    break;

                int clientFlows = possible.Count(f => f.Next.IsClient);
                int serverFlows = possible.Count(f => f.Next.IsServer);

                if (clientFlows > 0 && serverFlows > 0)
                    throw new InvalidOperationException("Invalid flows");
                if (clientFlows > 1)
                    throw new InvalidOperationException("2 or more client flows");

                if (clientFlows > 0)
                {
                    flow = possible[0].Next;
                    flow.Write(this);
                }
                if (serverFlows > 0)
                {
                    _messageStream.ReleaseHandshakeBuffer();
                    var cm = NextMessage();
                    bool change = false;
                    foreach (var pf in possible)
                        if (pf.Next.Read(this, cm))
                        {
                            flow = pf.Next;
                            change = true;
                            break;
                        }

                    if (!change)
                    {
                        WriteAlertUnexpectedMessage();
                        throw new TlsStreamException("Unexpected message");
                    }
                }
            }

            _messageStream.ReleaseHandshakeBuffer();

            if (Parameters.ReuseSessions && !_reuseSession)
            {
                TlsStreamSessions.Instance.Add(_serverHost, new TlsStreamSession(_sessionId, SecurityParameters.MasterSecret));
            }

            OnAuthFinished();
        }

        public override Task AuthenticateAsClientAsync()
        {
            throw new NotImplementedException();
        }

        public override void AuthenticateAsServer()
        {
            if (Parameters.Certificate != null && !Parameters.Certificate.HasPrivateKey)
                throw new TlsStreamException("Certificate with private key is required for server authentification");

            Role = Role.Server;
            _serverCertificate = Parameters.Certificate;

            GenerateRandom();
            _messageStream.EncodingSequenceNum = 0;
            _messageStream.DecodingSequenceNum = 0;

            MessageFlow flow = null;

            switch (this.SecurityParameters.Version)
            {
                case ProtocolVersion.Tls12:
                    flow = Tls12Flow;
                    break;
                case ProtocolVersion.Tls13:
                    throw new NotImplementedException();
                default:
                    throw new InvalidOperationException();
            }

            while (true)
            {
                var possible = flow.Flows.Where(f => f.Condition(this)).ToArray();
                if (possible.Length == 0)
                    throw new InvalidOperationException("Cannot find next state");

                if (possible.Any(f => f.Next.State == ConnectionState.ApplicationData))
                    break;

                int clientFlows = possible.Count(f => f.Next.IsClient);
                int serverFlows = possible.Count(f => f.Next.IsServer);

                if (clientFlows > 0 && serverFlows > 0)
                    throw new InvalidOperationException("Invalid flows");
                if (serverFlows > 1)
                    throw new InvalidOperationException("2 or more server flows");

                if (serverFlows > 0)
                {
                    flow = possible[0].Next;
                    flow.Write(this);
                }
                if (clientFlows > 0)
                {
                    _messageStream.ReleaseHandshakeBuffer();
                    var cm = NextMessage();
                    bool change = false;
                    foreach (var pf in possible)
                        if (pf.Next.Read(this, cm))
                        {
                            flow = pf.Next;
                            change = true;
                            break;
                        }

                    if (!change)
                    {
                        WriteAlertUnexpectedMessage();
                        throw new TlsStreamException("Unexpected message");
                    }
                }
            }

            _messageStream.ReleaseHandshakeBuffer();

            OnAuthFinished();
        }

        public override Task AuthenticateAsServerAsync()
        {
            throw new NotImplementedException();
        }

        #region Private methods

        private ContentMessage NextMessage()
        {
            ContentMessage message;
            try
            {
                message = _messageStream.ReadMessage();
            }
            catch (RecordOverflowException)
            {
                WriteAlertRecordOverflow();
                throw;
            }
            catch (InvalidSignatureException)
            {
                WriteAlertDecryptError();
                throw;
            }
            catch (AuthenticationException)
            {
                WriteAlertBadRecordMAC();
                throw;
            }

            return message;
        }

        private void WriteHandshakeMessageBuffered(HandshakeMessage message)
        {
            _messageStream.WriteHandshake(message);
            OnContentMessage(this, new ContentMessageEventArgs(message, false, Role == Role.Server));
        }

        private void WriteChangeCipherSpec()
        {
            _messageStream.WriteChangeCipherSpec();
        }

        private void WriteAlertCloseNotify()
        {
            _messageStream.WriteAlert(AlertLevel.Warning, AlertDescription.CloseNotify);
        }

        private void WriteAlertUnexpectedMessage()
        {
            _messageStream.WriteAlert(AlertLevel.Fatal, AlertDescription.UnexpectedMessage);
        }

        private void WriteAlertBadRecordMAC()
        {
            _messageStream.WriteAlert(AlertLevel.Fatal, AlertDescription.BadRecordMAC);
        }

        private void WriteAlertRecordOverflow()
        {
            _messageStream.WriteAlert(AlertLevel.Fatal, AlertDescription.BadRecordMAC);
        }

        private void WriteAlertHandshakeFailure()
        {
            _messageStream.WriteAlert(AlertLevel.Fatal, AlertDescription.HandshakeFailure);
        }

        private void WriteAlertBadCertificate()
        {
            _messageStream.WriteAlert(AlertLevel.Fatal, AlertDescription.BadCertificate);
        }

        private void WriteAlertDecryptError()
        {
            _messageStream.WriteAlert(AlertLevel.Fatal, AlertDescription.DecryptError);
        }

        private void WriteAlertProtocolVersion()
        {
            _messageStream.WriteAlert(AlertLevel.Fatal, AlertDescription.ProtocolVersion);
        }

        private void GenerateRandom()
        {
            byte[] random = new byte[32];
            _random.GetNextBytes(random);

            if (Role == Role.Client)
                SecurityParameters.ClientRandom = random;
            if (Role == Role.Server)
                SecurityParameters.ServerRandom = random;
        }

        private void OnAuthFinished()
        {
            if (_readBuffer == null)
            {
                _readBuffer = new byte[RecordMessageStream.PlaintextLimit];
                _readBufferOffset = 0;
                _readBufferLength = 0;
            }

            if (_writeBuffer == null)
            {
                _writeBuffer = new byte[RecordMessageStream.PlaintextLimit];
                _writeBufferLength = 0;
            }
        }

        private void CopyToReadBuffer(ApplicationData message)
        {
            Buffer.BlockCopy(message.Data, 0, _readBuffer, 0, message.Data.Length);
            _readBufferOffset = 0;
            _readBufferLength = message.Data.Length;
        }

        private int ReadFromBuffer(byte[] buffer, int offset, int count)
        {
            if (_readBufferOffset >= _readBufferLength)
            {
                _readBufferOffset = 0;
                _readBufferLength = 0;
            }

            if (count > _readBufferLength - _readBufferOffset)
                count = _readBufferLength - _readBufferOffset;
            if (count == 0)
                return 0;

            Array.Copy(_readBuffer, _readBufferOffset, buffer, offset, count);
            _readBufferOffset += count;

            return count;
        }

        private void FlushWriteBufferIfFull()
        {
            if (_writeBufferLength == _writeBuffer.Length)
            {
                _messageStream.WriteApplicationData(_writeBuffer);
                _writeBufferLength = 0;
                _innerStream.Flush();
            }
        }

        private async Task FlushWriteBufferIfFullAsync(CancellationToken cancellationToken)
        {
            if (_writeBufferLength == _writeBuffer.Length)
            {
                await _messageStream.WriteApplicationDataAsync(_writeBuffer, cancellationToken);
                _writeBufferLength = 0;
            }
        }

        #region Message generation methods

        private HandshakeMessage GenerateClientHello()
        {
            var extensions = new List<TlsExtension>
            {
                new ServerNameExtension
                {
                    ServerNameList = new ServerName[]
                    {
                        new ServerName
                        {
                            NameType = NameType.HostName,
                            HostName = _serverHost
                        }
                    }
                },
                new SignatureAlgorithmsExtension
                {
                    SupportedAlgorithms = new SignatureAndHashAlgorithm[]
                    {
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA1, Signature = SignatureAlgorithm.RSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA256, Signature = SignatureAlgorithm.RSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA384, Signature = SignatureAlgorithm.RSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA512, Signature = SignatureAlgorithm.RSA },

                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA1, Signature = SignatureAlgorithm.ECDSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA256, Signature = SignatureAlgorithm.ECDSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA384, Signature = SignatureAlgorithm.ECDSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA512, Signature = SignatureAlgorithm.ECDSA },

                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA1, Signature = SignatureAlgorithm.DSA },
                    }
                },
                new TlsExtension
                {
                    Type = ExtensionType.RenegotiationInfo,
                    Data = new byte[] { 0 }
                }
            };
            if (Parameters.ExtendedMasterSecret ?? true)
                extensions.Add(new ExtendedMasterSecret());
            //if (Settings.SupportedCurves?.Length > 0)
            //    extensions.Add(new SupportedGroups(Settings.SupportedCurves));
            extensions.Add(new SupportedPointFormats(ECPointFormat.Uncompressed));
            //if (Settings.Extensions != null)
            //    extensions.AddRange(Settings.Extensions);

            if (Parameters.ReuseSessions)
                _clientSession = TlsStreamSessions.Instance.Get(_serverHost);

            return new HandshakeMessage(SelectedCipher)
            {
                Body = new ClientHello
                {
                    ClientVersion = ProtocolVersion.Tls12,
                    Random = SecurityParameters.ClientRandom,
                    SessionID = _clientSession?.ID ?? new byte[0],
                    CipherSuites = Parameters.CipherSuites ?? TlsStreamSettings.Default.CipherSuites,
                    Extensions = extensions.ToArray()
                }
            };
        }

        private HandshakeMessage GenerateServerHello()
        {
            var commonCiphers = (Parameters.CipherSuites ?? TlsStreamSettings.Default.CipherSuites)
                .Where(cs => IsCompatible(cs, _serverCertificate))
                .Where(cs => _clientCipherSuites.Contains(cs));
            if (!commonCiphers.Any())
            {
                WriteAlertHandshakeFailure();
                throw new TlsStreamException("No common algorithm");
            }

            var extensions = new List<TlsExtension>()
            {
                new TlsExtension
                {
                    Type = ExtensionType.RenegotiationInfo,
                    Data = new byte[] { 0 }
                }
            };
            if (_clientExtensions.OfType<ExtendedMasterSecret>().Count() > 0 && (Parameters.ExtendedMasterSecret ?? true))
                extensions.Add(new ExtendedMasterSecret());

            return new HandshakeMessage(SelectedCipher)
            {
                Body = new ServerHello
                {
                    ServerVersion = ProtocolVersion.Tls12,
                    Random = SecurityParameters.ServerRandom,
                    CipherSuite = commonCiphers.First(),
                    Extensions = extensions,
                    SessionID = new byte[0]
                }
            };
        }

        private HandshakeMessage GenerateCertificate(X509Certificate certificate)
        {
            return new HandshakeMessage(SelectedCipher)
            {
                Body = new Certificate
                {
                    Certificates = new List<X509Certificate>
                    {
                        certificate
                    }
                }
            };
        }

        private HandshakeMessage GenerateClientKeyExchange()
        {
            return new HandshakeMessage(SelectedCipher)
            {
                Body = SelectedCipher.KeyExchange.GenerateClientKeyExchange()
            };
        }

        private HandshakeMessage GenerateServerKeyExchange()
        {
            return new HandshakeMessage(SelectedCipher)
            {
                Body = SelectedCipher.KeyExchange.GenerateServerKeyExchange()
            };
        }

        private HandshakeMessage GenerateServerHelloDone()
        {
            return new HandshakeMessage(SelectedCipher) { Body = new ServerHelloDone() };
        }

        private HandshakeMessage GenerateFinished()
        {
            return new HandshakeMessage(SelectedCipher) { Body = SelectedCipher.GetFinished() };
        }

        #endregion

        private void AnalyzeExtensions()
        {
            bool clientExtMasterSecret = _clientExtensions.OfType<ExtendedMasterSecret>().Count() > 0;
            bool serverExtMasterSecret = _serverExtensions.OfType<ExtendedMasterSecret>().Count() > 0;
            SecurityParameters.ExtendedMasterSecret = clientExtMasterSecret && serverExtMasterSecret;

            // Application-Layer Protocol Negotiation
            var clientAlpn = _clientExtensions.OfType<ApplicationLayerProtocolNegotiationExtension>().SingleOrDefault();
            var serverAlpn = _serverExtensions.OfType<ApplicationLayerProtocolNegotiationExtension>().SingleOrDefault();
            if (clientAlpn != null ^ serverAlpn != null)
                throw new TlsStreamException("Application-Layer Protocol Negotiation extension issue");

            if (clientAlpn != null && serverAlpn != null)
            {
                if (serverAlpn.ProtocolNames.Length != 1)
                    throw new TlsStreamException("Server ALPN invalid protocols count");

                if (!clientAlpn.ProtocolNames.Contains(serverAlpn.ProtocolNames[0]))
                    throw new TlsStreamException("Server proposed ALPN not in client list");

                ConnectionInfo.ApplicationLevelProtocolName = serverAlpn.ProtocolNames[0];
            }

            /* ConnectionInfo */
            ConnectionInfo.ExtendedMasterSecret = SecurityParameters.ExtendedMasterSecret;
            /* ConnectionInfo */
        }

        private bool CanBeSkipped(MessageInfo info)
        {
            return info == MessageInfo.Forbidden || info == MessageInfo.CanBeOmitted;
        }

        private static bool IsCompatible(CipherSuite cs, X509Certificate certificate)
        {
            var type = CipherSuiteBuilder.GetCertificateType(cs);
            switch (type)
            {
                case CertificateType.None:
                    return true;
                case CertificateType.RSA:
                    return certificate?.PublicKey?.Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.RSA;
                case CertificateType.DSA:
                    return certificate?.PublicKey?.Algorithm == OID.ISO.MemberBody.US.X957.X9Algorithm.DSA;
                case CertificateType.DiffieHellman:
                    return certificate?.PublicKey?.Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS3.DHKeyAgreement;
                case CertificateType.EC:
                    return certificate?.PublicKey?.Algorithm == OID.ISO.MemberBody.US.ANSI_X962.KeyType.ECPublicKey;
                default:
                    throw new NotImplementedException();
            }
        }

        #region Message decider methods

        private bool ShouldSendServerCertificate()
        {
            if (SelectedCipher.KeyExchange.ServerCertificateMessage == MessageInfo.Forbidden)
                return false;
            if (SelectedCipher.KeyExchange.ServerCertificateMessage == MessageInfo.Required)
                return true;
            throw new InvalidOperationException();
        }

        private bool ShouldSendServerKeyExchange()
        {
            return SelectedCipher.KeyExchange.ShouldSendServerKeyExchange();
        }

        #endregion

        #endregion

        #region Message read events

        internal void OnContentMessage(object sender, ContentMessageEventArgs e)
        {
            if (e.Message is HandshakeMessage)
            {
                var message = e.Message as HandshakeMessage;
                if (message.Body.Type == HandshakeType.ClientHello)
                    OnClientHello(message.Body as ClientHello);
                if (message.Body.Type == HandshakeType.ServerHello)
                    OnServerHello(message.Body as ServerHello);
                if (message.Body.Type == HandshakeType.ServerHelloDone)
                    OnServerHelloDone(message.Body as ServerHelloDone);
                if (message.Body.Type == HandshakeType.Certificate)
                {
                    if (e.FromServer)
                        OnServerCertificate(message.Body as Certificate);
                    if (e.FromClient)
                        OnClientCertificate(message.Body as Certificate);
                }
                if (message.Body.Type == HandshakeType.ServerKeyExchange)
                    OnServerKeyExchange(message.Body as ServerKeyExchange);
                if (message.Body.Type == HandshakeType.ClientKeyExchange)
                    OnClientKeyExchange(message.Body as ClientKeyExchange);
                if (message.Body.Type == HandshakeType.Finished)
                {
                    if (e.FromClient)
                        OnClientFinished(message.Body as Finished, e.WasRead);
                    if (e.FromServer)
                        OnServerFinished(message.Body as Finished, e.WasRead);
                }
            }
            if (e.Message is Alert)
            {
                var message = e.Message as Alert;
                OnAlert(message, e.WasRead);
            }
            if (e.Message is ChangeCipherSpec)
            {
                if (e.FromClient)
                    OnClientChangeCipherSpec();
                if (e.FromServer)
                    OnServerChangeCipherSpec();
            }
            if (e.Message is ApplicationData)
                OnApplicationData(e.Message as ApplicationData);
        }

        private void OnClientHello(ClientHello message)
        {
            SecurityParameters.ClientRandom = message.Random;
            _clientCipherSuites = message.CipherSuites;

            this._clientExtensions = message.Extensions;

            /* ConnectionInfo */
            ConnectionInfo.Client.OfferedExtendedMasterSecret = message.Extensions.OfType<ExtendedMasterSecret>().Any();
            /* ConnectionInfo */
        }

        private void OnServerHello(ServerHello message)
        {
            /* ConnectionInfo */
            ConnectionInfo.CipherSuite = message.CipherSuite;
            ConnectionInfo.SessionId = message.SessionID;
            ConnectionInfo.ReuseSession = false;
            /* ConnectionInfo */

            SelectedCipher = CipherSuiteBuilder.Resolve(message.CipherSuite);
            SelectedCipher.Init(SecurityParameters, Parameters, Role, _random);
            _messageStream.SelectedCipher = SelectedCipher;
            SecurityParameters.ServerRandom = message.Random.ToArray();
            _sessionId = message.SessionID;

            if (Role == Role.Client)
            {
                if (Parameters.ReuseSessions && _clientSession != null)
                {
                    if (ByteArray.Equals(_clientSession.ID, message.SessionID))
                    {
                        _reuseSession = true;
                        SecurityParameters.MasterSecret = _clientSession.MasterSecret;
                        SelectedCipher.GenerateKeyMaterial();

                        /* ConnectionInfo */
                        ConnectionInfo.MasterSecret = (byte[])SecurityParameters.MasterSecret.Clone();
                        ConnectionInfo.ReuseSession = true;
                        /* ConnectionInfo */
                    }
                    else
                    {
                        TlsStreamSessions.Instance.Remove(_serverHost);
                    }
                }
            }

            this._serverExtensions = message.Extensions.ToArray();

            AnalyzeExtensions();

            /* ConnectionInfo */
            ConnectionInfo.Server.OfferedExtendedMasterSecret = message.Extensions.OfType<ExtendedMasterSecret>().Any();
            /* ConnectionInfo */
        }

        private void OnServerCertificate(Certificate message)
        {
            var tree = X509Tree.Build(message.Certificates, new WindowsRootCertificateStore());

            SecurityParameters.ServerCertificate = tree.Leaves.First();

            bool trusted;

            if (Role == Role.Client)
            {
                if (Parameters.ServerCertificateValidateCallback != null)
                    trusted = Parameters.ServerCertificateValidateCallback(tree.Root.Certificate);
                else
                {
                    trusted =
                        SecurityParameters.ServerCertificate.IsHostAllowed(_serverHost) &&
                        tree.Validate(SecurityParameters.ServerCertificate);
                }

                if (!trusted)
                {
                    WriteAlertBadCertificate();
                    throw new TlsStreamException("Certificate chain is not trusted");
                }
            }
        }

        private void OnServerKeyExchange(ServerKeyExchange message)
        {

        }

        private void OnServerHelloDone(ServerHelloDone message)
        {

        }

        private void OnClientCertificate(Certificate message)
        {

        }

        private void OnClientKeyExchange(ClientKeyExchange message)
        {
            SelectedCipher.CalculateMasterSecret();
            SelectedCipher.GenerateKeyMaterial();

            /* ConnectionInfo */
            ConnectionInfo.MasterSecret = (byte[])SecurityParameters.MasterSecret.Clone();
            /* ConnectionInfo */
        }

        private void OnClientChangeCipherSpec()
        {
            if (Role == Role.Client)
                _messageStream.WriteEncrypted = true;
            if (Role == Role.Server)
                _messageStream.ReadEncrypted = true;

            SecurityParameters.ClientFinishedHash = SelectedCipher.Hash(SecurityParameters.HandshakeBuffer.ToArray());

            /* ConnectionInfo */
            ConnectionInfo.Client.FinishedMessageHash = (byte[])SecurityParameters.ClientFinishedHash.Clone();
            /* ConnectionInfo */
        }

        private void OnClientFinished(Finished message, bool validate)
        {
            if (validate)
            {
                if (!SelectedCipher.VerifyFinished(message))
                {
                    WriteAlertDecryptError();
                    throw new TlsStreamException("Invalid verify_data in Finished message");
                }
            }
        }

        private void OnServerChangeCipherSpec()
        {
            if (Role == Role.Client)
                _messageStream.ReadEncrypted = true;
            if (Role == Role.Server)
                _messageStream.WriteEncrypted = true;

            SecurityParameters.ServerFinishedHash = SelectedCipher.Hash(SecurityParameters.HandshakeBuffer.ToArray());

            /* ConnectionInfo */
            ConnectionInfo.Server.FinishedMessageHash = (byte[])SecurityParameters.ServerFinishedHash.Clone();
            /* ConnectionInfo */
        }

        private void OnServerFinished(Finished message, bool validate)
        {
            if (validate)
            {
                if (!SelectedCipher.VerifyFinished(message))
                    throw new TlsStreamException("Invalid verify_data in Finished message");
            }
        }

        private void OnApplicationData(ApplicationData message)
        {

        }

        private void OnAlert(Alert message, bool read)
        {
            if (!read)
                return;

            switch (message.Description)
            {
                case AlertDescription.CloseNotify:
                    if (!_isClosed)
                        WriteAlertCloseNotify();
                    _isClosed = true;
                    return;
            }

            if (message.Level == AlertLevel.Fatal)
                throw new TlsStreamException($"TLS Fatal error: {message.Description}");
        }

        #endregion

        #region Stream overrides

        #region Not supported members

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length
        {
            get
            {
                throw new InvalidOperationException("TlsStream doesn't support Length");
            }
        }

        public override long Position
        {
            get
            {
                throw new InvalidOperationException("TlsStream doesn't support Position");
            }

            set
            {
                throw new InvalidOperationException("TlsStream doesn't support Position");
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException("TlsStream doesn't support Seek");
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("TlsStream doesn't support SetLength");
        }

        #endregion

        private byte[] _readBuffer;
        private byte[] _writeBuffer;
        private int _readBufferOffset;
        private int _readBufferLength;
        private int _writeBufferLength;

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_isClosed)
            {
                WriteAlertCloseNotify();
                //TODO!!!
                //ReadRecordMessage(ConnectionState.Closed);

                if (!KeepOpen)
                    _innerStream.Dispose();
            }
        }

        public override void Flush()
        {
            if (_writeBufferLength > 0)
            {
                byte[] data = ByteArray.SubArray(_writeBuffer, 0, _writeBufferLength);
                _writeBufferLength = 0;
                _messageStream.WriteApplicationData(data);
                _innerStream.Flush();
            }
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (_writeBufferLength > 0)
            {
                byte[] data = ByteArray.SubArray(_writeBuffer, 0, _writeBufferLength);
                _writeBufferLength = 0;
                await _messageStream.WriteApplicationDataAsync(data, cancellationToken);
                await _innerStream.FlushAsync(cancellationToken);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            if (_readBufferOffset >= _readBufferLength && !_isClosed)
            {
                var message = _messageStream.ReadMessage();
                switch (message)
                {
                    case ApplicationData appData:
                        CopyToReadBuffer(appData);
                        break;
                    case Alert alert:
                        OnAlert(message as Alert, true);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            return ReadFromBuffer(buffer, offset, count);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            if (_readBufferOffset >= _readBufferLength && !_isClosed)
            {
                var message = await _messageStream.ReadMessageAsync(cancellationToken);
                switch (message)
                {
                    case ApplicationData appData:
                        CopyToReadBuffer(appData);
                        break;
                    case Alert alert:
                        OnAlert(message as Alert, true);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            return ReadFromBuffer(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return;

            while (count > 0)
            {
                int write = System.Math.Min(_writeBuffer.Length - _writeBufferLength, count);
                Buffer.BlockCopy(buffer, offset, _writeBuffer, _writeBufferLength, write);

                _writeBufferLength += write;
                offset += write;
                count -= write;

                FlushWriteBufferIfFull();
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return;

            while (count > 0)
            {
                int write = System.Math.Min(_writeBuffer.Length - _writeBufferLength, count);
                Buffer.BlockCopy(buffer, offset, _writeBuffer, _writeBufferLength, write);

                _writeBufferLength += write;
                offset += write;
                count -= write;

                await FlushWriteBufferIfFullAsync(cancellationToken);
            }
        }

        #endregion

        #region Stream additional methods

        public int Read(byte[] buffer) => Read(buffer, 0, buffer.Length);

        public void Write(byte[] buffer) => Write(buffer, 0, buffer.Length);

        #endregion
    }
}