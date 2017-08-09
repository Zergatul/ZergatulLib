using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math.EllipticCurves;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECDSAParameters
    {
        public ISecureRandom Random;
        public IEllipticCurve Curve;
    }
}