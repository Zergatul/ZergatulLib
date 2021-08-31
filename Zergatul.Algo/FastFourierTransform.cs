using System;
using System.Collections.Generic;
using System.Text;

using static System.Math;

namespace Zergatul.Algo
{
    public static class FastFourierTransform
    {
        public static Complex[] Calculate(Complex[] poly)
        {
            return Calculate(poly, Complex.GetPrincipalRootOfUnity(poly.Length));
        }

        public static Complex[] InverseCalculate(Complex[] values)
        {
            return InverseCalculate(values, Complex.GetPrincipalRootOfUnity(values.Length));
        }

        private static Complex[] Calculate(Complex[] poly, Complex root)
        {
            int n = poly.Length;
            if (n == 1)
                return new Complex[1] { poly[0] };

            Complex[] pe = new Complex[n / 2];
            Complex[] po = new Complex[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                pe[i] = poly[i << 1];
                po[i] = poly[(i << 1) | 1];
            }

            Complex[] ye = Calculate(pe, root * root);
            Complex[] yo = Calculate(po, root * root);

            Complex[] values = new Complex[n];
            Complex w = 1;
            for (int i = 0; i < n / 2; i++)
            {
                values[i] = ye[i] + w * yo[i];
                values[i + n / 2] = ye[i] - w * yo[i];
                w *= root;
            }

            return values;
        }

        private static Complex[] InverseCalculate(Complex[] values, Complex root)
        {
            int n = values.Length;
            if (n == 1)
                return new Complex[1] { values[0] };

            Complex[] ve = new Complex[n / 2];
            Complex[] vo = new Complex[n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                ve[i] = values[i << 1];
                vo[i] = values[(i << 1) | 1];
            }

            Complex[] ye = InverseCalculate(ve, root * root);
            Complex[] yo = InverseCalculate(vo, root * root);

            root = new Complex(root.Real, -root.Imaginary);

            Complex[] poly = new Complex[n];
            Complex x = 1;
            for (int i = 0; i < n / 2; i++)
            {
                poly[i] = (ye[i] + x * yo[i]) / 2;
                poly[i + n / 2] = (ye[i] - x * yo[i]) / 2;
                x *= root;
            }

            return poly;
        }

        public static Complex[] fft(Complex[] a, bool invert)
        {
            int n = a.Length;
            if (n == 1)
                return a;

            var a0 = new Complex[n / 2];
            var a1 = new Complex[n / 2];
            for (int i = 0, j = 0; i < n; i += 2, ++j)
            {
                a0[j] = a[i];
                a1[j] = a[i + 1];
            }

            a0 = fft(a0, invert);
            a1 = fft(a1, invert);

            double ang = 2 * PI / n * (invert ? -1 : 1);
            Complex w = 1;
            Complex wn = new Complex(Cos(ang), Sin(ang));

            for (int i = 0; i < n / 2; ++i)
            {
                a[i] = a0[i] + w * a1[i];
                a[i + n / 2] = a0[i] - w * a1[i];
                if (invert)
                {
                    a[i] /= 2;
                    a[i + n / 2] /= 2;
                }
                w *= wn;
            }

            return a;
        }
    }
}