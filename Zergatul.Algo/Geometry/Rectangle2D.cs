using System;

namespace Zergatul.Algo.Geometry
{
    public readonly struct Rectangle2D
    {
        public Point2D P1 { get; }
        public Point2D P2 { get; }

        public Rectangle2D(Point2D p1, Point2D p2)
        {
            P1 = new Point2D(System.Math.Min(p1.X, p2.X), System.Math.Min(p1.Y, p2.Y));
            P2 = new Point2D(System.Math.Max(p1.X, p2.X), System.Math.Max(p1.Y, p2.Y));
        }

        public bool IsInside(Point2D point)
        {
            return P1.X <= point.X && point.X <= P2.X && P1.Y <= point.Y && point.Y <= P2.Y;
        }
    }
}