using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class BLAKE224Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.BLAKE224;
        protected override string Algorithm => "BLAKE";
        protected override int Size => 224;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "7dc5313b1c04512a174bd6503b89607aecbee0903d40a8a569c94eed");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "7c270941a0b4a412db099b710da90112ce49f8510add4f896c07ace4");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "c8e92d7088ef87c1530aee2ad44dc720cc10589cc2ec58f95a15e51b");
                    md.Reset();
                }
        }
    }
}