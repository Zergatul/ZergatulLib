using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Math;
using Zergatul.Net.Tls.Extensions;

namespace Zergatul.Net.Tls
{
    // https://www.ietf.org/rfc/rfc5246.txt
    // https://tools.ietf.org/search/rfc4492#section-5.1
    // http://blog.fourthbit.com/2014/12/23/traffic-analysis-of-an-ssl-slash-tls-session
    public class TlsStream
    {
        private Stream _innerStream;
        private BinaryReader _reader;
        private TlsUtils _utils;

        internal CipherSuite SelectedCipher;
        private X509Certificate2 _serverCertificate;
        private byte[] _clientRandom;
        private byte[] _serverRandom;
        private ServerDHParams DHParams;

        public TlsStream(Stream innerStream)
        {
            this._innerStream = innerStream;
            this._reader = new BinaryReader(_innerStream);
            this._utils = new TlsUtils();

            this.SelectedCipher = CipherSuite.INVALID;
        }

        public void AuthenticateAsClient(string host)
        {
            var random = new Random
            {
                GMTUnixTime = _utils.GetGMTUnixTime(),
                RandomBytes = _utils.GetRandomBytes(28)
            };
            _clientRandom = random.ToArray();

            var message = new RecordMessage(this)
            {
                RecordType = ContentType.Handshake,
                Version = ProtocolVersion.Tls12,
                ContentMessages = new List<ContentMessage>
                {
                    new HandshakeMessage(this)
                    {
                        MessageType = HandshakeType.ClientHello,
                        Body = new ClientHello
                        {
                            ClientVersion = ProtocolVersion.Tls12,
                            Random = random,
                            CipherSuites = new CipherSuite[]
                            {
                                /*CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384,
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
                                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,*/
                                CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384,
                                CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
                                CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA,
                                CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA,
                                CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384,
                                CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256,
                                CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256,
                                CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256,
                                CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA,
                                CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA,
                                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
                                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384,
                                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256,
                                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA,
                                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA,
                                CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256,
                                CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256,
                                CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA,
                                CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA,
                                CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA,
                                CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA,
                                CipherSuite.TLS_RSA_WITH_RC4_128_SHA,
                                CipherSuite.TLS_RSA_WITH_RC4_128_MD5
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
            message.Read(_reader);

            ClientKeyExchange keys;
            switch (CipherSuiteDetail.Ciphers[SelectedCipher].KeyExchange)
            {
                case KeyExchangeAlgorithm.RSA:
                    //EncryptedPreMasterSecret
                    throw new NotImplementedException();
                case KeyExchangeAlgorithm.DHE_DSS:
                case KeyExchangeAlgorithm.DHE_RSA:
                case KeyExchangeAlgorithm.DH_DSS:
                case KeyExchangeAlgorithm.DH_RSA:
                case KeyExchangeAlgorithm.DH_anon:
                    var dh = new DiffieHellman(DHParams.DH_g, DHParams.DH_p, DHParams.DH_Ys, new DefaultSecureRandom());
                    dh.CalculateForBSide();
                    keys = new ClientKeyExchange(this)
                    {
                        DHPublic = new ClientDiffieHellmanPublic
                        {
                            DH_Yc = dh.Yb.ToBytes(ByteOrder.BigEndian, 42)
                        }
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }

            message = new RecordMessage(this)
            {
                RecordType = ContentType.Handshake,
                Version = ProtocolVersion.Tls12,
                ContentMessages = new List<ContentMessage>
                {
                    new HandshakeMessage(this)
                    {
                        MessageType = HandshakeType.ClientKeyExchange,
                        Body = keys
                    }
                }
            };
            message.Write(_innerStream);

            message = new RecordMessage(this);
            message.Read(_reader);
        }

        internal void OnContentMessage(ContentMessage message)
        {
            if (message is HandshakeMessage)
            {
                var handshakeMsg = message as HandshakeMessage;
                if (handshakeMsg.Body is ServerHello)
                    OnServerHello(handshakeMsg.Body as ServerHello);
                if (handshakeMsg.Body is Certificate)
                    OnCertificate(handshakeMsg.Body as Certificate);
                if (handshakeMsg.Body is ServerKeyExchange)
                    OnServerKeyExchange(handshakeMsg.Body as ServerKeyExchange);
            }
        }

        private void OnServerHello(ServerHello message)
        {
            SelectedCipher = message.CipherSuite;
            _serverRandom = message.Random.ToArray();
        }

        private void OnCertificate(Certificate message)
        {
            _serverCertificate = message.Certificates.First();
        }

        private void OnServerKeyExchange(ServerKeyExchange message)
        {
            switch (message.SignAndHashAlgo.Signature)
            {
                case SignatureAlgorithm.RSA:
                    var rsa = _serverCertificate.PublicKey.Key as System.Security.Cryptography.RSACryptoServiceProvider;

                    var dhParamsBytes = message.Params.ToArray();
                    DHParams = message.Params;

                    var signedBytes = new byte[64 + dhParamsBytes.Length];
                    Array.Copy(_clientRandom, 0, signedBytes, 0, 32);
                    Array.Copy(_serverRandom, 0, signedBytes, 32, 32);
                    Array.Copy(dhParamsBytes, 0, signedBytes, 64, dhParamsBytes.Length);

                    string oid;
                    switch (message.SignAndHashAlgo.Hash)
                    {
                        case HashAlgorithm.SHA1:
                            oid = System.Security.Cryptography.CryptoConfig.MapNameToOID("SHA1");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    if (!rsa.VerifyData(signedBytes, oid, message.Signature))
                        throw new TlsStreamException("Invalid signature");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
