using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.BlockCipher.CipherMode;
using Zergatul.Cryptography.BlockCipher;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    // http://www.ieee802.org/1/files/public/docs2011/bn-randall-test-vectors-0511-v1.pdf
    // https://pdfs.semanticscholar.org/114a/4222c53f1a6879f1a77f1bae2fc0f8f55348.pdf
    [TestClass]
    public class GCMTests
    {
        [TestMethod]
        public void TestCase1()
        {
            byte[] key = BitHelper.HexToBytes("00000000000000000000000000000000");
            byte[] plain = BitHelper.HexToBytes("");
            byte[] iv = BitHelper.HexToBytes("000000000000000000000000");
            byte[] a = BitHelper.HexToBytes("");

            var aes = new AES128();

            var gcm = new GCM();
            gcm.TagLength = 16;
            var enc = gcm.CreateEncryptor(aes, aes.CreateEncryptor(key));

            var data = enc.Encrypt(iv, plain, a);

            Assert.IsTrue(BitHelper.BytesToHex(data.CipherText) == "");
            Assert.IsTrue(BitHelper.BytesToHex(data.Tag) == "58e2fccefa7e3061367f1d57a4e7455a");
        }

        [TestMethod]
        public void TestCase2()
        {
            byte[] key = BitHelper.HexToBytes("00000000000000000000000000000000");
            byte[] plain = BitHelper.HexToBytes("00000000000000000000000000000000");
            byte[] iv = BitHelper.HexToBytes("000000000000000000000000");
            byte[] a = BitHelper.HexToBytes("");

            var aes = new AES128();

            var gcm = new GCM();
            gcm.TagLength = 16;
            var enc = gcm.CreateEncryptor(aes, aes.CreateEncryptor(key));

            var data = enc.Encrypt(iv, plain, a);

            Assert.IsTrue(BitHelper.BytesToHex(data.CipherText) == "0388dace60b6a392f328c2b971b2fe78");
            Assert.IsTrue(BitHelper.BytesToHex(data.Tag) == "ab6e47d42cec13bdf53a67b21257bddf");
        }

        //[TestMethod]
        //public void AES128_IV96_A560()
        //{
        //    byte[] key = BitHelper.HexToBytes("AD7A2BD03EAC835A6F620FDCB506B345");
        //    byte[] plain = BitHelper.HexToBytes("");
        //    byte[] iv = BitHelper.HexToBytes("12153524C0895E81B2C28465");
        //    byte[] a = BitHelper.HexToBytes("D609B1F056637A0D46DF998D88E5222AB2C2846512153524C0895E8108000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F30313233340001");

        //    var aes = new AES128();

        //    var gcm = new GCM();
        //    gcm.TagLength = 16;
        //    var enc = gcm.CreateEncryptor(aes, aes.CreateEncryptor(key));

        //    var cipher = enc.Encrypt(iv, plain, a);

        //    Assert.IsTrue(BitHelper.BytesToHex(cipher) == "f09478a9b09007d06f46e9b6a1da25dd");
        //}
    }
}