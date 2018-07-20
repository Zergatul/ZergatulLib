using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests
{
    [TestClass]
    public class RIPEMD160Tests
    {
        private static Provider[] _providers = new Provider[]
        {
            new DefaultProvider(),
            new OpenSslProvider()
        };

        [TestMethod]
        public void Test()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.RIPEMD160);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "9c1185a5c5e9fc54612808977ee8f548b2258d31");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "37f332f68db77bd9d7edd4969571ad671cf9dd3b");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "4e1ff644ca9f6e86167ccb30ff27e0d84ceb2a61");
                md.Reset();
            };
        }
    }
}