using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Tests.Hash
{
    [TestClass]
    public class MD5Tests
    {
        [TestMethod]
        public void Test1()
        {
            Assert.IsTrue(Hash("") == "d41d8cd98f00b204e9800998ecf8427e");
        }

        [TestMethod]
        public void Test2()
        {
            Assert.IsTrue(Hash("The quick brown fox jumps over the lazy dog") == "9e107d9d372bb6826bd81d3542a419d6");
        }

        [TestMethod]
        public void Test3()
        {
            Assert.IsTrue(Hash("The quick brown fox jumps over the lazy dog.") == "e4d909c290d0fb1ca068ffaddf22cbd0");
        }

        private static string Hash(string input) => Helper.Hash<MD5>(input);
    }
}
