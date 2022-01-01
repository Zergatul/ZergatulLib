using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zergatul.Algo.Geometry;

namespace Zergatul.Algo.Tests.Geometry
{
    [TestClass]
    public class VoronoiTests
    {
        [TestMethod]
        public void ZeroPointsTest()
        {
            var diagram = VoronoiDiagram.Fortune(new Point2D[0]);

            Assert.IsTrue(diagram.Vertices.Count == 0);
            Assert.IsTrue(diagram.HalfEdges.Count == 0);
            Assert.IsTrue(diagram.Faces.Count == 0);
        }

        [TestMethod]
        public void OnePointsTest()
        {
            var diagram = VoronoiDiagram.Fortune(new Point2D[1]
            {
                new Point2D(5, 5)
            });

            Assert.IsTrue(diagram.Vertices.Count == 0);
            Assert.IsTrue(diagram.HalfEdges.Count == 0);
            Assert.IsTrue(diagram.Faces.Count == 1);

            Assert.IsTrue(diagram.Faces[0] == null);
        }

        [TestMethod]
        public void TwoPointsTest()
        {
            var diagram = VoronoiDiagram.Fortune(new Point2D[2]
            {
                new Point2D(5, 5),
                new Point2D(5, 10)
            });

            Sort(diagram);

            Assert.IsTrue(diagram.Vertices.Count == 0);
            Assert.IsTrue(diagram.HalfEdges.Count == 2);
            Assert.IsTrue(diagram.Faces.Count == 2);

            Assert.IsTrue(diagram.HalfEdges[0].LeftIndex == 0);
            Assert.IsTrue(diagram.HalfEdges[0].RightIndex == 1);
            Assert.IsTrue(diagram.HalfEdges[0].vertex == null);
            Assert.IsTrue(diagram.HalfEdges[0].twin == diagram.HalfEdges[1]);
            Assert.IsTrue(diagram.HalfEdges[0].prev == null);
            Assert.IsTrue(diagram.HalfEdges[0].next == null);

            Assert.IsTrue(diagram.HalfEdges[1].LeftIndex == 1);
            Assert.IsTrue(diagram.HalfEdges[1].RightIndex == 0);
            Assert.IsTrue(diagram.HalfEdges[1].vertex == null);
            Assert.IsTrue(diagram.HalfEdges[1].twin == diagram.HalfEdges[0]);
            Assert.IsTrue(diagram.HalfEdges[1].prev == null);
            Assert.IsTrue(diagram.HalfEdges[1].next == null);

            Assert.IsTrue(diagram.Faces[0] == diagram.HalfEdges[0]);
            Assert.IsTrue(diagram.Faces[1] == diagram.HalfEdges[1]);
        }

