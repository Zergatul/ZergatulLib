using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zergatul.Algo.Tree;

namespace Zergatul.Algo.Tests.Tree.Lca
{
    [TestClass]
    public class BuildOrderArrayTests
    {
        [TestMethod]
        public void SingleVertexTest()
        {
            LowestCommonAncestor.BuildOrderArray(
                0,
                new[]
                {
                    new int[0]
                },
                out var order,
                out var first,
                out var height);

            Assert.IsTrue(order.SequenceEqual(new[] { 0 }));
            Assert.IsTrue(first.SequenceEqual(new[] { 0 }));
            Assert.IsTrue(height.SequenceEqual(new[] { 0 }));
        }

        [TestMethod]
        public void TwoVerticesTest()
        {
            LowestCommonAncestor.BuildOrderArray(
                1,
                new[]
                {
                    new int[0],
                    new[] { 0 }
                },
                out var order,
                out var first,
                out var height);

            Assert.IsTrue(order.SequenceEqual(new[] { 1, 0, 1 }));
            Assert.IsTrue(first.SequenceEqual(new[] { 1, 0 }));
            Assert.IsTrue(height.SequenceEqual(new[] { 0, 1, 0 }));
        }

        [TestMethod]
        public void SimpleTreeTest()
        {
            LowestCommonAncestor.BuildOrderArray(
                1,
                new[]
                {
                    new int[0],
                    new[] { 0, 2 },
                    new[] { 3, 4 },
                    new int[0],
                    new int[0],
                },
                out var order,
                out var first,
                out var height);

            Assert.IsTrue(order.SequenceEqual(new[] { 1, 0, 1, 2, 3, 2, 4, 2, 1 }));
            Assert.IsTrue(first.SequenceEqual(new[] { 1, 0, 3, 4, 6 }));
            Assert.IsTrue(height.SequenceEqual(new[] { 0, 1, 0, 1, 2, 1, 2, 1, 0 }));
        }

        [TestMethod]
        public void SevenVerticesTest()
        {
            LowestCommonAncestor.BuildOrderArray(
                0,
                new[]
                {
                    new int[] { 1, 2, 3 },
                    new int[] { 4, 5 },
                    new int[] { },
                    new int[] { 6 },
                    new int[] { },
                    new int[] { },
                    new int[] { },
                },
                out var order,
                out var first,
                out var height);

            Assert.IsTrue(order.SequenceEqual(new[] { 0, 1, 4, 1, 5, 1, 0, 2, 0, 3, 6, 3, 0 }));
            Assert.IsTrue(first.SequenceEqual(new[] { 0, 1, 7, 9, 2, 4, 10 }));
            Assert.IsTrue(height.SequenceEqual(new[] { 0, 1, 2, 1, 2, 1, 0, 1, 0, 1, 2, 1, 0 }));
        }

        [TestMethod]
        public void SevenVertices2Test()
        {
            LowestCommonAncestor.BuildOrderArray2(
                0,
                new[]
                {
                    new int[] { 1, 2, 3 },
                    new int[] { 4, 5 },
                    new int[] { },
                    new int[] { 6 },
                    new int[] { },
                    new int[] { },
                    new int[] { },
                },
                out var order,
                out var first,
                out var height);

            Assert.IsTrue(order.SequenceEqual(new[] { 0, 1, 4, 1, 5, 1, 0, 2, 0, 3, 6, 3, 0 }));
            Assert.IsTrue(first.SequenceEqual(new[] { 0, 1, 7, 9, 2, 4, 10 }));
            Assert.IsTrue(height.SequenceEqual(new[] { 0, 1, 1, 1, 2, 2, 2 }));
        }
    }
}