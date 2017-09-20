using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Symmetric;

namespace Zergatul.Cryptography.Tests.Symmetric
{
    [TestClass]
    public class TwofishTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            TestEncryptDecrypt(
                "00000000000000000000000000000000",
                "00000000000000000000000000000000",
                "00000000000000000000000000000000");
        }

        private static void TestEncryptDecrypt(string key, string plaintext, string ciphertext)
        {
            byte[] bkey = BitHelper.HexToBytes(key.Replace(" ", ""));
            byte[] bplain = BitHelper.HexToBytes(plaintext.Replace(" ", ""));
            byte[] bcipher = BitHelper.HexToBytes(ciphertext.Replace(" ", ""));

            var tf = new Twofish(bkey.Length);

            var enc = tf.CreateEncryptor(bkey);
            Assert.IsTrue(bcipher.SequenceEqual(enc(bplain)));

            var dec = tf.CreateDecryptor(bkey);
            Assert.IsTrue(bplain.SequenceEqual(dec(bcipher)));
        }
    }
}