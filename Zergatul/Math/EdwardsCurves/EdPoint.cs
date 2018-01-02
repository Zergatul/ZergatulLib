using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.EdwardsCurves
{
    public class EdPoint : IEquatable<EdPoint>
    {
        public BigInteger x;
        public BigInteger y;
        public BigInteger z;
        public BigInteger t;

        public EdCurve Curve;

        public bool Equals(EdPoint other)
        {
            return Equals(this, other);
        }

        public static bool Equals(EdPoint p1, EdPoint p2)
        {
            bool p1null = ReferenceEquals(p1, null);
            bool p2null = ReferenceEquals(p2, null);
            if (p1null && p2null)
                return true;
            if (p1null ^ p2null)
                return false;

            if (p1.Curve != p2.Curve)
                throw new InvalidOperationException();

            BigInteger
                X1 = p1.x, Y1 = p1.y, X2 = p2.x, Y2 = p2.y,
                T1 = p1.t, Z1 = p1.z, T2 = p2.t, Z2 = p2.z,
                p = p1.Curve.p;

            if (!((X1 * Z2 - X2 * Z1) % p).IsZero)
                return false;
            if (!((Y1 * Z2 - Y2 * Z1) % p).IsZero)
                return false;

            return true;
        }

        public static EdPoint Sum(EdPoint p1, EdPoint p2)
        {
            if (p1.Curve == null || p2.Curve == null)
                throw new InvalidOperationException();

            if (p1.Curve != p2.Curve)
                throw new InvalidOperationException();

            BigInteger
                X1 = p1.x, Y1 = p1.y, X2 = p2.x, Y2 = p2.y,
                T1 = p1.t, Z1 = p1.z, T2 = p2.t, Z2 = p2.z,
                p = p1.Curve.p, d = p1.Curve.d;

            BigInteger A = (Y1 - X1) * (Y2 - X2) % p;
            BigInteger B = (Y1 + X1) * (Y2 + X2) % p;
            BigInteger C = 2 * d * T1 * T2 % p;
            BigInteger D = 2 * Z1 * Z2 % p;
            BigInteger E = B - A;
            BigInteger F = D - C;
            BigInteger G = D + C;
            BigInteger H = B + A;

            return new EdPoint
            {
                x = E * F,
                y = G * H,
                z = F * G,
                t = E * H,

                Curve = p1.Curve
            };
        }

        public static EdPoint Product(BigInteger s, EdPoint p)
        {
            var q = new EdPoint
            {
                x = BigInteger.Zero,
                y = BigInteger.One,
                z = BigInteger.One,
                t = BigInteger.Zero,

                Curve = p.Curve
            };

            int bitSize = s.BitSize;
            for (int bit = 0; bit < bitSize; bit++)
            {
                if (s.IsBitSet(bit))
                    q = q + p;
                p = p + p;
            }

            return q;
        }

        public static bool operator ==(EdPoint p1, EdPoint p2) => Equals(p1, p2);
        public static bool operator !=(EdPoint p1, EdPoint p2) => !Equals(p1, p2);
        public static EdPoint operator +(EdPoint p1, EdPoint p2) => Sum(p1, p2);
        public static EdPoint operator *(BigInteger s, EdPoint p) => Product(s, p);
    }
}