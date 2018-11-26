using System;
using System.Collections.Generic;
using System.Linq;
using Zergatul.Cryptocurrency.Base;

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
            if (Time == 0)
                throw new InvalidOperationException();

            base.Sign();
        }
    }
}