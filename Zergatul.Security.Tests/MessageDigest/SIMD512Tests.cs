using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SIMD512Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.SIMD512;
        protected override string Algorithm => "SIMD";
        protected override int Size => 512;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "51a5af7e243cd9a5989f7792c880c4c3168c3d60c4518725fe5757d1f7a69c6366977eaba7905ce2da5d7cfd07773725f0935b55f3efb954996689a49b6d29e0");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "16e676965036d1760b810f86bc6c488dbc522b03a276ca7de62cfb651eba048fcbe273af51b21d0416709cd5e3434801ca782087deff150dff3af0c23e718b32");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "ca493ce78cc2a63b5a48393e61d113d59a930b3e76d062ab58177345c48b59890a08661d04dd6160a1b42d215f1e303d97ab0abb54e65f758f79aee2b182b34b");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "f251b032fa828d543b68cf931a24e2c99d06020e757cd836cb0828b45ceeba394f026a6e373f70634d79be49f1606da46f4a42af4d9179073fe0bf9c1adbfd3b");
                md.Reset();
            };
        }
    }
}