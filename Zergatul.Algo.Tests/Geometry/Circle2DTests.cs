using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zergatul.Algo.Geometry;

namespace Zergatul.Algo.Tests.Geometry
{
    [TestClass]
    public class Circle2DTests
    {
        [TestMethod]
        public void IntersectCircleNoneTest()
        {
            const double epsilon = 1e-10;

            void test(double x1, double y1, double r1, double x2, double y2, double r2)
            {
                double arcFrom, arcTo;
                Assert.IsTrue(Circle(x1, y1, r1).Intersect(Circle(x2, y2, r2), epsilon, out arcFrom, out arcTo) == Circle2D.IntersectResult.None);
                Assert.IsTrue(Circle(x2, y2, r2).Intersect(Circle(x1, y1, r1), epsilon, out arcFrom, out arcTo) == Circle2D.IntersectResult.None);
            }

            test(5, 5, 2, 2, 2, 2);
            test(5, 5, 2, 3, 2, 1.5);
            test(1, 0, 2, 5, 2, 2.4);
            test(1, 0, 2, 5, 0, 1.99);
            test(1, 0, 2, 1, 5, 2.9);
        }

        [TestMethod]
        public void IntersectCirclePointTest()
        {
            const double epsilon = 1e-10;

            bool anglesEquals(double a1, double a2)
            {
                return (a1 - a2) % Math.PI == 0;
            }

            void test(double x1, double y1, double r1, double x2, double y2, double r2, double arc1, double arc2)
            {
                double arcFrom, arcTo;

                Assert.IsTrue(Circle(x1, y1, r1).Intersect(Circle(x2, y2, r2), epsilon, out arcFrom, out arcTo) == Circle2D.IntersectResult.Point);
                Assert.IsTrue(anglesEquals(arcFrom, arc1));
                Assert.IsTrue(anglesEquals(arcTo, arc1));

                Assert.IsTrue(Circle(x2, y2, r2).Intersect(Circle(x1, y1, r1), epsilon, out arcFrom, out arcTo) == Circle2D.IntersectResult.Point);
                Assert.IsTrue(anglesEquals(arcFrom, arc1));
                Assert.IsTrue(anglesEquals(arcTo, arc2));
            }

            test(3, 3, 1, 5, 3, 1, 0, -Math.PI);
            test(3, 3, 1, 3, 5, 1, Math.PI / 2, -Math.PI / 2);
            test(3, 3, 3, 3, 5, 1, Math.PI / 2, Math.PI / 2);
            test(-3, -3, 5, 5, 3, 5, Math.Atan2(3, 4), Math.Atan2(-3, -4));
            test(-6, -6, 10, -2, -3, 5, Math.Atan2(3, 4), Math.Atan2(3, 4));
        }

        [TestMethod]
        public void IntersectCircleTwoPointsTest()
        {
            const double epsilon = 1e-10;

            bool anglesEquals((double, double) actual, (double, double) expected)
            {
                var (f1, t1) = actual;
                var (f2, t2) = expected;

                if (t1 < f1)
                    return false;

                if (t2 < f2)
                    return false;

                if ((f1 - f2) % Math.PI != 0)
                    return false;

                return Math.Abs((t1 - f1) - (t2 - f2)) < 1e-12;
            }

            void test(double x1, double y1, double r1, double x2, double y2, double r2, (double, double) arc1, (double, double) arc2)
            {
                double arcFrom, arcTo;

                Assert.IsTrue(Circle(x1, y1, r1).Intersect(Circle(x2, y2, r2), epsilon, out arcFrom, out arcTo) == Circle2D.IntersectResult.Arc);
                Assert.IsTrue(anglesEquals((arcFrom, arcTo), arc1));

                Assert.IsTrue(Circle(x2, y2, r2).Intersect(Circle(x1, y1, r1), epsilon, out arcFrom, out arcTo) == Circle2D.IntersectResult.Arc);
                Assert.IsTrue(anglesEquals((arcFrom, arcTo), arc2));
            }

            test(-6, -6, 10, 2, 5, 5, (0.64350110879328437, 1.240498971965643), (-2.8283888996257627, -Math.PI / 2));
            test(-6, -6, 10, 2, 5, 23, (-1.7416451720282431, 3.6256452527871703), (-2.3930173202193079, -2.0061679062013513));
            test(-6, -6, 10, -6, 6, 20, (-0.70758443672535554, 3.8491770903151488), (-1.9605570595923714, -1.1810355939974218));
        }

        [TestMethod]
        public void IntersectSameCircleTest()
        {
            const double epsilon = 1e-10;
            Assert.IsTrue(Circle(6, 6, 10).Intersect(Circle(6, 6, 10), epsilon, out double arcFrom, out double arcTo) == Circle2D.IntersectResult.Arc);
            Assert.IsTrue(arcFrom == 0);
            Assert.IsTrue(arcTo == 2 * Math.PI);
        }

        [TestMethod]
        public void IntersectInsideTest()
        {
            const double epsilon = 1e-10;

            void test(double x1, double y1, double r1, double x2, double y2, double r2)
            {
                double arcFrom, arcTo;
                Assert.IsTrue(Circle(x1, y1, r1).Intersect(Circle(x2, y2, r2), epsilon, out arcFrom, out arcTo) == Circle2D.IntersectResult.Inside);
                Assert.IsTrue(Circle(x2, y2, r2).Intersect(Circle(x1, y1, r1), epsilon, out arcFrom, out arcTo) == Circle2D.IntersectResult.Outside);
            }

            test(-6, -3, 10, -6, 6, 20);
            test(1, 0, 10, -6, 6, 20);
            test(3, 6, 10, -6, 6, 20);
        }

        private static Circle2D Circle(double x, double y, double r) => new Circle2D(new Point2D(x, y), r);
    }
}