using System;

using static System.Math;

namespace Zergatul.Algo
{
    public struct Complex : IEquatable<Complex>
    {
        public double Real { get; }
        public double Imaginary { get; }

        public Complex(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        #region Operators

        public static implicit operator Complex(double value)
        {
            return new Complex(value, 0);
        }

        public static implicit operator Complex((double, double) value)
        {
            return new Complex(value.Item1, value.Item2);
        }

        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
        }

        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.Real - c2.Real, c1.Imaginary - c2.Imaginary);
        }

        public static Complex operator *(Complex c1, Complex c2)
        {
            var (x, y) = c1;
            var (u, v) = c2;
            return new Complex(x * u - y * v, x * v + y * u);
        }

        public static Complex operator /(Complex c, double d)
        {
            return new Complex(c.Real / d, c.Imaginary / d);
        }

        public static Complex operator /(double d, Complex c)
        {
            var (x, y) = c;
            var sqrt = Sqrt(x * x + y * y);
            return new Complex(d * x / sqrt, -d * y / sqrt);
        }

        public static bool operator ==(Complex c1, Complex c2)
        {
            return c1.Real == c2.Real && c1.Imaginary == c2.Imaginary;
        }

        public static bool operator !=(Complex c1, Complex c2)
        {
            return c1.Real != c2.Real || c1.Imaginary != c2.Imaginary;
        }

        #endregion

        #region Overrides

        public void Deconstruct(out double real, out double imaginary)
        {
            real = Real;
            imaginary = Imaginary;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Complex other: return Equals(other);
                default: return false;
            }
        }

        public bool Equals(Complex other)
        {
            return Real == other.Real && Imaginary == other.Imaginary;
        }

        public bool Equals(Complex other, double relativeDiff)
        {
            return Abs(Real - other.Real) < relativeDiff && (Imaginary - other.Imaginary) < relativeDiff;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Real, Imaginary);
        }

        public override string ToString()
        {
            if (Real == 0)
            {
                if (Imaginary == 0)
                    return "0";
                else
                    return $"{Imaginary}*i";
            }
            else
            {
                if (Imaginary == 0)
                    return Real.ToString();
                else
                    return $"{Real}{(Imaginary > 0 ? "+" : "")}{Imaginary}*i";
            }
        }

        #endregion

        #region Static

        public static Complex GetPrincipalRootOfUnity(int n)
        {
            double alpha = 2 * PI / n;
            return new Complex(Cos(alpha), Sin(alpha));
        }

        #endregion
    }
}