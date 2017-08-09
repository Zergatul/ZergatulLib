using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.BlockCipher;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    [TestClass]
    public class ARIATests
    {
        [TestMethod]
        public void Key128_1()
        {
            TestEncryptDecrypt(
                "000102030405060708090a0b0c0d0e0f",
                "00112233445566778899aabbccddeeff",
                "d718fbd6ab644c739da95f3be6451778");
        }

        [TestMethod]
        public void Key192_1()
        {
            TestEncryptDecrypt(
                "000102030405060708090a0b0c0d0e0f1011121314151617",
                "00112233445566778899aabbccddeeff",
                "26449c1805dbe7aa25a468ce263a9e79");
        }

        [TestMethod]
        public void Key256_1()
        {
            TestEncryptDecrypt(
                "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
                "00112233445566778899aabbccddeeff",
                "f92bd7c79fb72e2f2b8f80c1972d24fc");
        }

        private static void TestEncryptDecrypt(string key, string plaintext, string ciphertext)
        {
            byte[] bkey = BitHelper.HexToBytes(key);
            byte[] bplain = BitHelper.HexToBytes(plaintext);
            byte[] bcipher = BitHelper.HexToBytes(ciphertext);

            ARIA aria;
            switch (bkey.Length * 8)
            {
                case 128: aria = new ARIA128(); break;
                case 192: aria = new ARIA192(); break;
                case 256: aria = new ARIA256(); break;
                default:
                    throw new InvalidOperationException();
            }

            var enc = aria.CreateEncryptor(bkey);
            Assert.IsTrue(bcipher.SequenceEqual(enc(bplain)));

            var dec = aria.CreateDecryptor(bkey);
            Assert.IsTrue(bplain.SequenceEqual(dec(bcipher)));
        }
    }
}
