using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency.Base
{
    public abstract class TransactionBase
    {
        public uint Version { get; set; }
        public uint LockTime { get; set; }
        public bool IsSegWit { get; set; }

        public byte[] RawSegWit { get; protected set; }
        public byte[] RawOriginal { get; protected set; }

        protected byte[] _id;
        public byte[] ID
        {
            get
            {
                if (_id == null)
                {
                    _id = DoubleSHA256.Hash(RawOriginal);
                    Array.Reverse(_id);
                }
                return _id;
            }
        }
        public string IDString => BitHelper.BytesToHex(ID);

        public abstract IEnumerable<TxInputBase> GetInputs();
        public abstract IEnumerable<TxOutputBase> GetOutputs();

        public abstract long? Fee { get; }

        public bool IsCoinbase =>
            GetInputs().Count() == 1 &&
            ByteArray.IsZero(GetInputs().First().PrevTx) &&
            GetInputs().First().SequenceNo == uint.MaxValue;

        public abstract bool TryParse(byte[] data, ref int index);

        public bool TryParse(byte[] data)
        {
            int index = 0;
            return TryParse(data, ref index);
        }

        public bool TryParseHex(string hex) => TryParse(BitHelper.HexToBytes(hex));

        public virtual void ParseHeader(byte[] data, ref int index)
        {
            Version = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;
        }

        public virtual void SerializeHeader(List<byte> buffer)
        {
            buffer.AddRange(BitHelper.GetBytes(Version, ByteOrder.LittleEndian));
        }

        public bool Verify()
        {
            if (IsCoinbase)
                return true;

            return GetInputs().All(input => input.Verify());
        }

        public bool Verify(ITransactionRepository<TransactionBase> repository)
        {
            foreach (var input in GetInputs())
            {
                if (!IsCoinbase)
                {
                    input.PrevTransaction = repository.GetTransaction(input.PrevTx);
                    if (input.PrevTransaction == null)
                        return false;
                }
            }

            return Verify();
        }

        public virtual void Sign()
        {
            if (Version == 0)
                throw new InvalidOperationException();

            var inputs = GetInputs();
            var outputs = GetOutputs();

            if (inputs == null || inputs.Count() == 0)
                throw new InvalidOperationException();
            foreach (var input in inputs)
            {
                input.Transaction = this;

                if (input.PrevTransaction == null)
                    throw new InvalidOperationException();
                if (input.PrevTxOutIndex < 0 || input.PrevTxOutIndex >= input.PrevTransaction.GetOutputs().Count())
                    throw new InvalidOperationException();

                if (input.PrevTx == null)
                    input.PrevTx = input.PrevTransaction.ID;

                if (!ByteArray.Equals(input.PrevTx, input.PrevTransaction.ID))
                    throw new InvalidOperationException();

                input.PrevOutput = input.PrevTransaction.GetOutputs().ElementAt(input.PrevTxOutIndex);
                input.PrevOutputExpandedScript = input.PrevOutput.Script.Expand();

                if (input.Address == null)
                    throw new InvalidOperationException();
                if (input.PrevTx?.Length != 32)
                    throw new InvalidOperationException();
                if (input.SequenceNo < 0xFFFFFFFE)
                    throw new InvalidOperationException();
            }

            if (outputs == null || outputs.Count() == 0)
                throw new InvalidOperationException();
            foreach (var output in outputs)
            {
                if (output.Address == null)
                    throw new InvalidOperationException();
                if (output.Amount == 0)
                    throw new InvalidOperationException();
            }

            foreach (var output in outputs)
            {
                output.Script = output.Address.CreateRedeemScript();
                output.ScriptRaw = output.Script.ToBytes();
            }

            foreach (var input in inputs)
            {
                input.Address.Sign(input);
            }
        }

        public virtual byte[] ToBytes()
        {
            var buffer = new List<byte>();
            SerializeHeader(buffer);

            if (IsSegWit)
            {
                buffer.AddRange(VarLengthInt.Serialize(0));
                buffer.Add(0x01);
            }

            var inputs = GetInputs();
            buffer.AddRange(VarLengthInt.Serialize(inputs.Count()));
            foreach (var input in inputs)
                input.Serialize(buffer);

            var outputs = GetOutputs();
            buffer.AddRange(VarLengthInt.Serialize(outputs.Count()));
            foreach (var output in outputs)
                output.Serialize(buffer);

            if (IsSegWit)
            {
                foreach (var input in inputs)
                {
                    if (input.SegWit != null)
                    {
                        buffer.AddRange(VarLengthInt.Serialize(input.SegWit.Length));
                        foreach (byte[] data in input.SegWit)
                        {
                            buffer.AddRange(VarLengthInt.Serialize(data.Length));
                            buffer.AddRange(data);
                        }
                    }
                    else
                        buffer.AddRange(VarLengthInt.Serialize(0));
                }
            }

            buffer.AddRange(BitHelper.GetBytes(LockTime, ByteOrder.LittleEndian));

            return buffer.ToArray();
        }

        public override string ToString() => IDString;
    }

    public abstract class TransactionBase<TxInput, TxOutput> : TransactionBase
        where TxInput : TxInputBase, new()
        where TxOutput : TxOutputBase, new()
    {
        public List<TxInput> Inputs { get; set; }
        public List<TxOutput> Outputs { get; set; }

        public override IEnumerable<TxInputBase> GetInputs() => Inputs;
        public override IEnumerable<TxOutputBase> GetOutputs() => Outputs;

        public override long? Fee
        {
            get
            {
                if (Inputs != null && Outputs != null)
                {
                    ulong inputsSum = 0;
                    for (int i = 0; i < Inputs.Count; i++)
                        if (Inputs[i].Amount == null)
                        {
                            if (Inputs[i].PrevTransaction != null)
                                inputsSum += Inputs[i].PrevTransaction.GetOutputs().ElementAt(Inputs[i].PrevTxOutIndex).Amount;
                        }
                        else
                            inputsSum += Inputs[i].Amount.Value;

                    ulong outputsSum = 0;
                    for (int i = 0; i < Outputs.Count; i++)
                        outputsSum += Outputs[i].Amount;

                    return checked((long)inputsSum - (long)outputsSum);
                }
                else
                    return null;
            }
        }

        public override bool TryParse(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            int startIndex = index;

            if (data.Length < index + 4)
                return false;

            ParseHeader(data, ref index);

            int segwitMarkerStart = index;
            int segwitMarkerEnd;
            int inputsCount = VarLengthInt.ParseInt32(data, ref index);
            IsSegWit = false;
            if (inputsCount == 0)
            {
                if (data.Length < index + 1)
                    return false;
                if (data[index++] != 1)
                    return false;
                IsSegWit = true;
                segwitMarkerEnd = segwitMarkerStart + 2;

                inputsCount = VarLengthInt.ParseInt32(data, ref index);
            }
            else
                segwitMarkerEnd = segwitMarkerStart;

            ParseInputs(data, ref index, inputsCount);

            int outputsCount = VarLengthInt.ParseInt32(data, ref index);
            ParseOutputs(data, ref index, outputsCount);

            int segwitStart = index;
            if (IsSegWit)
            {
                for (int i = 0; i < inputsCount; i++)
                {
                    int count = VarLengthInt.ParseInt32(data, ref index);
                    Inputs[i].SegWit = new byte[count][];
                    for (int j = 0; j < count; j++)
                    {
                        int length = VarLengthInt.ParseInt32(data, ref index);
                        if (data.Length < index + length)
                            return false;
                        Inputs[i].SegWit[j] = ByteArray.SubArray(data, index, length);
                        index += length;
                    }
                }
            }
            int segwitEnd = index;

            for (int i = 0; i < inputsCount; i++)
                Inputs[i].ParseAddress();

            if (data.Length < index + 4)
                return false;
            LockTime = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            RawSegWit = ByteArray.SubArray(data, startIndex, index - startIndex);
            var list = new List<byte>(256);
            for (int i = startIndex; i < segwitMarkerStart; i++)
                list.Add(data[i]);
            for (int i = segwitMarkerEnd; i < segwitStart; i++)
                list.Add(data[i]);
            for (int i = segwitEnd; i < index; i++)
                list.Add(data[i]);
            RawOriginal = list.ToArray();

            return true;
        }

        protected void ParseInputs(byte[] data, ref int index, int count)
        {
            Inputs = new List<TxInput>(count);
            for (int i = 0; i < count; i++)
            {
                TxInput input = new TxInput();
                input.Transaction = this;
                input.Parse(data, ref index);
                Inputs.Add(input);
            }
        }

        protected void ParseOutputs(byte[] data, ref int index, int count)
        {
            Outputs = new List<TxOutput>(count);
            for (int i = 0; i < count; i++)
            {
                TxOutput output = new TxOutput();
                output.Parse(data, ref index);
                Outputs.Add(output);
            }
        }
    }
}