using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.OpenSsl;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SHA384Tests
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
                using (var md = provider.GetMessageDigest(MessageDigests.SHA384))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "ca737f1014a48f4c0b6dd43cb177b0afd9e5169367544c494011e3317dbf9a509cb1e5dc1e85a941bbee3d7f2afbc9b1");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("Test vector from febooti.com"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "388bb2d487de48740f45fcb44152b0b665428c49def1aaf7c7f09a40c10aff1cd7c3fe3325193c4dd35d4eaa032f49b0");
                    md.Reset();
                }
        }
    }
}