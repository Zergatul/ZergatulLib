using System;

namespace Zergatul.Math
{
    public struct Rational<T> : IEquatable<Rational<T>>
        where T : IEquatable<T>, IComparable<T>
    {
        private T a;
        private T b;

        public Rational(long value)
        {
            var calc = IntegerCalculator<T>.Instance;
            a = calc.FromLong(value);
            b = calc.One;
        }

        public Rational(T a, T b)
        {
            var calc = IntegerCalculator<T>.Instance;
            if (b.Equals(calc.Zero))
                throw new DivideByZeroException();
            if (b.CompareTo(calc.Zero) < 0)
            {
                a = calc.Negate(a);
                b = calc.Negate(b);
            }

            if (a.Equals(calc.Zero))
            {
                this.a = calc.Zero;
                this.b = calc.One;
            }
            else
            {
                Simplify(ref a, ref b);
                this.a = a;
                this.b = b;
            }
        }

        #region Interface members & overrides

        public bool Equals(Rational<T> other)
        {
            return a.Equals(other.a) && b.Equals(other.b);
        }

        public override bool Equals(object obj)
        {
            if (obj is Rational<T> other)
                return Equals(other);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return unchecked(31 * a.GetHashCode()) ^ b.GetHashCode();
        }

        #endregion

        #region Operators

        public static implicit operator Rational<T>(int value) => new Rational<T>(value);
        public static implicit operator Rational<T>(long value) => new Rational<T>(value);

        public static explicit operator double(Rational<T> value)
        {
            var calc = IntegerCalculator<T>.Instance;
            return calc.ToDouble(value.a) / calc.ToDouble(value.b);
        }

        public static Rational<T> operator -(Rational<T> value)
        {
            return new Rational<T>(IntegerCalculator<T>.Instance.Negate(value.a), value.b);
        }

        public static Rational<T> operator +(Rational<T> left, Rational<T> right)
        {
            var calc = IntegerCalculator<T>.Instance;
            T gcd = Gcd(left.b, right.b);
            T leftbred = calc.Divide(left.b, gcd);
            T rightbred = calc.Divide(right.b, gcd);
            T a = calc.Add(calc.Multiply(left.a, rightbred), calc.Multiply(right.a, leftbred));
            T b = calc.Multiply(leftbred, right.b);
            return new Rational<T>(a, b);
        }

        public static Rational<T> operator -(Rational<T> left, Rational<T> right)
        {
            var calc = IntegerCalculator<T>.Instance;
            T gcd = Gcd(left.b, right.b);
            T leftbred = calc.Divide(left.b, gcd);
            T rightbred = calc.Divide(right.b, gcd);
            T a = calc.Substract(calc.Multiply(left.a, rightbred), calc.Multiply(right.a, leftbred));
            T b = calc.Multiply(leftbred, right.b);
            return new Rational<T>(a, b);
        }

        public static Rational<T> operator *(Rational<T> left, Rational<T> right)
        {
            var calc = IntegerCalculator<T>.Instance;
            T a = calc.Multiply(left.a, right.a);
            T b = calc.Multiply(left.b, right.b);
            return new Rational<T>(a, b);
        }

        public static Rational<T> operator /(Rational<T> left, Rational<T> right)
        {
            var calc = IntegerCalculator<T>.Instance;
            T a = calc.Multiply(left.a, right.b);
            T b = calc.Multiply(left.b, right.a);
            return new Rational<T>(a, b);
        }

        public static bool operator ==(Rational<T> left, Rational<T> right)
        {
            return left.a.Equals(right.a) && left.b.Equals(right.b);
        }

        public static bool operator !=(Rational<T> left, Rational<T> right)
        {
            return !left.a.Equals(right.a) || !left.b.Equals(right.b);
        }

        public static bool operator <(Rational<T> left, Rational<T> right)
        {
            GetCompareValues(left, right, out T lval, out T rval);
            return lval.CompareTo(rval) < 0;
        }

        public static bool operator >(Rational<T> left, Rational<T> right)
        {
            GetCompareValues(left, right, out T lval, out T rval);
            return lval.CompareTo(rval) > 0;
        }

        public static bool operator <=(Rational<T> left, Rational<T> right)
        {
            GetCompareValues(left, right, out T lval, out T rval);
            return lval.CompareTo(rval) <= 0;
        }

        public static bool operator >=(Rational<T> left, Rational<T> right)
        {
            GetCompareValues(left, right, out T lval, out T rval);
            return lval.CompareTo(rval) >= 0;
        }

        #endregion

        #region Helper static methods

        private static void GetCompareValues(Rational<T> r1, Rational<T> r2, out T value1, out T value2)
        {
            var calc = IntegerCalculator<T>.Instance;
            value1 = calc.Multiply(r1.a, r2.b);
            value2 = calc.Multiply(r2.a, r1.b);
        }

        private static void Simplify(ref T a, ref T b)
        {
            int sign = a.CompareTo(IntegerCalculator<T>.Instance.Zero) >= 0 ? 1 : -1;
            T gcd = Gcd(sign == 1 ? a : IntegerCalculator<T>.Instance.Negate(a), b);
            if (!gcd.Equals(IntegerCalculator<T>.Instance.One))
            {
                a = IntegerCalculator<T>.Instance.Divide(a, gcd);
                b = IntegerCalculator<T>.Instance.Divide(b, gcd);
            }
        }

        private static T Gcd(T a, T b)
        {
            while (b.CompareTo(IntegerCalculator<T>.Instance.Zero) > 0)
            {
                (a, b) = (b, IntegerCalculator<T>.Instance.Modulo(a, b));
            }
            return a;
        }

        #endregion
    }
}