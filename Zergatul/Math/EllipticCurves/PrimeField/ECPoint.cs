using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.EllipticCurves.PrimeField
{
#if !UseOpenSSL

    public class ECPoint : IEquatable<ECPoint>
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
                if (data.Length % 2 == 0)
                    throw new InvalidOperationException("Invalid EC point format");
                byte[] x = new byte[data.Length / 2];
                byte[] y = new byte[data.Length / 2];
                Array.Copy(data, 1, x, 0, x.Length);
                Array.Copy(data, 1 + x.Length, y, 0, y.Length);

                return new ECPoint
                {
                    x = new BigInteger(x, ByteOrder.BigEndian),
                    y = new BigInteger(y, ByteOrder.BigEndian),
                    Curve = curve
                };
            }
            else
            {
                if (data[0] != 2 && data[0] != 3)
                    throw new InvalidOperationException("Invalid EC point format");

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

    #region System.Object

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Curve.GetHashCode() ^ x.GetHashCode() ^ y.GetHashCode();
        }

    #endregion

    #region IEquatable<ECPoint>

        public bool Equals(ECPoint other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return this.Curve == other.Curve && this.x == other.x && this.y == other.y;
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

            EllipticCurve curve = p1.Curve;
            BigInteger λ;

            if (p1 == p2)
            {
                if (p1.y.IsZero)
                    return Infinity;
                λ = BigInteger.ModularDivision(3 * p1.x * p1.x + curve.a, 2 * p1.y, curve.p);
            }
            else
            {
                if (p1.x == p2.x)
                    return Infinity;
                λ = BigInteger.ModularDivision(p2.y - p1.y, p2.x - p1.x, curve.p);
            }

            var x3 = (λ * λ - p1.x - p2.x) % curve.p;
            var y3 = (λ * (p1.x - x3) - p1.y) % curve.p;
            if (x3 < 0)
                x3 += curve.p;
            if (y3 < 0)
                y3 += curve.p;

            return new ECPoint
            {
                Curve = curve,
                x = x3,
                y = y3
            };
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
                n = n + n;
            }
            return q;
        }

        public bool Validate()
        {
            return (y * y % Curve.p) == ((x * x * x + Curve.a * x + Curve.b) % Curve.p);
        }

    #endregion

    #region Operators

        public static bool operator ==(ECPoint p1, ECPoint p2)
        {
            if (ReferenceEquals(p1, null))
                return ReferenceEquals(p2, null);
            else
                return p1.Equals(p2);
        }

        public static bool operator !=(ECPoint p1, ECPoint p2)
        {
            if (ReferenceEquals(p1, null))
                return !ReferenceEquals(p2, null);
            else
                return !p1.Equals(p2);
        }

        public static ECPoint operator +(ECPoint p1, ECPoint p2)
        {
            return Sum(p1, p2);
        }

        public static ECPoint operator *(BigInteger m, ECPoint p)
        {
            return Multiplication(p, m);
        }

        public static ECPoint operator -(ECPoint p)
        {
            return new ECPoint
            {
                x = p.x,
                y = p.Curve.p - p.y,
                Curve = p.Curve
            };
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

        public byte[] ToUncompressed()
        {
            int len = (Curve.BitSize + 7) / 8;
            byte[] data = new byte[2 * len + 1];
            data[0] = 0x04;
            Array.Copy(x.ToBytes(ByteOrder.BigEndian, len), 0, data, 1, len);
            Array.Copy(y.ToBytes(ByteOrder.BigEndian, len), 0, data, 1 + len, len);

            return data;
        }

        public byte[] ToCompressed()
        {
            int len = (Curve.BitSize + 7) / 8;
            byte[] data = new byte[len + 1];
            if (y.IsOdd)
                data[0] = 0x03;
            else
                data[0] = 0x02;
            Array.Copy(x.ToBytes(ByteOrder.BigEndian, len), 0, data, 1, len);

            return data;
        }
    }

