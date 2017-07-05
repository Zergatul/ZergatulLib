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
        public BigInteger Xa { get; set; }

        /// <summary>
        /// {B} private key
        /// </summary>
        public BigInteger Xb { get; set; }

        /// <summary>
        /// {A} public key
        /// </summary>
        public BigInteger Ya { get; set; }

        /// <summary>
        /// {B} public key
        /// </summary>
        public BigInteger Yb { get; set; }

        /// <summary>
        /// Shared secret
        /// </summary>
        public BigInteger ZZ { get; private set; }

        private ISecureRandom _rnd;

        public DiffieHellman(BigInteger g, BigInteger p, ISecureRandom rnd)
        {
            this.p = p;
            this.g = g;
            this._rnd = rnd;
        }

        public void CalculateForASideStep1()
        {
            Xa = BigInteger.Random(p, _rnd);
            Ya = BigInteger.ModularExponentiation(g, Xa, p);
        }

        public void CalculateForASideStep2()
        {
            ZZ = BigInteger.ModularExponentiation(Yb, Xa, p);
        }

        public void CalculateForBSide()
        {
            Xb = BigInteger.Random(p, _rnd);

            Yb = BigInteger.ModularExponentiation(g, Xb, p);
            ZZ = BigInteger.ModularExponentiation(Ya, Xb, p);
        }
    }
}
