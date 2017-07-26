using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class RSAPublicKey
    {
        /// <summary>
        /// Modulus
        /// </summary>
        public BigInteger n;

        /// <summary>
        /// Public exponent
        /// </summary>
        public BigInteger e;
    }
}