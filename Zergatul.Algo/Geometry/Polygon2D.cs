using System;
using System.Collections.Generic;

namespace Zergatul.Algo.Geometry
{
    public readonly struct Polygon2D
    {
        public IReadOnlyList<Point2D> Points { get; }

        public Polygon2D(params Point2D[] points)
        {
            Points = points;

            if (GetSquare() < 0)
                Array.Reverse(points);
        }

        public double GetSquare()
        {
            double square = 0;
            foreach (var (p1, p2) in EnumerateEdges())
                square += (p1.X - p2.X) * (p1.Y + p2.Y) / 2;

            return square;
        }

        public bool IsInside(Point2D point)
        {
            foreach (var (p1, p2) in EnumerateEdges())
            {
                var line = new Line2D(p1, p2);
                double val = line.A * point.X + line.B * point.Y + line.C;
                if (val <= 0)
                    return false;
            }

            return true;
        }

        public bool IsInsideOrEdge(Point2D point)
        {
            foreach (var (p1, p2) in EnumerateEdges())
            {
                var line = new Line2D(p1, p2);
                double val = line.A * point.X + line.B * point.Y + line.C;
                if (val < 0)
                    return false;
            }

            return true;
        }

        private IEnumerable<(Point2D, Point2D)> EnumerateEdges()
        {
            for (int i = 1; i < Points.Count; i++)
                yield return (Points[i - 1], Points[i]);
            yield return (Points[^1], Points[0]);
        }
    }
}