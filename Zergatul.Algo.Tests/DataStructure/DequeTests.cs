using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zergatul.Algo.DataStructure;

namespace Zergatul.Algo.Tests.DataStructure
{
    [TestClass]
    public class DequeTests
    {
        [TestMethod]
        public void AddTest1()
        {
            var deque = new Deque<int>(2);
            deque.PushFirst(2);
            deque.PushLast(3);
            deque.PushFirst(1);
            deque.PushLast(4);
            deque.PushFirst(0);
            deque.PushLast(5);

            Assert.IsTrue(deque.PopFirst() == 0);
            Assert.IsTrue(deque.PopFirst() == 1);
            Assert.IsTrue(deque.PopFirst() == 2);
            Assert.IsTrue(deque.PopFirst() == 3);
            Assert.IsTrue(deque.PopFirst() == 4);
            Assert.IsTrue(deque.PopFirst() == 5);
        }

        [TestMethod]
        public void AddFirstTest1()
        {
            var deque = new Deque<int>(1);

            for (int i = 100; i > 0; i--)
                deque.PushFirst(i);

            Assert.IsTrue(deque.SequenceEqual(Enumerable.Range(1, 100)));
        }

        [TestMethod]
        public void AddFirstTest2()
        {
            var deque = new Deque<int>(100);

            for (int i = 100; i > 0; i--)
                deque.PushFirst(i);

            Assert.IsTrue(deque.SequenceEqual(Enumerable.Range(1, 100)));
        }

        [TestMethod]
        public void AddLastTest1()
        {
            var deque = new Deque<int>(1);

            for (int i = 1; i <= 100; i++)
                deque.PushLast(i);

            Assert.IsTrue(deque.SequenceEqual(Enumerable.Range(1, 100)));
        }

        [TestMethod]
        public void AddLastTest2()
        {
            var deque = new Deque<int>(100);

            for (int i = 1; i <= 100; i++)
                deque.PushLast(i);

            Assert.IsTrue(deque.SequenceEqual(Enumerable.Range(1, 100)));
        }
    }
}