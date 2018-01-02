using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Symmetric.CipherMode;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    [TestClass]
    public class Poly1305KeyGeneratorTests
    {
        [TestMethod]
        public void Poly1305KeyGenerator_Vector1()
        {
            byte[] key = BitHelper.HexToBytes("80 81 82 83 84 85 86 87 88 89 8a 8b 8c 8d 8e 8f 90 91 92 93 94 95 96 97 98 99 9a 9b 9c 9d 9e 9f".Replace(" ", ""));
            byte[] nonce = BitHelper.HexToBytes("00 00 00 00 00 01 02 03 04 05 06 07".Replace(" ", ""));

            var generator = new Poly1305KeyGenerator();
            var poly1305key = generator.GenerateKey(key, nonce);

            Assert.IsTrue(BitHelper.BytesToHex(poly1305key) == "8a d5 a0 8b 90 5f 81 cc 81 50 40 27 4a b2 94 71 a8 33 b6 37 e3 fd 0d a5 08 db b8 e2 fd d1 a6 46".Replace(" ", ""));
        }
    }
}