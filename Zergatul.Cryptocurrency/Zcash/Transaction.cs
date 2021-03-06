﻿using System;
using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Zcash
{
    public class Transaction : TransactionBase<TxInput, TxOutput>
    {
        public decimal? FeeZEC => Fee == null ? 0 : 1m * Fee.Value / BlockchainCryptoFactory.Zcash.Multiplier;

        public override bool TryParse(byte[] data, ref int index)
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

            for (int i = 0; i < inputsCount; i++)
                Inputs[i].ParseAddress();

            return true;
        }

        public override void Sign()
        {
            throw new System.NotImplementedException();
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