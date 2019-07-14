using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Hamsi224Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Hamsi224;
        protected override string Algorithm => "Hamsi";
        protected override int Size => 224;

        protected override int ShortTestsCount => 256;
        protected override int LongTestsCount => 128;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "b9f6eb1a9b990373f9d2cb125584333c69a3d41ae291845f05da221f");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "d8ddce06651055a10f1a936e1d1b478bdc094d841cae9a8594f21d1f");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "0e0bd268a3c7d9ca55b12a03ae18d4322394178d042d0e28c8b7da9b");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "0fba2a655daeaca00419b9d8f34362de967dc63efa40c31fd8048186");
                    md.Reset();
                }
        }
    }
}