using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class Transaction : TransactionBase<TxInput, TxOutput>
    {
        public decimal? FeeBTC => Fee == null ? 0 : 1m * Fee.Value / BlockchainCryptoFactory.Bitcoin.Multiplier;

        public override void Parse(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            int startIndex = index;

            if (data.Length < index + 4)
                throw new ArgumentException("Data too short", nameof(data));

            ParseHeader(data, ref index);

            int segwitMarkerStart = index;
            int segwitMarkerEnd;
            int inputsCount = VarLengthInt.ParseInt32(data, ref index);
            IsSegWit = false;
            if (inputsCount == 0)
            {
                if (data.Length < index + 1)
                    throw new TransactionParseException();
                if (data[index++] != 1)
                    throw new TransactionParseException();
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
                            throw new TransactionParseException();
                        Inputs[i].SegWit[j] = ByteArray.SubArray(data, index, length);
                        index += length;
                    }
                }
            }
            int segwitEnd = index;

            if (data.Length < index + 4)
                throw new ArgumentException("Data too short", nameof(data));
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