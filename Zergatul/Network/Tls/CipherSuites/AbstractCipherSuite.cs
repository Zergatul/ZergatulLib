using System;
using Zergatul.Cryptography;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls.CipherSuites
{
    internal abstract class AbstractCipherSuite
    {
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

        public static AbstractCipherSuite Resolve(CipherSuiteType type, SecurityParameters secParams, Role role, ISecureRandom random)
        {
            switch (type)
            {
                case CipherSuiteType.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
                    return new CipherSuite<DHEKeyExchange, AES128, SHA1>(secParams, role, BlockCipherMode.CBC, random);
                case CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
                    return new CipherSuite<DHEKeyExchange, AES256, SHA1>(secParams, role, BlockCipherMode.CBC, random);
                case CipherSuiteType.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
                    return new CipherSuite<DHEKeyExchange, AES128, SHA256>(secParams, role, BlockCipherMode.CBC, random);
                case CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
                    return new CipherSuite<DHEKeyExchange, AES256, SHA256>(secParams, role, BlockCipherMode.CBC, random);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}