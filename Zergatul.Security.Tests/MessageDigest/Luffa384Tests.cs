using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Luffa384Tests : NISTMDTestWithData
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Luffa384;
        protected override string Algorithm => "Luffa";
        protected override int Size => 384;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "117d3ad49024dfe2994f4e335c9b330b48c537a13a9b7fa465938e1a02ff862bcdf33838bc0f371b045d26952d3ea0c5");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "9a7abb797a840e2d423c34c91f559f6809bdb2916fb2e9effec2fa0a7a69881be9872480c635d20d2fd6e95d046601a7");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "e67f459e496dfe04a0091a2e2c253e5f48883472dc21dce1d6a0bb0359867fc11815d8e0f868bbfb102f412e24075107");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "0562884aee1ac17ff4e5a14827e5748be18f2adfc929adc22758a1ebac25633342c370ebd01739add49abf81fafd73af");
                    md.Reset();
                }
        }
    }
}