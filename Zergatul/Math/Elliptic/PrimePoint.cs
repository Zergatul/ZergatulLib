using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.Elliptic
{
    public class PrimePoint : Point, IEquatable<PrimePoint>
    {
        public static readonly PrimePoint Zero = new PrimePoint();
        public static readonly PrimePoint Infinity = new PrimePoint();

        public BigInteger ValueX { get; private set; }
        public BigInteger ValueY { get; private set; }

        private PrimePoint()
        {

        }

        public PrimePoint(byte[] x, byte[] y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            this.x = x;
            this.y = y;

            this.ValueX = new BigInteger(x, ByteOrder.BigEndian);
            this.ValueY = new BigInteger(y, ByteOrder.BigEndian);
        }

        public bool Equals(PrimePoint other)
        {
            if (this == other)
                return true;
            if (other == null)
                return false;
            if (this == Zero && other != Zero)
                return false;
            if (other == Zero && this != Zero)
                return false;
            if (this == Infinity && other != Infinity)
                return false;
            if (other == Infinity && this != Infinity)
                return false;

            return ByteArray.Equals(this.x, other.x) && ByteArray.Equals(this.y, other.y);
        }
    }
}