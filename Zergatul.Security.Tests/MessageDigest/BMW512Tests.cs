using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class BMW512Tests : NISTMDTestWithData
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.BMW512;
        protected override string Algorithm => "BMW";
        protected override int Size => 512;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "6a725655c42bc8a2a20549dd5a233a6a2beb01616975851fd122504e604b46af7d96697d0b6333db1d1709d6df328d2a6c786551b0cce2255e8c7332b4819c0e");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "2998d4cb31323e1169b458ab03a54d0b68e411a3c7cc7612adbf05bf901b8197dfd852c1c0099c09717d2fad3537207e737c6159c31d377d1ab8f5ed1ceeea06");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "5b16c95ca761db23747b6f7fbd051b31b33e4086dc967cd0f60f4f5e10ccd754de1544b8c5ea98970d8f1509aff66f1a6c454da123c7658079b9ddb64fa18523");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "8f37bef264289f61f3d713944d394a7ac1dd95d3fe5787b5d325a310bc9cd18783852bfee12fbdeaab3ad9a67f2b654e348714aed3acf7d7548e95591af68046");
                    md.Reset();
                }
        }
    }
}