using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SIMD384Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.SIMD384;
        protected override string Algorithm => "SIMD";
        protected override int Size => 384;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "5fdd62778fc213221890ad3bac742a4af107ce2692d6112e795b54b25dcd5e0f4bf3ef1b770ab34b38f074a5e0ecfcb5");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "d6ddced90788bfe7aa32739266796b353a678b29d2b12661ea007c4ac10783c8b353d4856339b0fa985aac7f495b7c6a");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "f4f9ba9a4e4e870079c69cf61b5e52c39bab522782fda17d093d8757231539f2bfb72c8dbbe36bea8321520f59a2d378");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));

                Assert.IsTrue(BitHelper.BytesToHex(digest) == "5415e493a51c203ced57a0fe38a90acabb744243432a2be240c760b560f13200dd28b1c82b85d34b9d635166ba2865c5");
                md.Reset();
            };
        }
    }
}