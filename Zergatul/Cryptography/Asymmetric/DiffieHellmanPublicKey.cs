using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class DiffieHellmanPublicKey : AbstractPublicKey
    {
        public override int KeySize
        {
            get
            {
                if (DH == null)
                    return 0;
                return DH.Parameters.KeySize;
            }
        }

        internal DiffieHellman DH;

        public BigInteger Value { get; private set; }
        public byte[] Value_Raw { get; private set; }

        public DiffieHellmanPublicKey(BigInteger value)
        {
            this.Value = value;
            CalcValueRaw();
        }

        public DiffieHellmanPublicKey(byte[] data)
        {
            this.Value_Raw = data;
            CalcValue();
        }

        private void CalcValue() => Value = new BigInteger(Value_Raw, ByteOrder.BigEndian);
        private void CalcValueRaw() => Value_Raw = Value.ToBytes(ByteOrder.BigEndian);

        public override AbstractEncryption ResolveEncryption()
        {
            throw new NotSupportedException();
        }

        public override AbstractSignature ResolveSignature()
        {
            throw new NotSupportedException();
        }

        public override AbstractKeyExchange ResolveKeyExchange()
        {
            var dh = new DiffieHellman();
            dh.PublicKey = this;
            return dh;
        }
    }
}