using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Hamsi256Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Hamsi256;
        protected override string Algorithm => "Hamsi";
        protected override int Size => 256;

        protected override int ShortTestsCount => 256;
        protected override int LongTestsCount => 128;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "750e9ec469f4db626bee7e0c10ddaa1bd01fe194b94efbabebd24764dc2b13e9");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "6b017b90971fdb646700dea0e50e7ac1f6a75a849b2809a55eedcde4c65daf1f");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "415e7fa87a20d942012c9b458507c247498043e09381a165a893e4d22c52246c");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "4dd401d97b909efbac9e471518a45b94b4a9f77a594478a0876b1b38d3d8d276");
                    md.Reset();
                }
        }
    }
}