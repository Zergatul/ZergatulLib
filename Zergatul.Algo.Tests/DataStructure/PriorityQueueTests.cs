using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zergatul.Algo.DataStructure;

namespace Zergatul.Algo.Tests.DataStructure
{
    [TestClass]
    public class PriorityQueueTests
    {
        [TestMethod]
        public void DequeueTest()
        {
            int[] data = Enumerable.Range(1, 100).ToArray();
            ArrayAlgorithms.Shuffle(data, new Random(0));

            var queue = new PriorityQueue<int>(data, Comparer<int>.Default);
            for (int i = 1; i <= 100; i++)
            {
                Assert.IsTrue(queue.Dequeue() == i);
            }
        }
    }
}