using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Symmetric;
using System.Collections.Generic;

namespace Zergatul.Cryptography.Tests.Symmetric
{
    [TestClass]
    public class TwofishTests
    {
        [TestMethod]
        public void Twofish_SchneierTests()
        {
            TestEncryptDecrypt(
                "00000000000000000000000000000000",
                "00000000000000000000000000000000",
                "9F589F5CF6122C32B6BFEC2F2AE8C35A");

            TestEncryptDecrypt(
                "0123456789ABCDEFFEDCBA98765432100011223344556677",
                "00000000000000000000000000000000",
                "CFD1D2E5A9BE9CDF501F13B892BD2248");

            TestEncryptDecrypt(
                "0123456789ABCDEFFEDCBA987654321000112233445566778899AABBCCDDEEFF",
                "00000000000000000000000000000000",
                "37527BE0052334B89F0CFCCAE87CFA20");
        }

        [TestMethod]
        public void Twofish_SchneierCycleTests()
        {
            TestCycleEncryptDecrypt(49, 16, "5D9D4EEFFA9151575524F115815A12E0");
            TestCycleEncryptDecrypt(49, 24, "E75449212BEEF9F4A390BD860A640941");
            TestCycleEncryptDecrypt(49, 32, "37FE26FF1CF66175F5DDF4C33B97A205");
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

        private static void TestCycleEncryptDecrypt(int cycles, int keylen, string ciphertext)
        {
            byte[] zeros = new byte[keylen];
            byte[] bcipher = BitHelper.HexToBytes(ciphertext.Replace(" ", ""));

            List<byte[]> keys = new List<byte[]> { zeros, zeros, zeros };
            List<byte[]> plaintexts = new List<byte[]> { zeros };

            var tf = new Twofish(keylen);

            for (int i = 0; i < cycles; i++)
            {
                byte[] key = ByteArray.SubArray(ByteArray.Concat(keys[keys.Count - 2], keys[keys.Count - 3]), 0, keylen);
                var enc = tf.CreateEncryptor(key);
                var data = enc(plaintexts[plaintexts.Count - 1]);
                keys.Add(data);
                plaintexts.Add(data);
            }

            Assert.IsTrue(keys.Last().SequenceEqual(bcipher));
        }
    }
}