using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.EllipticCurves.BinaryField
{
    public class ECPoint : IEquatable<ECPoint>
    {
        public static readonly ECPoint Infinity = new ECPoint();

        public EllipticCurve Curve;
        public BinaryPolynomial x;
        public BinaryPolynomial y;

        #region Instance methods

        public bool Validate()
        {
            var xSquare = BinaryPolynomial.ModularMultiplication(x, x, Curve.f);
            var ySquare = BinaryPolynomial.ModularMultiplication(y, y, Curve.f);
            return
                ySquare + BinaryPolynomial.ModularMultiplication(x, y, Curve.f) ==
                BinaryPolynomial.ModularMultiplication(xSquare, x, Curve.f) + BinaryPolynomial.ModularMultiplication(Curve.a, xSquare, Curve.f) + Curve.b;
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
                throw new ArgumentException("Points should belong to the same curve");

            EllipticCurve curve = p1.Curve;
            BinaryPolynomial λ;

            if (p1 == p2)
                λ = BinaryPolynomial.ModularDivision(p1.y, p1.x, curve.f) + p1.x;
            else
                λ = BinaryPolynomial.ModularDivision(p1.y + p2.y, p1.x + p2.x, curve.f);

            BinaryPolynomial x3 = BinaryPolynomial.ModularMultiplication(λ, λ, curve.f) % curve.f + λ + p1.x + p2.x + curve.a;
            BinaryPolynomial y3 = BinaryPolynomial.ModularMultiplication(p1.x + x3, λ, curve.f) + x3 + p1.y;

            return new ECPoint
            {
                x = x3,
                y = y3,
                Curve = curve
            };
        }

        public static ECPoint Multiplication(ECPoint point, BinaryPolynomial polynomial)
        {
            ECPoint result = Infinity;
            for (int i = polynomial.Degree; i >= 0; i--)
            {
                result = result + result;
                if (polynomial.IsBitSet(i))
                    result = result + point;
            }
            return result;
        }

        #endregion

        #region System.Object

        public override bool Equals(object obj)
        {
            if (obj is ECPoint)
                return Equals((ECPoint)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ Curve.GetHashCode();
        }

        #endregion

        #region IEquatable<ECPoint>

        public bool Equals(ECPoint other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (this.Curve != other.Curve)
                return false;
            return this.x == other.x && this.y == other.y;
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

        public static ECPoint operator *(BinaryPolynomial bp, ECPoint p)
        {
            return Multiplication(p, bp);
        }

        #endregion
    }
}