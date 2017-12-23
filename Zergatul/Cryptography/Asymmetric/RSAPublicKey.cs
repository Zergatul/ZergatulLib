using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class RSAPublicKey : AbstractPublicKey
    {
        public override int KeySize => n.BitSize;

        /// <summary>
        /// Modulus
        /// </summary>
        public BigInteger n;

        /// <summary>
        /// Public exponent
        /// </summary>
        public BigInteger e;

        public byte[] n_Raw { get; private set; }
        public byte[] e_Raw { get; private set; }

        public RSAPublicKey(BigInteger n, BigInteger e)
        {
            this.n = n;
            this.e = e;
            CalcNRaw();
            CalcERaw();
        }

        public RSAPublicKey(byte[] n, byte[] e)
        {
            this.n_Raw = n;
            this.e_Raw = e;
            CalcN();
            CalcE();
        }

        private void CalcN() => n = new BigInteger(n_Raw, ByteOrder.BigEndian);
        private void CalcE() => e = new BigInteger(e_Raw, ByteOrder.BigEndian);
        private void CalcNRaw() => n_Raw = n.ToBytes(ByteOrder.BigEndian);
        private void CalcERaw() => e_Raw = e.ToBytes(ByteOrder.BigEndian);

        public override AbstractEncryption ResolveEncryption()
        {
            var rsa = new RSAEncryption();
            rsa.PublicKey = this;
            return rsa;
        }

        public override AbstractSignature ResolveSignature()
        {
            var rsa = new RSASignature();
            rsa.PublicKey = this;
            return rsa;
        }

        public override AbstractKeyExchange ResolveKeyExchange()
        {
            throw new InvalidOperationException();
        }
    }
}