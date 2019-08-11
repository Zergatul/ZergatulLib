using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Skein512x256Tests : NISTMDTestWithData
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Skein512x256;
        protected override string Algorithm => "Skein512";
        protected override int Size => 256;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "39ccc4554a8b31853b9de7a1fe638a24cce6b35a55f2431009e18780335d2621");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "0977b339c3c85927071805584d5460d8f20da8389bbe97c59b1cfac291fe9527");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "b3250457e05d3060b1a4bbc1428bc75a3f525ca389aeab96cfa34638d96e492a");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "41e829d7fca71c7d7154ed8fc8a069f274dd664ae0ed29d365d919f4e575eebb");
                md.Reset();
            };
        }
    }
}