using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.BlockCipher;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    [TestClass]
    public class CamelliaTests
    {
        [TestMethod]
        public void Key128_1()
        {
            TestEncryptDecrypt(
                "0123456789abcdeffedcba9876543210",
                "0123456789abcdeffedcba9876543210",
                "67673138549669730857065648eabe43");
        }

        [TestMethod]
        public void Key192_1()
        {
            TestEncryptDecrypt(
                "0123456789abcdeffedcba98765432100011223344556677",
                "0123456789abcdeffedcba9876543210",
                "b4993401b3e996f84ee5cee7d79b09b9");
        }

        [TestMethod]
        public void Key256_1()
        {
            TestEncryptDecrypt(
                "0123456789abcdeffedcba987654321000112233445566778899aabbccddeeff",
                "0123456789abcdeffedcba9876543210",
                "9acc237dff16d76c20ef7c919e3a7509");
        }

        private static void TestEncryptDecrypt(string key, string plaintext, string ciphertext)
        {
            byte[] bkey = BitHelper.HexToBytes(key);
            byte[] bplain = BitHelper.HexToBytes(plaintext);
            byte[] bcipher = BitHelper.HexToBytes(ciphertext);

            Camellia cam;
            switch (bkey.Length * 8)
            {
                case 128: cam = new Camellia128(); break;
                case 192: cam = new Camellia192(); break;
                case 256: cam = new Camellia256(); break;
                default:
                    throw new InvalidOperationException();
            }

            var enc = cam.CreateEncryptor(bkey, BlockCipherMode.ECB);
            Assert.IsTrue(bcipher.SequenceEqual(enc.Encrypt(bplain)));

            var dec = cam.CreateDecryptor(bkey, BlockCipherMode.ECB);
            Assert.IsTrue(bplain.SequenceEqual(dec.Decrypt(bcipher)));
        }
    }
}
