using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.KDF;

namespace Zergatul.Cryptocurrency.Litecoin
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

        public Block()
            : base(BlockchainCryptoFactory.Litecoin)
        {

        }

        public static Block FromBytes(byte[] data)
        {
            var block = new Block();
            block.Parse(data);
            block._transactions = block._txs.Cast<Transaction>().ToArray();

            byte[] header = ByteArray.SubArray(data, 0, 80);
            var scrypt = new Scrypt();
            block.MiningWork = scrypt.DeriveKeyBytes(header, header, 1, 1024, 1, 32);
            Array.Reverse(block.MiningWork);

            return block;
        }

        public static Block FromHex(string hex) => FromBytes(BitHelper.HexToBytes(hex));
    }
}