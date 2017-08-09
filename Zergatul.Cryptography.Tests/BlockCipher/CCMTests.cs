using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.BlockCipher.CipherMode;

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

            var gcm = new CCM();
            gcm.TagLength = tagLen;
            gcm.OctetLength = 2;
            var enc = gcm.CreateEncryptor(aes, key);
            var dec = gcm.CreateDecryptor(aes, key);

            var data = enc.Encrypt(iv, plain, a);

            Assert.IsTrue(BitHelper.BytesToHex(data.CipherText) + BitHelper.BytesToHex(data.Tag) == hex_cipher.ToLower());

            var decrypted = dec.Decrypt(iv, data, a);
            Assert.IsTrue(BitHelper.BytesToHex(decrypted) == hex_plain.ToLower());
        }
    }
}