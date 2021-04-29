using System;

namespace Zergatul.Algo.Geometry
{
    public readonly struct Line2D : IEquatable<Line2D>
    {
        public double A { get; }
        public double B { get; }
        public double C { get; }

        public Line2D(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Line2D(Point2D p1, Point2D p2)
        {
            A = p1.Y - p2.Y;
            B = p2.X - p1.X;
            C = -A * p1.X - B * p1.Y;
        }

        public Line2D GetOrthogonal(Point2D point)
        {
            return new Line2D(-B, A, B * point.X - A * point.Y);
        }

        public Point2D? Intersect(Line2D line)
        {
            var (a1, b1, c1) = this;
            var (a2, b2, c2) = line;

            var denominator = a1 * b2 - a2 * b1;
            if (denominator == 0)
                return null;

            return new Point2D((b1 * c2 - b2 * c1) / denominator, (a2 * c1 - a1 * c2) / denominator);
        }

        public Point2D GetProjection(Point2D point)
        {
            var orthogonal = GetOrthogonal(point);
            return Intersect(orthogonal).Value;
        }

        public void Deconstruct(out double a, out double b, out double c)
        {
            a = A;
            b = B;
            c = C;
        }

        public string ToDesmos()
        {
            return $"{(double)A}x+{(double)B}y+{(double)C}=0";
        }

        #region Object overrides

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Line2D line:
                    return Equals(line);

                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Interfaces

        public bool Equals(Line2D line)
        {
            var (a1, b1, c1) = this;
            var (a2, b2, c2) = line;

            return a1 * b2 == a2 * b1 && a1 * c2 == a2 * c1;
        }

        #endregion
    }
}