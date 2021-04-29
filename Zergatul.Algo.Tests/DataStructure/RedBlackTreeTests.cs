using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Zergatul.Algo.DataStructure;

namespace Zergatul.Algo.Tests.DataStructure
{
    [TestClass]
    public class RedBlackTreeTests
    {
        [TestMethod]
        public void Add100ItemsBackwardTest()
        {
            var tree = new RedBlackTree<int>();
            Assert.IsTrue(tree.SequenceEqual(new int[0]));

            const int count = 100;
            for (int i = count; i >= 0; i--)
            {
                tree.Add(i);
                Assert.IsTrue(tree.SequenceEqual(Enumerable.Range(i, count - i + 1)));
            }
        }

        [TestMethod]
        public void Add100ItemsForwardTest()
        {
            var tree = new RedBlackTree<int>();
            Assert.IsTrue(tree.SequenceEqual(new int[0]));

            const int count = 100;
            for (int i = 0; i <= count; i++)
            {
                tree.Add(i);
                Assert.IsTrue(tree.SequenceEqual(Enumerable.Range(0, i + 1)));
            }
        }

        [TestMethod]
        public void Add1000ItemsRandomTest()
        {
            var tree = new RedBlackTree<int>();

            var rnd = new Random(0);
            var list = new List<int>();
            for (int i = 0; i < 1000; i++)
            {
                int item = rnd.Next(1000000000);
                list.Add(item);
                list.Sort();
                tree.Add(item);
                Assert.IsTrue(tree.SequenceEqual(list));
            }
        }

        [TestMethod]
        public void Remove100ItemsBackwardTest()
        {
            var tree = new RedBlackTree<int>(Enumerable.Range(0, 100));
            Assert.IsTrue(tree.SequenceEqual(Enumerable.Range(0, 100)));

            for (int i = 99; i >= 0; i--)
            {
                Assert.IsTrue(tree.Remove(i));
                Assert.IsTrue(tree.SequenceEqual(Enumerable.Range(0, i)));
            }
        }

        [TestMethod]
        public void Remove100ItemsForwardTest()
        {
            var tree = new RedBlackTree<int>(Enumerable.Range(0, 100));
            Assert.IsTrue(tree.SequenceEqual(Enumerable.Range(0, 100)));

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(tree.Remove(i));
                Assert.IsTrue(tree.SequenceEqual(Enumerable.Range(i + 1, 100 - i - 1)));
            }
        }

        [TestMethod]
        public void Remove1000ItemsRandomTest()
        {
            var rnd = new Random(0);
            var list = new List<int>();
            for (int i = 0; i < 1000; i++)
                list.Add(rnd.Next(1000000000));
            list.Sort();

            var tree = new RedBlackTree<int>(list);
            Assert.IsTrue(tree.Count == 1000);
            Assert.IsTrue(tree.SequenceEqual(list));

            for (int i = 0; i < 1000; i++)
            {
                int index = rnd.Next(list.Count);
                int item = list[index];
                list.RemoveAt(index);
                Assert.IsTrue(tree.Remove(item));
                Assert.IsTrue(tree.Count == 1000 - i - 1);
                Assert.IsTrue(tree.SequenceEqual(list));
            }
        }

        private IEnumerable<int> RevRange(int from, int to)
        {
            for (int i = from; i <= to; i++)
                yield return i;
        }
    }
}