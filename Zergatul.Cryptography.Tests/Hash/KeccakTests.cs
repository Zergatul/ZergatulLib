using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Hash;
using System.Text;

namespace Zergatul.Cryptography.Tests.Hash
{
    [TestClass]
    public class KeccakTests
    {
        [TestMethod]
        public void SHA3_224_Test()
        {
            Assert.IsTrue(Helper.Hash<SHA3_224>("") == "6b4e03423667dbb73b6e15454f0eb1abd4597f9a1b078e3f5b5a6bc7");
            Assert.IsTrue(Helper.Hash<SHA3_224>("!") == "9c7295bd0fbfcbd37135049ee844df5d9121a73957a875081cfe6939");
            Assert.IsTrue(Helper.Hash<SHA3_224>("qq") == "3eff34e3e9f27e74a2f125dc3982720051a61ae9ae21cbc41d3057b2");
            Assert.IsTrue(Helper.Hash<SHA3_224>("LUL") == "93d9949dfca60ca6eb577eeb2a2b1a6525d22cce68e0e65b76bad0f7");
            Assert.IsTrue(Helper.Hash<SHA3_224>("The quick brown fox jumps over the lazy dog") ==
                "d15dadceaa4d5d7bb3b48f446421d542e08ad8887305e28d58335795");
            Assert.IsTrue(Helper.Hash<SHA3_224>("The quick brown fox jumps over the lazy dog.") ==
                "2d0708903833afabdd232a20201176e8b58c5be8a6fe74265ac54db0");

            Assert.IsTrue(Helper.Hash<SHA3_224>(new string('*', 143)) ==
                "d1857c87b3d6ba3d8e1102d8b4a6d7545a0c56c59a9c938f009d23ad");
            Assert.IsTrue(Helper.Hash<SHA3_224>(new string('*', 144)) ==
                "1c879b964ea71aa7818e379fc19327fb06416033cb8b2a6bacf4e0d1");
            Assert.IsTrue(Helper.Hash<SHA3_224>(new string('*', 199)) ==
                "d0c0ca5047e223a49a26819c200870aa381427f043cd0a82ee7e5527");

            Assert.IsTrue(Helper.Hash<SHA3_224>(new string('#', 50000)) ==
                "63928d9f7dca23b3ac615a9fc0161da3e88f236f9042ee036746f36f");

            Assert.IsTrue(Helper.Hash<SHA3_224>(new byte[] { 0xFF }, 10000) ==
                "2b9baf4ce3dde5d8e9194d40c2f87f015e25961ff43b9605d0243b54");
        }

        [TestMethod]
        public void SHA3_256_Test()
        {
            Assert.IsTrue(Helper.Hash<SHA3_256>("") == "a7ffc6f8bf1ed76651c14756a061d662f580ff4de43b49fa82d80a4b80f8434a");
            Assert.IsTrue(Helper.Hash<SHA3_256>("The quick brown fox jumps over the lazy dog") ==
                "69070dda01975c8c120c3aada1b282394e7f032fa9cf32f4cb2259a0897dfc04");
            Assert.IsTrue(Helper.Hash<SHA3_256>("The quick brown fox jumps over the lazy dog.") ==
                "a80f839cd4f83f6c3dafc87feae470045e4eb0d366397d5c6ce34ba1739f734d");

            Assert.IsTrue(Helper.Hash<SHA3_256>(new string('*', 135)) ==
                "41d9757506877d29ca3a4e2c372e7165d7a3282f38d235de01418c6ac93a73b2");
            Assert.IsTrue(Helper.Hash<SHA3_256>(new string('*', 136)) ==
                "5224abc95021feafd89e36b41067884a08b39ff8e5ce0905c3a67d1857169e8a");

            Assert.IsTrue(Helper.Hash<SHA3_256>(new string('#', 50000)) ==
                "2ff91d7f09ba946c1908d74c7acd1ccf51ebb531f37bb29556535c880e33d39a");

            Assert.IsTrue(Helper.Hash<SHA3_256>(new byte[] { 0xFF }, 10000) ==
                "31d6f1ca56d8fecc76ccdfbdfad170242adfa2c61ed97d7c19ca8f330ebfe386");
        }

