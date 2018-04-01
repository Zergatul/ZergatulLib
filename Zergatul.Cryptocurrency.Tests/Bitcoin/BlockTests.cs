using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Bitcoin;
using System.IO;
using System.Linq;

namespace Zergatul.Cryptocurrency.Tests.Bitcoin
{
    [TestClass]
    public class BlockTests
    {
        private static ITransactionRepository<Transaction> _repository;

        [TestMethod]
        public void Bitcoin_Blk_v1_000001_Genesis()
        {
            var block = Block.FromHex("0100000000000000000000000000000000000000000000000000000000000000000000003ba3edfd7a7b12b27ac72c3e67768f617fc81bc3888a51323a9fb8aa4b1e5e4a29ab5f49ffff001d1dac2b7c0101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d0104455468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b73ffffffff0100f2052a01000000434104678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5fac00000000");

            Assert.IsTrue(block.BlockIDString == "000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f");

            Assert.IsTrue(block.Version == 1);
            Assert.IsTrue(block.PrevBlockIDString == "0000000000000000000000000000000000000000000000000000000000000000");
            Assert.IsTrue(block.MerkleRootString == "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");
            Assert.IsTrue(block.Date == new DateTime(2009, 1, 3, 18, 15, 5));
            Assert.IsTrue(block.Bits == 486604799);
            Assert.IsTrue(block.NonceUInt32 == 2083236893);

            Assert.IsTrue(block.Transactions.Length == 1);
            Assert.IsTrue(block.Transactions[0].IDString == "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");

            Assert.IsTrue(block.Validate(_repository));
        }

        [TestMethod]
        public void Bitcoin_Blk_v1_000170()
        {
            var block = Block.FromHex(File.ReadAllText("Bitcoin/Block-000170.txt"));

            Assert.IsTrue(block.BlockIDString == "00000000d1145790a8694403d4063f323d499e655c83426834d4ce2f8dd4a2ee");

            Assert.IsTrue(block.Version == 1);
            Assert.IsTrue(block.PrevBlockIDString == "000000002a22cfee1f2c846adbd12b3e183d4f97683f85dad08a79780a84bd55");
            Assert.IsTrue(block.MerkleRootString == "7dac2c5666815c17a3b36427de37bb9d2e2c5ccec3f8633eb91a4205cb4c10ff");
            Assert.IsTrue(block.Date == new DateTime(2009, 1, 12, 3, 30, 25));
            Assert.IsTrue(block.Bits == 486604799);
            Assert.IsTrue(block.NonceUInt32 == 1889418792);

            Assert.IsTrue(block.Transactions.Length == 2);
            Assert.IsTrue(block.Transactions[0].IDString == "b1fea52486ce0c62bb442b530a3f0132b826c74e473d1f2c220bfa78111c5082");
            Assert.IsTrue(block.Transactions[1].IDString == "f4184fc596403b9d638783cf57adfe4c75c605f6356fbc91338530e9831e9e16");

            Assert.IsTrue(block.Validate(_repository));
        }

        [TestMethod]
        public void Bitcoin_Blk_v1_050001()
        {
            var block = Block.FromHex(File.ReadAllText("Bitcoin/Block-050001.txt"));

            Assert.IsTrue(block.BlockIDString == "000000001c920d495e1eeef2452b6d1c6c229a919b28196c103ecffebabee141");

            Assert.IsTrue(block.MerkleRootString == "ee3a2d2b895cafacff526d06a55b55e049cf84a9735e4a63f7fd08f96d0f4649");

            Assert.IsTrue(block.ValidateMerkleRoot());
        }

        [TestMethod]
        public void Bitcoin_Blk_v1_080003()
        {
            var block = Block.FromHex(File.ReadAllText("Bitcoin/Block-080003.txt"));

            Assert.IsTrue(block.BlockIDString == "000000000042a2cf1cddf23ee040f9ee162f84db7898efc9a03c181b50c2f2a7");

            Assert.IsTrue(block.MerkleRootString == "6796c0612630413f02e962224fe9c9d77e50400e217c7c0d556b1b83beee6558");

            Assert.IsTrue(block.ValidateMerkleRoot());
        }

        [TestMethod]
        public void Bitcoin_Blk_v1_057043_Pizza()
        {
            var block = Block.FromHex(File.ReadAllText("Bitcoin/Block-057043.txt"));

            Assert.IsTrue(block.BlockIDString == "00000000152340ca42227603908689183edc47355204e7aca59383b0aaac1fd8");

            Assert.IsTrue(block.Version == 1);
            Assert.IsTrue(block.PrevBlockIDString == "0000000013e7e85518dac94d012d73253d3fdac5c30c4143b177f3086f129580");
            Assert.IsTrue(block.MerkleRootString == "5c1d2211f598cd6498f42b269fe3ce4a6fdb40eaa638f86a0579c4e63a721b5a");
            Assert.IsTrue(block.Date == new DateTime(2010, 5, 22, 18, 16, 31));
            Assert.IsTrue(block.Bits == 471178276);
            Assert.IsTrue(block.NonceUInt32 == 188133155);

            Assert.IsTrue(block.Validate(_repository));
        }

