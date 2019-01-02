using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class MD2Tests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new OpenSslProvider()
        };

        [TestMethod]
        public void Test()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.MD4);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "31d6cfe0d16ae931b73c59d7e0c089c0");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "1bee69a46ba811185c194762abaeae90");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "6578f2664bc56e0b5b3f85ed26ecc67b");
                md.Reset();
            };
        }
    }
}