using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SHA3x256Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.SHA3x256;
        protected override string Algorithm => "SHA3";
        protected override int Size => 256;

        protected override int ShortTestsCount => 1089;
        protected override int LongTestsCount => 100;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "a7ffc6f8bf1ed76651c14756a061d662f580ff4de43b49fa82d80a4b80f8434a");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "3a985da74fe225b2045c172d6bd390bd855f086e3e9d525b46bfe24511431532");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "69070dda01975c8c120c3aada1b282394e7f032fa9cf32f4cb2259a0897dfc04");
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