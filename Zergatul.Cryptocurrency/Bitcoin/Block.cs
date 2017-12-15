﻿using System;
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
    }
}