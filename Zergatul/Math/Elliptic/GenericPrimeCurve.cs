using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;

namespace Zergatul.Math.Elliptic
{
    public class GenericPrimeCurve : PrimeCurve
    {
        private BigInteger _a, _b, _prime, _order;

        public GenericPrimeCurve(byte[] a, byte[] b, byte[] prime, byte[] order, byte[] gx, byte[] gy, int? nid = null, string name = null, OID oid = null)
            : base(a, b, prime, order, gx, gy, nid, name, oid)
        {
            this._a = new BigInteger(a, ByteOrder.BigEndian);
            this._b = new BigInteger(b, ByteOrder.BigEndian);
            this._prime = new BigInteger(prime, ByteOrder.BigEndian);
            this._order = new BigInteger(order, ByteOrder.BigEndian);
        }

        public override PrimePoint Multiplication(byte[] value)
        {
            /*ECPoint n = p;
            ECPoint q = null;
            int bitSize = m.BitSize;
            for (int i = 0; i < bitSize; i++)
            {
                if (m.IsBitSet(i))
                    q = ReferenceEquals(q, null) ? n : Sum(q, n);
                n = n + n;
            }
            return q;*/
            return null;
        }

        public override PrimePoint Multiplication(PrimePoint point, byte[] value)
        {
            throw new NotImplementedException();
        }

        public override PrimePoint Double(PrimePoint point)
        {
            throw new NotImplementedException();
        }
    }
}