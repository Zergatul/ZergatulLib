using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.IO;

namespace Zergatul.Tests
{
    [TestClass]
    public class BitReaderTests
    {
        [TestMethod]
        public void Test1()
        {
            var ms = new MemoryStream(new byte[] { 1 });
            var br = new BitReader(ms);

            Assert.IsTrue(br.ReadBits(1) == 1);
            Assert.IsTrue(br.ReadBits(1) == 0);
            Assert.IsTrue(br.ReadBits(1) == 0);
            Assert.IsTrue(br.ReadBits(1) == 0);
            Assert.IsTrue(br.ReadBits(1) == 0);
            Assert.IsTrue(br.ReadBits(1) == 0);
            Assert.IsTrue(br.ReadBits(1) == 0);
            Assert.IsTrue(br.ReadBits(1) == 0);
        }
    }
}