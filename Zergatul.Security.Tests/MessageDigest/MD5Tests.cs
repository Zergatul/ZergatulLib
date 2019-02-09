using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class MD5Tests
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
                using (var md = provider.GetMessageDigest(MessageDigests.MD5))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "d41d8cd98f00b204e9800998ecf8427e");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "9e107d9d372bb6826bd81d3542a419d6");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "500ab6613c6db7fbd30c62f5ff573d0f");
                    md.Reset();
                }
        }
    }
}