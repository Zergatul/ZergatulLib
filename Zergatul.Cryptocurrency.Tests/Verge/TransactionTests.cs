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
        }

        [ClassInitialize]
        public static void InitRepository(TestContext context)
        {
            _repository = new SimpleTransactionRepository<Transaction>("Verge/Transactions.txt");
        }
    }
}