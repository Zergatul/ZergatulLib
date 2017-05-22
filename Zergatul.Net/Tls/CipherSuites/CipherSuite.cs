using System;
using System.Collections.Generic;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal abstract class CipherSuite
    {
        public CipherSuiteType Type { get; private set; }

        public CipherSuite(CipherSuiteType type)
        {
            this.Type = type;
        }

        public abstract void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader);
        public abstract ClientKeyExchange GetClientKeyExchange();
        public abstract void CalculateMasterSecret(byte[] clientRandom, byte[] serverRandom);
        public abstract Finished GetFinished(IEnumerable<byte> data);
        public abstract byte[] PseudoRandomFunction(byte[] secret, string label, IEnumerable<byte> seed);

        public static CipherSuite Resolve(CipherSuiteType type)
        {
            switch (type)
            {
                case CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
                    return new TLS_DHE_RSA_WITH_AES_256_GCM_SHA384();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}