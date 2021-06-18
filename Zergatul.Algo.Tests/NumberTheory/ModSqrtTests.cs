using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zergatul.Algo.NumberTheory;

namespace Zergatul.Algo.Tests.NumberTheory
{
    [TestClass]
    public class ModSqrtTests
    {
        [TestMethod]
        public void Test1()
        {
            var rnd = new Random(0);

            // default
            int p = 31177;
            foreach (int x in new[] { 2, 3, 4, 6, 8, 9, 31173, 31174, 31175, 31176 })
            {
                int sqrt = ModSqrt.Calculate(x, p, rnd);
                Assert.IsTrue(sqrt * sqrt % p == x);
            }

            // 5 mod 8
            p = 31181;
            foreach (int x in new[] { 4, 5, 6, 31177 })
            {
                int sqrt = ModSqrt.Calculate(x, p, rnd);
                Assert.IsTrue(sqrt * sqrt % p == x);
            }

            // 3 mod 4
            p = 31183;
            foreach (int x in new[] { 2, 4, 7, 8, 9, 31180 })
            {
                int sqrt = ModSqrt.Calculate(x, p, rnd);
                Assert.IsTrue(sqrt * sqrt % p == x);
            }
        }
    }
}