using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Shabal384Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Shabal384;
        protected override string Algorithm => "Shabal";
        protected override int Size => 384;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "ff093d67d22b06a674b5f384719150d617e0ff9c8923569a2ab60cda886df63c91a25f33cd71cc22c9eebc5cd6aee52a");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "66613058865271722c0295774aa77258a5082bebbb5a02f9d6aee9ad303fc71cbf19e2f599ddfde88cf0bf30a028e530");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "c08623c184f728d3e35c15bd74a27f0480de3a837f3a14bef7df70edc0e4a9500e100092d3e3f3b464ed18cbc1121bc5");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "5429528db7714c8b6191b19fbb44ad9192cd345a42f1f37b0c3ddde46000a79e20ae981ed47f3522c3a3ba39aafd9660");
                    md.Reset();
                }
        }
    }
}