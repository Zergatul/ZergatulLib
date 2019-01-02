using System;

namespace Zergatul.Security
{
    public abstract class BigInteger : IEquatable<BigInteger>, IComparable<BigInteger>, IEquatable<int>, IComparable<int>, IEquatable<long>, IComparable<long>, IDisposable
    {
        protected bool _disposed;

        public abstract BigInteger Add(BigInteger other);
        public abstract BigInteger Sub(BigInteger other);
        public abstract BigInteger Mult(BigInteger other);
        public abstract BigInteger Div(BigInteger other);
        public abstract BigInteger Mod(BigInteger other);
        public abstract void Div(BigInteger other, out BigInteger quotient, out BigInteger remainder);
        public abstract BigInteger ModInv(BigInteger other);
        public abstract BigInteger ModDiv(BigInteger other, BigInteger modulus);
        public abstract BigInteger ModExp(BigInteger other, BigInteger modulus);
        public abstract byte[] ToBytes(ByteOrder order = ByteOrder.BigEndian);
        public abstract byte[] ToBytes(int length, ByteOrder order = ByteOrder.BigEndian);
        public override string ToString() => ToString(10);
        public abstract string ToString(int radix);
        public abstract string ToString(int radix, char[] symbols);

        public abstract bool Equals(BigInteger other);
        public abstract int CompareTo(BigInteger other);
        public abstract bool Equals(int other);
        public abstract int CompareTo(int other);
        public abstract bool Equals(long other);
        public abstract int CompareTo(long other);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        ~BigInteger()
        {
            Dispose(false);
        }
    }
}