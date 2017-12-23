using System;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class DiffieHellman : AbstractKeyExchange<DiffieHellmanPrivateKey, DiffieHellmanPublicKey, DiffieHellmanParameters>
    {
        public override DiffieHellmanPrivateKey PrivateKey
        {
            get
            {
                return base.PrivateKey;
            }
            set
            {
                base.PrivateKey = value;
                value.DH = this;
            }
        }

        public override DiffieHellmanPublicKey PublicKey
        {
            get
            {
                return base.PublicKey;
            }
            set
            {
                base.PublicKey = value;
                value.DH = this;
            }
        }

        public override void GenerateKeyPair(int keySize)
        {
            PrivateKey = new DiffieHellmanPrivateKey(BigInteger.Random(Parameters.p, Random));
            PublicKey = new DiffieHellmanPublicKey(BigInteger.ModularExponentiation(Parameters.g, PrivateKey.Value, Parameters.p));
        }

        public override byte[] CalculateSharedSecret(DiffieHellmanPublicKey key)
        {
            if (PrivateKey == null)
                throw new InvalidOperationException("Private key is required");

            return BigInteger.ModularExponentiation(key.Value, PrivateKey.Value, Parameters.p).ToBytes(ByteOrder.BigEndian);
        }

        #region Converters

        public override AbstractSignature ToSignature()
        {
            throw new NotSupportedException();
        }

        public override AbstractKeyExchange ToKeyExchange()
        {
            return this;
        }

        #endregion
    }
}