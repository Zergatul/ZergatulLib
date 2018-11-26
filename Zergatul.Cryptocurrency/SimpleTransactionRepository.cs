using System;
using System.Collections.Generic;
using System.IO;
using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency
{
    public class SimpleTransactionRepository<T> : ITransactionRepository<T>
        where T : TransactionBase, new()
    {
        private Dictionary<string, T> _transactions = new Dictionary<string, T>();

        public SimpleTransactionRepository()
        {
            
        }

        public SimpleTransactionRepository(string filename)
        {
            foreach (var line in File.ReadAllLines(filename))
            {
                string[] parts = line.Split(':');
                if (!_transactions.ContainsKey(parts[0]))
                {
                    var tx = new T();
                    if (!tx.TryParseHex(parts[1]))
                        throw new InvalidOperationException();
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

        public void Add(T transaction)
        {
            _transactions.Add(transaction.IDString, transaction);
        }
    }
}