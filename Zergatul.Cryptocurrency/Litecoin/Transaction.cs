using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Litecoin
{
    public class Transaction : TransactionBase
    {
        public override IEnumerable<TxInputBase> GetInputs()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TxOutputBase> GetOutputs()
        {
            throw new NotImplementedException();
        }

        public override void Parse(byte[] data, ref int index)
        {
            throw new NotImplementedException();
        }
    }
}