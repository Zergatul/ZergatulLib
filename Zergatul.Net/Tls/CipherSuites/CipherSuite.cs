using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal abstract class CipherSuite
    {
        public CipherSuiteType Type { get; private set; }

        protected ByteArray _preMasterSecret;
        protected ByteArray _masterSecret;

        public CipherSuite(CipherSuiteType type)
        {
            this.Type = type;
        }

        public abstract void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader);

        public abstract ClientKeyExchange GetClientKeyExchange();

        public virtual void CalculateMasterSecret(ByteArray clientRandom, ByteArray serverRandom)
        {
            // RFC 5246 // Page 63
            // The master secret is always exactly 48 bytes in length.
            _masterSecret = PseudoRandomFunction(_preMasterSecret, "master secret", clientRandom + serverRandom, 48);

            // RFC 5246 // Page 63
            // The pre_master_secret should be deleted from memory once the master_secret has been computed.
            _preMasterSecret.ClearMemory();
        }

        public virtual Finished GetFinished(ByteArray data)
        {
            // RFC 5246 // Page 62
            /*
                In previous versions of TLS, the verify_data was always 12 octets
                long.  In the current version of TLS, it depends on the cipher
                suite.  Any cipher suite which does not explicitly specify
                verify_data_length has a verify_data_length equal to 12.  This
                includes all existing cipher suites.  Note that this
                representation has the same encoding as with previous versions.
                Future cipher suites MAY specify other lengths but such length
                MUST be at least 12 bytes.
            */
            return new Finished
            {
                Data = PseudoRandomFunction(_masterSecret, "client finished", Hash(data), 12).ToArray()
            };
        }

        protected ByteArray Hash(ByteArray data)
        {
            var algo = new System.Security.Cryptography.SHA256Managed();
            return new ByteArray(algo.ComputeHash(data.ToArray()));
        }

        protected ByteArray HMACHash(ByteArray secret, ByteArray seed)
        {
            // RFC 2104 // Page 2
            return Hash((secret ^ 0x5C) + Hash((secret ^ 0x36) + seed));
        }

        protected ByteArray PHash(ByteArray secret, ByteArray seed, int length)
        {
            // RFC 5246 // Page 14
            /*
                P_hash(secret, seed) = HMAC_hash(secret, A(1) + seed) +
                                       HMAC_hash(secret, A(2) + seed) +
                                       HMAC_hash(secret, A(3) + seed) + ...
                A() is defined as:
                     A(0) = seed
                     A(i) = HMAC_hash(secret, A(i - 1))
            */
            var a = seed;
            ByteArray result = new ByteArray();
            while (result.Length < length)
            {
                a = HMACHash(secret, a);
                result = result + HMACHash(secret, a + seed);
            }
            return result.Truncate(length);
        }

        public virtual ByteArray PseudoRandomFunction(ByteArray secret, string label, ByteArray seed, int length)
        {
            // RFC 5246 // Page 14
            // PRF(secret, label, seed) = P_<hash>(secret, label + seed)
            return PHash(secret, Encoding.ASCII.GetBytes(label) + seed, length);
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