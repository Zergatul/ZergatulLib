using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Algo.NumberTheory
{
    public static class JacobiSymbol
    {
        public static int Calculate(int a, int n)
        {
            // n - odd, n >= 3
            // 0 <= a < n

            if (a == 0)
                return 0;
            if (a == 1)
                return 1;

            // a = 2^e a1
            // a1 - odd
            int e = 0;
            int a1 = a;
            while ((a1 & 1) == 0)
            {
                e++;
                a1 >>= 1;
            }

            int s;
            // e - even
            if ((e & 1) == 0)
            {
                s = 1;
            }
            else
            {
                int mod8 = n & 7;
                // n = 1 (mod 8)
                // n = 7 (mod 8)
                if (mod8 == 1 || mod8 == 7)
                    s = 1;
                // n = 3 (mod 8)
                // n = 5 (mod 8)
                else
                    s = -1;
            }

            if (a1 == 1)
                return s;

            // n = 3 (mod 4)
            // a1 = 3 (mod 4)
            if ((n & 3) == 3 && (a1 & 3) == 3)
                s = -s;

            int n1 = n % a1;

            return s * Calculate(n1, a1);
        }
    }
}