#else

    public class ECPoint : IEquatable<ECPoint>
    {
        public EllipticCurve Curve;
        public BigInteger x;
        public BigInteger y;

        public static readonly ECPoint Infinity = new ECPoint();

        internal IntPtr EC_POINT;

        #region Constructors

        public static ECPoint FromBytes(byte[] data, EllipticCurve curve)
        {
            var point = new ECPoint();
            point.Curve = curve;

            point.EC_POINT = OpenSSL.EC_POINT_new(curve.EC_GROUP);
            if (point.EC_POINT == null)
                throw new InvalidOperationException();

            if (OpenSSL.EC_POINT_oct2point(curve.EC_GROUP, point.EC_POINT, data, data.Length, IntPtr.Zero) != 1)
                throw new InvalidOperationException();

            return point;
        }

        #endregion

        #region System.Object

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Curve.GetHashCode() ^ x.GetHashCode() ^ y.GetHashCode();
        }

        #endregion

        #region IEquatable<ECPoint>

        public bool Equals(ECPoint other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (this.Curve != other.Curve)
                return false;

            int cmp = OpenSSL.EC_POINT_cmp(this.Curve.EC_GROUP, this.EC_POINT, other.EC_POINT, IntPtr.Zero);
            if (cmp == -1)
                throw new InvalidOperationException();

            return cmp == 0;
        }

        #endregion

        #region Operations

        public static ECPoint Sum(ECPoint p1, ECPoint p2)
        {
            if (p1.Curve != p2.Curve)
                throw new ArgumentException("Points should belong to single curve");

            var point = new ECPoint();
            point.Curve = p1.Curve;

            point.EC_POINT = OpenSSL.EC_POINT_new(point.Curve.EC_GROUP);
            if (point.EC_POINT == null)
                throw new InvalidOperationException();

            if (OpenSSL.EC_POINT_add(point.Curve.EC_GROUP, point.EC_POINT, p1.EC_POINT, p2.EC_POINT, IntPtr.Zero) != 1)
                throw new InvalidOperationException();

            return point;
        }

        public static ECPoint Multiplication(ECPoint p, BigInteger m)
        {
            var point = new ECPoint();
            point.Curve = p.Curve;

            point.EC_POINT = OpenSSL.EC_POINT_new(point.Curve.EC_GROUP);
            if (point.EC_POINT == null)
                throw new InvalidOperationException();

            if (OpenSSL.EC_POINT_mul(point.Curve.EC_GROUP, point.EC_POINT, m.BIGNUM, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 1)
                throw new InvalidOperationException();

            return point;
        }

        public bool Validate()
        {
            return (y * y % Curve.p) == ((x * x * x + Curve.a * x + Curve.b) % Curve.p);
        }

        #endregion

        #region Operators

        public static bool operator ==(ECPoint p1, ECPoint p2)
        {
            if (ReferenceEquals(p1, null))
                return ReferenceEquals(p2, null);
            else
                return p1.Equals(p2);
        }

        public static bool operator !=(ECPoint p1, ECPoint p2)
        {
            if (ReferenceEquals(p1, null))
                return !ReferenceEquals(p2, null);
            else
                return !p1.Equals(p2);
        }

        public static ECPoint operator +(ECPoint p1, ECPoint p2)
        {
            return Sum(p1, p2);
        }

        public static ECPoint operator *(BigInteger m, ECPoint p)
        {
            return Multiplication(p, m);
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

        public byte[] ToUncompressed()
        {
            int length = OpenSSL.EC_POINT_point2oct(this.Curve.EC_GROUP, this.EC_POINT, OpenSSL.PointConversionForm.Uncompressed, null, 0, IntPtr.Zero);
            if (length == 0)
                throw new InvalidOperationException();

            byte[] result = new byte[length];
            if (OpenSSL.EC_POINT_point2oct(this.Curve.EC_GROUP, this.EC_POINT, OpenSSL.PointConversionForm.Uncompressed, result, length, IntPtr.Zero) == 0)
                throw new InvalidOperationException();

            return result;
        }

        public byte[] ToCompressed()
        {
            int length = OpenSSL.EC_POINT_point2oct(this.Curve.EC_GROUP, this.EC_POINT, OpenSSL.PointConversionForm.Compressed, null, 0, IntPtr.Zero);
            if (length == 0)
                throw new InvalidOperationException();

            byte[] result = new byte[length];
            if (OpenSSL.EC_POINT_point2oct(this.Curve.EC_GROUP, this.EC_POINT, OpenSSL.PointConversionForm.Compressed, result, length, IntPtr.Zero) == 0)
                throw new InvalidOperationException();

            return result;
        }
    }

#endif
}