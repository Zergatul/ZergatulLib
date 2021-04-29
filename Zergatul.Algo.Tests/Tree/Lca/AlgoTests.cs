using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Zergatul.Algo.Tree;

namespace Zergatul.Algo.Tests.Tree.Lca
{
    [TestClass]
    public class AlgoTests
    {
        [TestMethod]
        public void SevenVerticesTest()
        {
            var queries = GetAllQueries(
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
                }).ToArray();

            Assert.IsTrue(Query(queries, 0, 0, 0));
            Assert.IsTrue(Query(queries, 0, 1, 0));
            Assert.IsTrue(Query(queries, 1, 0, 0));
            Assert.IsTrue(Query(queries, 0, 5, 0));
            Assert.IsTrue(Query(queries, 5, 0, 0));
            Assert.IsTrue(Query(queries, 4, 5, 1));
            Assert.IsTrue(Query(queries, 1, 2, 0));
            Assert.IsTrue(Query(queries, 3, 1, 0));
            Assert.IsTrue(Query(queries, 2, 6, 0));
            Assert.IsTrue(Query(queries, 3, 6, 3));
            Assert.IsTrue(Query(queries, 6, 6, 6));
            Assert.IsTrue(Query(queries, 5, 6, 0));
            Assert.IsTrue(Query(queries, 5, 1, 1));
        }

        [TestMethod]
        public void TenVerticesTest()
        {
            var queries = GetAllQueries(
                6,
                new[]
                {
                    new int[] { 7, 1 },
                    new int[] { 4 },
                    new int[] { },
                    new int[] { },
                    new int[] { 5 },
                    new int[] { },
                    new int[] { 0, 9, 8 },
                    new int[] { 3 },
                    new int[] { 2 },
                    new int[] { },
                }).ToArray();

            Assert.IsTrue(Query(queries, 0, 9, 6));
        }

        private static bool Query(IEnumerable<Func<int, int, int>> queries, int v1, int v2, int res)
        {
            foreach (var query in queries)
            {
                if (query(v1, v2) != res)
                    return false;
            }
            return true;
        }

        private static IEnumerable<Func<int, int, int>> GetAllQueries(int root, int[][] adj)
        {
            yield return LowestCommonAncestor.SegmentTreeAlgo(root, adj).Query;
            yield return LowestCommonAncestor.FarachColtonBenderAlgo(root, adj).Query;
            yield return LowestCommonAncestor.SparseTableAlgo(root, adj).Query;
        }
    }
}