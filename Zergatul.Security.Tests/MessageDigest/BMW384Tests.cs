using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class BMW384Tests
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
                var md = provider.GetMessageDigest(MessageDigests.BMW384);

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
            };
        }

        [TestMethod]
        public void NistTest()
        {
            List<byte> list = new List<byte>();
            foreach (var line in File.ReadAllLines("MessageDigest/NISTData.txt"))
                list.AddRange(BitHelper.HexToBytes(line));
            byte[] data = list.ToArray();

            string[] digests = File.ReadAllLines("MessageDigest/BMW384.txt");

            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.BMW384);

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