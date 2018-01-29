using System;

namespace Zergatul.Cryptocurrency
{
    internal static class VarLengthInt
    {
        public static ulong Parse(byte[] data, ref int index)
        {
            if (data.Length < index + 1)
                throw new ArgumentOutOfRangeException();

            byte first = data[index++];

            if (first < 0xFD)
                return first;

            if (first == 0xFD)
            {
                index += 2;
                if (data.Length < index + 1)
                    throw new ArgumentOutOfRangeException();
                return BitHelper.ToUInt16(data, index - 2, ByteOrder.LittleEndian);
            }

            if (first == 0xFE)
            {
                index += 4;
                if (data.Length < index + 1)
                    throw new ArgumentOutOfRangeException();
                return BitHelper.ToUInt32(data, index - 4, ByteOrder.LittleEndian);
            }

            // first == 0xFF
            index += 8;
            if (data.Length < index + 1)
                throw new ArgumentOutOfRangeException();
            return BitHelper.ToUInt64(data, index - 8, ByteOrder.LittleEndian);
        }

        public static int ParseInt32(byte[] data, ref int index) => checked((int)Parse(data, ref index));

        public static byte[] Serialize(int value) => Serialize(checked((ulong)value));

        public static byte[] Serialize(ulong value)
        {
            if (value < 0xFD)
                return new[] { checked((byte)value) };

            throw new NotImplementedException();
        }
    }
}