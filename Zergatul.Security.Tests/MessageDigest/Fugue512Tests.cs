using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Fugue512Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.Fugue512;
        protected override string Algorithm => "Fugue";
        protected override int Size => 512;

        protected override int ShortTestsCount => 256;
        protected override int LongTestsCount => 128;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
                using (var md = provider.GetMessageDigest(Name))
                {
                    var digest = md.Digest();
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "3124f0cbb5a1c2fb3ce747ada63ed2ab3bcd74795cef2b0e805d5319fcc360b4617b6a7eb631d66f6d106ed0724b56fa8c1110f9b8df1c6898e7ca3c2dfccf79");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "6d3c8217e4ddcb6ad1e651ff24be6a50b177c11e0df51ddcf7e90787274927bf1d77bf870d74551c00818fd1d049f384f8c56a8c2c040de2febef2637dc4b20f");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "ee1e53e892bedd72d753bd4c9f704201708fb9b79177816051ebca1dc1af7ee928b8996df0862bbea24503be2781b1a036079a88627d4d248f2d0ec77b579b7f");
                    md.Reset();

                    digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                    Assert.IsTrue(BitHelper.BytesToHex(digest) == "8701ffc409b83f54450a38ab72c782a79327953bbcf4cee4a80ef4174c1d18fbed48576126ae3d4fe26df5a08acbb9b080a9af57480a10bfd63b6198e60548c4");
                    md.Reset();
                }
        }
    }
}