        [TestMethod]
        public void Bitcoin_Blk_v1_160720()
        {
            var block = Block.FromHex(File.ReadAllText("Bitcoin/Block-160720.txt"));

            Assert.IsTrue(block.BlockIDString == "0000000000000be899d85554ae2719639e3d3c87868727aec7c51bd5fd9dfe1a");

            Assert.IsTrue(block.Version == 1);
            Assert.IsTrue(block.PrevBlockIDString == "0000000000000c4d02272e78ab8d8c5410e2ab06bef6cd634aa35ad0dc5d4e08");
            Assert.IsTrue(block.MerkleRootString == "a3e613778ac1f7ed8bc5aef0c0c2b335bce7861a62b900623e0e2ecc5a0031d9");
            Assert.IsTrue(block.Date == new DateTime(2012, 1, 5, 12, 23, 24));
            Assert.IsTrue(block.Bits == 437155514);
            Assert.IsTrue(block.NonceUInt32 == 2642529779);

            Assert.IsTrue(block.Validate(_repository));
        }

        [TestMethod]
        public void Bitcoin_Blk_v1_170060_FirstP2SH()
        {
            var block = Block.FromHex(File.ReadAllText("Bitcoin/Block-170060.txt"));

            Assert.IsTrue(block.BlockIDString == "00000000000002dc756eebf4f49723ed8d30cc28a5f108eb94b1ba88ac4f9c22");

            Assert.IsTrue(block.Version == 1);
            Assert.IsTrue(block.PrevBlockIDString == "00000000000003bd2bf59124b97f3878a52a641d8ab9e9e7ec8628b381d32ad6");
            Assert.IsTrue(block.MerkleRootString == "be9686ff253cc9538a776618375e787c2a4a929c3741bdf412b2837bbf68d09b");
            Assert.IsTrue(block.Date == new DateTime(2012, 3, 7, 16, 33, 03));
            Assert.IsTrue(block.Bits == 436942092);
            Assert.IsTrue(block.NonceUInt32 == 557958899);

            string txs = string.Join(Environment.NewLine, block.Transactions.SelectMany(t => t.Inputs.Select(i => i.PrevTxIDString)).Distinct().OrderBy(_ => _));

            Assert.IsTrue(block.Validate(_repository));
        }

        [TestMethod]
        public void Bitcoin_Blk_v2_209080()
        {
            var block = Block.FromHex(File.ReadAllText("Bitcoin/Block-0000000000000306fbb4526ef225e86d9daa4ac968b67ed930188d3f3e771896.txt"));

            Assert.IsTrue(block.BlockIDString == "0000000000000306fbb4526ef225e86d9daa4ac968b67ed930188d3f3e771896");

            Assert.IsTrue(block.Version == 2);
            Assert.IsTrue(block.PrevBlockIDString == "00000000000001991b57a7f352ebc922fce2f6874a97e43b30fce9f21a6d925f");
            Assert.IsTrue(block.MerkleRootString == "089d7815a140010948d8d630bd475f91543bb339408a5f9e07f45990d6ee75b5");
            Assert.IsTrue(block.Date == new DateTime(2012, 11, 22, 15, 53, 36));
            Assert.IsTrue(block.Bits == 436533995);
            Assert.IsTrue(block.NonceUInt32 == 1350362980);

            Assert.IsTrue(block.Transactions.Length == 168);

            //Assert.IsTrue(block.Validate());
        }

        [TestMethod]
        public void Bitcoin_Blk_Parse497401()
        {
            var block = Block.FromHex(File.ReadAllText("Bitcoin/Block-497401.txt"));

            Assert.IsTrue(block.BlockIDString == "0000000000000000009c2165d2665885cd634e34ebca1fb9d46ba7fc3a64ae25");

            Assert.IsTrue(block.Version == 0x20000000);
            Assert.IsTrue(block.PrevBlockIDString == "0000000000000000001dfb2206314e39ee12a850382fc128dccf2c966e2aed25");
            Assert.IsTrue(block.MerkleRootString == "cf5c3692bda0a20c61041eb4c8808f98100df69f42ddcf94345f0c3192932543");
            Assert.IsTrue(block.Date == new DateTime(2017, 12, 3, 17, 7, 58));
            Assert.IsTrue(block.Bits == 402706678);
            Assert.IsTrue(block.NonceUInt32 == 335261844);

            Assert.IsTrue(block.Transactions.Length == 2301);
            Assert.IsTrue(block.Transactions[0].IDString == "69efdcb4169c4f09a3e83ec9bafef534b9ec3bbcf1e6b921f95a0b6f3f356301");

            Assert.IsTrue(block.ValidateMerkleRoot());
        }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _repository = new SimpleTransactionRepository<Transaction>("Bitcoin/Transactions.txt");
        }
    }
}