        [TestMethod]
        public void SHA3_384_Test()
        {
            Assert.IsTrue(Helper.Hash<SHA3_384>("") == "0c63a75b845e4f7d01107d852e4c2485c51a50aaaa94fc61995e71bbee983a2ac3713831264adb47fb6bd1e058d5f004");
            Assert.IsTrue(Helper.Hash<SHA3_384>("The quick brown fox jumps over the lazy dog") ==
                "7063465e08a93bce31cd89d2e3ca8f602498696e253592ed26f07bf7e703cf328581e1471a7ba7ab119b1a9ebdf8be41");
            Assert.IsTrue(Helper.Hash<SHA3_384>("The quick brown fox jumps over the lazy dog.") ==
                "1a34d81695b622df178bc74df7124fe12fac0f64ba5250b78b99c1273d4b080168e10652894ecad5f1f4d5b965437fb9");

            Assert.IsTrue(Helper.Hash<SHA3_384>(new string('*', 103)) ==
                "37066029f7225d9809bd60f3729b883cd86a8e89b1b22be833985d11aaf5770f9638a3c000c0645323734e098937da63");
            Assert.IsTrue(Helper.Hash<SHA3_384>(new string('*', 104)) ==
                "281cac74ef1a2a48df7107fb3d2880a15839d547029ea01fbfc9fc8ed3a2c49fd10dd4aa9fb093ecafb8c474448fa270");

            Assert.IsTrue(Helper.Hash<SHA3_384>(new string('#', 50000)) ==
                "3da354d25e6639e28276e534a4d75e590dd2da8ecdfcb4cf4b46340f29e0d1e61416868e01e10a01ee2d5344671fe679");

            Assert.IsTrue(Helper.Hash<SHA3_384>(new byte[] { 0xFF }, 10000) ==
                "e7db5bea864ca450e13cde37c6ed3e79403740af3332a0a0f70774ca7c6f334df2235e2161da9e5fb2b7adb8755df0f2");
        }

        [TestMethod]
        public void SHA3_512_Test()
        {
            Assert.IsTrue(Helper.Hash<SHA3_512>("") == "a69f73cca23a9ac5c8b567dc185a756e97c982164fe25859e0d1dcc1475c80a615b2123af1f5f94c11e3e9402c3ac558f500199d95b6d3e301758586281dcd26");
            Assert.IsTrue(Helper.Hash<SHA3_512>("The quick brown fox jumps over the lazy dog") ==
                "01dedd5de4ef14642445ba5f5b97c15e47b9ad931326e4b0727cd94cefc44fff23f07bf543139939b49128caf436dc1bdee54fcb24023a08d9403f9b4bf0d450");
            Assert.IsTrue(Helper.Hash<SHA3_512>("The quick brown fox jumps over the lazy dog.") ==
                "18f4f4bd419603f95538837003d9d254c26c23765565162247483f65c50303597bc9ce4d289f21d1c2f1f458828e33dc442100331b35e7eb031b5d38ba6460f8");

            Assert.IsTrue(Helper.Hash<SHA3_512>(new string('*', 71)) ==
                "26e92317b3e63f539e9ba0fbcf92115ff86426b5de60dd1e9b35354711b91b5fa6706ae1118f8da8b00716905dff4dce823f4d6b04567c8bb864f875d0f55b5b");
            Assert.IsTrue(Helper.Hash<SHA3_512>(new string('*', 72)) ==
                "cc0b2c1464206340d8bd3b5a6eb7ce0663ea2a79ac3440293685752c359386b3bcea8827f63839ec9aaea46a6f627583f8f5c497000e926eb8d7dfe3c31b5906");

            Assert.IsTrue(Helper.Hash<SHA3_512>(new string('#', 50000)) ==
                "5761fa4df56442a265f9ba0b468bfb5d69001b0b56372405b0c0f8c60a4a62b46e1685ebb92fe92d5921a60ab6148f71c99b3c703b7ce60b55aeb007b9e2081f");

            Assert.IsTrue(Helper.Hash<SHA3_512>(new byte[] { 0xFF }, 10000) ==
                "49b29a72a066d42a4a34507b4aa35deb543d1f27176c636275899d3861e46e5a6eee05f07bde5823337c4c20a5cb1eccc8883fc13456019eb970a62879bd1ec3");
        }