        [TestMethod]
        public void ThreePointsTest()
        {
            var diagram = VoronoiDiagram.Fortune(new Point2D[3]
            {
                new Point2D(5, 5),
                new Point2D(-5, 5),
                new Point2D(0, 0)
            });

            Sort(diagram);

            Assert.IsTrue(diagram.Vertices.Count == 1);
            Assert.IsTrue(diagram.HalfEdges.Count == 6);
            Assert.IsTrue(diagram.Faces.Count == 3);

            Assert.IsTrue(diagram.Vertices[0].Point == new Point2D(0, 5));

            Assert.IsTrue(diagram.HalfEdges[0].LeftIndex == 0);
            Assert.IsTrue(diagram.HalfEdges[0].RightIndex == 1);
            Assert.IsTrue(diagram.HalfEdges[0].vertex == diagram.Vertices[0]);
            Assert.IsTrue(diagram.HalfEdges[0].twin == diagram.HalfEdges[1]);
            Assert.IsTrue(diagram.HalfEdges[0].prev == null);
            Assert.IsTrue(diagram.HalfEdges[0].next == diagram.HalfEdges[2]);

            Assert.IsTrue(diagram.HalfEdges[1].LeftIndex == 1);
            Assert.IsTrue(diagram.HalfEdges[1].RightIndex == 0);
            Assert.IsTrue(diagram.HalfEdges[1].vertex == null);
            Assert.IsTrue(diagram.HalfEdges[1].twin == diagram.HalfEdges[0]);
            Assert.IsTrue(diagram.HalfEdges[1].prev == diagram.HalfEdges[4]);
            Assert.IsTrue(diagram.HalfEdges[1].next == null);

            Assert.IsTrue(diagram.HalfEdges[2].LeftIndex == 0);
            Assert.IsTrue(diagram.HalfEdges[2].RightIndex == 2);
            Assert.IsTrue(diagram.HalfEdges[2].vertex == null);
            Assert.IsTrue(diagram.HalfEdges[2].twin == diagram.HalfEdges[3]);
            Assert.IsTrue(diagram.HalfEdges[2].prev == diagram.HalfEdges[0]);
            Assert.IsTrue(diagram.HalfEdges[2].next == null);

            Assert.IsTrue(diagram.HalfEdges[3].LeftIndex == 2);
            Assert.IsTrue(diagram.HalfEdges[3].RightIndex == 0);
            Assert.IsTrue(diagram.HalfEdges[3].vertex == diagram.Vertices[0]);
            Assert.IsTrue(diagram.HalfEdges[3].twin == diagram.HalfEdges[2]);
            Assert.IsTrue(diagram.HalfEdges[3].prev == null);
            Assert.IsTrue(diagram.HalfEdges[3].next == diagram.HalfEdges[5]);

            Assert.IsTrue(diagram.HalfEdges[4].LeftIndex == 1);
            Assert.IsTrue(diagram.HalfEdges[4].RightIndex == 2);
            Assert.IsTrue(diagram.HalfEdges[4].vertex == diagram.Vertices[0]);
            Assert.IsTrue(diagram.HalfEdges[4].twin == diagram.HalfEdges[5]);
            Assert.IsTrue(diagram.HalfEdges[4].prev == null);
            Assert.IsTrue(diagram.HalfEdges[4].next == diagram.HalfEdges[1]);

            Assert.IsTrue(diagram.HalfEdges[5].LeftIndex == 2);
            Assert.IsTrue(diagram.HalfEdges[5].RightIndex == 1);
            Assert.IsTrue(diagram.HalfEdges[5].vertex == null);
            Assert.IsTrue(diagram.HalfEdges[5].twin == diagram.HalfEdges[4]);
            Assert.IsTrue(diagram.HalfEdges[5].prev == diagram.HalfEdges[3]);
            Assert.IsTrue(diagram.HalfEdges[5].next == null);

            Assert.IsTrue(diagram.Faces[0] == diagram.HalfEdges[0]);
            Assert.IsTrue(diagram.Faces[1] == diagram.HalfEdges[4]);
            Assert.IsTrue(diagram.Faces[2] == diagram.HalfEdges[3]);
        }

