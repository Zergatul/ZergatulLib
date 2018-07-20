using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests
{
    [TestClass]
    public class RIPEMD128Tests
    {
        private static Provider[] _providers = new Provider[]
        {
            new DefaultProvider()
        };

        [TestMethod]
        public void Test()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.RIPEMD128);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "cdf26213a150dc3ecb610f18f6b38b46");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "3fa9b57f053c053fbe2735b2380db596");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "ab076efaab01d30d16bb57f88d63c073");
                md.Reset();
            };
        }
    }
}