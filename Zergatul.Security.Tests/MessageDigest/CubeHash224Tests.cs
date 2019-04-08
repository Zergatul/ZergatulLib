using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class CubeHash224Tests : NISTMDTestWithData
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.CubeHash224;
        protected override string Algorithm => "CubeHash";
        protected override int Size => 224;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(MessageDigests.CubeHash224))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "f9802aa6955f4b7cf3b0f5a378fa0c9f138e0809d250966879c873ab");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "627c73689ffe429241791e40f4003bec4d652883ab0da288b220033f");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "6e11d0b1cd0aa96711fe5fb2512d07b5533a039be908012c3793b2d3");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "6b45504b39316bfd48dc44638a363c16b3f0263d66561b09d7d21fd7");
                    md.Reset();
                }
        }
    }
}