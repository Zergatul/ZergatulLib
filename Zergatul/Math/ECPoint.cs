using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math
{
    public class ECPoint
    {
        public EllipticCurve Curve;
        public BigInteger x;
        public BigInteger y;

        public static readonly ECPoint Infinity = new ECPoint();

        #region Constructors

        public static ECPoint FromBytes(byte[] data, EllipticCurve curve)
        {
            if (data[0] == 4) // uncompressed form
            {
                throw new NotImplementedException();
            }
            else
            {
                var point = new ECPoint
                {
                    x = new BigInteger(data, 1, data.Length - 1, ByteOrder.BigEndian),
                    Curve = curve
                };
                point.CalculateY((data[0] & 1) == 1);

                return point;
            }
        }

        public static ECPoint FromBytes(uint[] data, EllipticCurve curve)
        {
            byte[] bytes = new byte[data.Length * 4];
            for (int i = 0; i < data.Length; i++)
                Array.Copy(BitHelper.GetBytes(data[i], ByteOrder.BigEndian), 0, bytes, i * 4, 4);
            int leadingZeros = 0;
            while (bytes[leadingZeros] == 0)
                leadingZeros++;

            if (leadingZeros != 0)
            {
                byte[] result = new byte[bytes.Length - leadingZeros];
                Array.Copy(bytes, leadingZeros, result, 0, result.Length);
                return FromBytes(result, curve);
            }
            else
                return FromBytes(bytes, curve);
        }

        #endregion

        #region Operations

        public static ECPoint Sum(ECPoint p1, ECPoint p2)
        {
            if (ReferenceEquals(p1, Infinity))
                return p2;
            if (ReferenceEquals(p2, Infinity))
                return p1;
            if (p1.Curve != p2.Curve)
                throw new ArgumentException("Points should belong to single curve");

            return ComputePointFromGamma(p2.y - p1.y, p2.x - p1.x, p1, p2, p1.Curve);
        }

        public static ECPoint Double(ECPoint p)
        {
            return ComputePointFromGamma(3 * p.x * p.x + p.Curve.a, 2 * p.y, p, p, p.Curve);
        }

        public static ECPoint Multiplication(ECPoint p, BigInteger m)
        {
            ECPoint n = p;
            ECPoint q = null;
            int bitSize = m.BitSize;
            for (int i = 0; i < bitSize; i++)
            {
                if (m.IsBitSet(i))
                    q = ReferenceEquals(q, null) ? n : Sum(q, n);
                n = Double(n);
            }
            return q;
        }

        public bool Validate()
        {
            return (y * y % Curve.p) == ((x * x * x + Curve.a * x + Curve.b) % Curve.p);
        }

        private static ECPoint ComputePointFromGamma(BigInteger g1, BigInteger g2, ECPoint p1, ECPoint p2, EllipticCurve curve)
        {
            if (g1 < 0)
                g1 += curve.p;
            if (g2 < 0)
                g2 += curve.p;

            BigInteger gamma = (g1 * BigInteger.ModularInverse(g2, curve.p)) % curve.p;

            var p3 = new ECPoint();
            p3.Curve = curve;
            p3.x = (gamma * gamma - p1.x - p2.x) % curve.p;
            p3.y = (gamma * (p1.x - p3.x) - p1.y) % curve.p;
            if (p3.x < 0)
                p3.x += curve.p;
            if (p3.y < 0)
                p3.y += curve.p;
            return p3;
        }

        #endregion

        #region Operators

        public static bool operator ==(ECPoint p1, ECPoint p2)
        {
            bool p1null = ReferenceEquals(p1, null);
            bool p2null = ReferenceEquals(p2, null);
            if (p1null && p2null)
                return true;
            if (p1null ^ p2null)
                return false;
            return ReferenceEquals(p1, p2) || (p1.Curve == p2.Curve && p1.x == p2.x && p1.y == p2.y);
        }

        public static bool operator !=(ECPoint p1, ECPoint p2)
        {
            bool p1null = ReferenceEquals(p1, null);
            bool p2null = ReferenceEquals(p2, null);
            if (p1null && p2null)
                return false;
            if (p1null ^ p2null)
                return true;
            return !ReferenceEquals(p1, p2) && (p1.Curve != p2.Curve || p1.x == p2.x || p1.y == p2.y);
        }

        #endregion

        public void CalculateY(bool isOdd)
        {
            BigInteger y2 = (x * x * x + Curve.a * x + Curve.b) % Curve.p;
            BigInteger sqrt = BigInteger.ModularSquareRoot(y2, Curve.p);

            if (sqrt.IsOdd == isOdd)
                y = sqrt;
            else
                y = Curve.p - sqrt;
        }
    }
}