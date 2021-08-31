using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zergatul.Algo.NumberTheory;

namespace Zergatul.Algo.Tests.NumberTheory
{
    [TestClass]
    public class NumberTheoreticTransformTests
    {
        [TestMethod]
        public void Test1()
        {
            var ntt = new NumberTheoreticTransform(786433, 4);
            var res = new long[8];
            ntt.Multiply(new long[] { 1, 1 }, new long[] { 1, 1 }, res);

            Assert.IsTrue(res[0] == 1);
            Assert.IsTrue(res[1] == 2);
            Assert.IsTrue(res[2] == 1);
            Assert.IsTrue(res.Skip(3).All(x => x == 0));
        }

        [TestMethod]
        public void Test2()
        {
            var ntt = new NumberTheoreticTransform(786433, 4);
            var a = new long[8]
            {
                1, 5, 8, 10, 0, 0, 0, 0
            };
            ntt.Transform(a, 0);

            Assert.IsTrue(ArrayEquals(a, new long[8]
            {
                24, 490003, 286301, 458216, 786427, 323966, 500118, 300685
            }));
        }

        [TestMethod]
        public void Test3()
        {
            var ntt = new NumberTheoreticTransform(998244353, 4);
            var res = new long[8];
            ntt.Multiply(new long[] { 5, 6, 7, 8 }, new long[] { 9, 8, 7, 6 }, res);

            Assert.IsTrue(ArrayEquals(res, new long[8]
            {
                45, 94, 146, 200, 149, 98, 48, 0
            }));
        }

        private static bool ArrayEquals(long[] a1, long[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
        }
    }
}