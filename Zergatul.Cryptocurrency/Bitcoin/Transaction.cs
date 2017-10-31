using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class Transaction
    {
        public uint Version { get; set; }
        public List<Input> Inputs { get; set; }
        public List<Output> Outputs { get; set; }
        public uint LockTime { get; set; }

        public static Transaction FromBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var transaction = new Transaction();
            int index = 0;

            if (data.Length < index + 4)
                throw new ArgumentException("Data too short", nameof(data));
            transaction.Version = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            int inputsCount = checked((int)VarLengthInt.Parse(data, ref index));
            transaction.Inputs = new List<Input>();
            for (int i = 0; i < inputsCount; i++)
                transaction.Inputs.Add(new Input(data, ref index));

            int outputsCount = checked((int)VarLengthInt.Parse(data, ref index));
            transaction.Outputs = new List<Output>();
            for (int i = 0; i < outputsCount; i++)
                transaction.Outputs.Add(new Output(data, ref index));

            if (data.Length < index + 4)
                throw new ArgumentException("Data too short", nameof(data));
            transaction.LockTime = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            return transaction;
        }

        public static Transaction FromHex(string hex)
        {
            return FromBytes(BitHelper.HexToBytes(hex));
        }

        public class Input
        {
            public byte[] PrevTx { get; set; }
            public uint PrevTxOutIndex { get; set; }
            public byte[] Script { get; set; }
            public uint SequenceNo { get; set; }

            internal Input(byte[] data, ref int index)
            {
                if (data.Length < index + 32)
                    throw new ArgumentOutOfRangeException();
                PrevTx = ByteArray.SubArray(data, index, 32);
                index += 32;

                if (data.Length < index + 4)
                    throw new ArgumentOutOfRangeException();
                PrevTxOutIndex = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
                index += 4;

                int scriptLength = checked((int)VarLengthInt.Parse(data, ref index));
                if (data.Length < index + scriptLength)
                    throw new ArgumentOutOfRangeException();
                Script = ByteArray.SubArray(data, index, scriptLength);
                index += scriptLength;

                if (data.Length < index + 4)
                    throw new ArgumentOutOfRangeException();
                SequenceNo = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
                index += 4;
            }
        }

        public class Output
        {
            public ulong Amount { get; set; }
            public byte[] Script { get; set; }

            internal Output(byte[] data, ref int index)
            {
                if (data.Length < index + 8)
                    throw new ArgumentOutOfRangeException();
                Amount = BitHelper.ToUInt64(data, index, ByteOrder.LittleEndian);
                index += 8;

                int scriptLength = checked((int)VarLengthInt.Parse(data, ref index));
                if (data.Length < index + scriptLength)
                    throw new ArgumentOutOfRangeException();
                Script = ByteArray.SubArray(data, index, scriptLength);
                index += scriptLength;
            }
        }
    }
}