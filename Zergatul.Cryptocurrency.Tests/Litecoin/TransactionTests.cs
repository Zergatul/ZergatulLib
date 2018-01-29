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

        [TestMethod]
        public void Parse1()
        {

        }

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            //_repository = new TestTransactionRepository();
        }
    }
}