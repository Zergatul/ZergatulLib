using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.OpenSsl;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SHA224Tests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new ZergatulProvider(),
            new OpenSslProvider()
        };

        [TestMethod]
        public void Test()
        {
            foreach (var provider in _providers)
                using (var md = provider.GetMessageDigest(MessageDigests.SHA224))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "d14a028c2a3a2bc9476102bb288234c415a2b01f828ea62ac5b3e42f");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "730e109bd7a8a32b1cb9d9a09aa2325d2430587ddbc0c38bad911525");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "3628b402254caa96827e3c79c0a559e4558da8ee2b65f1496578137d");
                    md.Reset();
                }
        }
    }
}