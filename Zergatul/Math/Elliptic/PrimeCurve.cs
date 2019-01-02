using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;

namespace Zergatul.Math.Elliptic
{
    /// <summary>
    /// y² ≡ x³ + ax + b (mod p)
    /// </summary>
    public abstract class PrimeCurve : Curve<PrimePoint>
    {
        public byte[] Prime { get; protected set; }

        public PrimeCurve(byte[] a, byte[] b, byte[] prime, byte[] order, byte[] gx, byte[] gy, int? nid = null, string name = null, OID oid = null)
        {
            this.A = a;
            this.B = b;
            this.Prime = prime;
            this.Order = order;
            this.G = new PrimePoint(gx, gy);
            this.NID = nid;
            this.Name = name;
            this.OID = oid;
        }
    }
}