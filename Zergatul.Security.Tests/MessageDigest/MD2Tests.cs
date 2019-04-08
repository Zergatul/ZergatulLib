using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class MD2Tests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        [TestMethod]
        public void Test()
        {
            foreach (var provider in _providers)
                using (var md = provider.GetMessageDigest(MessageDigests.MD2))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "8350e5a3e24c153df2275c9f80692773");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("a"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "32ec01ec4a6dac72c0ab96fb34c0b5d1");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "da853b0d3f88d99b30283a69e6ded6bb");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("message digest"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "ab4f496bfb2a530b219ff33031fe06b0");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "4e8ddff3650292ab5a4108c3aa47940b");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "da33def2a42df13975352846c30338cd");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("12345678901234567890123456789012345678901234567890123456789012345678901234567890"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "d5976f79d83d3a0dc9806c3c66f3efd8");
                    md.Reset();
                }
        }
    }
}