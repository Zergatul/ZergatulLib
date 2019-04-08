using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class BMW384Tests : NISTMDTestWithData
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.BMW384;
        protected override string Algorithm => "BMW";
        protected override int Size => 384;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(MessageDigests.BMW384))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "1db2643911391720e712a8c24457ee456fabfd555f479156e4b24278d6f6bcfb03fab1ec2a2626b79f2880216bc29b29");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "ca60ffd15eab7f4809f1b8f8daa5687f2192f872cc554303181403626cf5311be3c8f86e49aab330278f8e1b411d3c60");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "83ccec15dde8458baab5986f5beeece8cc8062f6647f80efc9b203bbba1dfbcd3661941ea1307844b1d2f9ab1babd311");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "411e84c41bd59e1376fc905fe96d2ef58fd59970abba02ca53a7a662f9e6cedb4e7e43bd63717215cd86ea20282f2b36");
                    md.Reset();
                }
        }
    }
}