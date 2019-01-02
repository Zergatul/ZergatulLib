using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class BMW224Tests
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
                var md = provider.GetMessageDigest(MessageDigests.BMW224);

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
            };
        }

        [TestMethod]
        public void NistTest()
        {
            List<byte> list = new List<byte>();
            foreach (var line in File.ReadAllLines("MessageDigest/NISTData.txt"))
                list.AddRange(BitHelper.HexToBytes(line));
            byte[] data = list.ToArray();

            string[] digests = File.ReadAllLines("MessageDigest/BMW224.txt");

            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.BMW224);

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
    }
}