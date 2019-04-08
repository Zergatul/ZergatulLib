using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class JH384Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.JH384;
        protected override string Algorithm => "JH";
        protected override int Size => 384;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "2fe5f71b1b3290d3c017fb3c1a4d02a5cbeb03a0476481e25082434a881994b0ff99e078d2c16b105ad069b569315328");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "fc41b2b33438dc818a6ef99dd86f2c02a9c42ade5d0d3422f0cdd2289d50b6472c59798e569a0faec4c632e3340d1442");
                md.Reset();
            }
        }
    }
}