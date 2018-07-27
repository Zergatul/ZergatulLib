using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class RIPEMD256Tests
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
                var md = provider.GetMessageDigest(MessageDigests.RIPEMD256);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "02ba4c4e5f8ecd1877fc52d64d30e37a2d9774fb1e5d026380ae0168e3c5522d");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "c3b0c2f764ac6d576a6c430fb61a6f2255b4fa833e094b1ba8c1e29b6353036f");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "3bcbe8d6c9cf2cff39fb53e0dcef37f1554223da45d941d95836e1f5f84677eb");
                md.Reset();
            };
        }
    }
}