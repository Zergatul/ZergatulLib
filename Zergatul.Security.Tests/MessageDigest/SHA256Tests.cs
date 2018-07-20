using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests
{
    [TestClass]
    public class SHA256Tests
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
                var md = provider.GetMessageDigest(MessageDigests.SHA256);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "077b18fe29036ada4890bdec192186e10678597a67880290521df70df4bac9ab");
                md.Reset();
            };
        }
    }
}