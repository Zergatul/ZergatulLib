using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Symmetric;

namespace Zergatul.Cryptography.Tests.Symmetric
{
    [TestClass]
    public class KuznyechikTests
    {
        [TestMethod]
        public void Kuznyechik_DocTest()
        {
            TestEncryptDecrypt(1,
                "8899aabbccddeeff0011223344556677fedcba98765432100123456789abcdef",
                "1122334455667700ffeeddccbbaa9988",
                "7f679d90bebc24305a468d42b9d4edcd");
        }

        [TestMethod]
        public void Kuznyechik_Test1000Iterations()
        {
            TestEncryptDecrypt(1000, "8899aabbccddeeff0011223344556677fedcba98765432100123456789abcdef", "0f4e21c7f09ecebc0cde3f1272abf027", "a903b684fbea616eca252b3c31044178");
            TestEncryptDecrypt(1000, "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", "3dd5cb48f169acbc07dd52e98f8a0184", "7d3b2ee3e477eb64c9ee2fb2dfe4933d");
            TestEncryptDecrypt(1000, "0000000000000000000000000000000000000000000000000000000000000000", "1bf1f27e73aca19a02a1cafd40db9884", "98973c77885ff130e78cda6bb8a14843");
            TestEncryptDecrypt(1000, "1111111111111111111111111111111111111111111111111111111111111111", "fb59eb0e4a01ced242fa912d1a3d67ff", "3a0793f83331345780555ea891253a01");
            TestEncryptDecrypt(1000, "2222222222222222222222222222222222222222222222222222222222222222", "11572ddc13dda9288dc6f21b74f0b239", "4202ea45166a9bd699cf578ca211aaec");
            TestEncryptDecrypt(1000, "3333333333333333333333333333333333333333333333333333333333333333", "481cbc609add222ea3fc214f0cae46ef", "696ff402b0cfb36b2109b05c5ff9a1a3");
            TestEncryptDecrypt(1000, "4444444444444444444444444444444444444444444444444444444444444444", "79289e7cad336c1c2bc5eca380ff5ade", "9758570379e80a183d9fcbcf482c2326");
            TestEncryptDecrypt(1000, "5555555555555555555555555555555555555555555555555555555555555555", "ef5e616ce87a884e1572b2f766307058", "eae1705d4abcb894b49388209a04ee39");
            TestEncryptDecrypt(1000, "6666666666666666666666666666666666666666666666666666666666666666", "142b9466a9d56b5382ecf55e562bbf59", "0cd20f8a461b27eb971c8c0b2a95dab8");
            TestEncryptDecrypt(1000, "7777777777777777777777777777777777777777777777777777777777777777", "59a78034b3e24d5c684365495de2f11b", "0356dcafb0b0f9dd5f38bad606789552");
            TestEncryptDecrypt(1000, "8888888888888888888888888888888888888888888888888888888888888888", "eac30b5adef834cd248e848c86ed7ba3", "2d3f888d20db81fd31977e9908f5a855");
            TestEncryptDecrypt(1000, "9999999999999999999999999999999999999999999999999999999999999999", "1fbcfa0100fd7e4e384692ed44bfaa93", "fd04a12e5eaba82a0ee7c08991f1c87f");
            TestEncryptDecrypt(1000, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "51f8329d5a7fd1cf3314924adb40b69d", "2ae38642490a5ccb270fc46efd2c9031");
            TestEncryptDecrypt(1000, "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "74e177ba4e8a91d068c0a4aa5a0cda23", "480848612c7032976275f03dcfcd3b10");
            TestEncryptDecrypt(1000, "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc", "abc9515594a510bea54864dd3c189f0e", "092a628e93526546e6df133c93bce1dd");
            TestEncryptDecrypt(1000, "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd", "5cb83a2178d756b64b7898f5e950dcc7", "fa5252a757a1ef85a8f0cb40de70a48a");
            TestEncryptDecrypt(1000, "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee", "60a3faa7776429b3ca6e43e6864443cb", "53a546711b4b4b46e41607558801cb79");
            TestEncryptDecrypt(1000, "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff", "07ef7c4e4b400afeaf9dc10761dfb990", "d065768930fe33f62019b83bc2e94515");
        }

        private static void TestEncryptDecrypt(int rounds, string key, string plaintext, string ciphertext)
        {
            byte[] bkey = BitHelper.HexToBytes(key.Replace(" ", ""));
            byte[] bplain = BitHelper.HexToBytes(plaintext.Replace(" ", ""));
            byte[] bcipher = BitHelper.HexToBytes(ciphertext.Replace(" ", ""));
            byte[] data;

            var kuz = new Kuznyechik();

            var enc = kuz.CreateEncryptor(bkey);
            data = bplain;
            for (int i = 0; i < rounds; i++)
                data = enc(data);
            Assert.IsTrue(bcipher.SequenceEqual(data));

            var dec = kuz.CreateDecryptor(bkey);
            data = bcipher;
            for (int i = 0; i < rounds; i++)
                data = dec(data);
            Assert.IsTrue(bplain.SequenceEqual(data));
        }
    }
}