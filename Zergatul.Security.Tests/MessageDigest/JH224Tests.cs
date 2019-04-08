using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class JH224Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.JH224;
        protected override string Algorithm => "JH";
        protected override int Size => 224;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "2c99df889b019309051c60fecc2bd285a774940e43175b76b2626630");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "21e88480ebb76dd51a984d52e97fa0da620f885b94a172320131ab54");
                md.Reset();
            }
        }
    }
}