        [TestMethod]
        public void SHAKE128_Test()
        {
            Assert.IsTrue(SHAKE128("", 256) == "7f9c2ba4e88f827d616045507605853ed73b8093f6efbc88eb1a6eacfa66ef26");
            Assert.IsTrue(SHAKE128("The quick brown fox jumps over the lazy dog", 256) ==
                "f4202e3c5852f9182a0430fd8144f0a74b95e7417ecae17db0f8cfeed0e3e66e");
            Assert.IsTrue(SHAKE128("The quick brown fox jumps over the lazy dof", 256) ==
                "853f4538be0db9621a6cea659a06c1107b1f83f02b13d18297bd39d7411cf10c");

            Assert.IsTrue(SHAKE128("*", 2048) ==
                "7e500eafbf5b7eb520dbf1ea0d149aa3d652f7a9e305c5d7eac2bfe34513ca7c6e2368398fb59e41ca35ada6681e307e92001d53b9f9e91a7a744d9c64c4b888645b86b9bc3411f32315b30e36e90bd95921004f680edb884be2b04729ae0d2b9517614dd903f79d7a784fff23203d13c90611c5dadc4488657f0522b9e5836e41d8bbe11712e1e289794161976462cc6d98e593ef3eb994e79b00064acfe8916effdbbc238060f45830b04db40cd1604a90288a7417a2b076b126c091ef0b55bb7ad12f5bc927cb2dbd9c2b16c9e3ac54b453c4374c698501b2a9fb97f3490e6d56477bf0244dd3c2b0ce1da6b7477af54bc6e939fe32d8d36936d214839631");
        }

        [TestMethod]
        public void SHAKE256_Test()
        {
            Assert.IsTrue(SHAKE256("", 512) == "46b9dd2b0ba88d13233b3feb743eeb243fcd52ea62b81b82b50c27646ed5762fd75dc4ddd8c0f200cb05019d67b592f6fc821c49479ab48640292eacb3b7c4be");

            Assert.IsTrue(SHAKE256("*", 2048) ==
                "d37ae4cc3e03bb73790a92612f4dcaa12804eee120312331b1c65b2e164b5575e306f7e018065cdae26e33304453d7be3a22d31d91608a6aceccb627d26c4bf02dd408aa5fe6f5c276fec157528ed45e604ce674c438f4101fd649052dcd65fbc5450e0c7d81d030c4b75cd9601c264ae7acabc494d3834b27518d36c1bf81b6bcd0d3f1e7c801ae2eb97f7c233f0ae62a3926b9d33499dc043b16c453c93fde9bf11905c995ead3c0763d521910488ae9d64ba9b907997d9cd56c19815459bee65e37e6091fffc5ed5b7d7365db3a3e367975a4d6884a6cf711d515caa2ee7d7553781a07f243849a8409020788bafee51832e8f2f41f7f4c9eaa0f8e900aa4");
        }

        private static string SHAKE128(string input, int bits)
        {
            var shake = new SHAKE128(bits);
            shake.Update(Encoding.ASCII.GetBytes(input));
            return BitHelper.BytesToHex(shake.ComputeHash());
        }

        private static string SHAKE256(string input, int bits)
        {
            var shake = new SHAKE256(bits);
            shake.Update(Encoding.ASCII.GetBytes(input));
            return BitHelper.BytesToHex(shake.ComputeHash());
        }
    }
}