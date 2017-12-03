using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Bitcoin;
using System.IO;

namespace Zergatul.Cryptocurrency.Tests
{
    [TestClass]
    public class BlockTests
    {
        [TestMethod]
        public void ParseGenesis()
        {
            var block = Block.FromHex("0100000000000000000000000000000000000000000000000000000000000000000000003ba3edfd7a7b12b27ac72c3e67768f617fc81bc3888a51323a9fb8aa4b1e5e4a29ab5f49ffff001d1dac2b7c0101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff4d04ffff001d0104455468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b73ffffffff0100f2052a01000000434104678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5fac00000000");

            Assert.IsTrue(block.BlockIDString == "000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f");

            Assert.IsTrue(block.Version == 1);
            Assert.IsTrue(block.PrevBlockIDString == "0000000000000000000000000000000000000000000000000000000000000000");
            Assert.IsTrue(block.MerkleRootString == "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");
            Assert.IsTrue(block.Date == new DateTime(2009, 1, 3, 18, 15, 5));
            Assert.IsTrue(block.Bits == 486604799);
            Assert.IsTrue(block.Nonce == 2083236893);

            Assert.IsTrue(block.Transactions.Length == 1);
            Assert.IsTrue(block.Transactions[0].IDString == "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");

            Assert.IsTrue(block.ValidateMerkleRoot());
        }

        [TestMethod]
        public void Parse497401()
        {
            var block = Block.FromHex(File.ReadAllText("Block-0000000000000000009c2165d2665885cd634e34ebca1fb9d46ba7fc3a64ae25.txt"));

            Assert.IsTrue(block.BlockIDString == "0000000000000000009c2165d2665885cd634e34ebca1fb9d46ba7fc3a64ae25");

            Assert.IsTrue(block.Version == 1);
            Assert.IsTrue(block.PrevBlockIDString == "0000000000000000000000000000000000000000000000000000000000000000");
            Assert.IsTrue(block.MerkleRootString == "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");
            Assert.IsTrue(block.Date == new DateTime(2009, 1, 3, 18, 15, 5));
            Assert.IsTrue(block.Bits == 486604799);
            Assert.IsTrue(block.Nonce == 2083236893);

            Assert.IsTrue(block.Transactions.Length == 1);
            Assert.IsTrue(block.Transactions[0].IDString == "4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");

            Assert.IsTrue(block.ValidateMerkleRoot());
        }
    }
}