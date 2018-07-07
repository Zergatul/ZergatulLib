using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency.Verge
{
    public class Transaction : TransactionBase<TxInput, TxOutput>
    {
        public uint Time { get; set; }

        public decimal? FeeXVG => Fee == null ? 0 : 1m * Fee.Value / BlockchainCryptoFactory.Verge.Multiplier;

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
    }
}