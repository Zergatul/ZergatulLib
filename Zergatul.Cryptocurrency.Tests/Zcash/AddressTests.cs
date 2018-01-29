using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Zcash;

namespace Zergatul.Cryptocurrency.Tests.Zcash
{
    [TestClass]
    public class AddressTests
    {
        [TestMethod]
        public void P2PKHTests()
        {
            var addr = new P2PKHAddress();

            addr.FromPublicKey(BitHelper.HexToBytes("02a1a26871f4f6bc57ea7808c4cde6d17fff75d561f2266610ae2903c9734ea443"));
            Assert.IsTrue(addr.Value == "t1V1XXdqsVVNUXT2r8krESmxXpiPhu9SFCd");

            addr.FromPublicKey(BitHelper.HexToBytes("029e88ccf7c5c8ccd62eb07b1e924330982946e48e8840ad2b00d08a4ab7a03b98"));
            Assert.IsTrue(addr.Value == "t1ceRp6P4vuywBFUvjpkRiVy4iM4iCHVyfV");

            addr.FromPublicKeyHash(BitHelper.HexToBytes("2e35d7685e5ac73d582d45104abe9ce71afd1180"));
            Assert.IsTrue(addr.Value == "t1N5wb8Q37oR65Z2rCrpApZzxvpghjjTR6q");

            addr.FromPublicKeyHash(BitHelper.HexToBytes("66073d46510637a4cc8267cac493f768ee578b56"));
            Assert.IsTrue(addr.Value == "t1TB5buEfBHR2sBX2PWPrZyuZyab5qN9ktL");

            // 
        }
    }
}