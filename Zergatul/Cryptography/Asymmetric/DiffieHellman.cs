using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class DiffieHellman : AbstractAsymmetricAlgorithm<DiffieHellmanParameters, BigInteger, BigInteger, BigInteger>
    {
        public override BigInteger PrivateKey { get; set; }
        public override BigInteger PublicKey { get; set; }

        public override int KeySize => _parameters.p.BitSize;

        private DiffieHellmanParameters _parameters;
        private DHKeyExchange _keyExchange;

        public override void SetParameters(DiffieHellmanParameters parameters)
        {
            this._parameters = parameters;
        }

        public override void GenerateKeys(ISecureRandom random)
        {
            PrivateKey = BigInteger.Random(_parameters.p, random);
            PublicKey = BigInteger.ModularExponentiation(_parameters.g, PrivateKey, _parameters.p);
        }

        public override AbstractSignatureAlgorithm Signature
        {
            get
            {
                throw new NotSupportedException("Diffie Hellman doesn't support signing");
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

            public override BigInteger GenerateSharedSecret(BigInteger publicKey)
            {
                return BigInteger.ModularExponentiation(publicKey, dh.PrivateKey, dh._parameters.p);
            }

            public override byte[] GenerateSharedSecretBytes(BigInteger publicKey)
            {
                return GenerateSharedSecret(publicKey).ToBytes(ByteOrder.BigEndian, dh.KeySize / 8);
            }
        }
    }
}