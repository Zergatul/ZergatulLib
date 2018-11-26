using System;
using System.Linq;
using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Zcash
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

        public byte[] Solution;

        public Block()
            : base(BlockchainCryptoFactory.Zcash)
        {

        }

        public static Block FromBytes(byte[] data)
        {
            var block = new Block();
            block.Parse(data);
            block._transactions = block._txs.Cast<Transaction>().ToArray();

            return block;
        }

        public static Block FromHex(string hex) => FromBytes(BitHelper.HexToBytes(hex));

        protected override void Parse(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();

            if (data.Length < 140)
                throw new BlockParseException();

            Version = BitHelper.ToInt32(data, 0, ByteOrder.LittleEndian);
            PrevBlockID = ByteArray.SubArray(data, 4, 32);
            Array.Reverse(PrevBlockID);
            MerkleRoot = ByteArray.SubArray(data, 36, 32);
            Array.Reverse(MerkleRoot);
            Timestamp = BitHelper.ToUInt32(data, 100, ByteOrder.LittleEndian);
            Bits = BitHelper.ToUInt32(data, 104, ByteOrder.LittleEndian);
            Nonce = ByteArray.SubArray(data, 108, 32);

            int index = 140;
            int length = checked((int)VarLengthInt.Parse(data, ref index));
            Solution = ByteArray.SubArray(data, index, length);
            index += length;

            ParseTransactions(data, ref index);

            if (index != data.Length)
                throw new BlockParseException();

            BlockID = DoubleSHA256.Hash(data, 0, 80);
            Array.Reverse(BlockID);
        }
    }
}