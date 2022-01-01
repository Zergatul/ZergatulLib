using System;

namespace Zergatul.Algo.Geometry
{
    public readonly struct Vector2D
    {
        public double X { get; }
        public double Y { get; }

        public double Norm => Math.Sqrt(X * X + Y * Y);
        public Vector2D Normalized => this / Norm;
        public Vector2D Rot90Cw => new Vector2D(Y, -X);
        public Vector2D Rot90Ccw => new Vector2D(-Y, X);

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static double CrossProduct(Vector2D v1, Vector2D v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        public static double DotProduct(Vector2D v1, Vector2D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static double DotProduct(Vector2D v, Point2D p)
        {
            return v.X * p.X + v.Y * p.Y;
        }

        #region Operators

        public static Vector2D operator/(Vector2D vector, double value)
        {
            return new Vector2D(vector.X / value, vector.Y / value);
        }

        #endregion
    }
}