using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Generator;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Tests.Generator
{
    [TestClass]
    public class PKCS12v11Tests
    {
        [TestMethod]
        public void Test1()
        {
            var g = new PKCS12v11();
            var password = g.FormatPassword("hh87$-Jqo");
            var salt = new byte[] { 0xD3, 0x88, 0xD1, 0x8E, 0xD6, 0x0F, 0x71, 0x9E };

            var p = g.GenerateParameter(new SHA1(), PKCS12v11.IDIntegrityKey, password, salt, 2048, 20);

            Assert.IsTrue(BitHelper.BytesToHex(p) == "0a825661f3caafc1c88b9781c6de68e31ff805c1");
        }
    }
}