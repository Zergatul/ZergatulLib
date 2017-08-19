using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class DiffieHellman : AbstractAsymmetricAlgorithm<DiffieHellmanParameters, BigInteger, BigInteger, BigInteger, NullParam, NullParam>
    {
        public override BigInteger PrivateKey { get; set; }
        public override BigInteger PublicKey { get; set; }
        public override DiffieHellmanParameters Parameters { get; set; }

        public override int KeySize => Parameters.p.BitSize;

        private DHKeyExchange _keyExchange;

        public override void GenerateKeys()
        {
            PrivateKey = BigInteger.Random(Parameters.p, Random);
            PublicKey = BigInteger.ModularExponentiation(Parameters.g, PrivateKey, Parameters.p);
        }

        public override AbstractSignatureAlgorithm<NullParam, NullParam> Signature
        {
            get
            {
                throw new NotSupportedException("Diffie Hellman doesn't support signing");
            }
        }

        public override AbstractAsymmetricEncryption Encryption
        {
            get
            {
                throw new NotSupportedException("Diffie Hellman doesn't support ecryption");
            }
        }

        public override AbstractKeyExchangeAlgorithm<BigInteger, BigInteger> KeyExchange
        {
            get
            {
                if (_keyExchange == null)
                {
                    if (PrivateKey == null || PublicKey == null)
                        throw new InvalidOperationException("You should fill PrivateKey/PublicKey before using KeyExchange");
                    _keyExchange = new DHKeyExchange(this);
                }
                return _keyExchange;
            }
        }

        private class DHKeyExchange : AbstractKeyExchangeAlgorithm<BigInteger, BigInteger>
        {
            private DiffieHellman dh;

            public DHKeyExchange(DiffieHellman dh)
            {
                this.dh = dh;
            }

            public override void CalculateSharedSecret(BigInteger publicKey)
            {
                SharedSecret = BigInteger.ModularExponentiation(publicKey, dh.PrivateKey, dh.Parameters.p);
            }
        }
    }
}