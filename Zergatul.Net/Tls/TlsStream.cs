using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Math;
using Zergatul.Net.Tls.CipherSuites;
using Zergatul.Net.Tls.Extensions;

namespace Zergatul.Net.Tls
{
    // HMAC: Keyed-Hashing for Message Authentication
    // https://www.ietf.org/rfc/rfc2104.txt

    // The Transport Layer Security (TLS) Protocol Version 1.2
    // https://www.ietf.org/rfc/rfc5246.txt

    // https://tools.ietf.org/search/rfc4492#section-5.1
    // http://blog.fourthbit.com/2014/12/23/traffic-analysis-of-an-ssl-slash-tls-session
    public class TlsStream
    {
        private Stream _innerStream;
        private BinaryReader _reader;
        private TlsUtils _utils;
        private ISecureRandom _random;

        internal CipherSuite SelectedCipher;
        private Role _role;
        private X509Certificate2 _serverCertificate;

        private ServerDHParams DHParams;

        internal List<byte> HandshakeData;

        private SecurityParameters _secParams;
        internal ulong ClientSequenceNum;
        internal ulong ServerSequenceNum;

        public TlsStream(Stream innerStream, ISecureRandom random = null)
        {
            this._innerStream = innerStream;
            this.HandshakeData = new List<byte>();
            this._reader = new BinaryReader(innerStream);

            this._random = random ?? new DefaultSecureRandom();

            this._utils = new TlsUtils(this._random);

            this._secParams = new SecurityParameters();
        }

        public void AuthenticateAsClient(string host)
        {
            _role = Role.Client;

            var random = new Random
            {
                GMTUnixTime = _utils.GetGMTUnixTime(),
                RandomBytes = _utils.GetRandomBytes(28)
            };
            _secParams.ClientRandom = new ByteArray(random.ToArray());

            var message = new RecordMessage(this)
            {
                RecordType = ContentType.Handshake,
                Version = ProtocolVersion.Tls12,
                ContentMessages = new List<ContentMessage>
                {
                    new HandshakeMessage
                    {
                        MessageType = HandshakeType.ClientHello,
                        Body = new ClientHello
                        {
                            ClientVersion = ProtocolVersion.Tls12,
                            Random = random,
                            CipherSuites = new CipherSuiteType[]
                            {
                                /*CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384,
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,*/
                                /*CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384,
                                CipherSuiteType.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,*/
                                CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_CBC_SHA,
                                CipherSuiteType.TLS_DHE_RSA_WITH_AES_128_CBC_SHA,
                                /*CipherSuiteType.TLS_RSA_WITH_AES_256_GCM_SHA384,
                                CipherSuiteType.TLS_RSA_WITH_AES_128_GCM_SHA256,
                                CipherSuiteType.TLS_RSA_WITH_AES_256_CBC_SHA256,
                                CipherSuiteType.TLS_RSA_WITH_AES_128_CBC_SHA256,
                                CipherSuiteType.TLS_RSA_WITH_AES_256_CBC_SHA,
                                CipherSuiteType.TLS_RSA_WITH_AES_128_CBC_SHA,*/
                                CipherSuiteType.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
                                CipherSuiteType.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                                CipherSuiteType.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384,
                                CipherSuiteType.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256,
                                CipherSuiteType.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA,
                                CipherSuiteType.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA,
                                CipherSuiteType.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256,
                                CipherSuiteType.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256,
                                CipherSuiteType.TLS_DHE_DSS_WITH_AES_256_CBC_SHA,
                                CipherSuiteType.TLS_DHE_DSS_WITH_AES_128_CBC_SHA,
                                CipherSuiteType.TLS_RSA_WITH_3DES_EDE_CBC_SHA,
                                CipherSuiteType.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA,
                                CipherSuiteType.TLS_RSA_WITH_RC4_128_SHA,
                                CipherSuiteType.TLS_RSA_WITH_RC4_128_MD5
                            },
                            Extensions = new TlsExtension[]
                            {
                                new TlsExtension
                                {
                                    Type = ExtensionType.ServerName,
                                    Data = _utils.HexToBytes("00 0C 00 00 09 6C 6F 63 61 6C-68 6F 73 74")
                                },
                                new TlsExtension
                                {
                                    Type = ExtensionType.SupportedGroups,
                                    Data = _utils.HexToBytes("00 04 00 17 00 18")
                                },
                                new TlsExtension
                                {
                                    Type = ExtensionType.ECPointFormats,
                                    Data = _utils.HexToBytes("01 00")
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
                                    Type = ExtensionType.ExtendedMasterSecret
                                },
                                new TlsExtension
                                {
                                    Type = ExtensionType.RenegotiationInfo,
                                    Data = new byte[] { 0 }
                                }
                            }
                        }
                    }
                }
            };
            message.Write(_innerStream);

            message = new RecordMessage(this);
            message.OnContentMessageRead += OnContentMessage;
            message.Read(_reader);

            message = new RecordMessage(this)
            {
                RecordType = ContentType.Handshake,
                Version = ProtocolVersion.Tls12,
                ContentMessages = new List<ContentMessage>
                {
                    new HandshakeMessage
                    {
                        MessageType = HandshakeType.ClientKeyExchange,
                        Body = SelectedCipher.GetClientKeyExchange()
                    }
                }
            };
            message.Write(_innerStream);

            SelectedCipher.CalculateMasterSecret();

            message = new RecordMessage(this)
            {
                RecordType = ContentType.ChangeCipherSpec,
                Version = ProtocolVersion.Tls12,
                ContentMessages = new List<ContentMessage>
                {
                    new ChangeCipherSpec()
                }
            };
            // RFC 5246 // Page 63
            /*
                Note: ChangeCipherSpec messages, alerts, and any other record types
                are not handshake messages and are not included in the hash
                computations.
            */
            message.Write(_innerStream);

            SelectedCipher.GenerateKeyMaterial(_secParams);

            ClientSequenceNum = 0;
            ServerSequenceNum = 0;

            message = new RecordMessage(this)
            {
                RecordType = ContentType.Handshake,
                Version = ProtocolVersion.Tls12,
                Encrypted = true,
                ContentMessages = new List<ContentMessage>
                {
                    new HandshakeMessage
                    {
                        MessageType = HandshakeType.Finished,
                        Body = SelectedCipher.GetFinished(new ByteArray(HandshakeData.ToArray()))
                    }
                }
            };
            message.Write(_innerStream);

            message = new RecordMessage(this);
            message.OnContentMessageRead += OnContentMessage;
            message.Read(_reader);
        }

