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

        public EdPoint G { get; private set; }

        public EdCurve(BigInteger p, BigInteger d, BigInteger q, EdPoint G)
        {
            this.p = p;
            this.d = d;
            this.q = q;
            this.G = G;
            this.G.Curve = this;
            this.G.z = BigInteger.One;
            this.G.t = G.x * G.y % p;
        }

        public static readonly EdCurve Ed25519 = new EdCurve(
            p: BigInteger.ShiftLeft(BigInteger.One, 255) - 19,
            d: BigInteger.Parse("37095705934669439343138083508754565189542113879843219016388785533085940283555"),
            q: BigInteger.ShiftLeft(BigInteger.One, 252) + BigInteger.Parse("27742317777372353535851937790883648493"),
            G: new EdPoint
            {
                x = BigInteger.Parse("15112221349535400772501151409588531511454012693041857206046113283949847762202"),
                y = BigInteger.Parse("46316835694926478169428394003475163141307993866256225615783033603165251855960")
            }
        );
    }
}