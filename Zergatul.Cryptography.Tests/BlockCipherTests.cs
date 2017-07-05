using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.BlockCipher;

namespace Zergatul.Cryptography.Tests
{
    [TestClass]
    public class BlockCipherTests
    {
        [TestMethod]
        public void AES128_Enc_1()
        {
            var aes = new AES128();

            var enc = aes.CreateEncryptor(BitHelper.HexToBytes("2b7e151628aed2a6abf7158809cf4f3c"), BlockCipherMode.ECB);
            var result = enc.Encrypt(null, BitHelper.HexToBytes("3243f6a8885a308d313198a2e0370734"));
            var expected = BitHelper.HexToBytes("3925841d02dc09fbdc118597196a0b32");

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void AES128_Enc_2()
        {
            var aes = new AES128();

            var enc = aes.CreateEncryptor(BitHelper.HexToBytes("000102030405060708090a0b0c0d0e0f"), BlockCipherMode.ECB);
            var result = enc.Encrypt(null, BitHelper.HexToBytes("00112233445566778899aabbccddeeff"));
            var expected = BitHelper.HexToBytes("69c4e0d86a7b0430d8cdb78070b4c55a");

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void AES128_Dec_2()
        {
            var aes = new AES128();

            var dec = aes.CreateDecryptor(BitHelper.HexToBytes("000102030405060708090a0b0c0d0e0f"), BlockCipherMode.ECB);
            var result = dec.Decrypt(null, BitHelper.HexToBytes("69c4e0d86a7b0430d8cdb78070b4c55a"));
            var expected = BitHelper.HexToBytes("00112233445566778899aabbccddeeff");

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void AES192_Enc_1()
        {
            var aes = new AES192();

            var enc = aes.CreateEncryptor(BitHelper.HexToBytes("000102030405060708090a0b0c0d0e0f1011121314151617"), BlockCipherMode.ECB);
            var result = enc.Encrypt(null, BitHelper.HexToBytes("00112233445566778899aabbccddeeff"));
            var expected = BitHelper.HexToBytes("dda97ca4864cdfe06eaf70a0ec0d7191");

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void AES192_Dec_1()
        {
            var aes = new AES192();

            var dec = aes.CreateDecryptor(BitHelper.HexToBytes("000102030405060708090a0b0c0d0e0f1011121314151617"), BlockCipherMode.ECB);
            var result = dec.Decrypt(null, BitHelper.HexToBytes("dda97ca4864cdfe06eaf70a0ec0d7191"));
            var expected = BitHelper.HexToBytes("00112233445566778899aabbccddeeff");

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void AES256_Enc_1()
        {
            var aes = new AES256();

            var enc = aes.CreateEncryptor(BitHelper.HexToBytes("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f"), BlockCipherMode.ECB);
            var result = enc.Encrypt(null, BitHelper.HexToBytes("00112233445566778899aabbccddeeff"));
            var expected = BitHelper.HexToBytes("8ea2b7ca516745bfeafc49904b496089");

            Assert.IsTrue(result.SequenceEqual(expected));
        }

        [TestMethod]
        public void AES256_Dec_1()
        {
            var aes = new AES256();

            var dec = aes.CreateDecryptor(BitHelper.HexToBytes("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f"), BlockCipherMode.ECB);
            var result = dec.Decrypt(null, BitHelper.HexToBytes("8ea2b7ca516745bfeafc49904b496089"));
            var expected = BitHelper.HexToBytes("00112233445566778899aabbccddeeff");

            Assert.IsTrue(result.SequenceEqual(expected));
        }
    }
}
