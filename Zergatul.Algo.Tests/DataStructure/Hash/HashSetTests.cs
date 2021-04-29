using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Zergatul.Algo.DataStructure.Hash;

namespace Zergatul.Algo.Tests.DataStructure.Hash
{
    [TestClass]
    public class HashSetTests
    {
        [TestMethod]
        public void BasicTest()
        {
            var set = new CoalescedChainingHashSet<int>();

            Assert.IsFalse(set.Contains(123));
            Assert.IsFalse(set.Contains(124));
            Assert.IsFalse(set.Contains(125));
            Assert.IsTrue(set.Count == 0);

            Assert.IsTrue(set.Add(123));
            Assert.IsTrue(set.Contains(123));
            Assert.IsFalse(set.Contains(124));
            Assert.IsFalse(set.Contains(125));
            Assert.IsFalse(set.Add(123));
            Assert.IsTrue(set.Count == 1);

            Assert.IsTrue(set.Add(124));
            Assert.IsTrue(set.Contains(123));
            Assert.IsTrue(set.Contains(124));
            Assert.IsFalse(set.Contains(125));
            Assert.IsFalse(set.Add(124));
            Assert.IsTrue(set.Count == 2);

            Assert.IsTrue(set.Add(125));
            Assert.IsTrue(set.Contains(123));
            Assert.IsTrue(set.Contains(124));
            Assert.IsTrue(set.Contains(125));
            Assert.IsFalse(set.Add(125));
            Assert.IsTrue(set.Count == 3);
        }

        [TestMethod]
        public void ResizeTest()
        {
            var set = new CoalescedChainingHashSet<int>();

            for (int i = 0; i < 1000; i++)
                Assert.IsTrue(set.Add(i));

            Assert.IsTrue(set.Count == 1000);

            for (int i = -1000; i < 0; i++)
                Assert.IsFalse(set.Contains(i));

            for (int i = 0; i < 1000; i++)
                Assert.IsTrue(set.Contains(i));

            for (int i = 1000; i < 2000; i++)
                Assert.IsFalse(set.Contains(i));
        }

        [TestMethod]
        public void CellarStressTest()
        {
            var set = new CoalescedChainingHashSet<int>();

            for (int i = 0; i < 100; i++)
                Assert.IsTrue(set.Add(i << 16));

            for (int i = 0; i < 100; i++)
                Assert.IsTrue(set.Contains(i << 16));
        }

        [TestMethod]
        public void EnumerateTest()
        {
            var rnd = new Random(0);

            var set = new CoalescedChainingHashSet<int>();
            var list = new List<int>();
            for (int i = 0; i < 1000; i++)
            {
                int value = rnd.Next();
                if (!set.Contains(value))
                {
                    Assert.IsTrue(set.Add(value));
                    list.Add(value);
                }

                Assert.IsTrue(list.Except(set).Count() == 0);
                Assert.IsTrue(set.Except(list).Count() == 0);
            }
        }

        [TestMethod]
        public void RemoveTest()
        {
            var set = new CoalescedChainingHashSet<int>();

            Assert.IsTrue(set.Add(1));
            Assert.IsFalse(set.Remove(9));
            Assert.IsTrue(set.Remove(1));
            Assert.IsFalse(set.Remove(1));
            Assert.IsTrue(set.Count == 0);
            Assert.IsTrue(set.SequenceEqual(new int[0]));

            set.Clear();

            Assert.IsTrue(set.Add(1));
            Assert.IsTrue(set.Add(2));
            Assert.IsTrue(set.Add(3));
            Assert.IsTrue(set.Remove(2));
            Assert.IsTrue(set.Count == 2);
            Assert.IsTrue(set.SequenceEqual(new[] { 1, 3 }));

            set.Clear();

            Assert.IsTrue(set.Add(1));
            Assert.IsTrue(set.Add(9));
            Assert.IsTrue(set.Add(17));
            Assert.IsTrue(set.Remove(1));
            Assert.IsTrue(set.Count == 2);
            Assert.IsTrue(set.SequenceEqual(new[] { 9, 17 }));

            set.Clear();

            Assert.IsTrue(set.Add(1));
            Assert.IsTrue(set.Add(9));
            Assert.IsTrue(set.Add(17));
            Assert.IsTrue(set.Remove(9));
            Assert.IsTrue(set.Count == 2);
            Assert.IsTrue(set.SequenceEqual(new[] { 1, 17 }));

            set.Clear();

            Assert.IsTrue(set.Add(1));
            Assert.IsTrue(set.Add(9));
            Assert.IsTrue(set.Add(17));
            Assert.IsTrue(set.Remove(17));
            Assert.IsTrue(set.Count == 2);
            Assert.IsTrue(set.SequenceEqual(new[] { 1, 9 }));
        }

        [TestMethod]
        public void RandomRemoveTest()
        {
            int n = 100000;
            var remove = new int[n];
            for (int i = 0; i < n; i++)
                remove[i] = i;

            ArrayAlgorithms.Shuffle(remove, new Random(0));

            var set = new CoalescedChainingHashSet<int>(n);
            for (int i = 0; i < n; i++)
                set.Add(i);

            foreach (int i in remove)
                set.Remove(i);

            Assert.IsTrue(set.Count == 0);
        }
    }
}