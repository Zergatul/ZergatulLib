using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Symmetric.CipherMode;
using System.Linq;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    [TestClass]
    public class CCMTests
    {
        [TestMethod]
        public void Vector1()
        {
            TestEncryptDecrypt(
                hex_key: "C0 C1 C2 C3  C4 C5 C6 C7  C8 C9 CA CB  CC CD CE CF",
                hex_plain: "08 09 0A 0B  0C 0D 0E 0F  10 11 12 13  14 15 16 17  18 19 1A 1B  1C 1D 1E",
                hex_a: "00 01 02 03  04 05 06 07",
                hex_iv: "00 00 00 03  02 01 00 A0  A1 A2 A3 A4  A5",
                hex_cipher: "58 8C 97 9A  61 C6 63 D2  F0 66 D0 C2  C0 F9 89 80  6D 5F 6B 61  DA C3 84 17  E8 D1 2C FD  F9 26 E0");
        }

        [TestMethod]
        public void Vector2()
        {
            TestEncryptDecrypt(
                hex_key: "C0 C1 C2 C3  C4 C5 C6 C7  C8 C9 CA CB  CC CD CE CF",
                hex_plain: "08 09 0A 0B  0C 0D 0E 0F  10 11 12 13  14 15 16 17  18 19 1A 1B  1C 1D 1E 1F",
                hex_a: "00 01 02 03  04 05 06 07",
                hex_iv: "00 00 00 04  03 02 01 A0  A1 A2 A3 A4  A5",
                hex_cipher: "72 C9 1A 36  E1 35 F8 CF  29 1C A8 94  08 5C 87 E3  CC 15 C4 39  C9 E4 3A 3B  A0 91 D5 6E  10 40 09 16");
        }

        [TestMethod]
        public void Vector3()
        {
            TestEncryptDecrypt(
                hex_key: "C0 C1 C2 C3  C4 C5 C6 C7  C8 C9 CA CB  CC CD CE CF",
                hex_plain: "08 09 0A 0B  0C 0D 0E 0F  10 11 12 13  14 15 16 17  18 19 1A 1B  1C 1D 1E 1F  20",
                hex_a: "00 01 02 03  04 05 06 07",
                hex_iv: "00 00 00 05  04 03 02 A0  A1 A2 A3 A4  A5",
                hex_cipher: "51 B1 E5 F4  4A 19 7D 1D  A4 6B 0F 8E  2D 28 2A E8  71 E8 38 BB  64 DA 85 96  57 4A DA A7  6F BD 9F B0  C5");
        }

        [TestMethod]
        public void Vector4()
        {
            TestEncryptDecrypt(
                hex_key: "C0 C1 C2 C3  C4 C5 C6 C7  C8 C9 CA CB  CC CD CE CF",
                hex_plain: "0C 0D 0E 0F  10 11 12 13  14 15 16 17  18 19 1A 1B  1C 1D 1E",
                hex_a: "00 01 02 03  04 05 06 07  08 09 0A 0B",
                hex_iv: "00 00 00 06  05 04 03 A0  A1 A2 A3 A4  A5",
                hex_cipher: "A2 8C 68 65  93 9A 9A 79  FA AA 5C 4C  2A 9D 4A 91  CD AC 8C 96  C8 61 B9 C9  E6 1E F1");
        }

        [TestMethod]
        public void Vector5()
        {
            TestEncryptDecrypt(
                hex_key: "C0 C1 C2 C3  C4 C5 C6 C7  C8 C9 CA CB  CC CD CE CF",
                hex_plain: "0C 0D 0E 0F  10 11 12 13  14 15 16 17  18 19 1A 1B  1C 1D 1E 1F",
                hex_a: "00 01 02 03  04 05 06 07  08 09 0A 0B",
                hex_iv: "00 00 00 07  06 05 04 A0  A1 A2 A3 A4  A5",
                hex_cipher: "DC F1 FB 7B  5D 9E 23 FB  9D 4E 13 12  53 65 8A D8  6E BD CA 3E  51 E8 3F 07  7D 9C 2D 93");
        }

        [TestMethod]
        public void Vector6()
        {
            TestEncryptDecrypt(
                hex_key: "C0 C1 C2 C3  C4 C5 C6 C7  C8 C9 CA CB  CC CD CE CF",
                hex_plain: "0C 0D 0E 0F  10 11 12 13  14 15 16 17  18 19 1A 1B  1C 1D 1E 1F  20",
                hex_a: "00 01 02 03  04 05 06 07  08 09 0A 0B",
                hex_iv: "00 00 00 08  07 06 05 A0  A1 A2 A3 A4  A5",
                hex_cipher: "6F C1 B0 11  F0 06 56 8B  51 71 A4 2D  95 3D 46 9B  25 70 A4 BD  87 40 5A 04  43 AC 91 CB  94");
        }

        [TestMethod]
        public void Vector7()
        {
            TestEncryptDecrypt(
                tagLen: 10,
                hex_key: "C0 C1 C2 C3  C4 C5 C6 C7  C8 C9 CA CB  CC CD CE CF",
                hex_plain: "08 09 0A 0B  0C 0D 0E 0F  10 11 12 13  14 15 16 17  18 19 1A 1B  1C 1D 1E",
                hex_a: "00 01 02 03  04 05 06 07",
                hex_iv: "00 00 00 09  08 07 06 A0  A1 A2 A3 A4  A5",
                hex_cipher: "01 35 D1 B2  C9 5F 41 D5  D1 D4 FE C1  85 D1 66 B8  09 4E 99 9D  FE D9 6C 04  8C 56 60 2C  97 AC BB 74  90");
        }

        [TestMethod]
        public void Vector8()
        {
            TestEncryptDecrypt(
                tagLen: 10,
                hex_key: "C0 C1 C2 C3  C4 C5 C6 C7  C8 C9 CA CB  CC CD CE CF",
                hex_plain: "08 09 0A 0B  0C 0D 0E 0F  10 11 12 13  14 15 16 17  18 19 1A 1B  1C 1D 1E 1F",
                hex_a: "00 01 02 03  04 05 06 07",
                hex_iv: "00 00 00 0A  09 08 07 A0  A1 A2 A3 A4  A5",
                hex_cipher: "7B 75 39 9A  C0 83 1D D2  F0 BB D7 58  79 A2 FD 8F  6C AE 6B 6C  D9 B7 DB 24  C1 7B 44 33  F4 34 96 3F  34 B4");
        }

        [TestMethod]
        public void Vector9()
        {
            TestEncryptDecrypt(
                tagLen: 10,
                hex_key: "C0 C1 C2 C3  C4 C5 C6 C7  C8 C9 CA CB  CC CD CE CF",
                hex_plain: "08 09 0A 0B  0C 0D 0E 0F  10 11 12 13  14 15 16 17  18 19 1A 1B  1C 1D 1E 1F  20",
                hex_a: "00 01 02 03  04 05 06 07",
                hex_iv: "00 00 00 0B  0A 09 08 A0  A1 A2 A3 A4  A5",
                hex_cipher: "82 53 1A 60  CC 24 94 5A  4B 82 79 18  1A B5 C8 4D  F2 1C E7 F9  B7 3F 42 E1  97 EA 9C 07  E5 6B 5E B1  7E 5F 4E");
        }

        [TestMethod]
        public void Vector10()
        {
            TestEncryptDecrypt(
                tagLen: 4,
                hex_key: "40414243 44454647 48494a4b 4c4d4e4f",
                hex_plain: "20212223",
                hex_a: "00010203 04050607",
                hex_iv: "10111213 141516",
                hex_cipher: "7162015b 4dac255d");
        }

        [TestMethod]
        public void Vector11()
        {
            TestEncryptDecrypt(
                tagLen: 6,
                hex_key: "40414243 44454647 48494a4b 4c4d4e4f",
                hex_plain: "20212223 24252627 28292a2b 2c2d2e2f",
                hex_a: "00010203 04050607 08090a0b 0c0d0e0f",
                hex_iv: "10111213 14151617",
                hex_cipher: "d2a1f0e0 51ea5f62 081a7792 073d593d 1fc64fbf accd");
        }

        [TestMethod]
        public void Vector12()
        {
            TestEncryptDecrypt(
                tagLen: 8,
                hex_key: "40414243 44454647 48494a4b 4c4d4e4f",
                hex_plain: "20212223 24252627 28292a2b 2c2d2e2f 30313233 34353637",
                hex_a: "00010203 04050607 08090a0b 0c0d0e0f 10111213",
                hex_iv: "10111213 14151617 18191a1b",
                hex_cipher: "e3b201a9 f5b71a7a 9b1ceaec cd97e70b 6176aad9 a4428aa5 484392fb c1b09951");
        }

        [TestMethod]
        public void Vector13()
        {
            TestEncryptDecrypt(
                tagLen: 14,
                hex_key: "40414243 44454647 48494a4b 4c4d4e4f",
                hex_plain: "20212223 24252627 28292a2b 2c2d2e2f 30313233 34353637 38393a3b 3c3d3e3f",
                hex_a: string.Concat(Enumerable.Repeat(
                    "00010203 04050607 08090a0b 0c0d0e0f" +
                    "10111213 14151617 18191a1b 1c1d1e1f" +
                    "20212223 24252627 28292a2b 2c2d2e2f" +
                    "30313233 34353637 38393a3b 3c3d3e3f" +
                    "40414243 44454647 48494a4b 4c4d4e4f" +
                    "50515253 54555657 58595a5b 5c5d5e5f" +
                    "60616263 64656667 68696a6b 6c6d6e6f" +
                    "70717273 74757677 78797a7b 7c7d7e7f" +
                    "80818283 84858687 88898a8b 8c8d8e8f" +
                    "90919293 94959697 98999a9b 9c9d9e9f" +
                    "a0a1a2a3 a4a5a6a7 a8a9aaab acadaeaf" +
                    "b0b1b2b3 b4b5b6b7 b8b9babb bcbdbebf" +
                    "c0c1c2c3 c4c5c6c7 c8c9cacb cccdcecf" +
                    "d0d1d2d3 d4d5d6d7 d8d9dadb dcdddedf" +
                    "e0e1e2e3 e4e5e6e7 e8e9eaeb ecedeeef" +
                    "f0f1f2f3 f4f5f6f7 f8f9fafb fcfdfeff", 256)),
                hex_iv: "10111213 14151617 18191a1b 1c",
                hex_cipher: "69915dad 1e84c637 6a68c296 7e4dab61 5ae0fd1f aec44cc4 84828529 463ccf72 b4ac6bec 93e8598e 7f0dadbc ea5b");
        }

        private static void TestEncryptDecrypt(string hex_key, string hex_plain, string hex_iv, string hex_a, string hex_cipher, int tagLen = 8)
        {
            hex_key = hex_key.Replace(" ", "");
            hex_plain = hex_plain.Replace(" ", "");
            hex_iv = hex_iv.Replace(" ", "");
            hex_a = hex_a.Replace(" ", "");
            hex_cipher = hex_cipher.Replace(" ", "");

            byte[] key = BitHelper.HexToBytes(hex_key);
            byte[] plain = BitHelper.HexToBytes(hex_plain);
            byte[] iv = BitHelper.HexToBytes(hex_iv);
            byte[] a = BitHelper.HexToBytes(hex_a);

            AES aes;
            switch (key.Length)
            {
                case 16: aes = new AES128(); break;
                case 24: aes = new AES192(); break;
                case 32: aes = new AES256(); break;
                default:
                    throw new InvalidOperationException();
            }

            var ccm = new CCM();
            ccm.TagLength = tagLen;
            ccm.OctetLength = 15 - iv.Length;
            var enc = ccm.CreateEncryptor(aes, key);
            var dec = ccm.CreateDecryptor(aes, key);

            var data = enc.Encrypt(iv, plain, a);

            Assert.IsTrue(BitHelper.BytesToHex(data.CipherText) + BitHelper.BytesToHex(data.Tag) == hex_cipher.ToLower());

            var decrypted = dec.Decrypt(iv, data, a);
            Assert.IsTrue(BitHelper.BytesToHex(decrypted) == hex_plain.ToLower());
        }
    }
}