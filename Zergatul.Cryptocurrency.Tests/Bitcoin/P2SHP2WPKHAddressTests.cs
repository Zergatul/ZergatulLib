using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Bitcoin;

namespace Zergatul.Cryptocurrency.Tests.Bitcoin
{
    [TestClass]
    public class P2SHP2WPKHAddressTests
    {
        [TestMethod]
        public void FromPublicKeyHashTest1()
        {
            var addr = new P2SHP2WPKHAddress();
            addr.FromPublicKeyHash(BitHelper.HexToBytes("f9fd8d9b295410ac5c3caf1a30822d856e808e49"));
            Assert.IsTrue(addr.Value == "3E6wDvWHDWmsnz4uS5GBy4cRVLWFegW3nW");
        }
    }
}