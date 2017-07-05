using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.BlockCipher;

namespace Zergatul.Cryptography.Tests
{
    [TestClass]
    public class BlockCipherTests
    {
        [TestMethod]
        public void AES128_1()
        {
            var aes = new AES128();
            var enc = aes.CreateEncryptor(BitHelper.HexToBytes("2b7e151628aed2a6abf7158809cf4f3c"), BlockCipherMode.CBC);
            enc.Encrypt(BitHelper.HexToBytes("3243f6a8885a308d313198a2e0370734"), new byte[16]);
        }
    }
}
