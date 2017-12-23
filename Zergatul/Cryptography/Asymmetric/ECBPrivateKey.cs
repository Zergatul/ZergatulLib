using System;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// Elliptic Curve Binary Field private key
    /// </summary>
    public class ECBPrivateKey : AbstractPrivateKey
    {
        public override int KeySize
        {
            get
            {
                if (PublicKey == null)
                    return 0;
                return PublicKey.KeySize;
            }
        }

        internal ECBPublicKey PublicKey;

        public BinaryPolynomial Value { get; private set; }

        public byte[] Value_Raw { get; private set; }

        public ECBPrivateKey(BinaryPolynomial value)
        {
            this.Value = value;
            CalcValueRaw();
        }

        public ECBPrivateKey(byte[] data)
        {
            this.Value_Raw = data;
            CalcValue();
        }

        private void CalcValue() => Value = new BinaryPolynomial(Value_Raw, ByteOrder.BigEndian);
        private void CalcValueRaw() => Value_Raw = Value.ToBytes(ByteOrder.BigEndian);

        public override AbstractEncryption ResolveEncryption()
        {
            throw new NotImplementedException();
        }

        public override AbstractSignature ResolveSignature()
        {
            throw new NotImplementedException();
        }

        public override AbstractKeyExchange ResolveKeyExchange()
        {
            throw new NotImplementedException();
        }
    }
}