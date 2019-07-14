using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Hamsi384Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Hamsi384;
        protected override string Algorithm => "Hamsi";
        protected override int Size => 384;

        protected override int ShortTestsCount => 256;
        protected override int LongTestsCount => 128;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "3943cd34e3b96b197a8bf4bac7aa982d18530dd12f41136b26d7e88759255f21153f4a4bd02e523612b8427f9dd96c8d");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "9b190fae7e60ca27a2c3beeb5aad8d2f87b9450280ab547cc5748661bb86a71646dd0c3120bbc12a93b8caaccfa58884");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "810b536315560820b2a4ebaf4680ba4fd77b577ade9b326029d1d4954b4d203fd9ab519ca8ebf1132369d3926e4e4572");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "724ba854328a21a7734984ddc4eefa4ded73c5b94a8a1f7d5cdb51c1a1fc0d14d0777f4d6ac00b1460e95648201a89b7");
                    md.Reset();
                }
        }
    }
}