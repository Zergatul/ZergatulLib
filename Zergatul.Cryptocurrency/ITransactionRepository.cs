using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency
{
    public interface ITransactionRepository<T>
        where T : TransactionBase
    {
        T GetTransaction(byte[] id);
        T GetTransaction(string id);
    }
}