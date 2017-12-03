using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class Block
    {
        public int Version { get; set; }
        public byte[] PrevBlockID { get; set; }
        public byte[] MerkleRoot { get; set; }
        public uint Timestamp { get; set; }
        public uint Bits { get; set; }
        public uint Nonce { get; set; }
        public Transaction[] Transactions { get; set; }
        public byte[] BlockID { get; set; }

        public string PrevBlockIDString => BitHelper.BytesToHex(PrevBlockID);
        public string BlockIDString => BitHelper.BytesToHex(BlockID);
        public string MerkleRootString => BitHelper.BytesToHex(MerkleRoot);
        public DateTime Date => Constants.UnixTimeStart.AddSeconds(Timestamp);

        public static Block FromBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException();

            if (data.Length < 80)
                throw new BlockParseException();

            var block = new Block();

            block.Version = BitHelper.ToInt32(data, 0, ByteOrder.LittleEndian);
            block.PrevBlockID = ByteArray.SubArray(data, 4, 32);
            Array.Reverse(block.PrevBlockID);
            block.MerkleRoot = ByteArray.SubArray(data, 36, 32);
            Array.Reverse(block.MerkleRoot);
            block.Timestamp = BitHelper.ToUInt32(data, 68, ByteOrder.LittleEndian);
            block.Bits = BitHelper.ToUInt32(data, 72, ByteOrder.LittleEndian);
            block.Nonce = BitHelper.ToUInt32(data, 76, ByteOrder.LittleEndian);
            int index = 80;
            try
            {
                block.Transactions = new Transaction[VarLengthInt.Parse(data, ref index)];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new BlockParseException(ex);
            }

            for (int i = 0; i < block.Transactions.Length; i++)
            {
                block.Transactions[i] = Transaction.FromBytes(data, ref index);
            }

            if (index != data.Length)
                throw new BlockParseException();

            var hash256 = new DoubleSHA256();
            hash256.Update(data, 0, 80);
            block.BlockID = hash256.ComputeHash();
            Array.Reverse(block.BlockID);

            return block;
        }

        public static Block FromHex(string hex) => FromBytes(BitHelper.HexToBytes(hex));

        public bool ValidateMerkleRoot()
        {
            var hash256 = new DoubleSHA256();

            List<byte[]> hashes = new List<byte[]>(Transactions.Length);
            for (int i = 0; i < hashes.Count; i++)
            {
                byte[] copy = (byte[])Transactions[i].ID.Clone();
                Array.Reverse(copy);
                hashes.Add(copy);
            }

            while (hashes.Count > 1)
            {
                List<byte[]> newHashes = new List<byte[]>();
                for (int i = 0; i < hashes.Count / 2; i++)
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