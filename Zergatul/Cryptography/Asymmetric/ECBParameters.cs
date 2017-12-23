using Zergatul.Math.EllipticCurves.BinaryField;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECBParameters : AbstractParameters
    {
        public EllipticCurve Curve { get; private set; }

        public ECBParameters(EllipticCurve curve)
        {
            this.Curve = curve;
        }
    }
}