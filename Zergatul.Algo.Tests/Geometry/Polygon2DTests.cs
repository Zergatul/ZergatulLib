using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Algo.Geometry;

namespace Zergatul.Algo.Tests.Geometry
{
    [TestClass]
    public class Polygon2DTests
    {
        [TestMethod]
        public void ReversePointsTest()
        {
            var polygon = new Polygon2D(
                new Point2D(0, 0),
                new Point2D(10, 0),
                new Point2D(5, 5));

            Assert.IsTrue(polygon.Points[0] == new Point2D(0, 0));

            polygon = new Polygon2D(
                new Point2D(5, 5),
                new Point2D(10, 0),
                new Point2D(0, 0));

            Assert.IsTrue(polygon.Points[0] == new Point2D(0, 0));
        }

        [TestMethod]
        public void IsInsideTest()
        {
            var polygon = new Polygon2D(
                new Point2D(0, 0),
                new Point2D(10, 0),
                new Point2D(5, 5));

            Assert.IsTrue(polygon.IsInside(new Point2D(5, 2)));
        }
    }
}