using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SHA1Tests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new DefaultSecurityProvider(),
            new OpenSslProvider()
        };

        [TestMethod]
        public void Test()
        {
            foreach (var provider in _providers)
                using (var md = provider.GetMessageDigest(MessageDigests.SHA1))
                {

                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "da39a3ee5e6b4b0d3255bfef95601890afd80709");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "2fd4e1c67a2d28fced849ee1bb76e7391b93eb12");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "a7631795f6d59cd6d14ebd0058a6394a4b93d868");
                    md.Reset();
                }
        }
    }
}