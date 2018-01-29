using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.Bitcoin;
using Zergatul.Network;

namespace Zergatul.Cryptocurrency
{
    public abstract class BlockBase
    {
        public int Version { get; set; }
        public byte[] PrevBlockID { get; set; }
        public byte[] MerkleRoot { get; set; }
        public uint Timestamp { get; set; }
        public uint Bits { get; set; }
        public byte[] Nonce { get; set; }
        public byte[] BlockID { get; set; }

        public byte[] MiningWork { get; protected set; }

        public string PrevBlockIDString => BitHelper.BytesToHex(PrevBlockID);
        public string BlockIDString => BitHelper.BytesToHex(BlockID);
        public string MerkleRootString => BitHelper.BytesToHex(MerkleRoot);
        public DateTime Date => Constants.UnixTimeStart.AddSeconds(Timestamp);
        public string MiningWorkString => BitHelper.BytesToHex(MiningWork);

        protected TransactionBase[] _txs;
        protected BlockchainCryptoFactory _factory;

        protected BlockBase(BlockchainCryptoFactory factory)
        {
            this._factory = factory;
        }

        protected virtual void Parse(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();

            if (data.Length < 80)
                throw new BlockParseException();

            Version = BitHelper.ToInt32(data, 0, ByteOrder.LittleEndian);
            PrevBlockID = ByteArray.SubArray(data, 4, 32);
            Array.Reverse(PrevBlockID);
            MerkleRoot = ByteArray.SubArray(data, 36, 32);
            Array.Reverse(MerkleRoot);
            Timestamp = BitHelper.ToUInt32(data, 68, ByteOrder.LittleEndian);
            Bits = BitHelper.ToUInt32(data, 72, ByteOrder.LittleEndian);
            Nonce = ByteArray.SubArray(data, 76, 4);

            int index = 80;
            ParseTransactions(data, ref index);

            if (index != data.Length)
                throw new BlockParseException();

            var hash256 = new DoubleSHA256();
            hash256.Update(data, 0, 80);
            BlockID = hash256.ComputeHash();
            Array.Reverse(BlockID);
        }

        protected virtual void ParseTransactions(byte[] data, ref int index)
        {
            try
            {
                _txs = new TransactionBase[VarLengthInt.Parse(data, ref index)];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new BlockParseException(ex);
            }

            for (int i = 0; i < _txs.Length; i++)
            {
                _txs[i] = _factory.GetTransaction();
                _txs[i].Parse(data, ref index);
            }
        }

        public bool ValidateMerkleRoot()
        {
            var hash256 = new DoubleSHA256();

            List<byte[]> hashes = new List<byte[]>(_txs.Length);
            for (int i = 0; i < _txs.Length; i++)
            {
                byte[] copy = (byte[])_txs[i].ID.Clone();
                Array.Reverse(copy);
                hashes.Add(copy);
            }

            while (hashes.Count > 1)
            {
                List<byte[]> newHashes = new List<byte[]>();
                for (int i = 0; i < (hashes.Count + 1) / 2; i++)
                {
                    byte[] left = hashes[2 * i];
                    byte[] right = 2 * i + 1 >= hashes.Count ? left : hashes[2 * i + 1];
                    hash256.Reset();
                    hash256.Update(left);
                    hash256.Update(right);
                    newHashes.Add(hash256.ComputeHash());
                }
                hashes = newHashes;
            }

            Array.Reverse(hashes[0]);

            return ByteArray.Equals(MerkleRoot, hashes[0]);
        }
    }
}