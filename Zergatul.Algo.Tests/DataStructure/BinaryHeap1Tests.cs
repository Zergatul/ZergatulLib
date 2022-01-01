using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zergatul.Algo.DataStructure;

namespace Zergatul.Algo.Tests.DataStructure
{
    [TestClass]
    public class BinaryHeap1Tests
    {
        [TestMethod]
        public void Add10kTest()
        {
            var heap = new BinaryHeap1<int>();

            int[] data = Enumerable.Range(1, 10000).ToArray();
            ArrayAlgorithms.Shuffle(data, new Random(0));

            foreach (int x in data)
                heap.Push(x);

            for (int i = 1; i <= 10000; i++)
                Assert.IsTrue(heap.Pop() == i);
        }
    }
}