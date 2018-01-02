using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Symmetric.CipherMode;
using Zergatul.Cryptography.Symmetric;

namespace Zergatul.Cryptography.Tests.BlockCipher
{
    // http://www.ieee802.org/1/files/public/docs2011/bn-randall-test-vectors-0511-v1.pdf
    // https://pdfs.semanticscholar.org/114a/4222c53f1a6879f1a77f1bae2fc0f8f55348.pdf
    [TestClass]
    public class GCMTests
    {
        [TestMethod]
        public void GCM_TestCase1()
        {
            TestEncryptDecrypt(
                hex_key: "00000000000000000000000000000000",
                hex_plain: "",
                hex_a: "",
                hex_iv: "000000000000000000000000",
                hex_cipher: "",
                hex_tag: "58e2fccefa7e3061367f1d57a4e7455a");
        }

        [TestMethod]
        public void GCM_TestCase2()
        {
            TestEncryptDecrypt(
                hex_key: "00000000000000000000000000000000",
                hex_plain: "00000000000000000000000000000000",
                hex_a: "",
                hex_iv: "000000000000000000000000",
                hex_cipher: "0388dace60b6a392f328c2b971b2fe78",
                hex_tag: "ab6e47d42cec13bdf53a67b21257bddf");
        }

        [TestMethod]
        public void GCM_TestCase3()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b391aafd255",
                hex_a: "",
                hex_iv: "cafebabefacedbaddecaf888",
                hex_cipher: "42831ec2217774244b7221b784d0d49ce3aa212f2c02a4e035c17e2329aca12e21d514b25466931c7d8f6a5aac84aa051ba30b396a0aac973d58e091473f5985",
                hex_tag: "4d5c2af327cd64a62cf35abd2ba6fab4");
        }

        [TestMethod]
        public void GCM_TestCase4()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39",
                hex_a: "feedfacedeadbeeffeedfacedeadbeefabaddad2",
                hex_iv: "cafebabefacedbaddecaf888",
                hex_cipher: "42831ec2217774244b7221b784d0d49ce3aa212f2c02a4e035c17e2329aca12e21d514b25466931c7d8f6a5aac84aa051ba30b396a0aac973d58e091",
                hex_tag: "5bc94fbc3221a5db94fae95ae7121a47");
        }

        [TestMethod]
        public void GCM_TestCase5()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39",
                hex_a: "feedfacedeadbeeffeedfacedeadbeefabaddad2",
                hex_iv: "cafebabefacedbad",
                hex_cipher: "61353b4c2806934a777ff51fa22a4755699b2a714fcdc6f83766e5f97b6c742373806900e49f24b22b097544d4896b424989b5e1ebac0f07c23f4598",
                hex_tag: "3612d2e79e3b0785561be14aaca2fccb");
        }

        [TestMethod]
        public void GCM_TestCase6()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39",
                hex_a: "feedfacedeadbeeffeedfacedeadbeefabaddad2",
                hex_iv: "9313225df88406e555909c5aff5269aa6a7a9538534f7da1e4c303d2a318a728c3c0c95156809539fcf0e2429a6b525416aedbf5a0de6a57a637b39b",
                hex_cipher: "8ce24998625615b603a033aca13fb894be9112a5c3a211a8ba262a3cca7e2ca701e4a9a4fba43c90ccdcb281d48c7c6fd62875d2aca417034c34aee5",
                hex_tag: "619cc5aefffe0bfa462af43c1699d050");
        }

        [TestMethod]
        public void GCM_TestCase7()
        {
            TestEncryptDecrypt(
                hex_key: "000000000000000000000000000000000000000000000000",
                hex_plain: "",
                hex_a: "",
                hex_iv: "000000000000000000000000",
                hex_cipher: "",
                hex_tag: "cd33b28ac773f74ba00ed1f312572435");
        }

        [TestMethod]
        public void GCM_TestCase8()
        {
            TestEncryptDecrypt(
                hex_key: "000000000000000000000000000000000000000000000000",
                hex_plain: "00000000000000000000000000000000",
                hex_a: "",
                hex_iv: "000000000000000000000000",
                hex_cipher: "98e7247c07f0fe411c267e4384b0f600",
                hex_tag: "2ff58d80033927ab8ef4d4587514f0fb");
        }

        [TestMethod]
        public void GCM_TestCase9()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308feffe9928665731c",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b391aafd255",
                hex_a: "",
                hex_iv: "cafebabefacedbaddecaf888",
                hex_cipher: "3980ca0b3c00e841eb06fac4872a2757859e1ceaa6efd984628593b40ca1e19c7d773d00c144c525ac619d18c84a3f4718e2448b2fe324d9ccda2710acade256",
                hex_tag: "9924a7c8587336bfb118024db8674a14");
        }

        [TestMethod]
        public void GCM_TestCase10()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308feffe9928665731c",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39",
                hex_a: "feedfacedeadbeeffeedfacedeadbeefabaddad2",
                hex_iv: "cafebabefacedbaddecaf888",
                hex_cipher: "3980ca0b3c00e841eb06fac4872a2757859e1ceaa6efd984628593b40ca1e19c7d773d00c144c525ac619d18c84a3f4718e2448b2fe324d9ccda2710",
                hex_tag: "2519498e80f1478f37ba55bd6d27618c");
        }

        [TestMethod]
        public void GCM_TestCase11()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308feffe9928665731c",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39",
                hex_a: "feedfacedeadbeeffeedfacedeadbeefabaddad2",
                hex_iv: "cafebabefacedbad",
                hex_cipher: "0f10f599ae14a154ed24b36e25324db8c566632ef2bbb34f8347280fc4507057fddc29df9a471f75c66541d4d4dad1c9e93a19a58e8b473fa0f062f7",
                hex_tag: "65dcc57fcf623a24094fcca40d3533f8");
        }

        [TestMethod]
        public void GCM_TestCase12()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308feffe9928665731c",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39",
                hex_a: "feedfacedeadbeeffeedfacedeadbeefabaddad2",
                hex_iv: "9313225df88406e555909c5aff5269aa6a7a9538534f7da1e4c303d2a318a728c3c0c95156809539fcf0e2429a6b525416aedbf5a0de6a57a637b39b",
                hex_cipher: "d27e88681ce3243c4830165a8fdcf9ff1de9a1d8e6b447ef6ef7b79828666e4581e79012af34ddd9e2f037589b292db3e67c036745fa22e7e9b7373b",
                hex_tag: "dcf566ff291c25bbb8568fc3d376a6d9");
        }

        [TestMethod]
        public void GCM_TestCase13()
        {
            TestEncryptDecrypt(
                hex_key: "0000000000000000000000000000000000000000000000000000000000000000",
                hex_plain: "",
                hex_a: "",
                hex_iv: "000000000000000000000000",
                hex_cipher: "",
                hex_tag: "530f8afbc74536b9a963b4f1c4cb738b");
        }

        [TestMethod]
        public void GCM_TestCase14()
        {
            TestEncryptDecrypt(
                hex_key: "0000000000000000000000000000000000000000000000000000000000000000",
                hex_plain: "00000000000000000000000000000000",
                hex_a: "",
                hex_iv: "000000000000000000000000",
                hex_cipher: "cea7403d4d606b6e074ec5d3baf39d18",
                hex_tag: "d0d1c8a799996bf0265b98b5d48ab919");
        }

        [TestMethod]
        public void GCM_TestCase15()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308feffe9928665731c6d6a8f9467308308",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b391aafd255",
                hex_a: "",
                hex_iv: "cafebabefacedbaddecaf888",
                hex_cipher: "522dc1f099567d07f47f37a32a84427d643a8cdcbfe5c0c97598a2bd2555d1aa8cb08e48590dbb3da7b08b1056828838c5f61e6393ba7a0abcc9f662898015ad",
                hex_tag: "b094dac5d93471bdec1a502270e3cc6c");
        }

        [TestMethod]
        public void GCM_TestCase16()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308feffe9928665731c6d6a8f9467308308",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39",
                hex_a: "feedfacedeadbeeffeedfacedeadbeefabaddad2",
                hex_iv: "cafebabefacedbaddecaf888",
                hex_cipher: "522dc1f099567d07f47f37a32a84427d643a8cdcbfe5c0c97598a2bd2555d1aa8cb08e48590dbb3da7b08b1056828838c5f61e6393ba7a0abcc9f662",
                hex_tag: "76fc6ece0f4e1768cddf8853bb2d551b");
        }

        [TestMethod]
        public void GCM_TestCase17()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308feffe9928665731c6d6a8f9467308308",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39",
                hex_a: "feedfacedeadbeeffeedfacedeadbeefabaddad2",
                hex_iv: "cafebabefacedbad",
                hex_cipher: "c3762df1ca787d32ae47c13bf19844cbaf1ae14d0b976afac52ff7d79bba9de0feb582d33934a4f0954cc2363bc73f7862ac430e64abe499f47c9b1f",
                hex_tag: "3a337dbf46a792c45e454913fe2ea8f2");
        }

        [TestMethod]
        public void GCM_TestCase18()
        {
            TestEncryptDecrypt(
                hex_key: "feffe9928665731c6d6a8f9467308308feffe9928665731c6d6a8f9467308308",
                hex_plain: "d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a721c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39",
                hex_a: "feedfacedeadbeeffeedfacedeadbeefabaddad2",
                hex_iv: "9313225df88406e555909c5aff5269aa6a7a9538534f7da1e4c303d2a318a728c3c0c95156809539fcf0e2429a6b525416aedbf5a0de6a57a637b39b",
                hex_cipher: "5a8def2f0c9e53f1f75d7853659e2a20eeb2b22aafde6419a058ab4f6f746bf40fc0c3b780f244452da3ebf1c5d82cdea2418997200ef82e44ae7e3f",
                hex_tag: "a44a8266ee1c8eb0c8b5d4cf5ae9f19a");
        }

        [TestMethod]
        public void GCM_Test128_54ByteAuth()
        {
            TestEncryptDecrypt(
                hex_key: "AD7A2BD03EAC835A6F620FDCB506B345",
                hex_plain: "",
                hex_a: "D609B1F056637A0D46DF998D88E5222AB2C2846512153524C0895E8108000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F30313233340001",
                hex_iv: "12153524C0895E81B2C28465",
                hex_cipher: "",
                hex_tag: "F09478A9B09007D06F46E9B6A1DA25DD");
        }

        [TestMethod]
        public void GCM_Test256_54ByteAuth()
        {
            TestEncryptDecrypt(
                hex_key: "E3C08A8F06C6E3AD95A70557B23F75483CE33021A9C72B7025666204C69C0B72",
                hex_plain: "",
                hex_a: "D609B1F056637A0D46DF998D88E5222AB2C2846512153524C0895E8108000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F30313233340001",
                hex_iv: "12153524C0895E81B2C28465",
                hex_cipher: "",
                hex_tag: "2F0BC5AF409E06D609EA8B7D0FA5EA50");
        }

        [TestMethod]
        public void GCM_Test128_60BytePlain()
        {
            TestEncryptDecrypt(
                hex_key: "AD7A2BD03EAC835A6F620FDCB506B345",
                hex_plain: "08000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A0002",
                hex_a: "D609B1F056637A0D46DF998D88E52E00B2C2846512153524C0895E81",
                hex_iv: "12153524C0895E81B2C28465",
                hex_cipher: "701AFA1CC039C0D765128A665DAB69243899BF7318CCDC81C9931DA17FBE8EDD7D17CB8B4C26FC81E3284F2B7FBA713D",
                hex_tag: "4F8D55E7D3F06FD5A13C0C29B9D5B880");
        }

        [TestMethod]
        public void GCM_Test256_60BytePlain()
        {
            TestEncryptDecrypt(
                hex_key: "E3C08A8F06C6E3AD95A70557B23F75483CE33021A9C72B7025666204C69C0B72",
                hex_plain: "08000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A0002",
                hex_a: "D609B1F056637A0D46DF998D88E52E00B2C2846512153524C0895E81",
                hex_iv: "12153524C0895E81B2C28465",
                hex_cipher: "E2006EB42F5277022D9B19925BC419D7A592666C925FE2EF718EB4E308EFEAA7C5273B394118860A5BE2A97F56AB7836",
                hex_tag: "5CA597CDBB3EDB8D1A1151EA0AF7B436");
        }

        [TestMethod]
        public void GCM_Test128_60ByteAuth()
        {
            TestEncryptDecrypt(
                hex_key: "071B113B0CA743FECCCF3D051F737382",
                hex_plain: "",
                hex_a: "E20106D7CD0DF0761E8DCD3D88E5400076D457ED08000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A0003",
                hex_iv: "F0761E8DCD3D000176D457ED",
                hex_cipher: "",
                hex_tag: "0C017BC73B227DFCC9BAFA1C41ACC353");
        }

        [TestMethod]
        public void GCM_Test256_60ByteAuth()
        {
            TestEncryptDecrypt(
                hex_key: "691D3EE909D7F54167FD1CA0B5D769081F2BDE1AEE655FDBAB80BD5295AE6BE7",
                hex_plain: "",
                hex_a: "E20106D7CD0DF0761E8DCD3D88E5400076D457ED08000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A0003",
                hex_iv: "F0761E8DCD3D000176D457ED",
                hex_cipher: "",
                hex_tag: "35217C774BBC31B63166BCF9D4ABED07");
        }

        [TestMethod]
        public void GCM_Test128_75BytePlain()
        {
            TestEncryptDecrypt(
                hex_key: "88EE087FD95DA9FBF6725AA9D757B0CD",
                hex_plain: "08000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748490008",
                hex_a: "68F2E77696CE7AE8E2CA4EC588E54D002E58495C",
                hex_iv: "7AE8E2CA4EC500012E58495C",
                hex_cipher: "C31F53D99E5687F7365119B832D2AAE70741D593F1F9E2AB3455779B078EB8FEACDFEC1F8E3E5277F8180B43361F6512ADB16D2E38548A2C719DBA7228D840",
                hex_tag: "88F8757ADB8AA788D8F65AD668BE70E7");
        }

        [TestMethod]
        public void GCM_Test256_75BytePlain()
        {
            TestEncryptDecrypt(
                hex_key: "4C973DBC7364621674F8B5B89E5C15511FCED9216490FB1C1A2CAA0FFE0407E5",
                hex_plain: "08000F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748490008",
                hex_a: "68F2E77696CE7AE8E2CA4EC588E54D002E58495C",
                hex_iv: "7AE8E2CA4EC500012E58495C",
                hex_cipher: "BA8AE31BC506486D6873E4FCE460E7DC57591FF00611F31C3834FE1C04AD80B66803AFCF5B27E6333FA67C99DA47C2F0CED68D531BD741A943CFF7A6713BD0",
                hex_tag: "2611CD7DAA01D61C5C886DC1A8170107");
        }

        private static void TestEncryptDecrypt(string hex_key, string hex_plain, string hex_iv, string hex_a, string hex_cipher, string hex_tag)
        {
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

            var gcm = new GCM();
            gcm.TagLength = 16;
            var enc = gcm.CreateEncryptor(aes, key);
            var dec = gcm.CreateDecryptor(aes, key);

            var data = enc.Encrypt(iv, plain, a);

            Assert.IsTrue(BitHelper.BytesToHex(data.CipherText) == hex_cipher.ToLower());
            Assert.IsTrue(BitHelper.BytesToHex(data.Tag) == hex_tag.ToLower());

            var decrypted = dec.Decrypt(iv, data, a);
            Assert.IsTrue(BitHelper.BytesToHex(decrypted) == hex_plain.ToLower());
        }
    }
}