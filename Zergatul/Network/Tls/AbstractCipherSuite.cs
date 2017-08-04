using System;
using Zergatul.Cryptography;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls
{
    internal abstract class AbstractCipherSuite
    {
        protected SecurityParameters _secParams;
        protected Role _role;
        protected ISecureRandom _random;
        protected BlockCipherMode _blockCipherMode;

        public virtual void Init(SecurityParameters secParams, Role role, BlockCipherMode mode, ISecureRandom random)
        {
            this._secParams = secParams;
            this._role = role;
            this._blockCipherMode = mode;
            this._random = random;
        }

        public abstract ServerKeyExchange GetServerKeyExchange();
        public abstract void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader);
        public abstract void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer);

        public abstract ClientKeyExchange GetClientKeyExchange();
        public abstract void ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader);
        public abstract void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer);

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
                #region DHE
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
                    cs = new CipherSuiteImplementation<DHEKeyExchange, AES128, SHA1>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
                    cs = new CipherSuiteImplementation<DHEKeyExchange, AES256, SHA1>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA:
                    cs = new CipherSuiteImplementation<DHEKeyExchange, Camellia128, SHA1>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
                    cs = new CipherSuiteImplementation<DHEKeyExchange, AES128, SHA256>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
                    cs = new CipherSuiteImplementation<DHEKeyExchange, AES256, SHA256>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA:
                    cs = new CipherSuiteImplementation<DHEKeyExchange, Camellia256, SHA1>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_ARIA_128_CBC_SHA256:
                    cs = new CipherSuiteImplementation<DHEKeyExchange, ARIA128, SHA256>();
                    break;
                case CipherSuite.TLS_DHE_RSA_WITH_ARIA_256_CBC_SHA384:
                    cs = new CipherSuiteImplementation<DHEKeyExchange, ARIA256, SHA384>();
                    break;
                #endregion
                #region ECDHE
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
                    cs = new CipherSuiteImplementation<ECDHEKeyExchange, AES128, SHA1>();
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
                    cs = new CipherSuiteImplementation<ECDHEKeyExchange, AES256, SHA1>();
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
                    cs = new CipherSuiteImplementation<ECDHEKeyExchange, AES128, SHA256>();
                    break;
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
                    cs = new CipherSuiteImplementation<ECDHEKeyExchange, AES128, SHA384>();
                    break;
                #endregion
                default:
                    throw new NotImplementedException();
            }

            cs.Init(secParams, role, BlockCipherMode.CBC, random);

            return cs;
        }
    }
}