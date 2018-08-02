using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class CubeHash384Tests
    {
        private static Provider[] _providers = new Provider[]
        {
            new DefaultProvider()
        };

        [TestMethod]
        public void BasicTest()
        {
            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.CubeHash384);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "98ae93ebf4e58958497f610a22c8cf60f2292319283ca6459daed1707be06e7591c5f2d84bd3339e66c770e485bfa1fb");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "88f92a5b9a31038eb8f7340c7b221aab45f6d3283a1d0b8b0cec44617c1c1f3b801fee61d8994591e4b78e1e76f30691");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "625fc5be6a9c17b116677a285dd1fb96e78db708c6fdea1c4c14c0e5ee67e1711a04fb93766d7e0516b73f9420844a2b");
                md.Reset();

                digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "287cc1738bdb9575fd716bafbb02768ce5a57ae5c08ba12f5cf74fac27ab5707e577bc93539c07af9ab92c3b1b368997");
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

            string[] digests = File.ReadAllLines("MessageDigest/CubeHash384.txt");

            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.CubeHash384);

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