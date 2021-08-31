using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Zergatul.Algo.Tests.DataStructure
{
    [TestClass]
    public class SinglyLinkedListTests
    {
        [TestMethod]
        public void AddFirstLastTest()
        {
            var list = new SinglyLinkedList<int>();
            list.AddFirst(1);
            Assert.IsTrue(list.SequenceEqual(new[] { 1 }));

            list = new SinglyLinkedList<int>();
            list.AddLast(1);
            Assert.IsTrue(list.SequenceEqual(new[] { 1 }));

            list = new SinglyLinkedList<int>();
            list.AddFirst(2);
            list.AddFirst(1);
            Assert.IsTrue(list.SequenceEqual(new[] { 1, 2 }));

            list = new SinglyLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            Assert.IsTrue(list.SequenceEqual(new[] { 1, 2 }));

            list = new SinglyLinkedList<int>();
            list.AddLast(2);
            list.AddFirst(1);
            list.AddLast(3);
            list.AddFirst(0);
            Assert.IsTrue(list.SequenceEqual(new[] { 0, 1, 2, 3 }));
        }

        [TestMethod]
        public void AddAfterTest()
        {
            var list = new SinglyLinkedList<int>();
            list.AddFirst(1);
            list.AddAfter(list.First, 2);
            list.AddAfter(list.Last, 3);
            Assert.IsTrue(list.SequenceEqual(new[] { 1, 2, 3 }));

            list = new SinglyLinkedList<int>();
            list.AddAfter(null, 5);
            list.AddAfter(null, 4);
            list.AddAfter(null, 3);
            list.AddAfter(null, 2);
            list.AddAfter(null, 1);
            list.AddLast(6);
            Assert.IsTrue(list.SequenceEqual(new[] { 1, 2, 3, 4, 5, 6 }));
        }

        [TestMethod]
        public void RemoveAfterTest()
        {
            var list = new SinglyLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);
            list.AddLast(4);
            list.AddLast(5);
            list.RemoveAfter(null);
            list.RemoveAfter(list.First);
            list.RemoveAfter(list.First);
            Assert.IsTrue(list.SequenceEqual(new[] { 2, 5 }));

            list = new SinglyLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);
            list.AddLast(4);
            list.AddLast(5);
            list.RemoveAfter(list.First.Next.Next.Next);
            list.AddLast(10);
            Assert.IsTrue(list.SequenceEqual(new[] { 1, 2, 3, 4, 10 }));
        }
    }
}