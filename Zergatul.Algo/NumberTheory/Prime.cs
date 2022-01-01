using System;
//using Zergatul.Math;

namespace Zergatul.Algo.NumberTheory
{
    public static class Prime
    {
        public static bool MillerRabinProbablePrimeTest(ulong n, int iterations, Random rnd)
        {
            // n - 1 = 2^s r
            // r - odd
            int s = 0;
            ulong r = n - 1;
            while ((r & 1) == 0)
            {
                s++;
                r >>= 1;
            }

            //for (int iteration = 0; iteration < iterations; iteration++)
            //{
            //    // 2 <= a <= n - 2
            //    ulong a = (ulong)(rnd.Next(2, n - 1 > int.MaxValue ? int.MaxValue : (int)(n - 1)));
            //    ulong y = UInt128.ModPow(a, r, n);
            //    if (y != 1 && y != n - 1)
            //    {
            //        int j = 1;
            //        while (j <= s - 1 && y != n - 1)
            //        {
            //            y = new UInt128(y) * y % n;
            //            if (y == 1)
            //                return false;
            //            j++;
            //        }
            //        if (y != n - 1)
            //            return false;
            //    }
            //}

            return true;
        }
    }
}