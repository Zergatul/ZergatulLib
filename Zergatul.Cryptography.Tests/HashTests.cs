using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Hash;
using System.Text;

namespace Zergatul.Cryptography.Tests
{
    [TestClass]
    public class HashTests
    {
        [TestMethod]
        public void SHA224_1()
        {
            var hash = new SHA224();

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("d14a028c2a3a2bc9476102bb288234c415a2b01f828ea62ac5b3e42f");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void SHA224_2()
        {
            var hash = new SHA224();
            hash.Update(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("730E109BD7A8A32B1CB9D9A09AA2325D2430587DDBC0C38BAD911525");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void SHA256_1()
        {
            var hash = new SHA256();
            hash.Update(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("D7A8FBB307D7809469CA9ABCB0082E4F8D5651E46D3CDB762D02D0BF37C9E592");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void SHA256_2()
        {
            var hash = new SHA256();

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void SHA256_3()
        {
            var hash = new SHA256();
            hash.Update(Enumerable.Repeat((byte)7, 256).ToArray());

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("8a008a5fca6cac16762abfcc2641c6cdcf82478406871e00f7e86d78884c4192");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void SHA384_1()
        {
            var hash = new SHA384();

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void SHA512_1()
        {
            var hash = new SHA512();

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void SHA1_1()
        {
            var hash = new SHA1();

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("da39a3ee5e6b4b0d3255bfef95601890afd80709");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void SHA1_2()
        {
            var hash = new SHA1();
            hash.Update(Enumerable.Repeat((byte)7, 256).ToArray());

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("fd82fec504aac0efa6f4f4a89e09441cb6fd6a5b");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void MD5_1()
        {
            var hash = new MD5();

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("d41d8cd98f00b204e9800998ecf8427e");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void MD5_2()
        {
            var hash = new MD5();
            hash.Update(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("9e107d9d372bb6826bd81d3542a419d6");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void MD5_3()
        {
            var hash = new MD5();
            hash.Update(Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog."));

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("e4d909c290d0fb1ca068ffaddf22cbd0");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }
    }
}
