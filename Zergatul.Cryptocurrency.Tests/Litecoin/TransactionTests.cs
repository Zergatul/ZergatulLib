using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.Litecoin;

namespace Zergatul.Cryptocurrency.Tests.Litecoin
{
    [TestClass]
    public class TransactionTests
    {
        private static ITransactionRepository<Transaction> _repository;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _repository = new SimpleTransactionRepository<Transaction>("Litecoin/Transactions.txt");
        }
    }
}