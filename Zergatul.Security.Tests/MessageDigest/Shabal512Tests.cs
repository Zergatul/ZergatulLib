using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Shabal512Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Shabal512;
        protected override string Algorithm => "Shabal";
        protected override int Size => 512;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "fc2d5dff5d70b7f6b1f8c2fcc8c1f9fe9934e54257eded0cf2b539a2ef0a19ccffa84f8d9fa135e4bd3c09f590f3a927ebd603ac29eb729e6f2a9af031ad8dc6");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "4a7f0f707c1b0c1d12ddcfa8aa0f9d2410dd9bab57c2d56705fc1acb02066f99678738cedb20a2aba94842a441e77bc02656fe5690f98b421d029bfc4df09f91");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "f12f6893f4535d360b07ec15be706e5921b0358d736e61cb2e7ffd2157cd119dc1aeecbf2f1ac73552dc052ad4edcf8cbe87073a4db4d1b4f6a31e39edf5a96d");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "325b9f054d44ef049a3270f1f9a8add5ac1119b08616ff6f9536a3553b65bc0a5cc2d3740f4cf828d0148cadce23d7785a7a6e93ce5a2e953a17dfd62724520e");
                    md.Reset();
                }
        }
    }
}