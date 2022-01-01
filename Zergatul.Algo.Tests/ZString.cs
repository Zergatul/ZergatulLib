using System;

namespace Zergatul.Algo.Tests
{
    public struct ZString : IEquatable<ZString>, IComparable<ZString>
    {
        public byte[] Data;

        public ZString(byte[] data, int offset, int length)
        {
            Data = new byte[length];
            Array.Copy(data, offset, Data, 0, length);
        }

        public override int GetHashCode()
        {
            int num = 5381;
            int num2 = num;
            for (int i = 0; i < Data.Length; i += 2)
            {
                num = (((num << 5) + num) ^ Data[i]);

                if (i + 1 < Data.Length)
                    num2 = (((num2 << 5) + num2) ^ Data[i + 1]);
            }
            return num + num2 * 1566083941;
        }

        public bool Equals(ZString other)
        {
            if (Data.Length != other.Data.Length)
                return false;

            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i] != other.Data[i])
                    return false;
            }

            return true;
        }

        public int CompareTo(ZString other)
        {
            int min = System.Math.Min(Data.Length, other.Data.Length);
            for (int i = 0; i < min; i++)
            {
                int delta = Data[i] - other.Data[i];
                if (delta != 0)
                    return delta;
            }
            return Data.Length - other.Data.Length;
        }
    }
}