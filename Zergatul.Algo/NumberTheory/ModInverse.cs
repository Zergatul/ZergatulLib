namespace Zergatul.Algo.NumberTheory
{
    public static class ModInverse
    {
        public static int Calculate(int b, int m)
        {
            int x, y; // used in extended GCD algorithm
            int g = GcdExtended.Calculate(b, m, out x, out y);

            // Return -1 if b and m are not co-prime 
            if (g != 1)
                return -1;

            // m is added to handle negative x
            return (x % m + m) % m;
        }
    }
}