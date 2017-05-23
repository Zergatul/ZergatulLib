using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal abstract class CipherSuite
    {
        public CipherSuiteType Type { get; private set; }

        protected byte[] _preMasterSecret;
        protected byte[] _masterSecret;

        public CipherSuite(CipherSuiteType type)
        {
            this.Type = type;
        }

        public abstract void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader);

        public abstract ClientKeyExchange GetClientKeyExchange();

        public virtual void CalculateMasterSecret(byte[] clientRandom, byte[] serverRandom)
        {
            // RFC 5246 // Page 63
            // The master secret is always exactly 48 bytes in length.
            _masterSecret = PseudoRandomFunction(_preMasterSecret, "master secret", clientRandom.Concat(serverRandom))
                .Take(48).ToArray();

            // RFC 5246 // Page 63
            // The pre_master_secret should be deleted from memory once the master_secret has been computed.
            for (int i = 0; i < _preMasterSecret.Length; i++)
                _preMasterSecret[i] = 0;
        }

        public virtual Finished GetFinished(IEnumerable<byte> data)
        {
            return new Finished
            {
                Data = PseudoRandomFunction(_masterSecret, "client finished", Hash(data)).Take(12).ToArray()
            };
        }

        protected byte[] Hash(IEnumerable<byte> data)
        {
            var algo = new System.Security.Cryptography.SHA256Managed();
            return algo.ComputeHash(data.ToArray());
        }

        protected byte[] HMACHash(byte[] secret, IEnumerable<byte> seed)
        {
            return Hash(secret.Select(b => (byte)(b ^ 0x5C)).Concat(Hash(secret.Select(b => (byte)(b ^ 0x36)).Concat(seed))));
        }

        protected byte[] PHash(byte[] secret, IEnumerable<byte> seed)
        {
            return HMACHash(secret, seed.Concat(seed));
        }

        public virtual byte[] PseudoRandomFunction(byte[] secret, string label, IEnumerable<byte> seed)
        {
            return PHash(secret, Encoding.ASCII.GetBytes(label).Concat(seed));
        }

        public static CipherSuite Resolve(CipherSuiteType type)
        {
            switch (type)
            {
                case CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
                    return new TLS_DHE_RSA_WITH_AES_256_GCM_SHA384();
                case CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
                    return new TLS_DHE_RSA_WITH_AES_256_CBC_SHA();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}