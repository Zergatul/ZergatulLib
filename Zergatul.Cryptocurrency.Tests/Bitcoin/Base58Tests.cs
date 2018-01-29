using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Bitcoin;

namespace Zergatul.Cryptocurrency.Tests.Bitcoin
{
    [TestClass]
    public class Base58Tests
    {
        [TestMethod]
        public void ZeroTests()
        {
            Assert.IsTrue(Base58Encoding.Encode(new byte[] { 0 }, new byte[20]) == "1111111111111111111114oLvT2");
            Assert.IsTrue(ByteArray.Equals(new byte[21], Base58Encoding.Decode("1111111111111111111114oLvT2")));

            Assert.IsTrue(Base58Encoding.Encode(new byte[] { 0 }, BitHelper.HexToBytes("0000000000000000000000000000000000000001")) == "11111111111111111111BZbvjr");
            Assert.IsTrue(ByteArray.Equals(BitHelper.HexToBytes("000000000000000000000000000000000000000001"), Base58Encoding.Decode("11111111111111111111BZbvjr")));
        }
    }
}