using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class CubeHash256Tests : NISTMDTestWithData
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.CubeHash256;
        protected override string Algorithm => "CubeHash";
        protected override int Size => 256;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "44c6de3ac6c73c391bf0906cb7482600ec06b216c7c54a2a8688a6a42676577d");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "5151e251e348cbbfee46538651c06b138b10eeb71cf6ea6054d7ca5fec82eb79");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "acf01e86788983417a9583ac85d23f06d07e22c19a98df1b01cb55cd0fbb426d");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "a220b4bf5023e750c2a34dcd5564a8523d32e17fab6fbe0f18a0b0bf5a65632b");
                    md.Reset();
                }
        }
    }
}