        #region Message read events

        internal void OnContentMessage(object sender, ContentMessageReadEventArgs e)
        {
            if (e.Message is HandshakeMessage)
            {
                var message = e.Message as HandshakeMessage;
                if (message.Body is ServerHello)
                    OnServerHello(message.Body as ServerHello);
                if (message.Body is Certificate)
                    OnCertificate(message.Body as Certificate);
                if (message.Body is ServerKeyExchange)
                    OnServerKeyExchange(message.Body as ServerKeyExchange);
            }
        }

        private void OnServerHello(ServerHello message)
        {
            SelectedCipher = CipherSuite.Resolve(message.CipherSuite, _secParams, _role, _random);
            _secParams.ServerRandom = new ByteArray(message.Random.ToArray());
        }

        private void OnCertificate(Certificate message)
        {
            if (message.Certificates.Count == 1)
                _serverCertificate = message.Certificates[0];
            else
                throw new NotImplementedException();
        }

        private void OnServerKeyExchange(ServerKeyExchange message)
        {
            switch (message.SignAndHashAlgo.Signature)
            {
                case SignatureAlgorithm.RSA:
                    var rsa = _serverCertificate.PublicKey.Key as System.Security.Cryptography.RSACryptoServiceProvider;

                    var dhParamsBytes = message.Params.ToArray();
                    DHParams = message.Params;

                    var signedBytes = _secParams.ClientRandom + _secParams.ServerRandom + dhParamsBytes;

                    string oid;
                    switch (message.SignAndHashAlgo.Hash)
                    {
                        case HashAlgorithm.SHA1:
                            oid = System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA1");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    if (!rsa.VerifyData(signedBytes.ToArray(), oid, message.Signature))
                        throw new TlsStreamException("Invalid signature");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Sequnce Numbers

        internal ulong EncodingSequenceNum
        {
            get
            {
                if (_role == Role.Client)
                    return ClientSequenceNum;
                if (_role == Role.Server)
                    return ServerSequenceNum;
                throw new TlsStreamException("Invalid role");
            }
        }

        internal void IncEncodingSequenceNum()
        {
            if (_role == Role.Client)
            {
                ClientSequenceNum++;
                return;
            }
            if (_role == Role.Server)
            {
                ServerSequenceNum++;
                return;
            }
            throw new TlsStreamException("Invalid role");
        }

        #endregion
    }
}
