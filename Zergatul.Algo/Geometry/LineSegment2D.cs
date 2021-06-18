using System;

namespace Zergatul.Algo.Geometry
{
    public readonly struct LineSegment2D
    {
        public Point2D P1 { get; }
        public Point2D P2 { get; }

        public LineSegment2D(Point2D p1, Point2D p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public double GetDistanceSquaredTo(Point2D point)
        {
            var line = ToLine();
            var proj = line.GetProjection(point);
            if (new Rectangle2D(P1, P2).IsInside(proj))
                return point.GetDistanceSquaredTo(proj);
            else
                return System.Math.Min(P1.GetDistanceSquaredTo(point), P2.GetDistanceSquaredTo(point));
        }

        public Line2D ToLine()
        {
            return new Line2D(P1, P2);
        }
    }
}