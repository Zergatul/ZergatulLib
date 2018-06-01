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
    }
}