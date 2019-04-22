using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class ECHO384Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.ECHO384;
        protected override string Algorithm => "ECHO";
        protected override int Size => 384;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "134040763f840559b84b7a1ae5d6d64fc3659821a789cc64a7f1444c09ee7f81a54d72beee8273bae5ef18ec43aa5f34");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "94cbb881848c45b7f6649b7b36901d14973248d9bfa318bd830d1c14d749e7e9bf0a69ce738ac8a1fd361411a8dc9dae");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "d045abb41ef43012e0436855f10f1a115eeec1f346ff119e86bf96cf427f453b625f0df8ee2b123e335a9a38446702c6");
                md.Reset();
            }
        }
    }
}