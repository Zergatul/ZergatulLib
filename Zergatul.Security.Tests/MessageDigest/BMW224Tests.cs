using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class BMW224Tests : NISTMDTestWithData
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.BMW224;
        protected override string Algorithm => "BMW";
        protected override int Size => 224;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "e57c183da7e2cd3e90258ca04499b222420f9b6797bbab131b4d286e");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "278f7e6db8fd7c9353fc181d840bf20351e3a45229ff42983ac26697");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "e8d44179ec629ef235c67cb3ea1c1234ac4fd4ea1e692f5432bfcb46");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "246607792ad2625430c81e2c4ea1380add5b08fb8075daed4f401dbc");
                    md.Reset();
                }
        }
    }
}