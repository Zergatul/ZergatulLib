using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECPDSAParameters : ECPParameters
    {
        public AbstractHash Hash { get; set; }

        public ECPDSAParameters(EllipticCurve curve)
            : base(curve)
        {
        }
    }
}