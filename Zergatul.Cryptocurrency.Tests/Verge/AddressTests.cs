using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Verge;

namespace Zergatul.Cryptocurrency.Tests.Verge
{
    [TestClass]
    public class AddressTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var p2pkh = new P2PKHAddress();

            p2pkh.FromPublicKey(BitHelper.HexToBytes("02b9e05c26135463e63cd1dfa4dcea219208c6a8acd1ae93d640ab90c5d7b2bd51"));
            Assert.IsTrue(p2pkh.Value == "D6eDXzx2isGRmbr2APjn79KN13ATXMhbzj");

            p2pkh.FromPublicKey(BitHelper.HexToBytes("0350ffacf445640ef7f0935627e1b9d4d782dd73758fc3c013f72be232ca7ab4f0"));
            Assert.IsTrue(p2pkh.Value == "DHrVuj8GRnSuh7Hy1wHZ8935PArshxRxXt");

            p2pkh.FromPublicKey(BitHelper.HexToBytes("020dabfa8695b975152301d696d030fbaa6372186ae0a2cdca78af36267298d8e6"));
            Assert.IsTrue(p2pkh.Value == "DCgTGEEMo9WsuxYbSGGVeX2y4HCyyhReEf");

            p2pkh.FromPublicKey(BitHelper.HexToBytes("02f8870e3db2a15e4cd6dda67906801acaeacf23fae34037c06a7fb7cf596f1693"));
            Assert.IsTrue(p2pkh.Value == "DLFT8hdrJotcNA2U6cvu4hDRSBGZ7WTdPk");

            p2pkh.FromPublicKey(BitHelper.HexToBytes("022a7bcbbee8c725c68185dd8944190416c8b1ce20f675995feb02e20697408376"));
            Assert.IsTrue(p2pkh.Value == "DN2PUhfvPoMcgpazGhHp8KADvMVzLHbVHu");

            p2pkh.FromPublicKeyHash(BitHelper.HexToBytes("c42b4499a65dec267691b09a0fee17406147ccd9"));
            Assert.IsTrue(p2pkh.Value == "DP2LpAiqtW6toPCQUtDdz4z4BrkHrDaskP");

            p2pkh.FromPublicKeyHash(BitHelper.HexToBytes("577b84b41045e967fa36bb7a8e58400938c447e5"));
            Assert.IsTrue(p2pkh.Value == "DD7fJ9vZ4dohCXDkbLdntYcD1mdW5zhuLg");
        }
    }
}