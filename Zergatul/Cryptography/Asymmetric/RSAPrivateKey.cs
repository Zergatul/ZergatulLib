using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class RSAPrivateKey
    {
        /// <summary>
        /// Modulus
        /// </summary>
        public BigInteger n;

        /// <summary>
        /// Private exponent
        /// </summary>
        public BigInteger d;
    }
}