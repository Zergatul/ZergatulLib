using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency.Verge
{
    public class Transaction : TransactionBase<TxInput, TxOutput>
    {
        public uint Time { get; set; }

        public override void Parse(byte[] data, ref int index)
        {
            int start = index;

            Version = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            Time = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            int inputsCount = VarLengthInt.ParseInt32(data, ref index);
            ParseInputs(data, ref index, inputsCount);

            int outputsCount = VarLengthInt.ParseInt32(data, ref index);
            ParseOutputs(data, ref index, outputsCount);

            LockTime = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            index += 4;

            RawOriginal = ByteArray.SubArray(data, start, index - start);
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