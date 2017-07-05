using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Tests
{
    [TestClass]
    public class HashTests
    {
        [TestMethod]
        public void SHA256_1()
        {
            var hash = new SHA256();

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
            Assert.IsTrue(computed.SequenceEqual(correct));
        }

        [TestMethod]
        public void SHA256_2()
        {
            var hash = new SHA256();
            hash.Update(Enumerable.Repeat((byte)7, 256).ToArray());

            byte[] computed = hash.ComputeHash();
            byte[] correct = BitHelper.HexToBytes("8a008a5fca6cac16762abfcc2641c6cdcf82478406871e00f7e86d78884c4192");
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
    }
}
