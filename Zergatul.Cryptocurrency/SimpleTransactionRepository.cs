using System.Collections.Generic;
using System.IO;

namespace Zergatul.Cryptocurrency
{
    public class SimpleTransactionRepository<T> : ITransactionRepository<T>
        where T : TransactionBase, new()
    {
        private Dictionary<string, T> _transactions;

        public SimpleTransactionRepository(string filename)
        {
            _transactions = new Dictionary<string, T>();
            foreach (var line in File.ReadAllLines(filename))
            {
                string[] parts = line.Split(':');
                if (!_transactions.ContainsKey(parts[0]))
                {
                    var tx = new T();
                    tx.ParseHex(parts[1]);
                    _transactions.Add(parts[0], tx);
                }
            }
        }

        public T GetTransaction(string id)
        {
            T tx;
            if (_transactions.TryGetValue(id, out tx))
                return tx;
            else
                return null;
        }

        public T GetTransaction(byte[] id) => GetTransaction(BitHelper.BytesToHex(id));
    }
}