        [TestMethod]
        public void NinePointsTest()
        {
            const double epsilon = 1e-14;

            var diagram = VoronoiDiagram.Fortune(new Point2D[9]
            {
                new Point2D(-1, -1),
                new Point2D(-1, 1),
                new Point2D(1, 1),
                new Point2D(1, -1),
                new Point2D(0, 2),
                new Point2D(0, 3),
                new Point2D(3, 0),
                new Point2D(-4, 0),
                new Point2D(0, -5)
            });

            Sort(diagram);

            Assert.IsTrue(diagram.Vertices.Count == 12);
            Assert.IsTrue(diagram.HalfEdges.Count == 40);
            Assert.IsTrue(diagram.Faces.Count == 9);

            Assert.IsTrue(diagram.Vertices[0].Point.Equals(new Point2D(-3.59090909090909, -3.77272727272727), epsilon));
            Assert.IsTrue(diagram.Vertices[1].Point.Equals(new Point2D(-3.50000000000000, 3.50000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[2].Point.Equals(new Point2D(-2.33333333333333, 0.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[3].Point.Equals(new Point2D(-1.50000000000000, 2.50000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[4].Point.Equals(new Point2D(0, -2.875), epsilon));
            Assert.IsTrue(diagram.Vertices[5].Point.Equals(new Point2D(-0.00000000000000, -0.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[6].Point.Equals(new Point2D(0.00000000000000, -0.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[7].Point.Equals(new Point2D(-0.00000000000000, 1.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[8].Point.Equals(new Point2D(1.50000000000000, 2.50000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[9].Point.Equals(new Point2D(1.75000000000000, -0.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[10].Point.Equals(new Point2D(3.50000000000000, 3.50000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[11].Point.Equals(new Point2D(3.64285714285714, -3.78571428571429), epsilon));

            //var edges = new (int, int, int?, (int, int)?, (int, int)?)[]
            //{
            //    (0, 1, 2, (0, 2), (0, 7)),
            //    (1, 0, 5, (1, 7), (1, 2)),
            //    (0, 2, 5, (0, 3), (0, 1)),
            //    (2, 0, 6, (2, 1), (2, 3)),
            //    (0, 3, 6, (0, 8), (0, 2)),
            //    (3, 0, 4, (3, 2), (3, 8)),
            //    (0, 7, 0, (0, 1), (0, 8)),
            //    (7, 0, 2, (7, 8), (7, 1)),
            //    (0, 8, 4, (0, 7), (0, 3)),
            //    (8, 0, 0, (8, 3), (8, 7)),
            //    (1, 2, 7, (1, 0), (1, 4)),
            //    (2, 1, 5, (2, 4), (2, 0)),
            //    (1, 4, 3, (1, 2), (1, 5)),
            //    (4, 1, 7, (4, 5), (4, 2)),
            //    (1, 5, 1, (1, 4), (1, 7)),
            //    (5, 1, 3, (5, 7), (5, 4)),
            //    (1, 7, 2, (1, 5), (1, 0)),
            //    (7, 1, 1, (7, 0), (7, 5)),
            //    (2, 3, 9, (2, 0), (2, 6)),
            //    (3, 2, 6, (3, 6), (3, 0)),
            //    (2, 4, 7, (2, 5), (2, 1)),
            //    (4, 2, 8, (4, 1), (4, 5)),
            //    (2, 5, 8, (2, 6), (2, 4)),
            //    (5, 2, 10, (5, 4), (5, 6)),
            //    (2, 6, 10, (2, 3), (2, 5)),
            //    (6, 2, 9, (6, 5), (6, 3)),
            //    (3, 6, 9, (3, 8), (3, 2)),
            //    (6, 3, 11, (6, 2), (6, 8)),
            //    (3, 8, 11, (3, 0), (3, 6)),
            //    (8, 3, 4, (8, 6), (8, 0)),
            //    (4, 5, 3, (4, 2), (4, 1)),
            //    (5, 4, 8, (5, 1), (5, 2)),
            //    (5, 6, null, (5, 2), null),
            //    (6, 5, 10, null, (6, 2)),
            //    (5, 7, 1, null, (5, 1)),
            //    (7, 5, null, (7, 1), null),
            //    (6, 8, null, (6, 3), null),
            //    (8, 6, 11, null, (8, 3)),
            //    (7, 8, 0, null, (7, 0)),
            //    (8, 7, null, (8, 0), null)
            //};

            //for (int i = 0; i < diagram.HalfEdges.Count; i++)
            //{
            //    var (li, ri, vi, previ, nexti) = edges[i];

            //    var vertex = vi == null ? null : diagram.Vertices[vi.Value];
            //    var prev = previ == null ? null : diagram.HalfEdges.Single(e => (e.LeftIndex, e.RightIndex) == previ.Value);
            //    var next = nexti == null ? null : diagram.HalfEdges.Single(e => (e.LeftIndex, e.RightIndex) == nexti.Value);

            //    Assert.IsTrue(diagram.HalfEdges[i].LeftIndex == li);
            //    Assert.IsTrue(diagram.HalfEdges[i].RightIndex == ri);
            //    Assert.IsTrue(diagram.HalfEdges[i].vertex == vertex);
            //    Assert.IsTrue(diagram.HalfEdges[i].twin == diagram.HalfEdges[i ^ 1]);
            //    Assert.IsTrue(diagram.HalfEdges[i].prev == prev);
            //    Assert.IsTrue(diagram.HalfEdges[i].next == next);
            //}

            //Assert.IsTrue(diagram.Faces[0] == diagram.HalfEdges[0]);
            //Assert.IsTrue(diagram.Faces[1] == diagram.HalfEdges[4]);
            //Assert.IsTrue(diagram.Faces[2] == diagram.HalfEdges[3]);
        }

        [TestMethod]
        public void FourPointsTest()
        {
            const double epsilon = 1e-14;

            //var diagram = VoronoiDiagram.Fortune(new Point2D[]
            //{
            //    new Point2D(-2, -2),
            //    new Point2D(3, 3),
            //    new Point2D(0, -6),
            //    new Point2D(2, 5)
            //});

            var diagram = VoronoiDiagram.Fortune(new Point2D[]
            {
                new Point2D(-2, -2),
                new Point2D(3, 3),
                new Point2D(-6, 0),
                new Point2D(5, 2)
            });

            Sort(diagram);

            Assert.IsTrue(diagram.Vertices.Count == 12);
            Assert.IsTrue(diagram.HalfEdges.Count == 40);
            Assert.IsTrue(diagram.Faces.Count == 9);

            Assert.IsTrue(diagram.Vertices[0].Point.Equals(new Point2D(-3.59090909090909, -3.77272727272727), epsilon));
            Assert.IsTrue(diagram.Vertices[1].Point.Equals(new Point2D(-3.50000000000000, 3.50000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[2].Point.Equals(new Point2D(-2.33333333333333, 0.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[3].Point.Equals(new Point2D(-1.50000000000000, 2.50000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[4].Point.Equals(new Point2D(0, -2.875), epsilon));
            Assert.IsTrue(diagram.Vertices[5].Point.Equals(new Point2D(-0.00000000000000, -0.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[6].Point.Equals(new Point2D(0.00000000000000, -0.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[7].Point.Equals(new Point2D(-0.00000000000000, 1.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[8].Point.Equals(new Point2D(1.50000000000000, 2.50000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[9].Point.Equals(new Point2D(1.75000000000000, -0.00000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[10].Point.Equals(new Point2D(3.50000000000000, 3.50000000000000), epsilon));
            Assert.IsTrue(diagram.Vertices[11].Point.Equals(new Point2D(3.64285714285714, -3.78571428571429), epsilon));

            //var edges = new (int, int, int?, (int, int)?, (int, int)?)[]
            //{
            //    (0, 1, 2, (0, 2), (0, 7)),
            //    (1, 0, 5, (1, 7), (1, 2)),
            //    (0, 2, 5, (0, 3), (0, 1)),
            //    (2, 0, 6, (2, 1), (2, 3)),
            //    (0, 3, 6, (0, 8), (0, 2)),
            //    (3, 0, 4, (3, 2), (3, 8)),
            //    (0, 7, 0, (0, 1), (0, 8)),
            //    (7, 0, 2, (7, 8), (7, 1)),
            //    (0, 8, 4, (0, 7), (0, 3)),
            //    (8, 0, 0, (8, 3), (8, 7)),
            //    (1, 2, 7, (1, 0), (1, 4)),
            //    (2, 1, 5, (2, 4), (2, 0)),
            //    (1, 4, 3, (1, 2), (1, 5)),
            //    (4, 1, 7, (4, 5), (4, 2)),
            //    (1, 5, 1, (1, 4), (1, 7)),
            //    (5, 1, 3, (5, 7), (5, 4)),
            //    (1, 7, 2, (1, 5), (1, 0)),
            //    (7, 1, 1, (7, 0), (7, 5)),
            //    (2, 3, 9, (2, 0), (2, 6)),
            //    (3, 2, 6, (3, 6), (3, 0)),
            //    (2, 4, 7, (2, 5), (2, 1)),
            //    (4, 2, 8, (4, 1), (4, 5)),
            //    (2, 5, 8, (2, 6), (2, 4)),
            //    (5, 2, 10, (5, 4), (5, 6)),
            //    (2, 6, 10, (2, 3), (2, 5)),
            //    (6, 2, 9, (6, 5), (6, 3)),
            //    (3, 6, 9, (3, 8), (3, 2)),
            //    (6, 3, 11, (6, 2), (6, 8)),
            //    (3, 8, 11, (3, 0), (3, 6)),
            //    (8, 3, 4, (8, 6), (8, 0)),
            //    (4, 5, 3, (4, 2), (4, 1)),
            //    (5, 4, 8, (5, 1), (5, 2)),
            //    (5, 6, null, (5, 2), null),
            //    (6, 5, 10, null, (6, 2)),
            //    (5, 7, 1, null, (5, 1)),
            //    (7, 5, null, (7, 1), null),
            //    (6, 8, null, (6, 3), null),
            //    (8, 6, 11, null, (8, 3)),
            //    (7, 8, 0, null, (7, 0)),
            //    (8, 7, null, (8, 0), null)
            //};

            //for (int i = 0; i < diagram.HalfEdges.Count; i++)
            //{
            //    var (li, ri, vi, previ, nexti) = edges[i];

            //    var vertex = vi == null ? null : diagram.Vertices[vi.Value];
            //    var prev = previ == null ? null : diagram.HalfEdges.Single(e => (e.LeftIndex, e.RightIndex) == previ.Value);
            //    var next = nexti == null ? null : diagram.HalfEdges.Single(e => (e.LeftIndex, e.RightIndex) == nexti.Value);

            //    Assert.IsTrue(diagram.HalfEdges[i].LeftIndex == li);
            //    Assert.IsTrue(diagram.HalfEdges[i].RightIndex == ri);
            //    Assert.IsTrue(diagram.HalfEdges[i].vertex == vertex);
            //    Assert.IsTrue(diagram.HalfEdges[i].twin == diagram.HalfEdges[i ^ 1]);
            //    Assert.IsTrue(diagram.HalfEdges[i].prev == prev);
            //    Assert.IsTrue(diagram.HalfEdges[i].next == next);
            //}

            //Assert.IsTrue(diagram.Faces[0] == diagram.HalfEdges[0]);
            //Assert.IsTrue(diagram.Faces[1] == diagram.HalfEdges[4]);
            //Assert.IsTrue(diagram.Faces[2] == diagram.HalfEdges[3]);
        }

        [TestMethod]
        public void Test()
        {
            const double epsilon = 1e-14;

            var diagram = VoronoiDiagram.Fortune(new Point2D[]
            {
                new Point2D(1, 1),
                new Point2D(2, 1),
                new Point2D(3, 1),
                new Point2D(1, 2),
                new Point2D(2, 2),
                new Point2D(3, 2),
            });
        }

        private static void Sort(VoronoiDiagram diagram)
        {
            diagram.Vertices.Sort((v1, v2) =>
            {
                int compare = v1.Point.X.CompareTo(v2.Point.X);
                if (compare != 0)
                    return compare;

                return v1.Point.Y.CompareTo(v2.Point.Y);
            });

            diagram.HalfEdges.Sort((e1, e2) =>
            {
                int min1 = Math.Min(e1.LeftIndex, e1.RightIndex);
                int min2 = Math.Min(e2.LeftIndex, e2.RightIndex);

                int compare = min1 - min2;
                if (compare != 0)
                    return compare;

                int max1 = Math.Max(e1.LeftIndex, e1.RightIndex);
                int max2 = Math.Max(e2.LeftIndex, e2.RightIndex);

                compare = max1 - max2;
                if (compare != 0)
                    return compare;

                return e1.LeftIndex - e2.LeftIndex;
            });
        }
    }
}