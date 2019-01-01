using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Groestl384Tests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new DefaultSecurityProvider()
        };

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.Groestl384);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "ac353c1095ace21439251007862d6c62f829ddbe6de4f78e68d310a9205a736d8b11d99bffe448f57a1cfa2934f044a5");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "32c39f82ab41ee4fdb1582f83dde41089d47b904988b1a9a647553cb1a502cf07df7eb1e11dc3d66bec096a39a790336");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopqopqrpqrsqrstrstustuvtuvwuvwxvwxywxyzxyzayzabzabcabcdbcdecdefdefg"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "33625fdddcc2809a83b912d70910d3b5e1408ef017c949617c5543bb835939f13484e60bfe6ff27acf225c7a4b596504");
                md.Reset();
            };
        }
    }
}