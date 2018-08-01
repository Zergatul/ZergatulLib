﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.MessageDigest
{
    [TestClass]
    public class CubeHash512Tests
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
                var md = provider.GetMessageDigest(MessageDigests.CubeHash512);

                var digest = md.Digest();
                Assert.IsTrue(BitHelper.BytesToHex(digest) == "4a1d00bbcfcb5a9562fb981e7f7db3350fe2658639d948b9d57452c22328bb32f468b072208450bad5ee178271408be0b16e5633ac8a1e3cf9864cfbfc8e043a");
                md.Reset();

                //digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
                //Assert.IsTrue(BitHelper.BytesToHex(digest) == "459e2280a7cdb0c721d8d9dbeb9ed339659dc9e7b158e9dd2d328d946cb21474dc9177edfc93602f1aadb31944c795c9b5df859a3dc6132d4f0a4c476aaf797f");
                //md.Reset();

                //digest = md.Digest(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));
                //Assert.IsTrue(BitHelper.BytesToHex(digest) == "8d023cb777de2059af5007032c253fe0148a35c0a98def536cf22d5af064d8279fc2c80f52fe2d08462beaa8011e30350618a01f4763dcd53d8a09afcac75c65");
                //md.Reset();

                //digest = md.Digest(Encoding.ASCII.GetBytes("abc"));
                //Assert.IsTrue(BitHelper.BytesToHex(digest) == "f40245973e80d79d0f4b9b202ddd4505b81b8830501bea31612b5817aae387921dcefd808ca2c78020aff59345d6f91f0ee6b2eee113f0cbcf22b64381387e8a");
                //md.Reset();
            };
        }

        [TestMethod]
        public void NistTest()
        {
            List<byte> list = new List<byte>();
            foreach (var line in File.ReadAllLines("MessageDigest/NISTData.txt"))
                list.AddRange(BitHelper.HexToBytes(line));
            byte[] data = list.ToArray();

            string[] digests = File.ReadAllLines("MessageDigest/Luffa512.txt");

            foreach (var provider in _providers)
            {
                var md = provider.GetMessageDigest(MessageDigests.Luffa512);

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