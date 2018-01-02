using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool Verify(ITransactionRepository<Transaction> repository)
        {
            if (IsCoinbase)
                return true;

            foreach (var input in Inputs)
            {
                if (input.Script == null)
                    return false;

                var prevTx = repository.GetTransaction(input.PrevTx);
                if (prevTx == null)
                    return false;

                if (input.PrevTxOutIndex >= prevTx.Outputs.Count)
                    return false;

                var prevOutput = prevTx.Outputs[checked((int)input.PrevTxOutIndex)];
                var script = prevOutput.Script;

                if (script == null)
                    return false;

                if (input.Script.Code.Any(o => o.Data == null))
                    return false;

                byte[] verifyBytes = GetVerifyBytes(input, prevOutput.ScriptRaw);
                if (!script.Run(verifyBytes, input.Script.Code.Select(o => o.Data).ToArray()))
                    return false;

                // BIP-0016
                if (script.IsPayToScriptHash)
                {
                    Script serializedScript;
                    try
                    {
                        serializedScript = Script.FromBytes(input.Script.Code.Last().Data);
                    }
                    catch (ScriptParseException)
                    {
                        return false;
                    }

                    if (!serializedScript.Run(verifyBytes, input.Script.Code.Take(input.Script.Code.Count - 1).Select(o => o.Data).ToArray()))
                        return false;
                }
            }

            return true;
        }

        private byte[] GetVerifyBytes(Input verifyInput, byte[] prevSript)
        {
            List<byte> txCopy = new List<byte>();

            txCopy.AddRange(BitHelper.GetBytes(Version, ByteOrder.LittleEndian));

            txCopy.AddRange(VarLengthInt.Serialize(Inputs.Count));
            foreach (var input in Inputs)
                if (input != verifyInput)
                    input.SerializeWithoutScripts(txCopy);
                else
                    input.SerializeWithScript(txCopy, prevSript);

            txCopy.AddRange(VarLengthInt.Serialize(Outputs.Count));
            foreach (var output in Outputs)
                output.Serialize(txCopy);

            txCopy.AddRange(BitHelper.GetBytes(LockTime, ByteOrder.LittleEndian));

            return txCopy.ToArray();
        }

        public override string ToString()
        {
            if (IDString != null)
                return "Bitcoin.Transaction: " + IDString;
            else
                return "Bitcoin.Transaction";
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
                if (Script?.IsPayToPublicKeyHashInput ?? false)
                {
                    var addr = new P2PKHAddress();
                    addr.FromPublicKey(Script.Code[1].Data);
                    Address = addr;
                }
            }

            public void SerializeWithoutScripts(List<byte> bytes)
            {
                bytes.AddRange(PrevTx.Reverse());
                bytes.AddRange(BitHelper.GetBytes(PrevTxOutIndex, ByteOrder.LittleEndian));
                bytes.Add(0); // script length
                bytes.AddRange(BitHelper.GetBytes(SequenceNo, ByteOrder.LittleEndian));
            }

            public void SerializeWithScript(List<byte> bytes, byte[] script)
            {
                bytes.AddRange(PrevTx.Reverse());
                bytes.AddRange(BitHelper.GetBytes(PrevTxOutIndex, ByteOrder.LittleEndian));
                bytes.AddRange(VarLengthInt.Serialize(script.Length));
                bytes.AddRange(script);
                bytes.AddRange(BitHelper.GetBytes(SequenceNo, ByteOrder.LittleEndian));
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
                {
                    var addr = new P2PKHAddress();
                    addr.FromPublicKeyHash(Script.Code[2].Data);
                    Address = addr;
                }
                if (Script.IsPayToPublicKey)
                    Address = P2PKAddress.FromPublicKey(Script.Code[0].Data);
                if (Script.IsPayToScriptHash)
                {
                    var addr = new P2SHAddress();
                    addr.FromScriptHash(Script.Code[1].Data);
                    Address = addr;
                }
            }

            public void Serialize(List<byte> bytes)
            {
                bytes.AddRange(BitHelper.GetBytes(Amount, ByteOrder.LittleEndian));
                bytes.AddRange(VarLengthInt.Serialize(ScriptRaw.Length));
                bytes.AddRange(ScriptRaw);
            }
        }
    }
}