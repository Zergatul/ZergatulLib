using System;

namespace Zergatul.Math
{
    public abstract class IntegerCalculator<T>
        where T : IEquatable<T>, IComparable<T>
    {
        public static IntegerCalculator<T> Instance { get; set; }

        public abstract T Zero { get; }
        public abstract T One { get; }

        public abstract T FromInt(int value);
        public abstract T FromLong(long value);
        public abstract double ToDouble(T value);

        public abstract T Negate(T value);
        public abstract T Add(T value1, T value2);
        public abstract T Substract(T value1, T value2);
        public abstract T Multiply(T value1, T value2);
        public abstract T Divide(T value1, T value2);
        public abstract T Modulo(T value1, T value2);
    }

    public class LongIntegerCalculator : IntegerCalculator<long>
    {
        public override long Zero => 0;
        public override long One => 1;

        public override long FromInt(int value) => value;
        public override long FromLong(long value) => value;
        public override double ToDouble(long value) => value;

        public override long Negate(long value) => checked(-value);
        public override long Add(long value1, long value2) => checked(value1 + value2);
        public override long Substract(long value1, long value2) => checked(value1 - value2);
        public override long Multiply(long value1, long value2) => checked(value1 * value2);
        public override long Divide(long value1, long value2) => checked(value1 / value2);
        public override long Modulo(long value1, long value2) => checked(value1 % value2);
    }

    //public class BigIntegerCalculator : IntegerCalculator<BigInteger>
    //{
    //    public override BigInteger Zero => BigInteger.Zero;
    //    public override BigInteger One => BigInteger.One;

    //    public override BigInteger FromInt(int value) => value;
    //    public override BigInteger FromLong(long value) => value;
    //    public override double ToDouble(BigInteger value) => (double)value;

    //    public override BigInteger Negate(BigInteger value) => -value;
    //    public override BigInteger Add(BigInteger value1, BigInteger value2) => value1 + value2;
    //    public override BigInteger Substract(BigInteger value1, BigInteger value2) => value1 - value2;
    //    public override BigInteger Multiply(BigInteger value1, BigInteger value2) => value1 * value2;
    //    public override BigInteger Divide(BigInteger value1, BigInteger value2) => value1 / value2;
    //    public override BigInteger Modulo(BigInteger value1, BigInteger value2) => value1 % value2;
    //}
}