using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Zergatul.Cryptography.Hash;
using System.Linq;

namespace Zergatul.Cryptography.Tests
{
    [TestClass]
    public class RIPEMDTests
    {
        [TestMethod]
        [Ignore]
        public void Test128()
        {
            Assert.IsTrue(Hash(128, "") == "cdf26213a150dc3ecb610f18f6b38b46");
            Assert.IsTrue(Hash(128, "a") == "86be7afa339d0fc7cfc785e72f578d33");
            Assert.IsTrue(Hash(128, "abc") == "c14a12199c66e4ba84636b0f69144c77");
            Assert.IsTrue(Hash(128, "message digest") == "9e327b3d6e523062afc1132d7df9d1b8");
            Assert.IsTrue(Hash(128, "abcdefghijklmnopqrstuvwxyz") == "fd2aa607f71dc8f510714922b371834e");
            Assert.IsTrue(Hash(128, "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq") == "a1aa0689d0fafa2ddc22e88b49133a06");
            Assert.IsTrue(Hash(128, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789") == "d1e959eb179c911faea4624c60c5c702");
            Assert.IsTrue(Hash(128, string.Concat(Enumerable.Repeat("1234567890", 8))) == "3f45ef194732c2dbb2c4a2c769795fa3");
            Assert.IsTrue(Hash(128, new string('a', 1000000)) == "4a7f5723f954eba1216c9d8f6320431f");
        }

        [TestMethod]
        public void Test160()
        {
            Assert.IsTrue(Hash(160, "") == "9c1185a5c5e9fc54612808977ee8f548b2258d31");
            Assert.IsTrue(Hash(160, "a") == "0bdc9d2d256b3ee9daae347be6f4dc835a467ffe");
            Assert.IsTrue(Hash(160, "abc") == "8eb208f7e05d987a9b044a8e98c6b087f15a0bfc");
            Assert.IsTrue(Hash(160, "message digest") == "5d0689ef49d2fae572b881b123a85ffa21595f36");
            Assert.IsTrue(Hash(160, "abcdefghijklmnopqrstuvwxyz") == "f71c27109c692c1b56bbdceb5b9d2865b3708dbc");
            Assert.IsTrue(Hash(160, "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq") == "12a053384a9c0c88e405a06c27dcf49ada62eb2b");
            Assert.IsTrue(Hash(160, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789") == "b0e20b6e3116640286ed3a87a5713079b21f5189");
            Assert.IsTrue(Hash(160, string.Concat(Enumerable.Repeat("1234567890", 8))) == "9b752e45573d4b39f4dbd3323cab82bf63326bfb");
            Assert.IsTrue(Hash(160, new string('a', 1000000)) == "52783243c1697bdbe16d37f97f68f08325dc1528");

            Assert.IsTrue(Hash(160, "The quick brown fox jumps over the lazy dog") == "37f332f68db77bd9d7edd4969571ad671cf9dd3b");
            Assert.IsTrue(Hash(160, "The quick brown fox jumps over the lazy cog") == "132072df690933835eb8b6ad0b77e7b6f14acad7");
        }

        [TestMethod]
        [Ignore]
        public void Test256()
        {
            Assert.IsTrue(Hash(256, "") == "02ba4c4e5f8ecd1877fc52d64d30e37a2d9774fb1e5d026380ae0168e3c5522d");
            Assert.IsTrue(Hash(256, "a") == "f9333e45d857f5d90a91bab70a1eba0cfb1be4b0783c9acfcd883a9134692925");
            Assert.IsTrue(Hash(256, "abc") == "afbd6e228b9d8cbbcef5ca2d03e6dba10ac0bc7dcbe4680e1e42d2e975459b65");
            Assert.IsTrue(Hash(256, "message digest") == "87e971759a1ce47a514d5c914c392c9018c7c46bc14465554afcdf54a5070c0e");
            Assert.IsTrue(Hash(256, "abcdefghijklmnopqrstuvwxyz") == "649d3034751ea216776bf9a18acc81bc7896118a5197968782dd1fd97d8d5133");
            Assert.IsTrue(Hash(256, "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq") == "3843045583aac6c8c8d9128573e7a9809afb2a0f34ccc36ea9e72f16f6368e3f");
            Assert.IsTrue(Hash(256, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789") == "5740a408ac16b720b84424ae931cbb1fe363d1d0bf4017f1a89f7ea6de77a0b8");
            Assert.IsTrue(Hash(256, string.Concat(Enumerable.Repeat("1234567890", 8))) == "06fdcc7a409548aaf91368c06a6275b553e3f099bf0ea4edfd6778df89a890dd");
            Assert.IsTrue(Hash(256, new string('a', 1000000)) == "ac953744e10e31514c150d4d8d7b677342e33399788296e43ae4850ce4f97978");
        }

        [TestMethod]
        [Ignore]
        public void Test320()
        {
            Assert.IsTrue(Hash(320, "") == "22d65d5661536cdc75c1fdf5c6de7b41b9f27325ebc61e8557177d705a0ec880151c3a32a00899b8");
            Assert.IsTrue(Hash(320, "a") == "ce78850638f92658a5a585097579926dda667a5716562cfcf6fbe77f63542f99b04705d6970dff5d");
            Assert.IsTrue(Hash(320, "abc") == "de4c01b3054f8930a79d09ae738e92301e5a17085beffdc1b8d116713e74f82fa942d64cdbc4682d");
            Assert.IsTrue(Hash(320, "message digest") == "3a8e28502ed45d422f68844f9dd316e7b98533fa3f2a91d29f84d425c88d6b4eff727df66a7c0197");
            Assert.IsTrue(Hash(320, "abcdefghijklmnopqrstuvwxyz") == "cabdb1810b92470a2093aa6bce05952c28348cf43ff60841975166bb40ed234004b8824463e6b009");
            Assert.IsTrue(Hash(320, "abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq") == "d034a7950cf722021ba4b84df769a5de2060e259df4c9bb4a4268c0e935bbc7470a969c9d072a1ac");
            Assert.IsTrue(Hash(320, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789") == "ed544940c86d67f250d232c30b7b3e5770e0c60c8cb9a4cafe3b11388af9920e1b99230b843c86a4");
            Assert.IsTrue(Hash(320, string.Concat(Enumerable.Repeat("1234567890", 8))) == "557888af5f6d8ed62ab66945c6d2a0a47ecd5341e915eb8fea1d0524955f825dc717e4a008ab2d42");
            Assert.IsTrue(Hash(320, new string('a', 1000000)) == "bdee37f4371e20646b8b0d862dda16292ae36f40965e8c8509e63d1dbddecc503e2b63eb9245bb66");
        }

        private static string Hash(int bits, string input)
        {
            AbstractHash ripemd;
            switch (bits)
            {
                case 160: ripemd = new RIPEMD160(); break;
                default:
                    throw new NotImplementedException();
            }
            ripemd.Update(Encoding.ASCII.GetBytes(input));
            return BitHelper.BytesToHex(ripemd.ComputeHash());
        }
    }
}
