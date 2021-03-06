﻿using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zergatul.Security.Tests.KeyDerivationFunction
{
    [TestClass]
    public class ScryptTests
    {
        private static SecurityProvider[] _providers = new SecurityProvider[]
        {
            new DefaultSecurityProvider(),
            new OpenSslProvider()
        };

        [TestMethod]
        public void RFCTest1()
        {
            Test("", "", 16, 1, 1,
                "77 d6 57 62 38 65 7b 20 3b 19 ca 42 c1 8a 04 97" +
                "f1 6b 48 44 e3 07 4a e8 df df fa 3f ed e2 14 42" +
                "fc d0 06 9d ed 09 48 f8 32 6a 75 3a 0f c8 1f 17" +
                "e8 d3 e0 fb 2e 0d 36 28 cf 35 e2 0c 38 d1 89 06");
        }

        [TestMethod]
        public void RFCTest2()
        {
            Test("password", "NaCl", 1024, 8, 16,
                "fd ba be 1c 9d 34 72 00 78 56 e7 19 0d 01 e9 fe" +
                "7c 6a d7 cb c8 23 78 30 e7 73 76 63 4b 37 31 62" +
                "2e af 30 d9 2e 22 a3 88 6f f1 09 27 9d 98 30 da" +
                "c7 27 af b9 4a 83 ee 6d 83 60 cb df a2 cc 06 40");
        }

        [TestMethod]
        public void RFCTest3()
        {
            Test("pleaseletmein", "SodiumChloride", 16384, 8, 1,
                "70 23 bd cb 3a fd 73 48 46 1c 06 cd 81 fd 38 eb" +
                "fd a8 fb ba 90 4f 8e 3e a9 b5 43 f6 54 5d a1 f2" +
                "d5 43 29 55 61 3f 0f cf 62 d4 97 05 24 2a 9a f9" +
                "e6 1e 85 dc 0d 65 1e 40 df cf 01 7b 45 57 58 87");
        }

        [TestMethod]
        public void RFCTest4()
        {
            Test("pleaseletmein", "SodiumChloride", 1048576, 8, 1,
                "21 01 cb 9b 6a 51 1a ae ad db be 09 cf 70 f8 81" +
                "ec 56 8d 57 4a 2f fd 4d ab e5 ee 98 20 ad aa 47" +
                "8e 56 fd 8f 4b a5 d0 9f fa 1c 6d 92 7c 40 f4 c3" +
                "37 30 40 49 e8 a9 52 fb cb f4 5c 6f a7 7a 41 a4");
        }

        [TestMethod]
        public void Test1()
        {
            Test("password", "ricmoo", 262144, 8, 1,
                "e286ed0298808c0b4bb4272ce947091b0da06bb530c4cbab3923e44ff48bbc25");
        }

        [TestMethod]
        public void VergeBlock2371624Test()
        {
            byte[] header =
                BitHelper.GetBytes(2052, ByteOrder.LittleEndian)
                .Concat(BitHelper.HexToBytes("76ade9efafe7538556635b76aea54d69f8fda80c84dc5240ef92f2c4e20eb1c8").Reverse())
                .Concat(BitHelper.HexToBytes("c4e2672cd15661f5e2e899949b15a1a337c30e6a57f97bb9447419c0106169f6").Reverse())
                .Concat(BitHelper.GetBytes(1532543943, ByteOrder.LittleEndian))
                .Concat(BitHelper.HexToBytes("1b01703c").Reverse())
                .Concat(BitHelper.GetBytes(463989813, ByteOrder.LittleEndian))
                .ToArray();
            TestBlock(header, 1024, 1, 1, "0000000000013df456086c73377b554e559e0b319fe0cba7d08772d54f15269c");
        }

        private static void Test(string pwd, string salt, ulong N, int r, int p, string hex)
        {
            byte[] bytes1 = BitHelper.HexToBytes(hex.Replace(" ", ""));

            foreach (var provider in _providers)
            {
                var kdf = provider.GetKeyDerivationFunction(KeyDerivationFunctions.Scrypt);
                kdf.Init(new ScryptParameters
                {
                    Password = Encoding.ASCII.GetBytes(pwd),
                    Salt = Encoding.ASCII.GetBytes(salt),
                    N = N,
                    r = r,
                    p = p,
                    KeyLength = bytes1.Length
                });
                byte[] bytes2 = kdf.GetKeyBytes();
                Assert.IsTrue(ByteArray.Equals(bytes1, bytes2));
            };
        }

        private static void TestBlock(byte[] header, ulong N, int r, int p, string hash)
        {
            foreach (var provider in _providers)
            {
                var kdf = provider.GetKeyDerivationFunction(KeyDerivationFunctions.Scrypt);
                kdf.Init(new ScryptParameters
                {
                    Password = header,
                    Salt = header,
                    N = N,
                    r = r,
                    p = p,
                    KeyLength = 32
                });
                byte[] bytes = kdf.GetKeyBytes();
                Assert.IsTrue(BitHelper.BytesToHex(bytes.Reverse().ToArray()) == hash);
            };
        }
    }
}