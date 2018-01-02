using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class Block : BlockBase
    {
        private Transaction[] _transactions;
        public Transaction[] Transactions
        {
            get
            {
                return _transactions;
            }
            set
            {
                _transactions = value;
                _txs = value.Cast<TransactionBase>().ToArray();
            }
        }

        public static Block FromBytes(byte[] data)
        {
            var block = new Block();
            block.Parse(data);
            block._transactions = block._txs.Cast<Transaction>().ToArray();
            block.MiningWork = block.BlockID;
            return block;
        }

        public static Block FromHex(string hex) => FromBytes(BitHelper.HexToBytes(hex));

        protected override TransactionBase ParseTransaction(byte[] data, ref int index)
        {
            return Transaction.FromBytes(data, ref index);
        }

        public bool Validate(ITransactionRepository<Transaction> repository)
        {
            if (!ValidateMerkleRoot())
                return false;

            switch (Version)
            {
                // https://github.com/bitcoin/bips/blob/master/bip-0034.mediawiki
                case 2:
                    var coinbase = Transactions[0];
                    var script = coinbase.Inputs[0].Script;
                    if (script != null) // script is valid
                    {
                        if (script.Code.Count == 0)
                            return false;
//                        if (script.Code[0] != )
                    }
                    else
                        throw new NotImplementedException();
                    break;
            }

            foreach (var tx in Transactions)
                if (!tx.Verify(repository))
                    return false;

            return true;
        }
    }
}