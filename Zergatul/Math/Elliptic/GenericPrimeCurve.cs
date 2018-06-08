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
        public BigInteger ValueA { get; private set; }
        public BigInteger ValueB { get; private set; }
        public BigInteger ValuePrime { get; private set; }
        public BigInteger ValueOrder { get; private set; }

        public GenericPrimeCurve(byte[] a, byte[] b, byte[] prime, byte[] order, byte[] gx, byte[] gy, int? nid = null, string name = null, OID oid = null)
            : base(a, b, prime, order, gx, gy, nid, name, oid)
        {
            ValueA = new BigInteger(a, ByteOrder.BigEndian);
            ValueB = new BigInteger(b, ByteOrder.BigEndian);
            ValuePrime = new BigInteger(prime, ByteOrder.BigEndian);
            ValueOrder = new BigInteger(order, ByteOrder.BigEndian);

            BitSize = ValuePrime.BitSize;
        }

        public override PrimePoint Sum(PrimePoint point1, PrimePoint point2)
        {
            if (point1 == null)
                throw new ArgumentNullException(nameof(point1));
            if (point2 == null)
                throw new ArgumentNullException(nameof(point2));

            if (point1 == PrimePoint.Infinity)
                return point2;
            if (point2 == PrimePoint.Infinity)
                return point1;

            BigInteger λ;
            if (point1.Equals(point2))
                λ = BigInteger.ModularDivision(3 * point1.ValueX * point1.ValueX + ValueA, 2 * point1.ValueY, ValuePrime);
            else
                λ = BigInteger.ModularDivision(point2.ValueY - point1.ValueY, point2.ValueX - point1.ValueX, ValuePrime);

            var x3 = (λ * λ - point1.ValueX - point2.ValueX) % ValuePrime;
            var y3 = (λ * (point1.ValueX - x3) - point1.ValueY) % ValuePrime;

            return new PrimePoint(x3.ToBytes(ByteOrder.BigEndian, (BitSize + 7) / 8), y3.ToBytes(ByteOrder.BigEndian, (BitSize + 7) / 8));
        }

        public override PrimePoint Multiplication(byte[] value) => Multiplication(G, value);

        public override PrimePoint Multiplication(PrimePoint point, byte[] value)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            PrimePoint n = point;
            PrimePoint q = PrimePoint.Zero;

            int len = value.Length;
            BigInteger m = new BigInteger(value, ByteOrder.BigEndian);
            m %= ValueOrder;
            int size = m.BitSize;
            for (int i = 0; i < size; i++)
            {
                if (m.IsBitSet(i))
                    q = q == PrimePoint.Zero ? n : Sum(q, n);
                n = Double(n);
            }

            return q;
        }

        public override PrimePoint Double(PrimePoint point) => Sum(point, point);
    }
}