using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Skein512x384Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Skein512x384;
        protected override string Algorithm => "Skein512";
        protected override int Size => 384;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "dd5aaf4589dc227bd1eb7bc68771f5baeaa3586ef6c7680167a023ec8ce26980f06c4082c488b4ac9ef313f8cbe70808");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "b4329745321c8f6b788a04526dad856b4a87f510ee496b743f61b048209fc3261c1ebbb8a35040a7ff58c34378c4536c");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "f814c107f3465e7c54048a5503547deddc377264f05c706b0d19db4847b354855ee52ab6a785c238c9e710d848542041");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "9e4dd8e86404618af1263331ed39199826f797d587bbcab1d66ee92ce6bfb6e1dba2f3df7473c1c51239e64c5f51e37d");
                md.Reset();
            };
        }
    }
}