using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class SHAvite3x512Tests : NISTMDTest
    {
        protected override SecurityProvider[] Providers => new SecurityProvider[]
        {
            new ZergatulProvider()
        };

        protected override string Name => MessageDigests.SHAvite3x512;
        protected override string Algorithm => "SHAvite3";
        protected override int Size => 512;

        protected override int ShortTestsCount => 256;
        protected override int LongTestsCount => 128;

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in Providers)
            {
                var md = provider.GetMessageDigest(Name);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "a485c1b2578459d1efc5dddd840bb0b4a650ac82fe68f58c4442ccda747da006b2d1dc6b4a4eb7d84ff91e1f466fef429d259acd995dddcad16fa545c7a6e5ba");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "0fb0b216b377e6d95db1b6d9b6c8b59f08d4e29814071c8c0f827b32e68c15362f24bcc15ad6b1c925a03f00092997f7628cb47f27c9ad7a22e4c00fbb2c16e3");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "4dbd97835c4e5cfa14799884a7adc96688dd808ff53d5c4cfe7db89a55ee98d0260791ec0c9b5466482ab3f6f236da7e65e1cb6d1ee624f61a5b2b79f63c4120");
                md.Reset();
            }
        }
    }
}