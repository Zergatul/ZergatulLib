using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class RSAPrivateKey : AbstractPrivateKey
    {
        public override int KeySize => n.BitSize;

        /// <summary>
        /// Modulus
        /// </summary>
        public BigInteger n { get; private set; }

        /// <summary>
        /// Private exponent
        /// </summary>
        public BigInteger d { get; private set; }

        public byte[] n_Raw { get; private set; }
        public byte[] d_Raw { get; private set; }

        public RSAPrivateKey(BigInteger n, BigInteger d)
        {
            this.n = n;
            this.d = d;
            CalcNRaw();
            CalcDRaw();
        }

        public RSAPrivateKey(byte[] n, byte[] d)
        {
            this.n_Raw = n;
            this.d_Raw = d;
            CalcN();
            CalcD();
        }

        private void CalcN() => n = new BigInteger(n_Raw, ByteOrder.BigEndian);
        private void CalcD() => d = new BigInteger(d_Raw, ByteOrder.BigEndian);
        private void CalcNRaw() => n_Raw = n.ToBytes(ByteOrder.BigEndian);
        private void CalcDRaw() => d_Raw = d.ToBytes(ByteOrder.BigEndian);

        public override AbstractEncryption ResolveEncryption()
        {
            var rsa = new RSAEncryption();
            rsa.PrivateKey = this;
            return rsa;
        }

        public override AbstractSignature ResolveSignature()
        {
            var rsa = new RSASignature();
            rsa.PrivateKey = this;
            return rsa;
        }

        public override AbstractKeyExchange ResolveKeyExchange()
        {
            throw new InvalidOperationException();
        }
    }
}