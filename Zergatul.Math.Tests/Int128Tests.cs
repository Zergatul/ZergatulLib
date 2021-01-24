using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.Tests
{
    [TestClass]
    public class Int128Tests
    {
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void ConstructorLong1()
        {
            new UInt128(-1);
        }

        [TestMethod]
        public void Add1()
        {
            var result = new UInt128(0, uint.MaxValue, uint.MaxValue, uint.MaxValue) + new UInt128(0, 0, 0, 1);
            Assert.IsTrue(result._w1 == 1);
            Assert.IsTrue(result._w2 == 0);
            Assert.IsTrue(result._w3 == 0);
            Assert.IsTrue(result._w4 == 0);
        }

        [TestMethod]
        public void Add2()
        {
            var result = new UInt128(uint.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue) + new UInt128(0, 0, 0, 1);
            Assert.IsTrue(result._w1 == 0);
            Assert.IsTrue(result._w2 == 0);
            Assert.IsTrue(result._w3 == 0);
            Assert.IsTrue(result._w4 == 0);
        }

        [TestMethod]
        public void Sub1()
        {
            var result = new UInt128(0) - new UInt128(0);
            Assert.IsTrue(result._w1 == 0);
            Assert.IsTrue(result._w2 == 0);
            Assert.IsTrue(result._w3 == 0);
            Assert.IsTrue(result._w4 == 0);
        }

        [TestMethod]
        public void Sub2()
        {
            var result = new UInt128(0) - new UInt128(1);
            Assert.IsTrue(result._w1 == uint.MaxValue);
            Assert.IsTrue(result._w2 == uint.MaxValue);
            Assert.IsTrue(result._w3 == uint.MaxValue);
            Assert.IsTrue(result._w4 == uint.MaxValue);
        }

        [TestMethod]
        public void Sub3()
        {
            var result = new UInt128(0) - new UInt128(100) + new UInt128(100);
            Assert.IsTrue(result._w1 == 0);
            Assert.IsTrue(result._w2 == 0);
            Assert.IsTrue(result._w3 == 0);
            Assert.IsTrue(result._w4 == 0);
        }

        [TestMethod]
        public void Sub4()
        {
            var result = new UInt128(1, 0, 0, 0) - new UInt128(1);
            Assert.IsTrue(result._w1 == 0);
            Assert.IsTrue(result._w2 == uint.MaxValue);
            Assert.IsTrue(result._w3 == uint.MaxValue);
            Assert.IsTrue(result._w4 == uint.MaxValue);
        }

        [TestMethod]
        public void Sub5()
        {
            var result = new UInt128(0x7FFFFFFF, 0, 0, 0) - new UInt128(0x7FFFFFFF, 0, 0, 1) + new UInt128(1);
            Assert.IsTrue(result._w1 == 0);
            Assert.IsTrue(result._w2 == 0);
            Assert.IsTrue(result._w3 == 0);
            Assert.IsTrue(result._w4 == 0);
        }

        [TestMethod]
        public void MultUInt64Test1()
        {
            var a = new UInt128(0x432ee3b5, 0x26279359, 0xb1e8ce68, 0xedfd4729);
            var c = a * 0xeb333d4e6f86bda8;
            Assert.IsTrue(c.ToString() == "64601122168941242890299213733887866856");
        }

        [TestMethod]
        public void MultTest1()
        {
            var result = new UInt128(0, 0, 0, 1) * new UInt128(0, 0, 0, 1);
            Assert.IsTrue(result._w1 == 0);
            Assert.IsTrue(result._w2 == 0);
            Assert.IsTrue(result._w3 == 0);
            Assert.IsTrue(result._w4 == 1);
        }

        [TestMethod]
        public void MultTest2()
        {
            var result = new UInt128(0, 0, 1, 0) * new UInt128(0, 0, 1, 0);
            Assert.IsTrue(result._w1 == 0);
            Assert.IsTrue(result._w2 == 1);
            Assert.IsTrue(result._w3 == 0);
            Assert.IsTrue(result._w4 == 0);
        }

        [TestMethod]
        public void MultTest3()
        {
            var result = new UInt128(0, 1000, 0, 0) * new UInt128(0, 0, 1000, 0);
            Assert.IsTrue(result._w1 == 1000000);
            Assert.IsTrue(result._w2 == 0);
            Assert.IsTrue(result._w3 == 0);
            Assert.IsTrue(result._w4 == 0);
        }

        [TestMethod]
        public void MultTest4()
        {
            var value = new UInt128(0, 0, 0xe8136678, 0xc03d579d);
            Assert.IsTrue((value * value).ToString() == "279652792706452397536070523945292273225");
        }

        [TestMethod]
        public void DivUInt1()
        {
            var result = new UInt128(0, 0, 0, 1) / 1;
            Assert.IsTrue(result._w1 == 0);
            Assert.IsTrue(result._w2 == 0);
            Assert.IsTrue(result._w3 == 0);
            Assert.IsTrue(result._w4 == 1);
        }

        [TestMethod]
        public void DivUInt2()
        {
            var result = new UInt128(0x40000000, 0x80000000, 0x80000000, 0x80000000) / 1;
            Assert.IsTrue(result._w1 == 0x40000000);
            Assert.IsTrue(result._w2 == 0x80000000);
            Assert.IsTrue(result._w3 == 0x80000000);
            Assert.IsTrue(result._w4 == 0x80000000);
        }

        [TestMethod]
        public void DivUInt3()
        {
            var result = new UInt128(0x40000000, 0x80000000, 0x80000000, 0x80000000) / 0x40000000;
            Assert.IsTrue(result._w1 == 1);
            Assert.IsTrue(result._w2 == 2);
            Assert.IsTrue(result._w3 == 2);
            Assert.IsTrue(result._w4 == 2);
        }

        [TestMethod]
        public void DivUInt4()
        {
            var result = new UInt128(0x40000000, 0, 0, 0) / 0xF0120050;
            Assert.IsTrue(result._w1 == 0);
            Assert.IsTrue(result._w2 == 0x443F25D7);
            Assert.IsTrue(result._w3 == 0x7ACA2D46);
            Assert.IsTrue(result._w4 == 0xDFD35220);
        }

        [TestMethod]
        public void DivModUInt32Test1()
        {
            (UInt128 quotient, uint remainder) = UInt128.DivMod(new UInt128(0x40000000, 0, 0, 0), 0xF0120050);
            Assert.IsTrue(quotient._w1 == 0);
            Assert.IsTrue(quotient._w2 == 0x443F25D7);
            Assert.IsTrue(quotient._w3 == 0x7ACA2D46);
            Assert.IsTrue(quotient._w4 == 0xDFD35220);
            Assert.IsTrue(remainder == 0x47B65600);

            (quotient, remainder) = UInt128.DivMod(new UInt128(0x40000000, 0, 0, 1), 0xF0120050);
            Assert.IsTrue(quotient._w1 == 0);
            Assert.IsTrue(quotient._w2 == 0x443F25D7);
            Assert.IsTrue(quotient._w3 == 0x7ACA2D46);
            Assert.IsTrue(quotient._w4 == 0xDFD35220);
            Assert.IsTrue(remainder == 0x47B65601);
        }

        [TestMethod]
        public void DivModUInt32Test2()
        {
            (UInt128 quotient, uint remainder) = UInt128.DivMod(new UInt128(1), 1000000000);
            Assert.IsTrue(quotient._w1 == 0);
            Assert.IsTrue(quotient._w2 == 0);
            Assert.IsTrue(quotient._w3 == 0);
            Assert.IsTrue(quotient._w4 == 0);
            Assert.IsTrue(remainder == 1);
        }

        [TestMethod]
        public void DivModUInt64Test1()
        {
            (UInt128 quotient, ulong remainder) = UInt128.DivMod(new UInt128(0x40000000, 0, 0, 0), 0xFFAA0033F0120053);
            Assert.IsTrue(quotient.ToString() == "4617745626341620003");
            Assert.IsTrue(remainder == 8169941719047076263);
        }

        [TestMethod]
        public void DivModUInt64Test2()
        {
            (UInt128 quotient, ulong remainder) = UInt128.DivMod(new UInt128(0x77777777, 0, 0, 0), 0x1000000012345053);
            Assert.IsTrue(quotient.ToString() == "137735689015141587286");
            Assert.IsTrue(remainder == 493238794373933342);
        }

        [TestMethod]
        public void DivModUInt64Test3()
        {
            (UInt128 quotient, ulong remainder) = UInt128.DivMod(new UInt128(0x1554e077, 0x52bbd7aa, 0x24b33b5a, 0x2cc1599d), 0xdbadc0dd75bfb67c);
            Assert.IsTrue(quotient.ToString() == "1791241489515940566");
            Assert.IsTrue(remainder == 5914510596799591925);
        }

        [TestMethod]
        public void DivModUInt64Test4()
        {
            (UInt128 quotient, ulong remainder) = UInt128.DivMod(new UInt128(0x788f9c77, 0xa2337e4f, 0x2cac4da4, 0xac542e32), 0x911a412dc312ab92);
            Assert.IsTrue(quotient.ToString() == "15326797898383882995");
            Assert.IsTrue(remainder == 8269982068987969180);
        }

        [TestMethod]
        public void DivModUInt64Test5()
        {
            (UInt128 quotient, ulong remainder) = UInt128.DivMod(new UInt128(0x019628f9, 0x64e53844, 0x6578c024, 0xf760e934), 0x1ba73d343e343425);
            Assert.IsTrue(quotient.ToString() == "1058352530957364455");
            Assert.IsTrue(remainder == 1964773617093394385);
        }

        [TestMethod]
        public void DivModUInt64Test6()
        {
            (UInt128 quotient, ulong remainder) = UInt128.DivMod(new UInt128(0x44e45c42, 0x45decabb, 0x40892fbe, 0x3c11cc85), 0xe5a9164bd6);
            Assert.IsTrue(quotient.ToString() == "92837260927654870576025560");
            Assert.IsTrue(remainder == 94492083701);
        }

        [TestMethod]
        public void ModPowUInt64Test1()
        {
            Assert.IsTrue(UInt128.ModPow(123456789, 987654321, 4123456789) == 3277405746);
        }

        [TestMethod]
        public void ModPowUInt64Test2()
        {
            Assert.IsTrue(UInt128.ModPow(17446744073709551615, 16446744073709551615, 18446744073709551615) == 17583758942629216265);
        }

        [TestMethod]
        public void ToStringTest()
        {
            Assert.IsTrue(new UInt128(0).ToString() == "0");
            Assert.IsTrue(new UInt128(1).ToString() == "1");
            Assert.IsTrue(new UInt128(10).ToString() == "10");
            Assert.IsTrue(new UInt128(4294967295).ToString() == "4294967295");
            Assert.IsTrue(new UInt128(4294967296).ToString() == "4294967296");
            Assert.IsTrue(new UInt128(0x40000000, 0, 0, 0).ToString() == "85070591730234615865843651857942052864");
        }
    }
}