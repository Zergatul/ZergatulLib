using System;
using System.Collections.Generic;
using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Litecoin
{
    public class Transaction : TransactionBase
    {
        public override long? Fee
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IEnumerable<TxInputBase> GetInputs()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TxOutputBase> GetOutputs()
        {
            throw new NotImplementedException();
        }

        public override void Sign()
        {
            throw new System.NotImplementedException();
        }

        public override bool TryParse(byte[] data, ref int index)
        {
            throw new NotImplementedException();
        }
    }
}