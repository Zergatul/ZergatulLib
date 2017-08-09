using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.BlockCipher;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    [TestClass]
    public class AESTests
    {
        [TestMethod]
        public void Key128_1()
        {
            TestEncryptDecrypt(
                "2b7e151628aed2a6abf7158809cf4f3c",
                "3243f6a8885a308d313198a2e0370734",
                "3925841d02dc09fbdc118597196a0b32");
        }

        [TestMethod]
        public void Key128_2()
        {
            TestEncryptDecrypt(
                "000102030405060708090a0b0c0d0e0f",
                "00112233445566778899aabbccddeeff",
                "69c4e0d86a7b0430d8cdb78070b4c55a");
        }

        [TestMethod]
        public void Key192_1()
        {
            TestEncryptDecrypt(
                "000102030405060708090a0b0c0d0e0f1011121314151617",
                "00112233445566778899aabbccddeeff",
                "dda97ca4864cdfe06eaf70a0ec0d7191");
        }

        [TestMethod]
        public void Key256_1()
        {
            TestEncryptDecrypt(
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
                "00112233445566778899aabbccddeeff",
                "8ea2b7ca516745bfeafc49904b496089");
        }

        private static void TestEncryptDecrypt(string key, string plaintext, string ciphertext)
        {
            byte[] bkey = BitHelper.HexToBytes(key);
            byte[] bplain = BitHelper.HexToBytes(plaintext);
            byte[] bcipher = BitHelper.HexToBytes(ciphertext);

            AES aes;
            switch (bkey.Length * 8)
            {
                case 128: aes = new AES128(); break;
                case 192: aes = new AES192(); break;
                case 256: aes = new AES256(); break;
                default:
                    throw new InvalidOperationException();
            }

            var enc = aes.CreateEncryptor(bkey);
            Assert.IsTrue(bcipher.SequenceEqual(enc(bplain)));

            var dec = aes.CreateDecryptor(bkey);
            Assert.IsTrue(bplain.SequenceEqual(dec(bcipher)));
        }
    }
}