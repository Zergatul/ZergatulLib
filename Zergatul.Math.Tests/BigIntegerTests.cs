using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Zergatul.Math.Tests
{
    [TestClass]
    public class BigIntegerTests
    {
        [TestMethod]
        public void ConstructorFromBytesBE_1()
        {
            var bi = new BigInteger(new byte[] { 0, 0, 0, 0 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi._bits.Length == 0);
        }

        [TestMethod]
        public void ConstructorFromBytesBE_2()
        {
            var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 2 }).ToArray(), ByteOrder.BigEndian);
            Assert.IsTrue(bi._bits.Length == 1);
            Assert.IsTrue(bi._bits[0] == 2);
        }

        [TestMethod]
        public void ConstructorFromBytesBE_3()
        {
            var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 1, 0, 0, 0, 2 }).ToArray(), ByteOrder.BigEndian);
            Assert.IsTrue(bi._bits.Length == 2);
            Assert.IsTrue(bi._bits[0] == 2);
            Assert.IsTrue(bi._bits[1] == 1);
        }

        [TestMethod]
        public void ConstructorFromBytesBE_4()
        {
            var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 1, 0, 0, 0, 0, 2 }).ToArray(), ByteOrder.BigEndian);
            Assert.IsTrue(bi._bits.Length == 2);
            Assert.IsTrue(bi._bits[0] == 2);
            Assert.IsTrue(bi._bits[1] == 256);
        }

        [TestMethod]
        public void ConstructorFromBytesBE_5()
        {
            var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 1, 0, 0, 0, 0, 0, 2 }).ToArray(), ByteOrder.BigEndian);
            Assert.IsTrue(bi._bits.Length == 2);
            Assert.IsTrue(bi._bits[0] == 2);
            Assert.IsTrue(bi._bits[1] == 256 * 256);
        }

        [TestMethod]
        public void ConstructorFromBytesBE_6()
        {
            var bi = new BigInteger(Enumerable.Repeat((byte)0, 1000).Concat(new byte[] { 1, 0, 0, 0, 0, 0, 0, 2 }).ToArray(), ByteOrder.BigEndian);
            Assert.IsTrue(bi._bits.Length == 2);
            Assert.IsTrue(bi._bits[0] == 2);
            Assert.IsTrue(bi._bits[1] == 256 * 256 * 256);
        }

        [TestMethod]
        public void ConstructorFromBytesLE_1()
        {
            var bi = new BigInteger(new byte[] { 0, 0, 0, 0 }, ByteOrder.LittleEndian);
            Assert.IsTrue(bi._bits.Length == 0);
        }

        [TestMethod]
        public void ConstructorFromBytesLE_2()
        {
            var bi = new BigInteger(new byte[] { 2 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
            Assert.IsTrue(bi._bits.Length == 1);
            Assert.IsTrue(bi._bits[0] == 2);
        }

        [TestMethod]
        public void ConstructorFromBytesLE_3()
        {
            var bi = new BigInteger(new byte[] { 2, 0, 0, 0, 1 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
            Assert.IsTrue(bi._bits.Length == 2);
            Assert.IsTrue(bi._bits[0] == 2);
            Assert.IsTrue(bi._bits[1] == 1);
        }

        [TestMethod]
        public void ConstructorFromBytesLE_4()
        {
            var bi = new BigInteger(new byte[] { 2, 0, 0, 0, 0, 1 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
            Assert.IsTrue(bi._bits.Length == 2);
            Assert.IsTrue(bi._bits[0] == 2);
            Assert.IsTrue(bi._bits[1] == 256);
        }

        [TestMethod]
        public void ConstructorFromBytesLE_5()
        {
            var bi = new BigInteger(new byte[] { 2, 0, 0, 0, 0, 0, 1 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
            Assert.IsTrue(bi._bits.Length == 2);
            Assert.IsTrue(bi._bits[0] == 2);
            Assert.IsTrue(bi._bits[1] == 256 * 256);
        }

        [TestMethod]
        public void ConstructorFromBytesLE_6()
        {
            var bi = new BigInteger(new byte[] { 2, 0, 0, 0, 0, 0, 0, 1 }.Concat(Enumerable.Repeat((byte)0, 1000)).ToArray(), ByteOrder.LittleEndian);
            Assert.IsTrue(bi._bits.Length == 2);
            Assert.IsTrue(bi._bits[0] == 2);
            Assert.IsTrue(bi._bits[1] == 256 * 256 * 256);
        }

        [TestMethod]
        public void ToString10_1()
        {
            var bi = new BigInteger(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString() == "18446744073709551616");
        }

        [TestMethod]
        public void ToString10_2()
        {
            var bi = new BigInteger(new byte[] { 1, 0, 0, 0, 0 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString() == "4294967296");
        }

        [TestMethod]
        public void ToString2_1()
        {
            var bi = new BigInteger(new byte[] { 255, 0, 0, 0, 0, 255 }, ByteOrder.BigEndian);
            Assert.IsTrue(bi.ToString(2) == "111111110000000000000000000000000000000011111111");
        }

        [TestMethod]
        public void ContructorFromString_Zero()
        {
            string number = new string('0', 1000);
            for (int radix = 2; radix <= 36; radix++)
                Assert.IsTrue(new BigInteger(number, radix) == BigInteger.Zero);
        }

        [TestMethod]
        public void ContructorFromString10_1()
        {
            string number = "1234567890";
            var bi = new BigInteger(number);
            Assert.IsTrue(bi.ToString() == number);
        }

        [TestMethod]
        public void ContructorFromString10_2()
        {
            string number = "1" + new string('5', 20000);
            var bi = new BigInteger(number);
            Assert.IsTrue(bi.ToString() == number);
        }

        [TestMethod]
        public void ContructorFromString2_1()
        {
            string number = "100000000000000000000000000000000";
            var bi = new BigInteger(number, 2);
            Assert.IsTrue(bi.ToString() == System.Math.Pow(2, 32).ToString());
        }

        [TestMethod]
        public void ContructorFromString16_1()
        {
            string number = "FFFFFFFFFFFF";
            var bi = new BigInteger(number, 16);

            Assert.IsTrue(bi._bits.Length == 2);
            Assert.IsTrue(bi._bits[0] == 0xFFFFFFFF);
            Assert.IsTrue(bi._bits[1] == 0xFFFF);
        }

        [TestMethod]
        public void Equals_1()
        {
            Assert.IsTrue(new BigInteger(1) == new BigInteger(1));
        }

        [TestMethod]
        public void Equals_2()
        {
            Assert.IsTrue(new BigInteger(1) != BigInteger.Zero);
        }

        [TestMethod]
        public void Equals_3()
        {
            int zeros = 10000;
            var bi1 = new BigInteger("1" + new string('0', zeros));
            var bi2 = new BigInteger("1" + new string('0', zeros));
            Assert.IsTrue(bi1 == bi2);
        }

        [TestMethod]
        public void Equals_4()
        {
            int zeros = 10000;
            var bi1 = new BigInteger("1" + new string('0', zeros));
            var bi2 = new BigInteger("1" + new string('0', zeros - 1) + "1");
            Assert.IsTrue(bi1 != bi2);
        }

        [TestMethod]
        public void Add_1()
        {
            Assert.IsTrue(BigInteger.Zero + BigInteger.Zero == BigInteger.Zero);
        }

        [TestMethod]
        public void Add_2()
        {
            Assert.IsTrue(BigInteger.Zero + BigInteger.One == BigInteger.One);
        }

        [TestMethod]
        public void Add_3()
        {
            int zeros = 50000;
            string number = "9" + new string('0', zeros);
            var bi = new BigInteger(number);
            BigInteger result = BigInteger.Zero;
            for (int i = 0; i < 1000; i++)
                result = result + bi;

            Assert.IsTrue(result.ToString() == number + "000");
        }

        [TestMethod]
        public void CompareTo_1()
        {
            Assert.IsTrue(BigInteger.Zero.CompareTo(BigInteger.Zero) == 0);
        }

        [TestMethod]
        public void CompareTo_2()
        {
            Assert.IsTrue(new BigInteger(123).CompareTo(new BigInteger(124)) < 0);
        }

        [TestMethod]
        public void CompareTo_3()
        {
            Assert.IsTrue(new BigInteger(123).CompareTo(new BigInteger(122)) > 0);
        }

        [TestMethod]
        public void CompareTo_4()
        {
            string hex1 = "1" + new string('F', 200000) + "A";
            string hex2 = "1" + new string('F', 200000) + "B";
            Assert.IsTrue(new BigInteger(hex1, 16).CompareTo(new BigInteger(hex2, 16)) < 0);
            Assert.IsTrue(new BigInteger(hex2, 16).CompareTo(new BigInteger(hex1, 16)) > 0);
        }

        [TestMethod]
        public void Multiply_1()
        {
            Assert.IsTrue(new BigInteger(long.MaxValue) * BigInteger.Zero == BigInteger.Zero);
            Assert.IsTrue(BigInteger.Zero * new BigInteger(long.MaxValue) == BigInteger.Zero);
        }

        [TestMethod]
        public void Multiply_2()
        {
            Assert.IsTrue(new BigInteger(long.MaxValue) * BigInteger.One == new BigInteger(long.MaxValue));
            Assert.IsTrue(BigInteger.One * new BigInteger(long.MaxValue) == new BigInteger(long.MaxValue));
        }

        [TestMethod]
        public void Multiply_3()
        {
            string dec1 = "7" + new string('5', 300);
            string dec2 = "1" + new string('0', 50);
            string result = "7" + new string('5', 300) + new string('0', 50);
            Assert.IsTrue(new BigInteger(dec1) * new BigInteger(dec2) == new BigInteger(result));
        }

        [TestMethod]
        public void Multiply_4()
        {
            string hex1 = "1" + new string('0', 8);
            string hex2 = "2" + new string('0', 8);
            string result = "2" + new string('0', 16);
            Assert.IsTrue(new BigInteger(hex1, 16) * new BigInteger(hex2, 16) == new BigInteger(result, 16));
        }

        [TestMethod]
        public void Multiply_5()
        {
            Assert.IsTrue(new BigInteger("08396905339595156620") * new BigInteger("13568775824401428918") == new BigInteger("113935726171686031717971661160007137160"));
        }

        [TestMethod]
        public void Division_1()
        {
            var result = BigInteger.One.Division(new BigInteger(2));
            Assert.IsTrue(result.Item1.IsZero);
            Assert.IsTrue(result.Item2.ToString() == "1");
        }

        [TestMethod]
        public void Division_2()
        {
            var bi1 = new BigInteger(Enumerable.Repeat((byte)244, 500).ToArray(), ByteOrder.BigEndian);
            var bi2 = new BigInteger(Enumerable.Repeat((byte)245, 500).ToArray(), ByteOrder.BigEndian);
            var result = bi1.Division(bi2);
            Assert.IsTrue(result.Item1.IsZero);
            Assert.IsTrue(result.Item2 == bi1);
        }
    }
}
