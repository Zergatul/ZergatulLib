﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Certificate;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Network.Tls.Extensions;
using Zergatul.Network.Tls.Messages;

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

    // https://tools.ietf.org/search/rfc4492#section-5.1
    // http://blog.fourthbit.com/2014/12/23/traffic-analysis-of-an-ssl-slash-tls-session

    // TODO
    // * work as proxy stream for another tls stream implementation, for debugging purposes
    // * refactor dhe_psk, and ecdhe_psk to use single code
    // * tls sessions?
    // * client certificates
    // * RecordMessage change to RecordMessageStream, it can read and write content messages, automatically releases write buffer and so on
    public partial class TlsStream : Stream
    {
        public TlsStreamSettings Settings { get; set; }
        public ConnectionInfo ConnectionInfo { get; private set; }

        private Stream _innerStream;
        private BinaryReader _reader;
        private TlsUtils _utils;
        private ISecureRandom _random;

        internal CipherSuiteBuilder SelectedCipher;
        internal Role Role;
        internal bool WriteEncrypted;
        internal bool ReadEncrypted;

        private CipherSuite[] _clientCipherSuites;

        private List<ContentMessage> _messageWriteBuffer = new List<ContentMessage>();
        private RecordMessage _messageReadBuffer;

        private string _serverHost;

        internal SecurityParameters SecurityParameters;
        internal ulong ClientSequenceNum;
        internal ulong ServerSequenceNum;
        private bool _isClosed;

        private X509Certificate _clientCertificate;
        private X509Certificate _serverCertificate;
        private TlsExtension[] _clientExtensions;
        private TlsExtension[] _serverExtensions;
        private TlsStreamSession _clientSession;
        private TlsStreamSession _serverSession;
        private bool _reuseSession;

        private Flows _flows;

        public TlsStream(Stream innerStream, ProtocolVersion version = ProtocolVersion.Tls12, ISecureRandom random = null)
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
            this._reader = new BinaryReader(innerStream);

            this._random = random ?? new DefaultSecureRandom();

            this._utils = new TlsUtils(this._random);

            this._messageReadBuffer = new RecordMessage(this);

            this.SecurityParameters = new SecurityParameters();
            this.SecurityParameters.Version = version;
            this.SecurityParameters.HandshakeData = new List<byte>();

            this.Settings = TlsStreamSettings.Default;
            this.ConnectionInfo = new ConnectionInfo();

            this._flows = new Flows(this);
        }

        public void AuthenticateAsClient(string host)
        {
            Role = Role.Client;
            _serverHost = host;

            GenerateRandom();
            ClientSequenceNum = 0;
            ServerSequenceNum = 0;

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

            ClearMessageBuffer();
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
                    ReleaseMessageWriteBuffer();
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

            ReleaseMessageWriteBuffer();
        }

        public void AuthenticateAsServer(string host, X509Certificate certificate = null)
        {
            if (certificate != null && !certificate.HasPrivateKey)
                throw new TlsStreamException("Certificate with private key is required for server authentification");

            Role = Role.Server;
            _serverCertificate = certificate;

            GenerateRandom();
            ClientSequenceNum = 0;
            ServerSequenceNum = 0;

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

            ClearMessageBuffer();
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
                    ReleaseMessageWriteBuffer();
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

            ReleaseMessageWriteBuffer();
        }

        #region Private methods

        private ContentMessage NextMessage()
        {
            ContentMessage message;
            try
            {
                message = _messageReadBuffer.ReadNext(_reader);
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
            if (_messageReadBuffer.Version != ProtocolVersion.Tls12)
            {
                WriteAlertProtocolVersion();
                throw new TlsStreamException("Only TLS 1.2 is supported");
            }

            return message;
        }

        private void WriteRecordMessage(RecordMessage message)
        {
            message.Write(_innerStream);
        }

        private void WriteHandshakeMessage(HandshakeMessage message)
        {
            WriteRecordMessage(new RecordMessage(this)
            {
                RecordType = ContentType.Handshake,
                Version = ProtocolVersion.Tls12,
                ContentMessages = new List<ContentMessage> { message }
            });
        }

        private void ClearMessageBuffer()
        {
            _messageWriteBuffer.Clear();
        }

        private void WriteHandshakeMessageBuffered(HandshakeMessage message)
        {
            _messageWriteBuffer.Add(message);
            OnContentMessage(this, new ContentMessageEventArgs(message, false, Role == Role.Server));
        }

        private void ReleaseMessageWriteBuffer()
        {
            if (_messageWriteBuffer.Count > 0)
            {
                var message = new RecordMessage(this)
                {
                    RecordType = ContentType.Handshake,
                    Version = ProtocolVersion.Tls12,
                    ContentMessages = _messageWriteBuffer
                };
                message.Write(_innerStream);

                ClearMessageBuffer();
            }
        }

        private void WriteChangeCipherSpec()
        {
            ReleaseMessageWriteBuffer();

            WriteRecordMessage(new RecordMessage(this)
            {
                RecordType = ContentType.ChangeCipherSpec,
                Version = ProtocolVersion.Tls12,
                ContentMessages = new List<ContentMessage>
                {
                    new ChangeCipherSpec()
                }
            });
        }

        private void WriteAlert(AlertLevel level, AlertDescription desc)
        {
            WriteRecordMessage(new RecordMessage(this)
            {
                RecordType = ContentType.Alert,
                Version = ProtocolVersion.Tls12,
                ContentMessages = new List<ContentMessage>
                    {
                        new Alert
                        {
                            Level = level,
                            Description = desc
                        }
                    }
            });
        }

        private void WriteAlertCloseNotify()
        {
            WriteAlert(AlertLevel.Warning, AlertDescription.CloseNotify);
        }

        private void WriteAlertUnexpectedMessage()
        {
            WriteAlert(AlertLevel.Fatal, AlertDescription.UnexpectedMessage);
        }

        private void WriteAlertBadRecordMAC()
        {
            WriteAlert(AlertLevel.Fatal, AlertDescription.BadRecordMAC);
        }

        private void WriteAlertRecordOverflow()
        {
            WriteAlert(AlertLevel.Fatal, AlertDescription.BadRecordMAC);
        }

        private void WriteAlertHandshakeFailure()
        {
            WriteAlert(AlertLevel.Fatal, AlertDescription.HandshakeFailure);
        }

        private void WriteAlertBadCertificate()
        {
            WriteAlert(AlertLevel.Fatal, AlertDescription.BadCertificate);
        }

        private void WriteAlertDecryptError()
        {
            WriteAlert(AlertLevel.Fatal, AlertDescription.DecryptError);
        }

        private void WriteAlertProtocolVersion()
        {
            WriteAlert(AlertLevel.Fatal, AlertDescription.ProtocolVersion);
        }

        private void GenerateRandom()
        {
            var random = new Random
            {
                GMTUnixTime = _utils.GetGMTUnixTime(),
                RandomBytes = _utils.GetRandomBytes(28)
            };
            if (Role == Role.Client)
                SecurityParameters.ClientRandom = random.ToArray();
            if (Role == Role.Server)
                SecurityParameters.ServerRandom = random.ToArray();
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
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA512, Signature = SignatureAlgorithm.RSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA512, Signature = SignatureAlgorithm.ECDSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA256, Signature = SignatureAlgorithm.RSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA384, Signature = SignatureAlgorithm.RSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA1, Signature = SignatureAlgorithm.RSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA256, Signature = SignatureAlgorithm.ECDSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA384, Signature = SignatureAlgorithm.ECDSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA1, Signature = SignatureAlgorithm.ECDSA },
                        new SignatureAndHashAlgorithm { Hash = HashAlgorithm.SHA1, Signature = SignatureAlgorithm.DSA },
                    }
                },
                new TlsExtension
                {
                    Type = ExtensionType.RenegotiationInfo,
                    Data = new byte[] { 0 }
                }
            };
            if (Settings.SupportExtendedMasterSecret)
                extensions.Add(new ExtendedMasterSecret());
            if (Settings.SupportedCurves?.Length > 0)
                extensions.Add(new SupportedGroups(Settings.SupportedCurves));
            extensions.Add(new SupportedPointFormats(ECPointFormat.Uncompressed));

            if (Settings.ReuseSessions)
                _clientSession = TlsStreamSessions.Instance[_serverHost];

            return new HandshakeMessage(this)
            {
                Body = new ClientHello
                {
                    ClientVersion = ProtocolVersion.Tls12,
                    Random = SecurityParameters.ClientRandom,
                    SessionID = _clientSession?.ID ?? new byte[0],
                    CipherSuites = Settings.CipherSuites,
                    Extensions = extensions.ToArray()
                }
            };
        }

        private HandshakeMessage GenerateServerHello()
        {
            var commonCiphers = Settings.CipherSuites.Where(cs => _clientCipherSuites.Contains(cs));
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
            if (_clientExtensions.OfType<ExtendedMasterSecret>().Count() > 0 && Settings.SupportExtendedMasterSecret)
                extensions.Add(new ExtendedMasterSecret());

            return new HandshakeMessage(this)
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
            return new HandshakeMessage(this)
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
            return new HandshakeMessage(this)
            {
                Body = SelectedCipher.KeyExchange.GenerateClientKeyExchange()
            };
        }

        private HandshakeMessage GenerateServerKeyExchange()
        {
            return new HandshakeMessage(this)
            {
                Body = SelectedCipher.KeyExchange.GenerateServerKeyExchange()
            };
        }

        private HandshakeMessage GenerateServerHelloDone()
        {
            return new HandshakeMessage(this) { Body = new ServerHelloDone() };
        }

        private HandshakeMessage GenerateFinished()
        {
            return new HandshakeMessage(this) { Body = SelectedCipher.GetFinished() };
        }

        #endregion

        private void AnalyzeExtensions()
        {
            bool clientExtMasterSecret = _clientExtensions.OfType<ExtendedMasterSecret>().Count() > 0;
            bool serverExtMasterSecret = _serverExtensions.OfType<ExtendedMasterSecret>().Count() > 0;
            SecurityParameters.ExtendedMasterSecret = clientExtMasterSecret && serverExtMasterSecret;

            /* ConnectionInfo */
            ConnectionInfo.ExtendedMasterSecret = SecurityParameters.ExtendedMasterSecret;
            /* ConnectionInfo */
        }

        private bool CanBeSkipped(MessageInfo info)
        {
            return info == MessageInfo.Forbidden || info == MessageInfo.CanBeOmitted;
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
            ConnectionInfo.CipherSuite = message.CipherSuite;

            SelectedCipher = CipherSuiteBuilder.Resolve(message.CipherSuite);
            SelectedCipher.Init(SecurityParameters, Settings, Role, _random);
            SecurityParameters.ServerRandom = message.Random.ToArray();

            if (Role == Role.Client)
            {
                if (Settings.ReuseSessions)
                {
                    _serverSession = TlsStreamSessions.Instance.AddSession(_serverHost, message.SessionID);
                    if (ByteArray.Equals(_clientSession?.ID, _serverSession?.ID))
                        _reuseSession = true;
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
                if (Settings.ServerCertificateValidationOverride != null)
                    trusted = Settings.ServerCertificateValidationOverride(tree.Root.Certificate);
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
        }

        private void OnClientChangeCipherSpec()
        {
            if (Role == Role.Client)
                WriteEncrypted = true;
            if (Role == Role.Server)
                ReadEncrypted = true;

            SecurityParameters.ClientFinishedHandshakeData = SecurityParameters.HandshakeData.ToArray();
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
                ReadEncrypted = true;
            if (Role == Role.Server)
                WriteEncrypted = true;

            SecurityParameters.ServerFinishedHandshakeData = SecurityParameters.HandshakeData.ToArray();
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

        #region Sequnce Numbers

        internal ulong DecodingSequenceNum
        {
            get
            {
                if (Role == Role.Client)
                    return ServerSequenceNum;
                if (Role == Role.Server)
                    return ClientSequenceNum;
                throw new TlsStreamException("Invalid role");
            }
        }

        internal void IncDecodingSequenceNum()
        {
            if (Role == Role.Client)
            {
                ServerSequenceNum++;
                return;
            }
            if (Role == Role.Server)
            {
                ClientSequenceNum++;
                return;
            }
            throw new TlsStreamException("Invalid role");
        }

        internal ulong EncodingSequenceNum
        {
            get
            {
                if (Role == Role.Client)
                    return ClientSequenceNum;
                if (Role == Role.Server)
                    return ServerSequenceNum;
                throw new TlsStreamException("Invalid role");
            }
        }

        internal void IncEncodingSequenceNum()
        {
            if (Role == Role.Client)
            {
                ClientSequenceNum++;
                return;
            }
            if (Role == Role.Server)
            {
                ServerSequenceNum++;
                return;
            }
            throw new TlsStreamException("Invalid role");
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

        private List<byte> _readBuffer = new List<byte>();

        public override void Flush()
        {
            
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > _readBuffer.Count && !_isClosed)
            {
                var message = _messageReadBuffer.ReadNext(_reader);
                if (message is ApplicationData)
                {
                    var appData = message as ApplicationData;
                    _readBuffer.AddRange(appData.Data);
                }
                if (message is Alert)
                    OnAlert(message as Alert, true);
            }

            if (count > _readBuffer.Count)
                count = _readBuffer.Count;
            for (int i = 0; i < count; i++)
                buffer[offset + i] = _readBuffer[i];
            _readBuffer.RemoveRange(0, count);

            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var data = new byte[count];
            Array.Copy(buffer, offset, data, 0, count);

            int limit = RecordMessage.PlaintextLimit;
            int chunks = (count + limit - 1) / limit;
            for (int c = 0; c < chunks; c++)
                WriteRecordMessage(new RecordMessage(this)
                {
                    RecordType = ContentType.ApplicationData,
                    Version = ProtocolVersion.Tls12,
                    ContentMessages = new List<ContentMessage>
                    {
                        new ApplicationData
                        {
                            Data = ByteArray.SubArray(data, c * limit, System.Math.Min(limit, count - c * limit))
                        }
                    }
                });
        }

        public override void Close()
        {
            if (!_isClosed)
            {
                WriteAlertCloseNotify();
                //TODO!!!
                //ReadRecordMessage(ConnectionState.Closed);
            }
        }

        #endregion

        #region Stream additional methods

        public int Read(byte[] buffer) => Read(buffer, 0, buffer.Length);

        public void Write(byte[] buffer) => Write(buffer, 0, buffer.Length);

        #endregion
    }
}