using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.BlockCipher;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    [TestClass]
    public class RC2Tests
    {
        [TestMethod]
        public void Test_1_64_Key()
        {
            TestEncryptDecrypt("88", 64, "0000000000000000", "61a8a244adacccf0");
        }

        [TestMethod]
        public void Test_7_64_Key()
        {
            TestEncryptDecrypt("88bca90e90875a", 64, "0000000000000000", "6ccf4308974c267f");
        }

        [TestMethod]
        public void Test_8_63_Key()
        {
            TestEncryptDecrypt("0000000000000000", 63, "0000000000000000", "ebb773f993278eff");
        }

        [TestMethod]
        public void Test_8_64_Key()
        {
            TestEncryptDecrypt("ffffffffffffffff", 64, "ffffffffffffffff", "278b27e42e2f0d49");
        }

        [TestMethod]
        public void Test_16_64_Key()
        {
            TestEncryptDecrypt("88bca90e90875a7f0f79c384627bafb2", 64, "0000000000000000", "1a807d272bbe5db1");
        }

        [TestMethod]
        public void Test_16_128_Key()
        {
            TestEncryptDecrypt("88bca90e90875a7f0f79c384627bafb2", 128, "0000000000000000", "2269552ab0f85ca6");
        }

        [TestMethod]
        public void Test_33_129_Key()
        {
            TestEncryptDecrypt("88bca90e90875a7f0f79c384627bafb216f80a6f85920584c42fceb0be255daf1e", 129, "0000000000000000", "5b78d3a43dfff1f1");
        }

        private static void TestEncryptDecrypt(string key, int bits, string plaintext, string ciphertext)
        {
            byte[] bkey = BitHelper.HexToBytes(key);
            byte[] bplain = BitHelper.HexToBytes(plaintext);
            byte[] bcipher = BitHelper.HexToBytes(ciphertext);

            var rc2 = new RC2(bkey.Length, bits);

            var enc = rc2.CreateEncryptor(bkey, BlockCipherMode.ECB);
            Assert.IsTrue(bcipher.SequenceEqual(enc.Encrypt(bplain)));

            var dec = rc2.CreateDecryptor(bkey, BlockCipherMode.ECB);
            Assert.IsTrue(bplain.SequenceEqual(dec.Decrypt(bcipher)));
        }
    }
}