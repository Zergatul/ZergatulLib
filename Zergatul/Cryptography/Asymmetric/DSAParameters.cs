using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class DSAParameters : AbstractParameters
    {
        public AbstractHash Hash { get; set; }

        public BigInteger p { get; private set; }
        public BigInteger q { get; private set; }
        public BigInteger g { get; private set; }

        public DSAParameters(BigInteger p, BigInteger q, BigInteger g)
        {
            this.p = p;
            this.q = q;
            this.g = g;
        }
    }
}