using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Keccak224Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Keccak224;
        protected override string Algorithm => "Keccak";
        protected override int Size => 224;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "f71837502ba8e10837bdd8d365adb85591895602fc552b48b7390abd");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "c30411768506ebe1c2871b1ee2e87d38df342317300a9b97a95ec6a8");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "310aee6b30c47350576ac2873fa89fd190cdc488442f3ef654cf23fe");
                md.Reset();
            }
        }
    }
}