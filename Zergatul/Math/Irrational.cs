using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Math
{
    //public struct Irrational
    //{
    //    private long a;
    //    private long b;
    //    private Root[] roots;

    //    public Irrational(long value)
    //    {
    //        a = value;
    //        b = 1;
    //    }

    //    #region Operators

    //    public static Irrational operator +(Irrational left, Irrational right)
    //    {
    //        if (left.roots == null && right.roots == null)
    //        {
    //            long a = left.a * right.b + right.a * left.b;
    //            long b = left.b * right.b;
    //            (a, b) = Simplify(a, b);
    //            if (a == 0)
    //                b = 1;
    //            return new Irrational { a = a, b = b };
    //        }
    //        throw new NotImplementedException();
    //    }

    //    public static Irrational operator *(Irrational left, Irrational right)
    //    {
    //        long a = left.a * right.a;
    //        long b = left.b * right.b;
    //        if (a == 0)
    //            b = 1;
    //        if (left.roots == null || right.roots == null)
    //        {
    //            return new Irrational { a = a, b = b };
    //        }
    //        throw new NotImplementedException();
    //    }

    //    #endregion

    //    #region Helper static methods

    //    private static (long, long) Simplify(long a, long b)
    //    {
    //        long gcd = Gcd(a, b);
    //        return (a / gcd, b / gcd);
    //    }

    //    private static long Gcd(long a, long b)
    //    {
    //        while (b > 0)
    //        {
    //            (a, b) = (b, a % b);
    //        }
    //        return a;
    //    }

    //    #endregion

    //    private struct Root
    //    {
    //        public long A, B;
    //        public long Sqrt;
    //    }
    //}
}