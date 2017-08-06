using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Hash;
using System.Linq;

namespace Zergatul.Cryptography.Tests.Hash
{
    [TestClass]
    public class SHA2Tests
    {
        [TestMethod]
        public void Test224()
        {
            Assert.IsTrue(Hash224("") ==
                "d14a028c2a3a2bc9476102bb288234c415a2b01f828ea62ac5b3e42f");
            Assert.IsTrue(Hash224("The quick brown fox jumps over the lazy dog") ==
                "730e109bd7a8a32b1cb9d9a09aa2325d2430587ddbc0c38bad911525");
        }

        [TestMethod]
        public void Test256()
        {
            Assert.IsTrue(Hash256("") ==
                "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
            Assert.IsTrue(Hash256("The quick brown fox jumps over the lazy dog") ==
                "d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592");
            Assert.IsTrue(Hash256(Enumerable.Repeat((byte)7, 256).ToArray()) ==
                "8a008a5fca6cac16762abfcc2641c6cdcf82478406871e00f7e86d78884c4192");
        }

        [TestMethod]
        public void Test384()
        {
            Assert.IsTrue(Hash384("") ==
                "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b");
            Assert.IsTrue(Hash384("abc") ==
                "cb00753f45a35e8bb5a03d699ac65007272c32ab0eded1631a8b605a43ff5bed8086072ba1e7cc2358baeca134c825a7");
            Assert.IsTrue(Hash384("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq") ==
                "3391fdddfc8dc7393707a65b1b4709397cf8b1d162af05abfe8f450de5f36bc6b0455a8520bc4e6f5fe95b1fe3c8452b");
            Assert.IsTrue(Hash384("abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmnhijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu") ==
                "09330c33f71147e83d192fc782cd1b4753111b173b3b05d22fa08086e3b0f712fcc7c71a557e2db966c3e9fa91746039");
            Assert.IsTrue(Hash384(new string('a', 1000000)) ==
                "9d0e1809716474cb086e834e310a4a1ced149e9c00f248527972cec5704c2a5b07b8b3dc38ecc4ebae97ddd87f3d8985");
        }

        [TestMethod]
        public void Test512()
        {
            Assert.IsTrue(Hash512("") ==
                "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e");
        }

        private static string Hash224(string input) => Helper.Hash<SHA224>(input);
        private static string Hash256(string input) => Helper.Hash<SHA256>(input);
        private static string Hash256(byte[] input) => Helper.Hash<SHA256>(input);
        private static string Hash384(string input) => Helper.Hash<SHA384>(input);
        private static string Hash512(string input) => Helper.Hash<SHA512>(input);
    }
}