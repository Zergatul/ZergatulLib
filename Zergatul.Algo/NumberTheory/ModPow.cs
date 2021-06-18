namespace Zergatul.Algo.NumberTheory
{
    public static class ModPow
    {
        public static int Calculate(long x, long y, int p)
        {
            long res = 1;
            while (y > 0)
            {
                if ((y & 1) != 0)
                    res = (res * x) % p;

                y = y >> 1;
                x = (x * x) % p;
            }
            return (int)res;
        }
    }
}