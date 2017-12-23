using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// Elliptic Curve Prime Field private key
    /// </summary>
    public class ECPPrivateKey : AbstractPrivateKey
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

        internal ECPPublicKey PublicKey;

        public BigInteger Value { get; private set; }

        public byte[] Value_Raw { get; private set; }

        public ECPPrivateKey(BigInteger value)
        {
            this.Value = value;
            CalcValueRaw();
        }

        public ECPPrivateKey(byte[] data)
        {
            this.Value_Raw = data;
            CalcValue();
        }

        private void CalcValue() => Value = new BigInteger(Value_Raw, ByteOrder.BigEndian);
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