using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SHA3x224Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.SHA3x224;
        protected override string Algorithm => "SHA3";
        protected override int Size => 224;

        protected override int ShortTestsCount => 1153;
        protected override int LongTestsCount => 100;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "6b4e03423667dbb73b6e15454f0eb1abd4597f9a1b078e3f5b5a6bc7");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "e642824c3f8cf24ad09234ee7d3c766fc9a3a5168d0c94ad73b46fdf");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "d15dadceaa4d5d7bb3b48f446421d542e08ad8887305e28d58335795");
                md.Reset();
            }
        }

        public override void NIST_MonteCarlo()
        {

        }

        public override void NIST_ExtremelyLong()
        {

        }
    }
}