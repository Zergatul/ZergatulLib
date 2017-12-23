using Zergatul.Cryptography.Hash;
using Zergatul.Math.EllipticCurves.BinaryField;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECBDSAParameters : ECBParameters
    {
        public AbstractHash Hash { get; set; }

        public ECBDSAParameters(EllipticCurve curve)
            : base(curve)
        {
        }
    }
}