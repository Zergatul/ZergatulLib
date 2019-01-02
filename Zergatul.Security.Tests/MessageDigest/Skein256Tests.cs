using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class Skein256Tests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new DefaultSecurityProvider()
        };

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.Skein512_256);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "39ccc4554a8b31853b9de7a1fe638a24cce6b35a55f2431009e18780335d2621");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "b3250457e05d3060b1a4bbc1428bc75a3f525ca389aeab96cfa34638d96e492a");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "41e829d7fca71c7d7154ed8fc8a069f274dd664ae0ed29d365d919f4e575eebb");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "0977b339c3c85927071805584d5460d8f20da8389bbe97c59b1cfac291fe9527");
                md.Reset();
            };
        }

        [TestMethod]
        public void NistTest()
        {
            List<byte> list = new List<byte>();
            foreach (var line in File.ReadAllLines("MessageDigest/NISTData.txt"))
                list.AddRange(BitHelper.HexToBytes(line));
            byte[] data = list.ToArray();

            string[] digests = File.ReadAllLines("MessageDigest/Skein256.txt");

            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.Skein512_256);

                int index = 0;
                for (int i = 0; i < 2048; i++)
                {
                    if (i % 8 == 0)
                    {
                        var digest = md.Digest(data, index, i / 8);
                        Assert.IsTrue(BitHelper.BytesToHex(digest) == digests[i]);
                        md.Reset();
                    }
                    index += (i + 7) / 8;
                }
            }
        }

        [TestMethod]
        public void LongTest()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.Skein512_256);

                byte[] data = Encoding.ASCII.GetBytes("abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmnhijklmno");

                for (int i = 0; i < 16777216; i++)
                    md.Update(data);
                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "6e44df5b41110404a558111eb4bbd4b088aef7de2d5833171a9695c091f8b9b1");
            }
        }
    }
}