using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class JH512Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.JH512;
        protected override string Algorithm => "JH";
        protected override int Size => 512;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "90ecf2f76f9d2c8017d979ad5ab96b87d58fc8fc4b83060f3f900774faa2c8fabe69c5f4ff1ec2b61d6b316941cedee117fb04b1f4c5bc1b919ae841c50eec4f");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "a05eab9c641cb901107d9880bcdf0eedb19b0073188896365921bd200225d9176cf136e7af90d67bdb05dfa3037e48b757d23a905b2270db67255b9eca982973");
                md.Reset();
            }
        }
    }
}