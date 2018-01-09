using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Litecoin
{
    public class Transaction : TransactionBase
    {
        //public uint Version { get; set; }
        //public List<Input> Inputs { get; set; }
        //public List<Output> Outputs { get; set; }
        //public uint LockTime { get; set; }

        public static Transaction FromBytes(byte[] data, ref int index)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var transaction = new Transaction();
            //int startIndex = index;

            //if (data.Length < index + 4)
            //    throw new ArgumentException("Data too short", nameof(data));
            //transaction.Version = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            //index += 4;

            //if (data.Length < index + 2)
            //    throw new TransactionParseException();
            //bool segwit = data[index] == 0 && (data[index + 1] & 1) == 1;
            //if (segwit)
            //    index += 2;

            //int inputsCount = checked((int)VarLengthInt.Parse(data, ref index));
            //transaction.Inputs = new List<Input>();
            //for (int i = 0; i < inputsCount; i++)
            //    transaction.Inputs.Add(new Input(data, ref index));

            //int outputsCount = checked((int)VarLengthInt.Parse(data, ref index));
            //transaction.Outputs = new List<Output>();
            //for (int i = 0; i < outputsCount; i++)
            //    transaction.Outputs.Add(new Output(data, ref index));

            //if (segwit)
            //{
            //    if (data.Length < index + 1)
            //        throw new TransactionParseException();
            //    int count = data[index++];
            //    transaction.SegWit = new byte[count][];
            //    for (int i = 0; i < count; i++)
            //    {
            //        int length = checked((int)VarLengthInt.Parse(data, ref index));
            //        if (data.Length < index + length)
            //            throw new TransactionParseException();
            //        transaction.SegWit[i] = ByteArray.SubArray(data, index, length);
            //        index += length;
            //    }
            //}

            //if (data.Length < index + 4)
            //    throw new ArgumentException("Data too short", nameof(data));
            //transaction.LockTime = BitHelper.ToUInt32(data, index, ByteOrder.LittleEndian);
            //index += 4;

            //transaction.Raw = ByteArray.SubArray(data, startIndex, index - startIndex);

            return transaction;
        }

        public static Transaction FromHex(string hex)
        {
            int index = 0;
            return FromBytes(BitHelper.HexToBytes(hex), ref index);
        }
    }
}