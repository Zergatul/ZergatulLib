using System;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Security.Default
{
    class ECPublicKey : PublicKey
    {
        public ECPoint Point { get; }

        public ECPublicKey(ECPoint point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            Point = point;
        }
    }
}