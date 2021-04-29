using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Zergatul.Algo.Tests
{
    [TestClass]
    public class ArrayAlgorithmsTests
    {
        [TestMethod]
        public void FindKthTest()
        {
            var rnd = new Random(0);
            Comparison<int> comparison = (i1, i2) => i1.CompareTo(i2);

            var array = Enumerable.Range(0, 10).ToArray();
            for (int i = 0; i < 10; i++)
            {
                ArrayAlgorithms.Shuffle(array, rnd);
                Assert.IsTrue(ArrayAlgorithms.FindKth(array, i, rnd, comparison) == i);
            }

            array = Enumerable.Range(1000, 1000).ToArray();
            for (int i = 0; i < 1000; i++)
            {
                ArrayAlgorithms.Shuffle(array, rnd);
                Assert.IsTrue(ArrayAlgorithms.FindKth(array, i, rnd, comparison) == 1000 + i);
            }
        }
    }
}