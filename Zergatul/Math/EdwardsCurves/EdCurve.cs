using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.EdwardsCurves
{
    public class EdCurve
    {
        /// <summary>
        /// Base field
        /// </summary>
        public BigInteger p { get; private set; }

        /// <summary>
        /// Curve constant
        /// </summary>
        public BigInteger d { get; private set; }

        /// <summary>
        /// Group order
        /// </summary>
        public BigInteger q { get; private set; }

        internal BigInteger SqrtMinusOne { get; private set; }

        public EdPoint G { get; private set; }

        public EdCurve(BigInteger p, BigInteger d, BigInteger q, EdPoint G, BigInteger sqrtMinusOne)
        {
            this.p = p;
            this.d = d;
            this.q = q;
            this.G = G;
            this.G.Curve = this;

            if (G.z == null)
                this.G.z = BigInteger.One;
            if (G.t == null)
                this.G.t = G.x * G.y % p;
            if (sqrtMinusOne == null)
                this.SqrtMinusOne = BigInteger.ModularExponentiation(new BigInteger(2), (p - 1) / 4, p);
            else
                this.SqrtMinusOne = sqrtMinusOne;
        }

        public static readonly EdCurve Ed25519 = new EdCurve(
            p: BigInteger.Parse("7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffed", 16),
            d: BigInteger.Parse("52036cee2b6ffe738cc740797779e89800700a4d4141d8ab75eb4dca135978a3", 16),
            q: BigInteger.Parse("1000000000000000000000000000000014def9dea2f79cd65812631a5cf5d3ed", 16),
            sqrtMinusOne: BigInteger.Parse("2b8324804fc1df0b2b4d00993dfbd7a72f431806ad2fe478c4ee1b274a0ea0b0", 16),
            G: new EdPoint
            {
                x = BigInteger.Parse("216936d3cd6e53fec0a4e231fdd6dc5c692cc7609525a7b2c9562d608f25d51a", 16),
                y = BigInteger.Parse("6666666666666666666666666666666666666666666666666666666666666658", 16),
                z = BigInteger.One,
                t = BigInteger.Parse("67875f0fd78b766566ea4e8e64abe37d20f09f80775152f56dde8ab3a5b7dda3", 16)
            }
        );
    }
}