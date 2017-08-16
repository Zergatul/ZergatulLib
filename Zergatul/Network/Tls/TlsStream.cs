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
    // The Transport Layer Security (TLS) Protocol Version 1.2
    // https://tools.ietf.org/html/rfc5246

    // Pre-Shared Key Ciphersuites for Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc4279

    // AES Galois Counter Mode (GCM) Cipher Suites for TLS
    // https://tools.ietf.org/html/rfc5288

    // AES-CCM Cipher Suites for Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc6655

    // ChaCha20-Poly1305 Cipher Suites for Transport Layer Security (TLS)
    // https://tools.ietf.org/html/rfc7905

    // IANA Registry TLS Parameters
    // https://www.iana.org/assignments/tls-parameters/tls-parameters.xhtml

    // https://tools.ietf.org/search/rfc4492#section-5.1
    // http://blog.fourthbit.com/2014/12/23/traffic-analysis-of-an-ssl-slash-tls-session

    // TODO
    // * Check message length, allow splitting messages on record layer
    // * work as proxy stream for another tls stream implementation, for debugging purposes
    // * on finished - send tls alert!
    // * check for tls version!
    public partial class TlsStream : Stream
    {
        public TlsStreamSettings Settings { get; set; }

        private Stream _innerStream;
        private BinaryReader _reader;
        private TlsUtils _utils;
        private ISecureRandom _random;

        internal ConnectionState State;
        internal CipherSuiteBuilder SelectedCipher;
        internal Role Role;
        internal bool WriteEncrypted;
        internal bool ReadEncrypted;

        private CipherSuite[] _clientCipherSuites;

        private List<ContentMessage> _messageBuffer;

        private string _clientHost;

        internal SecurityParameters SecurityParameters;
        internal ulong ClientSequenceNum;
        internal ulong ServerSequenceNum;

        private TlsExtension[] _clientExtensions;
        private TlsExtension[] _serverExtensions;

        public TlsStream(Stream innerStream, ISecureRandom random = null)
        {
            this._innerStream = innerStream;
            this._reader = new BinaryReader(innerStream);

            this._random = random ?? new DefaultSecureRandom();

            this._utils = new TlsUtils(this._random);

            this.SecurityParameters = new SecurityParameters();
            this.SecurityParameters.HandshakeData = new List<byte>();

            this.Settings = TlsStreamSettings.Default;
        }

        public void AuthenticateAsClient(string host)
        {
            Role = Role.Client;
            State = ConnectionState.Start;
            _clientHost = host;

            GenerateRandom();

            WriteHandshakeMessage(GenerateClientHello());

            ReadRecordMessage(ConnectionState.ServerHelloDone);

            WriteHandshakeMessage(GenerateClientKeyExchange());

            WriteChangeCipherSpec();

            ClientSequenceNum = 0;
            ServerSequenceNum = 0;

            WriteHandshakeMessage(GenerateFinished());

            ReadRecordMessage(ConnectionState.ServerFinished);
        }

        public void AuthenticateAsServer(string host, X509Certificate certificate)
        {
            Role = Role.Server;
            State = ConnectionState.Start;

            if (!certificate.HasPrivateKey)
                throw new TlsStreamException("Certificate with private key is required for server authentification");

            GenerateRandom();

            ReadRecordMessage(ConnectionState.ClientHello);

            ClearMessageBuffer();
            WriteHandshakeMessageBuffered(GenerateServerHello());
            WriteHandshakeMessageBuffered(GenerateCertificate(certificate));
            WriteHandshakeMessageBuffered(GenerateServerKeyExchange(certificate));
            WriteHandshakeMessageBuffered(GenerateServerHelloDone());
            ReleaseMessageBuffer();

            ClientSequenceNum = 0;
            ServerSequenceNum = 0;

            ReadRecordMessage(ConnectionState.ClientFinished);

            WriteChangeCipherSpec();

            WriteHandshakeMessage(GenerateFinished());
        }

        #region Private methods

        private void ReadRecordMessage(ConnectionState waitFor)
        {
            while (State != waitFor)
            {
                var message = new RecordMessage(this);
                message.OnContentMessage += OnContentMessage;
                message.Read(_reader);
            }
        }

        private RecordMessage ReadSingleRecordMessage()
        {
            var message = new RecordMessage(this);
            message.OnContentMessage += OnContentMessage;
            message.Read(_reader);
            return message;
        }

        private void WriteRecordMessage(RecordMessage message)
        {
            message.OnContentMessage += OnContentMessage;
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
            if (_messageBuffer != null)
                _messageBuffer.Clear();
            else
                _messageBuffer = new List<ContentMessage>();
        }

        private void WriteHandshakeMessageBuffered(HandshakeMessage message)
        {
            _messageBuffer.Add(message);
            OnContentMessage(this, new ContentMessageEventArgs(message, false, Role == Role.Server));
        }

        private void ReleaseMessageBuffer()
        {
            var message = new RecordMessage(this)
            {
                RecordType = ContentType.Handshake,
                Version = ProtocolVersion.Tls12,
                ContentMessages = _messageBuffer
            };
            message.Write(_innerStream);
        }

        private void WriteChangeCipherSpec()
        {
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
                            HostName = _clientHost
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
                extensions.Add(new SupportedEllipticCurves(Settings.SupportedCurves));
            extensions.Add(new SupportedPointFormats(ECPointFormat.Uncompressed));

            return new HandshakeMessage(this)
            {
                Body = new ClientHello
                {
                    ClientVersion = ProtocolVersion.Tls12,
                    Random = SecurityParameters.ClientRandom,
                    CipherSuites = Settings.CipherSuites,
                    Extensions = extensions.ToArray()
                }
            };
        }

        private HandshakeMessage GenerateServerHello()
        {
            var commonCiphers = Settings.CipherSuites.Where(cs => _clientCipherSuites.Contains(cs));
            if (!commonCiphers.Any())
                throw new TlsStreamException("No common algorithm");

            var extensions = new List<TlsExtension>()
            {
                new TlsExtension
                {
                    Type = ExtensionType.RenegotiationInfo,
                    Data = new byte[] { 0 }
                }
            };
            if (_clientExtensions.OfType<ExtendedMasterSecret>().Count() > 1 && Settings.SupportExtendedMasterSecret)
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

        private HandshakeMessage GenerateServerKeyExchange(X509Certificate certificate)
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

        private void AnalyzeExtensions()
        {
            bool clientExtMasterSecret = _clientExtensions.OfType<ExtendedMasterSecret>().Count() > 0;
            bool serverExtMasterSecret = _serverExtensions.OfType<ExtendedMasterSecret>().Count() > 0;
            SecurityParameters.ExtendedMasterSecret = clientExtMasterSecret && serverExtMasterSecret;
        }

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
                if (message.Level == AlertLevel.Fatal)
                    throw new TlsStreamException($"TLS Fatal error: {message.Description}. Current state: {State}");
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
            if (State != ConnectionState.Start)
                throw new TlsStreamException("Unexpected ClientHello message");

            SecurityParameters.ClientRandom = message.Random;
            _clientCipherSuites = message.CipherSuites;

            this._clientExtensions = message.Extensions;

            State = ConnectionState.ClientHello;
        }

        private void OnServerHello(ServerHello message)
        {
            if (State != ConnectionState.ClientHello)
                throw new TlsStreamException("Unexpected ServerHello message");

            SelectedCipher = CipherSuiteBuilder.Resolve(message.CipherSuite);
            SelectedCipher.Init(SecurityParameters, Role, _random);
            SecurityParameters.ServerRandom = message.Random.ToArray();

            this._serverExtensions = message.Extensions.ToArray();

            AnalyzeExtensions();

            State = ConnectionState.ServerHello;
        }

        private void OnServerCertificate(Certificate message)
        {
            if (State != ConnectionState.ServerHello)
                throw new TlsStreamException("Unexpected Certificate message");

            var chain = X509Chain.Build(message.Certificates, new WindowsRootCertificateStore());
            if (!chain.Validate())
                throw new TlsStreamException("Certificate chain is not trusted");

            SecurityParameters.ServerCertificate = chain.List.Last();

            State = ConnectionState.ServerCertificate;
        }

        private void OnServerKeyExchange(ServerKeyExchange message)
        {
            if (State != ConnectionState.ServerCertificate)
                throw new TlsStreamException("Unexpected ServerKeyExchange message");

            State = ConnectionState.ServerKeyExchange;
        }

        private void OnServerHelloDone(ServerHelloDone message)
        {
            if (State != ConnectionState.ServerKeyExchange)
                throw new TlsStreamException("Unexpected ServerHelloDone message");

            State = ConnectionState.ServerHelloDone;
        }

        private void OnClientCertificate(Certificate message)
        {
            if (State != ConnectionState.ServerHelloDone)
                throw new TlsStreamException("Unexpected Certificate message");

            State = ConnectionState.ClientCertificate;
        }

        private void OnClientKeyExchange(ClientKeyExchange message)
        {
            if (State != ConnectionState.ServerHelloDone)
                throw new TlsStreamException("Unexpected ClientKeyExchange message");

            SelectedCipher.CalculateMasterSecret();
            SelectedCipher.GenerateKeyMaterial();

            State = ConnectionState.ClientKeyExchange;
        }

        private void OnClientChangeCipherSpec()
        {
            if (State != ConnectionState.ClientKeyExchange)
                throw new TlsStreamException("Unexpected ChangeCipherSpec message");

            if (Role == Role.Client)
                WriteEncrypted = true;
            if (Role == Role.Server)
                ReadEncrypted = true;

            SecurityParameters.ClientFinishedHandshakeData = SecurityParameters.HandshakeData.ToArray();

            State = ConnectionState.ClientChangeCipherSpec;
        }

        private void OnClientFinished(Finished message, bool validate)
        {
            if (State != ConnectionState.ClientChangeCipherSpec)
                throw new TlsStreamException("Unexpected Finished message");

            if (validate)
            {
                if (!SelectedCipher.VerifyFinished(message))
                    throw new TlsStreamException("Invalid verify_data in Finished message");
            }

            State = ConnectionState.ClientFinished;
        }

        private void OnServerChangeCipherSpec()
        {
            if (State != ConnectionState.ClientFinished)
                throw new TlsStreamException("Unexpected ChangeCipherSpec message");

            if (Role == Role.Client)
                ReadEncrypted = true;
            if (Role == Role.Server)
                WriteEncrypted = true;

            SecurityParameters.ServerFinishedHandshakeData = SecurityParameters.HandshakeData.ToArray();

            State = ConnectionState.ServerChangeCipherSpec;
        }

        private void OnServerFinished(Finished message, bool validate)
        {
            if (State != ConnectionState.ServerChangeCipherSpec)
                throw new TlsStreamException("Unexpected Finished message");

            if (validate)
            {
                if (!SelectedCipher.VerifyFinished(message))
                    throw new TlsStreamException("Invalid verify_data in Finished message");
            }

            State = ConnectionState.ServerFinished;
        }

        private void OnApplicationData(ApplicationData message)
        {
            if (State != ConnectionState.ServerFinished)
                throw new TlsStreamException("Unexpected ApplicationData message");
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
            while (_readBuffer.Count < count)
            {
                var message = ReadSingleRecordMessage();
                foreach (var cm in message.ContentMessages)
                    if (cm is ApplicationData)
                    {
                        var appData = cm as ApplicationData;
                        _readBuffer.AddRange(appData.Data);
                    }
            }

            for (int i = 0; i < count; i++)
                buffer[offset + i] = _readBuffer[i];

            _readBuffer.RemoveRange(0, count);

            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var data = new byte[count];
            Array.Copy(buffer, offset, data, 0, count);

            WriteRecordMessage(new RecordMessage(this)
            {
                RecordType = ContentType.ApplicationData,
                Version = ProtocolVersion.Tls12,
                ContentMessages = new List<ContentMessage>
                {
                    new ApplicationData { Data = data }
                }
            });
        }

        #endregion

        #region Stream additional methods

        public void Write(byte[] buffer) => Write(buffer, 0, buffer.Length);

        #endregion
    }
}