using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class BMW256Tests : NISTMDTestWithData
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.BMW256;
        protected override string Algorithm => "BMW";
        protected override int Size => 256;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "82cac4bf6f4c2b41fbcc0e0984e9d8b76d7662f8e1789cdfbd85682acc55577a");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "ca0981a78ac2c97ecb358267f6d8d88216366024ef0d7137938b5a3165898dff");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "a0fac9ecaf13d3cf742c09c52bba65d919709a7e181d6b9d63858e6bf968069a");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "57d11fc94bdf98e6a0d0bf1d4ddda3f4205e873666a644b5bb585e171ad87d34");
                    md.Reset();
                }
        }
    }
}