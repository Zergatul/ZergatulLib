using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Zcash;
using System.IO;

namespace Zergatul.Cryptocurrency.Tests.Zcash
{
    [TestClass]
    public class BlockTests
    {
        private static ITransactionRepository<Transaction> _repository;

        [TestMethod]
        public void Zcash_Blk_Parse200000()
        {
            var block = Block.FromHex(File.ReadAllText("Zcash/Block-200000.txt"));

            Assert.IsTrue(block.PrevBlockIDString == "0000000007a0ba7630c7c90a52ba75744d6a4ac4a217fd3c9b4a1f94ae54e264");
            Assert.IsTrue(block.MerkleRootString == "c89f77d8fafa8c9fd83f23f873119fd1b65683530d25abf46cb757ce525929a2");
            Assert.IsTrue(block.Bits == 0x1C113270);

            Assert.IsTrue(block.Transactions.Length == 3);

            var p2pkh = new P2PKHAddress();
            var p2sh = new P2SHAddress();

            #region tx 1

            var tx = block.Transactions[0];

            Assert.IsTrue(tx.IDString == "300ecab537aee50ec36cb5141992e8d753ab53dfc2913cf008cd028c1ffe8fd9");
            Assert.IsTrue(tx.Outputs[0].Address.Value == "t1VpYecBW4UudbGcy4ufh61eWxQCoFaUrPs");
            Assert.IsTrue(tx.Outputs[0].AmountZEC == 10.00027088m);
            Assert.IsTrue(tx.Outputs[1].Address.Value == "t3ZnCNAvgu6CSyHm1vWtrx3aiN98dSAGpnD");
            Assert.IsTrue(tx.Outputs[1].AmountZEC == 2.5m);

            #endregion

            #region tx 2

            tx = block.Transactions[1];

            Assert.IsTrue(tx.IDString == "8db32c1353707a9a65a3444ebbd0420adfcf6f9b1b5403df17dbc9d78bf7c5b3");
            Assert.IsTrue(tx.Inputs[0].Address.Value == "t1h5NPSv8u9toAk5cEGD4d3c5PHvaKXYBip");
            Assert.IsTrue(tx.Inputs[1].Address.Value == "t1RvSjYKZT6HptjG56GeuhkcfuniKLrbjxx");
            Assert.IsTrue(tx.Inputs[2].Address.Value == "t1HthTrciD2kKEet2dE29no3Lfm8oLV1gTY");
            Assert.IsTrue(tx.Inputs[3].Address.Value == "t1fE9n3pvYNPmKHWSXX5Sj5rmkXU1KFetKv");
            Assert.IsTrue(tx.Inputs[4].Address.Value == "t1LW8Nu2tGAABsbPmnyLheunTCMpNNycZWD");
            Assert.IsTrue(tx.Inputs[5].Address.Value == "t1dbRBuRwDm7cRoU6Gpkj24V7wBSThsP5Uj");
            Assert.IsTrue(tx.Outputs[0].Address.Value == "t1QEgd5NXKRnRRMiH9d2cnfRQnHLQic6aYV");
            Assert.IsTrue(tx.Outputs[1].Address.Value == "t1ctyPqmnxK28kFKVSNVY5aJ2VXerEfLxwC");

            Assert.IsTrue(tx.Verify(_repository));
            Assert.IsTrue(tx.FeeZEC == 0.00017088m);

            #endregion

            #region tx 3

            tx = block.Transactions[2];

            Assert.IsTrue(tx.IDString == "8c065a5fcf675c56fe438e4aa701ff1769b905c4b0f1f8e1a38068bd0c179c6d");
            Assert.IsTrue(tx.Inputs[0].Address.Value == "t1VhnvAS5Q6fGPH3QmbhqBGbYvGo1qk1kz7");
            Assert.IsTrue(tx.Inputs[1].Address.Value == "t1VhnvAS5Q6fGPH3QmbhqBGbYvGo1qk1kz7");
            Assert.IsTrue(tx.Inputs[2].Address.Value == "t1VhnvAS5Q6fGPH3QmbhqBGbYvGo1qk1kz7");
            Assert.IsTrue(tx.Inputs[3].Address.Value == "t1VhnvAS5Q6fGPH3QmbhqBGbYvGo1qk1kz7");
            Assert.IsTrue(tx.Outputs[0].Address.Value == "t1c5N8e2Z9r9at4bb5rz88WaTyAaDzfyWno");
            Assert.IsTrue(tx.Outputs[1].Address.Value == "t1fWioxSZGxqwhUS5WNTfgqhvs9jD2uKnxJ");
            //Assert.IsTrue(tx.FeeZEC == 0.0001m);

            #endregion
        }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _repository = new SimpleTransactionRepository<Transaction>("Zcash/Transactions.txt");
        }
    }
}