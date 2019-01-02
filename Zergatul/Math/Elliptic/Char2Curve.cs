using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.Elliptic
{
    /// <summary>
    /// y² + xy = x³ + ax² + b in F(2ᵐ)
    /// </summary>
    public class Char2Curve : Curve<Char2Point>
    {
        public byte[] f { get; protected set; }

        public override Char2Point Double(Char2Point point)
        {
            throw new NotImplementedException();
        }

        public override Char2Point Multiplication(byte[] value)
        {
            throw new NotImplementedException();
        }

        public override Char2Point Multiplication(Char2Point point, byte[] value)
        {
            throw new NotImplementedException();
        }

        public override Char2Point Sum(Char2Point point1, Char2Point point2)
        {
            throw new NotImplementedException();
        }
    }
}