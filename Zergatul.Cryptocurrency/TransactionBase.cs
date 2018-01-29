using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency
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
                    var hash = new DoubleSHA256();
                    hash.Update(RawOriginal);
                    _id = hash.ComputeHash();
                    Array.Reverse(_id);
                }
                return _id;
            }
        }
        public string IDString => BitHelper.BytesToHex(ID);

        public abstract IEnumerable<TxInputBase> GetInputs();
        public abstract IEnumerable<TxOutputBase> GetOutputs();

        public bool IsCoinbase =>
            GetInputs().Count() == 1 &&
            ByteArray.IsZero(GetInputs().First().PrevTx) &&
            GetInputs().First().SequenceNo == uint.MaxValue;

        protected byte[] _header;
        protected byte[] _tail;

        public abstract void Parse(byte[] data, ref int index);

        public void Parse(byte[] data)
        {
            int index = 0;
            Parse(data, ref index);
        }

        public void ParseHex(string hex) => Parse(BitHelper.HexToBytes(hex));

        public bool Verify()
        {
            if (IsCoinbase)
                return true;

            return GetInputs().All(input => input.Verify());
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