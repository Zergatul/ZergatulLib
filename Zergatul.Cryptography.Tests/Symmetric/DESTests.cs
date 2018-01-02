using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Symmetric;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    [TestClass]
    public class DESTests
    {
        [TestMethod]
        public void DES_Test1()
        {
            TestEncryptDecrypt("133457799BBCDFF1", "0123456789abcdef", "85e813540f0ab405");
        }

        [TestMethod]
        public void DES_Test2()
        {
            TestEncryptDecrypt("8000000000000000", "0000000000000000", "95A8D72813DAA94D");
            TestEncryptDecrypt("8000000000000000", "0000000000000000", "F749E1F8DEFAF605", 100);
            TestEncryptDecrypt("8000000000000000", "0000000000000000", "F396DD0B33D04244", 1000);
        }

        [TestMethod]
        public void DES_Test3()
        {
            TestEncryptDecrypt("4000000000000000", "0000000000000000", "0EEC1487DD8C26D5");
            TestEncryptDecrypt("4000000000000000", "0000000000000000", "E5BEE86B600F3B48", 100);
            TestEncryptDecrypt("4000000000000000", "0000000000000000", "1D5931D700EF4E15", 1000);
        }

        [TestMethod]
        public void DES_Test4()
        {
            TestEncryptDecrypt("E6E6E6E6E6E6E6E6", "E6E6E6E6E6E6E6E6", "8A8DD870C9B14AF2");
            TestEncryptDecrypt("E6E6E6E6E6E6E6E6", "E6E6E6E6E6E6E6E6", "C4259776E0BEE1D8", 100);
            TestEncryptDecrypt("E6E6E6E6E6E6E6E6", "E6E6E6E6E6E6E6E6", "5208CA02F7FB142D", 1000);
        }

        private static void TestEncryptDecrypt(string key, string plaintext, string ciphertext)
        {
            byte[] bkey = BitHelper.HexToBytes(key);
            byte[] bplain = BitHelper.HexToBytes(plaintext);
            byte[] bcipher = BitHelper.HexToBytes(ciphertext);

            var des = new DES();

            var enc = des.CreateEncryptor(bkey);
            Assert.IsTrue(bcipher.SequenceEqual(enc(bplain)));

            var dec = des.CreateDecryptor(bkey);
            Assert.IsTrue(bplain.SequenceEqual(dec(bcipher)));
        }

        private static void TestEncryptDecrypt(string key, string plaintext, string ciphertext, int iterations)
        {
            byte[] bkey = BitHelper.HexToBytes(key);
            byte[] bplain = BitHelper.HexToBytes(plaintext);
            byte[] bcipher = BitHelper.HexToBytes(ciphertext);

            var des = new DES();

            var enc = des.CreateEncryptor(bkey);
            byte[] cipher = bplain;
            for (int i = 0; i < iterations; i++)
                cipher = enc(cipher);
            Assert.IsTrue(bcipher.SequenceEqual(cipher));

            var dec = des.CreateDecryptor(bkey);
            byte[] plain = cipher;
            for (int i = 0; i < iterations; i++)
                plain = dec(plain);
            Assert.IsTrue(bplain.SequenceEqual(plain));
        }
    }
}