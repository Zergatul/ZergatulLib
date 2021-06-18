using System;

namespace Zergatul.Algo.NumberTheory
{
    public static class ModSqrt
    {
        public static int Calculate(int a, int p, Random rnd)
        {
            // p is prime
            int jacobi = JacobiSymbol.Calculate(a, p);
            if (jacobi == -1)
                return 0;

            // p = 3 (mod 4)
            if ((p & 3) == 3)
            {
                return ModPow.Calculate(a, (p + 1) >> 2, p);
            }

            // p = 5 (mod 8)
            if ((p & 7) == 5)
            {
                int d = ModPow.Calculate(a, (p - 1) >> 2, p);
                if (d == 1)
                    return ModPow.Calculate(a, (p + 3) >> 3, p);
                else
                    return 2 * a * ModPow.Calculate(4 * a % p, (p - 5) >> 3, p) % p;
            }

            int b;
            while (true)
            {
                b = rnd.Next(1, p);
                if (JacobiSymbol.Calculate(b, p) == -1)
                    break;
            }

            // p - 1 = 2^s t
            int s = 0;
            int t = p - 1;
            while ((t & 1) == 0)
            {
                s++;
                t >>= 1;
            }

            int aInv = ModInverse.Calculate(a, p);
            int c = ModPow.Calculate(b, t, p);
            int r = ModPow.Calculate(a, (t + 1) >> 1, p);

            for (int i = 1; i < s; i++)
            {
                int x = (int)((long)r * r * aInv % p);
                int d = ModPow.Calculate(x, 1 << (s - i - 1), p);
                if (d == p - 1)
                    r = r * c % p;
                c = c * c % p;
            }

            return r;
        }
    }
}