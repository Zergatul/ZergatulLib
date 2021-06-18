namespace Zergatul.Algo.NumberTheory
{
    public static class GcdExtended
    {
        public static int Calculate(int a, int b, out int x, out int y)
        {
            // Base Case 
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }

            int x1, y1; // To store results of recursive call 
            int gcd = Calculate(b % a, a, out x1, out y1);

            // Update x and y using results of recursive 
            // call 
            x = y1 - (b / a) * x1;
            y = x1;

            return gcd;
        }
    }
}