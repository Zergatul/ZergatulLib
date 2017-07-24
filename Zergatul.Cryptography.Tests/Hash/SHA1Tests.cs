using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Hash;
using System.Text;
using System.Linq;

namespace Zergatul.Cryptography.Tests.Hash
{
    [TestClass]
    public class SHA1Tests
    {
        [TestMethod]
        public void Test1()
        {
            Assert.IsTrue(Hash("") == "da39a3ee5e6b4b0d3255bfef95601890afd80709");
        }

        [TestMethod]
        public void Test2()
        {
            Assert.IsTrue(Hash(Enumerable.Repeat((byte)7, 256).ToArray()) == "fd82fec504aac0efa6f4f4a89e09441cb6fd6a5b");
        }

        private static string Hash(byte[] input) => Helper.Hash<SHA1>(input);

        private static string Hash(string input) => Helper.Hash<SHA1>(input);
    }
}
