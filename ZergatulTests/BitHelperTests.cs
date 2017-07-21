using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul;

namespace Zergatul.Tests
{
    [TestClass]
    public class BitHelperTests
    {
        [TestMethod]
        public void RotateLeft128AndXor_1()
        {
            byte[] a = new byte[16];
            byte[] b = new byte[16];
            b[15] = 1;
            BitHelper.RotateLeft128AndXor(a, 0, b, 0, 1);
            Assert.IsTrue(a.SequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }));
        }

        [TestMethod]
        public void RotateLeft128AndXor_2()
        {
            byte[] a = new byte[16];
            byte[] b = new byte[16];
            b[15] = 1;
            BitHelper.RotateLeft128AndXor(a, 0, b, 0, 65);
            Assert.IsTrue(a.SequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 }));
        }

        [TestMethod]
        public void RotateLeft128AndXor_3()
        {
            byte[] a = new byte[16];
            byte[] b = new byte[16];
            b[0] = 0x80;
            BitHelper.RotateLeft128AndXor(a, 0, b, 0, 1);
            Assert.IsTrue(a.SequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }));
        }

        [TestMethod]
        public void RotateLeft128AndXor_4()
        {
            byte[] a = new byte[16];
            byte[] b = new byte[16];
            b[0] = 0x80;
            BitHelper.RotateLeft128AndXor(a, 0, b, 0, 65);
            Assert.IsTrue(a.SequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }));
        }

        [TestMethod]
        public void RotateRight128AndXor_1()
        {
            byte[] a = new byte[16];
            byte[] b = new byte[16];
            b[15] = 1;
            BitHelper.RotateRight128AndXor(a, 0, b, 0, 1);
            Assert.IsTrue(a.SequenceEqual(new byte[] { 128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
        }

        [TestMethod]
        public void RotateRight128AndXor_2()
        {
            byte[] a = new byte[16];
            byte[] b = new byte[16];
            b[0] = 1;
            BitHelper.RotateRight128AndXor(a, 0, b, 0, 65);
            Assert.IsTrue(a.SequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 128, 0, 0, 0, 0, 0, 0 }));
        }
    }
}
