using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency.Verge
{
    public class Transaction : TransactionBase<TxInput, TxOutput>
    {
        public uint Time { get; set; }

        public decimal? FeeXVG => Fee == null ? 0 : 1m * Fee.Value / BlockchainCryptoFactory.Verge.Multiplier;

        public Transaction()
            : base()
        {

        }

        public Transaction(string hex)
            : this()
        {
            ParseHex(hex);
        }

        public override void Parse(byte[] data, ref int index)
        {
            int start = index;

            ParseHeader(data, ref index);

            int inputsCount = VarLengthInt.ParseInt32(data, ref index);
            ParseInputs(data, ref index, inputsCount);

            int outputsCount = VarLengthInt.ParseInt32(data, ref index);
            ParseOutputs(data, ref index, outputsCount);

            LockTime = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            RawOriginal = ByteArray.SubArray(data, start, index - start);
        }

        public override void ParseHeader(byte[] data, ref int index)
        {
            base.ParseHeader(data, ref index);

            Time = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;
        }

        public override void SerializeHeader(List<byte> buffer)
        {
            base.SerializeHeader(buffer);

            buffer.AddRange(BitHelper.GetBytes(Time, ByteOrder.LittleEndian));
        }

        public override void Sign()
        {
            if (Version == 0)
                throw new InvalidOperationException();
            if (Time == 0)
                throw new InvalidOperationException();

            if (Inputs == null || Inputs.Count == 0)
                throw new InvalidOperationException();
            if (Inputs.Any(i => i.PrevTx?.Length != 32))
                throw new InvalidOperationException();
            if (Inputs.Any(i => (i.Address as P2PKHAddress)?.PrivateKey == null))
                throw new InvalidOperationException();
            if (Inputs.Any(i => i.SequenceNo != uint.MaxValue))
                throw new InvalidOperationException();
            if (Inputs.Any(i => !ByteArray.Equals(i.PrevTx, i.PrevTransaction.ID)))
                throw new InvalidOperationException();

            if (Outputs == null || Outputs.Count == 0)
                throw new InvalidOperationException();
            if (Outputs.Any(o => (o.Address as P2PKHAddress)?.Hash == null))
                throw new InvalidOperationException();
            if (Outputs.Any(o => o.Amount == 0))
                throw new InvalidOperationException();

            foreach (var output in Outputs)
                output.CreateScript();

            foreach (var input in Inputs)
                input.Sign();
        }

        public bool Verify(ITransactionRepository<Transaction> repository)
        {
            foreach (var input in Inputs)
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

        public byte[] ToBytes()
        {
            var buffer = new List<byte>();
            SerializeHeader(buffer);
            buffer.AddRange(VarLengthInt.Serialize(Inputs.Count));
            foreach (var input in Inputs)
                input.Serialize(buffer);
            buffer.AddRange(VarLengthInt.Serialize(Outputs.Count));
            foreach (var output in Outputs)
                output.Serialize(buffer);
            buffer.AddRange(BitHelper.GetBytes(LockTime, ByteOrder.LittleEndian));

            return buffer.ToArray();
        }
    }
}