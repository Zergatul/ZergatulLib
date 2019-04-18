using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SHAvite3x384Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.SHAvite3x384;
        protected override string Algorithm => "SHAvite3";
        protected override int Size => 384;

        protected override int ShortTestsCount => 256;
        protected override int LongTestsCount => 128;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "814b55553ce7c0841f8ff0321e6287f9f50a8e0cae811932385ecc1b7c386b4eb14edb79c8381babf09276b69d1bb3ee");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "f4c0fe6fb39bf9ece48e3e0c8ea15d27ba295e5454d53396fecb944a902801f98f078be0649dbd0183ec22f5ca095830");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "67e488432df469c810797aaa65c7e6622096c094439fedebba892ccab1547332f9fa506f9ea1ecf6d150a896141eeba6");
                md.Reset();
            }
        }
    }
}