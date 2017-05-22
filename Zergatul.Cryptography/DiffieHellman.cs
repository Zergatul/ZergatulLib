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

        /// <summary>
        /// {A} private key
        /// </summary>
        public BigInteger Xa { get; private set; }

        /// <summary>
        /// {B} private key
        /// </summary>
        public BigInteger Xb { get; private set; }

        /// <summary>
        /// {A} public key
        /// </summary>
        public BigInteger Ya { get; private set; }

        /// <summary>
        /// {B} public key
        /// </summary>
        public BigInteger Yb { get; private set; }

        /// <summary>
        /// Shared secret
        /// </summary>
        public BigInteger ZZ { get; private set; }

        private ISecureRandom _rnd;

        public DiffieHellman(BigInteger g, BigInteger p, BigInteger Ya, ISecureRandom rnd)
        {
            this.p = p;
            this.g = g;
            this.Ya = Ya;
            this._rnd = rnd;
        }

        public void CalculateForBSide()
        {
            var bytes = new byte[42];
            _rnd.GetBytes(bytes);

            Xb = new BigInteger(bytes, ByteOrder.BigEndian);

            Yb = BigInteger.ModularExponentiation(g, Xb, p);
            ZZ = BigInteger.ModularExponentiation(Ya, Xb, p);
        }
    }
}
