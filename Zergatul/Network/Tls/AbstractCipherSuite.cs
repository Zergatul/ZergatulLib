﻿using System;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.BlockCipher.CipherMode;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls
{
    internal abstract class AbstractCipherSuite
    {
        protected SecurityParameters _secParams;
        protected Role _role;
        protected ISecureRandom _random;

        public virtual void Init(SecurityParameters secParams, Role role, ISecureRandom random)
        {
            this._secParams = secParams;
            this._role = role;
            this._random = random;
        }

        public abstract ServerKeyExchange GetServerKeyExchange();
        public abstract void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader);
        public abstract void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer);
        public abstract byte[] GetKeyExchangeDataToSign(ServerKeyExchange message);

        public abstract ClientKeyExchange GetClientKeyExchange();
        public abstract void ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader);
        public abstract void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer);

        public abstract byte[] CreateSignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash);
        public abstract bool VerifySignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash, byte[] signature);

        public abstract void CalculateMasterSecret();
        public abstract void GenerateKeyMaterial();

        public abstract ByteArray GetFinishedVerifyData(ByteArray data, Role role);
        public abstract Finished GetFinished(ByteArray data);

        public abstract ByteArray Encode(ByteArray data, ContentType type, ProtocolVersion version, ulong sequenceNum);
        public abstract ByteArray Decode(ByteArray data, ContentType type, ProtocolVersion version, ulong sequenceNum);

        public static AbstractCipherSuite Resolve(CipherSuite type, SecurityParameters secParams, Role role, ISecureRandom random)
        {
            AbstractCipherSuite cs;
            switch (type)
            {
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
                    cs = new HMACCipherSuiteDefaultPRF<DHEKeyExchange, RSASignature, AES128, CBC, SHA1>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
                    cs = new HMACCipherSuiteDefaultPRF<DHEKeyExchange, RSASignature, AES256, CBC, SHA1>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA:
                    cs = new HMACCipherSuiteDefaultPRF<DHEKeyExchange, RSASignature, Camellia128, CBC, SHA1>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
                    cs = new HMACCipherSuiteDefaultPRF<DHEKeyExchange, RSASignature, AES128, CBC, SHA256>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
                    cs = new HMACCipherSuiteDefaultPRF<DHEKeyExchange, RSASignature, AES256, CBC, SHA256>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA:
                    cs = new HMACCipherSuiteDefaultPRF<DHEKeyExchange, RSASignature, Camellia256, CBC, SHA1>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
                    cs = new AEADCipherSuiteDefaultPRF<DHEKeyExchange, RSASignature, AES128, GCM>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
                    cs = new AEADCipherSuite<DHEKeyExchange, RSASignature, AES256, GCM, SHA384>();
                    break;
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA:
                    cs = new HMACCipherSuiteDefaultPRF<ECDHEKeyExchange, ECDSASignature, AES128, CBC, SHA1>();
                    break;
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA:
                    cs = new HMACCipherSuiteDefaultPRF<ECDHEKeyExchange, ECDSASignature, AES256, CBC, SHA1>();
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
                    cs = new HMACCipherSuiteDefaultPRF<ECDHEKeyExchange, RSASignature, AES128, CBC, SHA1>();
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
                    cs = new HMACCipherSuiteDefaultPRF<ECDHEKeyExchange, RSASignature, AES256, CBC, SHA1>();
                    break;
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
                    cs = new HMACCipherSuiteDefaultPRF<ECDHEKeyExchange, ECDSASignature, AES128, CBC, SHA256>();
                    break;
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
                    cs = new HMACCipherSuite<ECDHEKeyExchange, ECDSASignature, AES256, CBC, SHA256, SHA384>();
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
                    cs = new HMACCipherSuiteDefaultPRF<ECDHEKeyExchange, RSASignature, AES128, CBC, SHA256>();
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
                    cs = new HMACCipherSuite<ECDHEKeyExchange, RSASignature, AES256, CBC, SHA384, SHA384>();
                    break;
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
                    cs = new AEADCipherSuiteDefaultPRF<ECDHEKeyExchange, ECDSASignature, AES128, GCM>();
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
                    cs = new AEADCipherSuiteDefaultPRF<ECDHEKeyExchange, RSASignature, AES128, GCM>();
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
                    cs = new AEADCipherSuite<ECDHEKeyExchange, RSASignature, AES128, GCM, SHA384>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_ARIA_128_CBC_SHA256:
                    cs = new HMACCipherSuiteDefaultPRF<DHEKeyExchange, RSASignature, ARIA128, CBC, SHA256>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_ARIA_256_CBC_SHA384:
                    cs = new HMACCipherSuiteDefaultPRF<DHEKeyExchange, RSASignature, ARIA256, CBC, SHA384>();
                    break;
                /*case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
                    break;
                case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
                    break;*/
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
                    cs = new CCMCipherSuite<DHEKeyExchange, RSASignature, AES128>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
                    cs = new CCMCipherSuite<DHEKeyExchange, RSASignature, AES256>();
                    break;
                /*case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
                    break;
                case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
                    break;*/
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
                    cs = new CCMCipherSuite<DHEKeyExchange, RSASignature, AES128>(8);
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
                    cs = new CCMCipherSuite<DHEKeyExchange, RSASignature, AES256>(8);
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
                    cs = new ChaCha20CipherSuite<ECDHEKeyExchange, RSASignature, SHA256>();
                    break;
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
                    cs = new ChaCha20CipherSuite<ECDHEKeyExchange, ECDSASignature, SHA256>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
                    cs = new ChaCha20CipherSuite<DHEKeyExchange, RSASignature, SHA256>();
                    break;
                default:
                    throw new NotImplementedException();
            }

            cs.Init(secParams, role, random);

            return cs;
        }
    }
}