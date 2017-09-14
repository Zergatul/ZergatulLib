using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Symmetric;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    [TestClass]
    public class BlowfishTests
    {
        [TestMethod]
        public void EricYoungVectors()
        {
            TestEncryptDecrypt("0000000000000000", "0000000000000000", "4EF997456198DD78");
            TestEncryptDecrypt("FFFFFFFFFFFFFFFF", "FFFFFFFFFFFFFFFF", "51866FD5B85ECB8A");
            TestEncryptDecrypt("3000000000000000", "1000000000000001", "7D856F9A613063F2");
            TestEncryptDecrypt("1111111111111111", "1111111111111111", "2466DD878B963C9D");
            TestEncryptDecrypt("0123456789ABCDEF", "1111111111111111", "61F9C3802281B096");
            TestEncryptDecrypt("1111111111111111", "0123456789ABCDEF", "7D0CC630AFDA1EC7");
            TestEncryptDecrypt("0000000000000000", "0000000000000000", "4EF997456198DD78");
            TestEncryptDecrypt("FEDCBA9876543210", "0123456789ABCDEF", "0ACEAB0FC6A0A28D");
            TestEncryptDecrypt("7CA110454A1A6E57", "01A1D6D039776742", "59C68245EB05282B");
            TestEncryptDecrypt("0131D9619DC1376E", "5CD54CA83DEF57DA", "B1B8CC0B250F09A0");
            TestEncryptDecrypt("07A1133E4A0B2686", "0248D43806F67172", "1730E5778BEA1DA4");
            TestEncryptDecrypt("3849674C2602319E", "51454B582DDF440A", "A25E7856CF2651EB");
            TestEncryptDecrypt("04B915BA43FEB5B6", "42FD443059577FA2", "353882B109CE8F1A");
            TestEncryptDecrypt("0113B970FD34F2CE", "059B5E0851CF143A", "48F4D0884C379918");
            TestEncryptDecrypt("0170F175468FB5E6", "0756D8E0774761D2", "432193B78951FC98");
            TestEncryptDecrypt("43297FAD38E373FE", "762514B829BF486A", "13F04154D69D1AE5");
            TestEncryptDecrypt("07A7137045DA2A16", "3BDD119049372802", "2EEDDA93FFD39C79");
            TestEncryptDecrypt("04689104C2FD3B2F", "26955F6835AF609A", "D887E0393C2DA6E3");
            TestEncryptDecrypt("37D06BB516CB7546", "164D5E404F275232", "5F99D04F5B163969");
            TestEncryptDecrypt("1F08260D1AC2465E", "6B056E18759F5CCA", "4A057A3B24D3977B");
            TestEncryptDecrypt("584023641ABA6176", "004BD6EF09176062", "452031C1E4FADA8E");
            TestEncryptDecrypt("025816164629B007", "480D39006EE762F2", "7555AE39F59B87BD");
            TestEncryptDecrypt("49793EBC79B3258F", "437540C8698F3CFA", "53C55F9CB49FC019");
            TestEncryptDecrypt("4FB05E1515AB73A7", "072D43A077075292", "7A8E7BFA937E89A3");
            TestEncryptDecrypt("49E95D6D4CA229BF", "02FE55778117F12A", "CF9C5D7A4986ADB5");
            TestEncryptDecrypt("018310DC409B26D6", "1D9D5C5018F728C2", "D1ABB290658BC778");
            TestEncryptDecrypt("1C587F1C13924FEF", "305532286D6F295A", "55CB3774D13EF201");
            TestEncryptDecrypt("0101010101010101", "0123456789ABCDEF", "FA34EC4847B268B2");
            TestEncryptDecrypt("1F1F1F1F0E0E0E0E", "0123456789ABCDEF", "A790795108EA3CAE");
            TestEncryptDecrypt("E0FEE0FEF1FEF1FE", "0123456789ABCDEF", "C39E072D9FAC631D");
            TestEncryptDecrypt("0000000000000000", "FFFFFFFFFFFFFFFF", "014933E0CDAFF6E4");
            TestEncryptDecrypt("FFFFFFFFFFFFFFFF", "0000000000000000", "F21E9A77B71C49BC");
            TestEncryptDecrypt("0123456789ABCDEF", "0000000000000000", "245946885754369A");
            TestEncryptDecrypt("FEDCBA9876543210", "FFFFFFFFFFFFFFFF", "6B5C5A9C5D9E0A5A");
        }

        [TestMethod]
        public void NonStandardKeySizesTests()
        {
            TestEncryptDecrypt("90", "0001 0203 0405 0607", "c710 970f a369 1224");
            TestEncryptDecrypt("9085", "0001 0203 0405 0607", "5f27 1f50 ddf8 6c12");
            TestEncryptDecrypt("908505", "0001 0203 0405 0607", "3ee6 5f4b 32b0 2c0a");
            TestEncryptDecrypt("90850593", "0001 0203 0405 0607", "22cb 550a d5af ed5d");
            TestEncryptDecrypt("9085059347", "0001 0203 0405 0607", "9974 b1ad 2115 426f");
            TestEncryptDecrypt("9085059347AF", "0001 0203 0405 0607", "c5ed befa 4a41 9d24");
            TestEncryptDecrypt("9085059347AF08", "0001 0203 0405 0607", "68a3 492c a1b5 ee92");

            TestEncryptDecrypt("9085 0593 47af 0887 28", "0001 0203 0405 0607", "3ef5 191c 48fb 5341");
            TestEncryptDecrypt("9085 0593 47af 0887 3ef5 191c 48fb 5341", "0001 0203 0405 0607", "eb29 5a1f 4412 1fd9");
            TestEncryptDecrypt("9085 0593 47af 0887 3ef5 191c 48fb 5341 eb29 5a1f 4412 1fd9", "0001 0203 0405 0607", "b382 9a11 70ec 0d9c");
            TestEncryptDecrypt("9085 0593 47af 0887 3ef5 191c 48fb 5341 eb29 5a1f 4412 1fd9 b382 9a11 70ec 0d9c", "0001 0203 0405 0607", "02a8 9bcf 85dd 56f8");
            TestEncryptDecrypt("9085 0593 47af 0887 3ef5 191c 48fb 5341 eb29 5a1f 4412 1fd9 b382 9a11 70ec 0d9c 01", "0001 0203 0405 0607", "e605 86d3 f230 68e3");
        }

        private static void TestEncryptDecrypt(string key, string plaintext, string ciphertext)
        {
            byte[] bkey = BitHelper.HexToBytes(key.Replace(" ", ""));
            byte[] bplain = BitHelper.HexToBytes(plaintext.Replace(" ", ""));
            byte[] bcipher = BitHelper.HexToBytes(ciphertext.Replace(" ", ""));

            var bf = new Blowfish(bkey.Length);

            var enc = bf.CreateEncryptor(bkey);
            Assert.IsTrue(bcipher.SequenceEqual(enc(bplain)));

            var dec = bf.CreateDecryptor(bkey);
            Assert.IsTrue(bplain.SequenceEqual(dec(bcipher)));
        }
    }
}