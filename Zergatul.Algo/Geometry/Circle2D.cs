using System;

namespace Zergatul.Algo.Geometry
{
    public readonly struct Circle2D
    {
        public Point2D Center { get; }
        public double Radius { get; }

        public Circle2D(Point2D center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Contains(Point2D point)
        {
            double dx = point.X - Center.X;
            double dy = point.Y - Center.Y;
            return dx * dx + dy * dy <= Radius * Radius;
        }

        public IntersectResult Intersect(Circle2D other, double pointEpsilon, out double arcFrom, out double arcTo)
        {
            // x^2 + y^2 = r1^2
            // (x - xc2)^2 + (y - yc2)^2 = r2^2
            // x^2 + y^2 = r1^2
            // x^2 - 2x xc2 + xc2^2 + y^2 - 2y yc2 + yc2^2 = r2^2
            // -
            // 2x xc2 + 2y yc2 = r1^2 - r2^2 + xc2^2 + yc2^2
            // x = (r1^2 - r2^2 + xc2^2 + yc2^2 - 2y yc2) / (2 xc2)

            double r1 = Radius;
            double xc2 = other.Center.X - Center.X;
            double yc2 = other.Center.Y - Center.Y;
            double r2 = other.Radius;
            double e2 = pointEpsilon * pointEpsilon;

            arcFrom = default;
            arcTo = default;

            if (Math.Abs(xc2) > pointEpsilon)
            {
                // x = ay + b
                double a = -yc2 / xc2;
                double b = (r1 * r1 - r2 * r2 + xc2 * xc2 + yc2 * yc2) / (2 * xc2);

                // (ay + b)^2 + y^2 = r1^2
                // (a^2+1) y^2 + 2ab y + b^2 = r1^2

                double d = 4 * a * a * b * b - 4 * (a * a + 1) * (b * b - r1 * r1);
                if (d < -e2)
                {
                    if (Contains(other.Center))
                    {
                        if (r1 < r2)
                            return IntersectResult.Inside;
                        else
                            return IntersectResult.Outside;
                    }
                    return IntersectResult.None;
                }
                else if (d > e2)
                {
                    double ds = Math.Sqrt(d);
                    double y1 = (-2 * a * b - ds) / (2 * (a * a + 1));
                    double y2 = (-2 * a * b + ds) / (2 * (a * a + 1));
                    double x1 = a * y1 + b;
                    double x2 = a * y2 + b;

                    double a1 = Math.Atan2(y1, x1);
                    double a2 = Math.Atan2(y2, x2);

                    // check a1 -> a2
                    double a2n = a2 > a1 ? a2 : a2 + 2 * Math.PI;
                    double mid = (a1 + a2n) / 2;
                    Point2D midp = new Point2D(r1 * Math.Cos(mid), r1 * Math.Sin(mid));
                    if (other.Contains(midp))
                    {
                        arcFrom = a1;
                        arcTo = a2n;
                    }
                    else
                    {
                        arcFrom = a2;
                        arcTo = a1 > a2 ? a1 : a1 + 2 * Math.PI;
                    }
                    return IntersectResult.Arc;
                }
                else
                {
                    double y = -a * b / (a * a + 1);
                    double x = a * y + b;

                    arcFrom = arcTo = Math.Atan2(y, x);

                    return IntersectResult.Point;
                }
            }
            else
            {
                if (Math.Abs(yc2) < pointEpsilon)
                {
                    if (Math.Abs(r1 - r2) < pointEpsilon)
                    {
                        arcFrom = 0;
                        arcTo = 2 * Math.PI;
                        return IntersectResult.Arc;
                    }
                    else
                    {
                        if (r1 < r2)
                            return IntersectResult.Inside;
                        else
                            return IntersectResult.Outside;
                    }
                }

                // x^2 + y^2 = r1^2
                // x^2 + (y - yc2)^2 = r2^2
                // ---
                // x^2 + y^2 = r1^2
                // x^2 + y^2 - 2y yc2 + yc2^2 = r2^2
                // ---
                // 2y yc2 - yc2^2 = r1^2 - r2^2
                // y = (r1^2 - r2^2 + yc2^2) / 2 yc2

                double y = (r1 * r1 - r2 * r2 + yc2 * yc2) / (2 * yc2);
                if (Math.Abs(y) - r1 > pointEpsilon)
                {
                    if (Contains(other.Center))
                    {
                        if (r1 < r2)
                            return IntersectResult.Inside;
                        else
                            return IntersectResult.Outside;
                    }
                    return IntersectResult.None;
                }
                else if (r1 - Math.Abs(y) > pointEpsilon)
                {
                    double x1 = Math.Sqrt(r1 * r1 - y * y);
                    double x2 = -x1;

                    double a1 = Math.Atan2(y, x1);
                    double a2 = Math.Atan2(y, x2);

                    // check a1 -> a2
                    double a2n = a2 > a1 ? a2 : a2 + 2 * Math.PI;
                    double mid = (a1 + a2n) / 2;
                    Point2D midp = new Point2D(r1 * Math.Cos(mid), r1 * Math.Sin(mid));
                    if (other.Contains(midp))
                    {
                        arcFrom = a1;
                        arcTo = a2n;
                    }
                    else
                    {
                        arcFrom = a2;
                        arcTo = a1 > a2 ? a1 : a1 + 2 * Math.PI;
                    }
                    return IntersectResult.Arc;
                }
                else
                {
                    arcFrom = arcTo = Math.Atan2(y, 0);
                    return IntersectResult.Point;
                }
            }
        }

        public enum IntersectResult
        {
            None,
            Inside,
            Outside,
            Point,
            Arc,
            Full
        }
    }
}