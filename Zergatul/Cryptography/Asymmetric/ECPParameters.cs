using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECPParameters : AbstractParameters
    {
        public EllipticCurve Curve { get; private set; }

        public ECPParameters(EllipticCurve curve)
        {
            this.Curve = curve;
        }
    }
}