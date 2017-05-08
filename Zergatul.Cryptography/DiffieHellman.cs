using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography
{
    // https://www.ietf.org/rfc/rfc2631.txt
    public class DiffieHellman
    {
        public BigInteger p { get; private set; }
        public BigInteger g { get; private set; }
        public BigInteger Xa { get; private set; }
        public BigInteger Xb { get; private set; }
        public BigInteger Ya { get; private set; }
        public BigInteger Yb { get; private set; }
        public BigInteger ZZ { get; private set; }

        public DiffieHellman(byte[] g, byte[] p, byte[] Ya)
        {
            this.p = new BigInteger(p, ByteOrder.BigEndian);
            this.g = new BigInteger(g, ByteOrder.BigEndian);
            this.Ya = new BigInteger(Ya, ByteOrder.BigEndian);
        }

        public void CalculateForBSide()
        {
            var bytes = new byte[42];
            var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            /*Xb = new BigInteger(bytes);

            Yb = BigInteger.ModPow(g, Xb, p);
            ZZ = BigInteger.ModPow(Ya, Xb, p);*/
        }
    }
}
