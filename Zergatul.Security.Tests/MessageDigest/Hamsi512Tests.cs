using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Hamsi512Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Hamsi512;
        protected override string Algorithm => "Hamsi";
        protected override int Size => 512;

        protected override int ShortTestsCount => 256;
        protected override int LongTestsCount => 128;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "5cd7436a91e27fc809d7015c3407540633dab391127113ce6ba360f0c1e35f404510834a551610d6e871e75651ea381a8ba628af1dcf2b2be13af2eb6247290f");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "3d8314afb2f974d1d7f594282326b481bf0378ac85f7f1fbc85c1b30e5a21e7543b79a55abcac09adcc63f5d870b884cd8c66d45507b0af5e9bda033784e8b7d");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "d7453c84a10eab2d4eef9d8862ced59e0640fe0f3fb088812a8b71ac5ac68953b213492ce3d83415f22c7033573b66e28417da0cb728a18e8914e08140d0948c");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "bf33474a4fcfa7f363441d0de165a570b7a56b56a2ca04b2a7fb263608756da92f0539f23011816673ded6c2f9cf7251d33421dd3d9e3d04bc3d3f64831bc7ee");
                    md.Reset();
                }
        }
    }
}