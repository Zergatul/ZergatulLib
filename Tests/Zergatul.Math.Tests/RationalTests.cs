using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Zergatul.Math.Tests
{
    using RationalLong = Rational<long>;
    using BigInt = System.Numerics.BigInteger;
    using RationalBigInt = Rational<System.Numerics.BigInteger>;

    [TestClass]
    public class RationalTests
    {
        [TestInitialize]
        public void Init()
        {
            IntegerCalculator<long>.Instance = new LongIntegerCalculator();
            IntegerCalculator<BigInt>.Instance = new BigIntegerCalculator();
        }

        [TestMethod]
        public void ContructorTest1()
        {
            Assert.IsTrue((double)new RationalLong(0) == 0d);
            Assert.IsTrue((double)new RationalLong(-1) == -1d);
            Assert.IsTrue((double)new RationalLong(1) == 1d);
            Assert.IsTrue((double)new RationalLong(1, 2) == 0.5d);
            Assert.IsTrue((double)new RationalLong(-1, 2) == -0.5d);
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void ContructorTest2()
        {
            new RationalLong(0, 0);
        }

        [TestMethod]
        public void ContructorTest3()
        {
            Assert.IsTrue(new RationalLong(0) == new RationalLong(0, 100));
            Assert.IsTrue(new RationalLong(-1) == new RationalLong(100, -100));
            Assert.IsTrue(new RationalLong(1, 2) == new RationalLong(100, 200));
            Assert.IsTrue(new RationalLong(2, -6) == new RationalLong(-4, 12));
        }

        [TestMethod]
        public void DivideTest1()
        {
            RationalBigInt value = 1;
            for (int i = 2; i <= 100; i++)
                value /= i;

            var d = (double)value;
            Assert.IsTrue(d.ToString("E5") == "1.07151E-158");

            for (int i = 2; i <= 100; i++)
                value *= i;

            Assert.IsTrue(value == 1);
        }
    }

    public class BigIntegerCalculator : IntegerCalculator<BigInt>
    {
        public override BigInt Zero => BigInt.Zero;
        public override BigInt One => BigInt.One;

        public override BigInt FromInt(int value) => value;
        public override BigInt FromLong(long value) => value;
        public override double ToDouble(BigInt value) => (double)value;

        public override BigInt Negate(BigInt value) => -value;
        public override BigInt Add(BigInt value1, BigInt value2) => value1 + value2;
        public override BigInt Substract(BigInt value1, BigInt value2) => value1 - value2;
        public override BigInt Multiply(BigInt value1, BigInt value2) => value1 * value2;
        public override BigInt Divide(BigInt value1, BigInt value2) => value1 / value2;
        public override BigInt Modulo(BigInt value1, BigInt value2) => value1 % value2;
    }
}