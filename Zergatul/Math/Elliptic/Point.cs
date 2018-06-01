using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.Elliptic
{
    public abstract class Point
    {
        public byte[] x { get; protected set; }
        public byte[] y { get; protected set; }
    }
}