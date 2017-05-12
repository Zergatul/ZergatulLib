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
        public void ConstructorLong1()
        {
            var result = new Int128(-1);
            Assert.IsTrue(result._high1 == -1);
            Assert.IsTrue(result._high2 == uint.MaxValue);
            Assert.IsTrue(result._low1 == uint.MaxValue);
            Assert.IsTrue(result._low2 == uint.MaxValue);
        }

        [TestMethod]
        public void Add1()
        {
            var result = new Int128(0, uint.MaxValue, uint.MaxValue, uint.MaxValue) + new Int128(0, 0, 0, 1);
            Assert.IsTrue(result._high1 == 1);
            Assert.IsTrue(result._high2 == 0);
            Assert.IsTrue(result._low1 == 0);
            Assert.IsTrue(result._low2 == 0);
        }

        [TestMethod]
        public void Add2()
        {
            var result = new Int128(int.MaxValue, uint.MaxValue, uint.MaxValue, uint.MaxValue) + new Int128(0, 0, 0, 1);
            Assert.IsTrue(result._high1 == int.MinValue);
            Assert.IsTrue(result._high2 == 0);
            Assert.IsTrue(result._low1 == 0);
            Assert.IsTrue(result._low2 == 0);
        }

        [TestMethod]
        public void Sub1()
        {
            var result = new Int128(0) - new Int128(0);
            Assert.IsTrue(result._high1 == 0);
            Assert.IsTrue(result._high2 == 0);
            Assert.IsTrue(result._low1 == 0);
            Assert.IsTrue(result._low2 == 0);
        }

        [TestMethod]
        public void Sub2()
        {
            var result = new Int128(0) - new Int128(1);
            Assert.IsTrue(result._high1 == -1);
            Assert.IsTrue(result._high2 == uint.MaxValue);
            Assert.IsTrue(result._low1 == uint.MaxValue);
            Assert.IsTrue(result._low2 == uint.MaxValue);
        }

        [TestMethod]
        public void Sub3()
        {
            var result = new Int128(0) - new Int128(100) + new Int128(100);
            Assert.IsTrue(result._high1 == 0);
            Assert.IsTrue(result._high2 == 0);
            Assert.IsTrue(result._low1 == 0);
            Assert.IsTrue(result._low2 == 0);
        }

        [TestMethod]
        public void Sub4()
        {
            var result = new Int128(1, 0, 0, 0) - new Int128(1);
            Assert.IsTrue(result._high1 == 0);
            Assert.IsTrue(result._high2 == uint.MaxValue);
            Assert.IsTrue(result._low1 == uint.MaxValue);
            Assert.IsTrue(result._low2 == uint.MaxValue);
        }

        [TestMethod]
        public void Sub5()
        {
            var result = new Int128(0x7FFFFFFF, 0, 0, 0) - new Int128(0x7FFFFFFF, 0, 0, 1) + new Int128(1);
            Assert.IsTrue(result._high1 == 0);
            Assert.IsTrue(result._high2 == 0);
            Assert.IsTrue(result._low1 == 0);
            Assert.IsTrue(result._low2 == 0);
        }

        [TestMethod]
        public void Mult1()
        {
            var result = new Int128(0, 0, 0, 1) * new Int128(0, 0, 0, 1);
            Assert.IsTrue(result._high1 == 0);
            Assert.IsTrue(result._high2 == 0);
            Assert.IsTrue(result._low1 == 0);
            Assert.IsTrue(result._low2 == 1);
        }

        [TestMethod]
        public void Mult2()
        {
            var result = new Int128(0, 0, 1, 0) * new Int128(0, 0, 1, 0);
            Assert.IsTrue(result._high1 == 0);
            Assert.IsTrue(result._high2 == 1);
            Assert.IsTrue(result._low1 == 0);
            Assert.IsTrue(result._low2 == 0);
        }

        [TestMethod]
        public void Mult3()
        {
            var result = new Int128(0, 1000, 0, 0) * new Int128(0, 0, 1000, 0);
            Assert.IsTrue(result._high1 == 1000000);
            Assert.IsTrue(result._high2 == 0);
            Assert.IsTrue(result._low1 == 0);
            Assert.IsTrue(result._low2 == 0);
        }

        [TestMethod]
        public void DivUInt1()
        {
            var result = new Int128(0, 0, 0, 1) / 1;
            Assert.IsTrue(result._high1 == 0);
            Assert.IsTrue(result._high2 == 0);
            Assert.IsTrue(result._low1 == 0);
            Assert.IsTrue(result._low2 == 1);
        }

        [TestMethod]
        public void DivUInt2()
        {
            var result = new Int128(0x40000000, 0x80000000, 0x80000000, 0x80000000) / 1;
            Assert.IsTrue(result._high1 == 0x40000000);
            Assert.IsTrue(result._high2 == 0x80000000);
            Assert.IsTrue(result._low1 == 0x80000000);
            Assert.IsTrue(result._low2 == 0x80000000);
        }

        [TestMethod]
        public void DivUInt3()
        {
            var result = new Int128(0x40000000, 0x80000000, 0x80000000, 0x80000000) / 0x40000000;
            Assert.IsTrue(result._high1 == 1);
            Assert.IsTrue(result._high2 == 2);
            Assert.IsTrue(result._low1 == 2);
            Assert.IsTrue(result._low2 == 2);
        }

        [TestMethod]
        public void DivUInt4()
        {
            var result = new Int128(0x40000000, 0, 0, 0) / 0xF0120050;
            Assert.IsTrue(result._high1 == 0);
            Assert.IsTrue(result._high2 == 0x443f25d7);
            Assert.IsTrue(result._low1 == 0x7aca2d46);
            Assert.IsTrue(result._low2 == 0xdfd35220);
        }
    }
}
