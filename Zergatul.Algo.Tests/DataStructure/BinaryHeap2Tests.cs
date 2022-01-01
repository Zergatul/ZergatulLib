using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Zergatul.Algo.DataStructure;

namespace Zergatul.Algo.Tests.DataStructure
{
    [TestClass]
    public class BinaryHeap2Tests
    {
        [TestMethod]
        public void Add10kTest()
        {
            var heap = new BinaryHeap2<int, string>();

            int[] data = Enumerable.Range(1, 10000).ToArray();
            ArrayAlgorithms.Shuffle(data, new Random(0));

            foreach (int x in data)
                heap.Push(x, x.ToString());

            for (int i = 1; i <= 10000; i++)
                Assert.IsTrue(heap.Pop().Key == i);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var heap = new BinaryHeap2<int, string>();
            var node1 = heap.Push(10, "node1");
            var node2 = heap.Push(5, "node2");
            var node3 = heap.Push(3, "node3");
            var node4 = heap.Push(8, "node4");
            var node5 = heap.Push(7, "node5");
            var node6 = heap.Push(20, "node6");

            heap.Peek2(out var top1, out var top2);
            Assert.IsTrue(top1.Key == 3);
            Assert.IsTrue(top2.Key == 5);

            heap.Update(node2, 2);

            Assert.IsTrue(heap.Peek().Key == 2);
            Assert.IsTrue(heap.Peek().Value == "node2");
            heap.Peek2(out top1, out top2);
            Assert.IsTrue(top1.Key == 2);
            Assert.IsTrue(top2.Key == 3);

            heap.Update(node1, 1);

            Assert.IsTrue(heap.Peek().Key == 1);
            Assert.IsTrue(heap.Peek().Value == "node1");
            heap.Peek2(out top1, out top2);
            Assert.IsTrue(top1.Key == 1);
            Assert.IsTrue(top2.Key == 2);

            heap.Update(node6, 6);
            heap.Update(node5, 5);
            heap.Update(node4, 4);

            Assert.IsTrue(heap.Pop() == node1);
            Assert.IsTrue(heap.Pop() == node2);
            Assert.IsTrue(heap.Pop() == node3);
            Assert.IsTrue(heap.Pop() == node4);
            Assert.IsTrue(heap.Pop() == node5);
            Assert.IsTrue(heap.Pop() == node6);
        }

        [TestMethod]
        public void RemoveTest()
        {
            var heap = new BinaryHeap2<int, string>();

            const int count = 10000;
            int[] data = Enumerable.Range(1, count).ToArray();
            ArrayAlgorithms.Shuffle(data, new Random(0));

            var nodes = new List<BinaryHeap2<int, string>.Node>();
            foreach (int x in data)
                nodes.Add(heap.Push(x, x.ToString()));

            // remove all odd
            var odd = nodes.Where(n => n.Key % 2 == 1);
            foreach (var node in odd)
                heap.Remove(node);

            for (int i = 2; i <= count; i += 2)
            {
                Assert.IsTrue(heap.Pop().Key == i);
            }
        }
    }
}