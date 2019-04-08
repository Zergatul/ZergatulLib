using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Groestl256Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Groestl256;
        protected override string Algorithm => "Groestl";
        protected override int Size => 256;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "1a52d11d550039be16107f9c58db9ebcc417f16f736adb2502567119f0083467");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "8c7ad62eb26a21297bc39c2d7293b4bd4d3399fa8afab29e970471739e28b301");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "f48290b1bcacee406a0429b993adb8fb3d065f4b09cbcdb464a631d4a0080aaf");
                md.Reset();
            };
        }
    }
}