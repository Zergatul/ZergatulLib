using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.BlockCipher;

namespace Zergatul.Cryptography.Tests
{
    [TestClass]
    public class ARIATests
    {
        [TestMethod]
        public void Key128_1()
        {
            byte[] key = BitHelper.HexToBytes("000102030405060708090a0b0c0d0e0f");
            string plaintext = "00112233445566778899aabbccddeeff";
            var aria = new ARIA128();

            var enc = aria.CreateEncryptor(key, BlockCipherMode.ECB);
            var ciphertext = enc.Encrypt(null, BitHelper.HexToBytes(plaintext));
            Assert.IsTrue(BitHelper.BytesToHex(ciphertext) == "d718fbd6ab644c739da95f3be6451778");

            var dec = aria.CreateDecryptor(key, BlockCipherMode.ECB);
            var plaintext2 = dec.Decrypt(null, ciphertext);
            Assert.IsTrue(BitHelper.BytesToHex(plaintext2) == plaintext);
        }

        [TestMethod]
        public void Key192_1()
        {
            byte[] key = BitHelper.HexToBytes("000102030405060708090a0b0c0d0e0f1011121314151617");
            string plaintext = "00112233445566778899aabbccddeeff";
            var aria = new ARIA192();

            var enc = aria.CreateEncryptor(key, BlockCipherMode.ECB);
            var ciphertext = enc.Encrypt(null, BitHelper.HexToBytes(plaintext));
            Assert.IsTrue(BitHelper.BytesToHex(ciphertext) == "26449c1805dbe7aa25a468ce263a9e79");

            var dec = aria.CreateDecryptor(key, BlockCipherMode.ECB);
            var plaintext2 = dec.Decrypt(null, ciphertext);
            Assert.IsTrue(BitHelper.BytesToHex(plaintext2) == plaintext);
        }

        [TestMethod]
        public void Key256_1()
        {
            byte[] key = BitHelper.HexToBytes("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f");
            string plaintext = "00112233445566778899aabbccddeeff";
            var aria = new ARIA256();

            var enc = aria.CreateEncryptor(key, BlockCipherMode.ECB);
            var ciphertext = enc.Encrypt(null, BitHelper.HexToBytes(plaintext));
            Assert.IsTrue(BitHelper.BytesToHex(ciphertext) == "f92bd7c79fb72e2f2b8f80c1972d24fc");

            var dec = aria.CreateDecryptor(key, BlockCipherMode.ECB);
            var plaintext2 = dec.Decrypt(null, ciphertext);
            Assert.IsTrue(BitHelper.BytesToHex(plaintext2) == plaintext);
        }
    }
}
