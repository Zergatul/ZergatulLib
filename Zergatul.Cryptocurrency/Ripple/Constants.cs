using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptocurrency.Ripple
{
    static class Constants
    {
        public static readonly char[] Dictionary = "rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz".ToCharArray();
        public static readonly EllipticCurve Curve = EllipticCurve.secp256k1;
    }
}