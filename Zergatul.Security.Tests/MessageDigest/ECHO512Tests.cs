using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class ECHO512Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.ECHO512;
        protected override string Algorithm => "ECHO";
        protected override int Size => 512;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "158f58cc79d300a9aa292515049275d051a28ab931726d0ec44bdd9faef4a702c36db9e7922fff077402236465833c5cc76af4efc352b4b44c7fa15aa0ef234e");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "3bf04ec89d67e0dafd1b8ab26b176abaead6b3cdc706ff7198c3c6045e77d4eaf64cd90af9c5a7674919b90ff8c9b4a7554d6cfeffb334406ec233fb0b0dd6bc");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "fe61eba97bdfcaa027ded44a5f883fcb900b97449596d7b4a7187c76e71ad750e6117b529bd69992bec015bef862d16d62c384b600cb300d486e565f94202abf");
                md.Reset();
            }
        }
    }
}