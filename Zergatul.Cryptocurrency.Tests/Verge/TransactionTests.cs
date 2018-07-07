using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptocurrency.Verge;

namespace Zergatul.Cryptocurrency.Tests.Verge
{
    [TestClass]
    public class TransactionTests
    {
        private static SimpleTransactionRepository<Transaction> _repository;

        [TestMethod]
        public void Verge_Tx_Test1()
        {
            var tx = _repository.GetTransaction("cbb2745f2b4236178c3331477874c6726b571191e00f3d33420d44e343e509e9");
            Assert.IsTrue(tx.Verify(_repository));
            Assert.IsTrue(tx.FeeXVG == 0.110008m);
        }

        [TestMethod]
        public void Verge_Tx_Test2()
        {
            var tx = _repository.GetTransaction("957189ca86faa8f9f071c30e6d103d2085fa2607a5b31531bcdf9d9cd038b088");

            Assert.IsTrue(tx.Inputs[0].Address.Value == "DEiqRRAhMaMK7bnfQducWkyQj6K3K3JqGH");

            Assert.IsTrue(tx.Outputs[ 0].Address.Value == "D6XWszJCQvvyhe9i64rWNQBthK25VM7QTw" && tx.Outputs[ 0].AmountXVG ==    0.27349900m);
            Assert.IsTrue(tx.Outputs[ 1].Address.Value == "DN9pYRNuJ7iHeYdLtSGVA5zuGqnjCuQp8t" && tx.Outputs[ 1].AmountXVG ==    7.81398000m);
            Assert.IsTrue(tx.Outputs[ 2].Address.Value == "DGHxghntGfThsH5Yqm7QJt4VbGMbpnBnB8" && tx.Outputs[ 2].AmountXVG ==    5.76787400m);
            Assert.IsTrue(tx.Outputs[ 3].Address.Value == "DLPMApA1gGaHYX6gzL89Mrp2b2QtDRcrDf" && tx.Outputs[ 3].AmountXVG ==    5.39663300m);
            Assert.IsTrue(tx.Outputs[ 4].Address.Value == "DQYRA8YECBZnuBWPyZZnFcNJEayV1hbhUP" && tx.Outputs[ 4].AmountXVG ==    0.31135700m);
            Assert.IsTrue(tx.Outputs[ 5].Address.Value == "DRxaEYPPmjiX5mRb1QQjPCLUdHNW1j1VPz" && tx.Outputs[ 5].AmountXVG ==   51.24066800m);
            Assert.IsTrue(tx.Outputs[ 6].Address.Value == "DRKuGJj3WWrpgDWcZeVbre2XcRgai1xgHJ" && tx.Outputs[ 6].AmountXVG ==    1.60060000m);
            Assert.IsTrue(tx.Outputs[ 7].Address.Value == "DHw5JhR9qiz6So58nUbyyhuDN4M76ESVmY" && tx.Outputs[ 7].AmountXVG ==    1.55717000m);
            Assert.IsTrue(tx.Outputs[ 8].Address.Value == "DGtsbQviJ9Z6jufCzNHi68XRrnpADLxf7U" && tx.Outputs[ 8].AmountXVG ==    4.32819000m);
            Assert.IsTrue(tx.Outputs[ 9].Address.Value == "DB8LERPXsLbuDQ9me8R4VdFA5x1bLcwVBG" && tx.Outputs[ 9].AmountXVG ==    0.42132600m);
            Assert.IsTrue(tx.Outputs[10].Address.Value == "DAAabFmV26pW3RaRnGnNeWo1UhFaqpyGKt" && tx.Outputs[10].AmountXVG ==    3.20257000m);
            Assert.IsTrue(tx.Outputs[11].Address.Value == "D5oh8rB1a9qypmYrNSPTjHKz94LxZLFrt1" && tx.Outputs[11].AmountXVG ==   51.69100000m);
            Assert.IsTrue(tx.Outputs[12].Address.Value == "DF2aWBraVYqToy3hP6rcaUbCndox8Qq4FJ" && tx.Outputs[12].AmountXVG == 5426.71792800m);
            Assert.IsTrue(tx.Outputs[13].Address.Value == "DJBRXjhvj6D854UJ8WKxwAfizHyLTBAFH1" && tx.Outputs[13].AmountXVG ==    4.39502000m);
            Assert.IsTrue(tx.Outputs[14].Address.Value == "DCbcepdzbfAUXkGMe7A8NxUZu5bEZVE12L" && tx.Outputs[14].AmountXVG ==    6.24122000m);
            Assert.IsTrue(tx.Outputs[15].Address.Value == "D5SfYUkKbWc89X5yHLodp7W6VGFD3oEYeJ" && tx.Outputs[15].AmountXVG ==   10.51144000m);
            Assert.IsTrue(tx.Outputs[16].Address.Value == "DNhTvF1hf2NcX1NiC44Rj699ZxWx6DMJTh" && tx.Outputs[16].AmountXVG ==    0.94698100m);
            Assert.IsTrue(tx.Outputs[17].Address.Value == "D6i9hRLdN96mvhKhaqQNLAxd3Fwtg8SJzy" && tx.Outputs[17].AmountXVG ==    0.30037600m);
            Assert.IsTrue(tx.Outputs[18].Address.Value == "DPHmUJQd955w2g6BNqeiL2SRsJbRoCaWus" && tx.Outputs[18].AmountXVG ==    0.35673300m);
            Assert.IsTrue(tx.Outputs[19].Address.Value == "D6eDXzx2isGRmbr2APjn79KN13ATXMhbzj" && tx.Outputs[19].AmountXVG ==    0.45006500m);
            Assert.IsTrue(tx.Outputs[20].Address.Value == "D9Dxr7Suxmj6KZah81Auk3oypjmAouRjro" && tx.Outputs[20].AmountXVG ==    1.64247000m);

            Assert.IsTrue(tx.Verify(_repository));
            Assert.IsTrue(tx.FeeXVG == 0.1m);
        }

        [ClassInitialize]
        public static void InitRepository(TestContext context)
        {
            _repository = new SimpleTransactionRepository<Transaction>("Verge/Transactions.txt");
        }
    }
}