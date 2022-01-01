using System;

namespace Zergatul.Algo.Geometry
{
    public readonly struct Point2D : IEquatable<Point2D>
    {
        public double X { get; }
        public double Y { get; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Point2D Center(Point2D p1, Point2D p2)
        {
            return new Point2D(0.5 * (p1.X + p2.X), 0.5 * (p1.Y + p2.Y));
        }

        public double GetDistanceSquaredTo(Point2D point)
        {
            double dx = point.X - X;
            double dy = point.Y - Y;
            return dx * dx + dy * dy;
        }

        public Point2D Mirror(Point2D center)
        {
            double dx = center.X - X;
            double dy = center.Y - Y;

            return new Point2D(center.X + dx, center.Y + dy);
        }

        public Point2D Mirror(Line2D center)
        {
            var orth = center.GetOrthogonal(this);
            var point = orth.Intersect(center);
            if (point == null)
                throw new InvalidOperationException();
            return Mirror(point.Value);
        }

        public bool Equals(Point2D other, double epsilon)
        {
            return Math.Abs(X - other.X) < epsilon && Math.Abs(Y - other.Y) < epsilon;
        }

        #region Object overrides

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Point2D point:
                    return Equals(point);

                default:
                    return false;
            }
        }

        #endregion

        #region Interfaces

        public bool Equals(Point2D other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }

        #endregion

        #region Operators

        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return !p1.Equals(p2);
        }

        public static Vector2D operator -(Point2D p1, Point2D p2)
        {
            return new Vector2D(p1.X - p2.X, p1.Y - p2.Y);
        }

        #endregion
    }
}