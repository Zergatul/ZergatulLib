using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.Litecoin;

namespace Zergatul.Cryptocurrency.Tests.Litecoin
{
    class TestTransactionRepository : ITransactionRepository<Transaction>
    {
        private Dictionary<string, Transaction> _transactions;

        public TestTransactionRepository()
        {
            _transactions = new Dictionary<string, Transaction>();
            foreach (var line in File.ReadAllLines("Litecoin/Transactions.txt"))
            {
                string[] parts = line.Split(':');
                if (!_transactions.ContainsKey(parts[0]))
                    _transactions.Add(parts[0], Transaction.FromHex(parts[1]));
            }
        }

        public Transaction GetTransaction(string id)
        {
            Transaction result;
            if (_transactions.TryGetValue(id, out result))
                return result;
            else
                return null;
        }

        public Transaction GetTransaction(byte[] id) => GetTransaction(BitHelper.BytesToHex(id));
    }
}