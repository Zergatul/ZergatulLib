using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Zergatul.Algo.Sorting;

namespace Zergatul.Algo.Tests.Sorting
{
    [TestClass]
    public class TimSortTests
    {
        [TestMethod]
        public void Test1()
        {
            var rnd = new Random(0);
            int[] array = new int[100];
            for (int i = 0; i < array.Length; i++)
                array[i] = rnd.Next(100);

            int[] copy = (int[])array.Clone();
            Array.Sort(copy);

            TimSort.Sort(array, Comparer<int>.Default);

            AssertArraysEquals(array, copy);
        }

        [TestMethod]
        public void Test2()
        {
            var rnd = new Random(0);
            int[] array = new int[100000];
            for (int i = 0; i < array.Length; i++)
                array[i] = rnd.Next(100);

            int[] copy = (int[])array.Clone();
            Array.Sort(copy);

            TimSort.Sort(array, Comparer<int>.Default);

            AssertArraysEquals(array, copy);
        }

        [TestMethod]
        public void Test3()
        {
            var rnd = new Random(0);
            int[] array = new int[100000];
            for (int i = 0; i < array.Length; i++)
                array[i] = rnd.Next(1000000000);

            int[] copy = (int[])array.Clone();
            Array.Sort(copy);

            TimSort.Sort(array, Comparer<int>.Default);

            AssertArraysEquals(array, copy);
        }

        private static void AssertArraysEquals(int[] a1, int[] a2)
        {
            Assert.IsTrue(a1.Length == a2.Length);
            for (int i = 0; i < a1.Length; i++)
                Assert.IsTrue(a1[i] == a2[i]);
        }
    }
}