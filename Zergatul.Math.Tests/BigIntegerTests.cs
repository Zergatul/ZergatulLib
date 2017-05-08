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
        public void ContructorFromString2_1()
        {
            string number = "100000000000000000000000000000000";
            var bi = new BigInteger(number, 2);
            Assert.IsTrue(bi.ToString() == System.Math.Pow(2, 32).ToString());
        }
    }
}
