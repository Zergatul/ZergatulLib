using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.EllipticCurves.BinaryField
{
    public class ECPoint
    {
        public EllipticCurve Curve;
        public BigInteger x;
        public BigInteger y;
    }
}