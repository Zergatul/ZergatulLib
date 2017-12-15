using System;
using System.Collections.Generic;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class Transaction : TransactionBase
    {
        public uint Version { get; set; }
        public List<Input> Inputs { get; set; }
        public List<Output> Outputs { get; set; }
        public uint LockTime { get; set; }
        public byte[][] SegWit { get; set; }

        public static Transaction FromBytes(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var transaction = new Transaction();
            int startIndex = index;

            if (data.Length < index + 4)
                throw new ArgumentException("Data too short", nameof(data));
            transaction.Version = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            if (data.Length < index + 2)
                throw new TransactionParseException();
            bool segwit = data[index] == 0 && (data[index + 1] & 1) == 1;
            if (segwit)
                index += 2;

            int inputsCount = checked((int)VarLengthInt.Parse(data, ref index));
            transaction.Inputs = new List<Input>();
            for (int i = 0; i < inputsCount; i++)
                transaction.Inputs.Add(new Input(data, ref index));

            int outputsCount = checked((int)VarLengthInt.Parse(data, ref index));
            transaction.Outputs = new List<Output>();
            for (int i = 0; i < outputsCount; i++)
                transaction.Outputs.Add(new Output(data, ref index));

            if (segwit)
            {
                if (data.Length < index + 1)
                    throw new TransactionParseException();
                int count = data[index++];
                transaction.SegWit = new byte[count][];
                for (int i = 0; i < count; i++)
                {
                    int length = checked((int)VarLengthInt.Parse(data, ref index));
                    if (data.Length < index + length)
                        throw new TransactionParseException();
                    transaction.SegWit[i] = ByteArray.SubArray(data, index, length);
                    index += length;
                }
            }

            if (data.Length < index + 4)
                throw new ArgumentException("Data too short", nameof(data));
            transaction.LockTime = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            transaction.Raw = ByteArray.SubArray(data, startIndex, index - startIndex);

            return transaction;
        }

        public static Transaction FromBytes(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            int index = 0;
            return FromBytes(data, ref index);
        }

        public static Transaction FromHex(string hex) => FromBytes(BitHelper.HexToBytes(hex));

        public bool IsCoinbase =>
            Inputs.Count == 1 &&
            ByteArray.IsZero(Inputs[0].PrevTx) &&
            Inputs[0].SequenceNo == uint.MaxValue;

        public bool Verify()
        {
            return false;
        }

        public class Input
        {
            public byte[] PrevTx { get; set; }
            public uint PrevTxOutIndex { get; set; }
            public byte[] ScriptRaw { get; set; }
            public uint SequenceNo { get; set; }

            public string PrevTxIDString => BitHelper.BytesToHex(PrevTx);
            public Script Script { get; set; }
            public Address Address { get; set; }

            internal Input(byte[] data, ref int index)
            {
                if (data.Length < index + 32)
                    throw new ArgumentOutOfRangeException();
                PrevTx = ByteArray.SubArray(data, index, 32);
                Array.Reverse(PrevTx);
                index += 32;

                if (data.Length < index + 4)
                    throw new ArgumentOutOfRangeException();
                PrevTxOutIndex = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
                index += 4;

                int scriptLength = checked((int)VarLengthInt.Parse(data, ref index));
                if (data.Length < index + scriptLength)
                    throw new ArgumentOutOfRangeException();
                ScriptRaw = ByteArray.SubArray(data, index, scriptLength);
                index += scriptLength;

                if (data.Length < index + 4)
                    throw new ArgumentOutOfRangeException();
                SequenceNo = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
                index += 4;

                // Parse script
                try
                {
                    Script = Script.FromBytes(ScriptRaw);
                }
                catch (ScriptParseException)
                {
                    Script = null;
                }
                if (Script.IsPayToPublicKeyHashInput)
                    Address = P2PKHAddress.FromPublicKey(Script.Code[1].Data);
            }
        }

        public class Output
        {
            public ulong Amount { get; set; }
            public decimal AmountBTC => (decimal)Amount / 100000000;
            public byte[] ScriptRaw { get; set; }

            public Script Script { get; set; }
            public Address Address { get; set; }

            internal Output(byte[] data, ref int index)
            {
                if (data.Length < index + 8)
                    throw new ArgumentOutOfRangeException();
                Amount = BitHelper.ToUInt64(data, index, ByteOrder.LittleEndian);
                index += 8;

                int scriptLength = checked((int)VarLengthInt.Parse(data, ref index));
                if (data.Length < index + scriptLength)
                    throw new ArgumentOutOfRangeException();
                ScriptRaw = ByteArray.SubArray(data, index, scriptLength);
                index += scriptLength;

                // Parse script
                try
                {
                    Script = Script.FromBytes(ScriptRaw);
                }
                catch (ScriptParseException)
                {
                    Script = null;
                }
                if (Script.IsPayToPublicKeyHash)
                    Address = P2PKHAddress.FromPublicKeyHash(Script.Code[2].Data);
                if (Script.IsPayToPublicKey)
                    Address = P2PKAddress.FromPublicKey(Script.Code[0].Data);
                if (Script.IsPayToScriptHash)
                    Address = P2SHAddress.FromScriptHash(Script.Code[1].Data);
            }
        }
    }
}