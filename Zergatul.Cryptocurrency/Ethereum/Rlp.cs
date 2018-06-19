using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency.Ethereum
{
    public class Rlp
    {
        public static byte[] Encode(RlpItem rdl)
        {
            var result = new List<byte>();

            rdl.SanityCheck();

            if (rdl.String != null)
            {
                if (rdl.String.Length == 1 && rdl.String[0] < 0x80)
                    result.Add(rdl.String[0]);
                else if (rdl.String.Length <= 55)
                {
                    result.Add((byte)(0x80 + rdl.String.Length));
                    result.AddRange(rdl.String);
                }
                else
                {
                    EncodeLength(result, rdl.String.Length, 0xB7);
                    result.AddRange(rdl.String);
                }
            }

            if (rdl.Items != null)
            {
                var list = new List<byte>();
                for (int i = 0; i < rdl.Items.Length; i++)
                    list.AddRange(Encode(rdl.Items[i]));

                if (list.Count <= 55)
                    result.Add((byte)(0xC0 + list.Count));
                else
                    EncodeLength(result, list.Count, 0xF7);

                result.AddRange(list);
            }

            return result.ToArray();
        }

        public static RlpItem Decode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length == 0)
                throw new ArgumentException();

            int index = 0;
            var item = DecodeOne(data, ref index);
            if (index != data.Length)
                throw new InvalidOperationException("Unexpected end");

            return item;
        }

        private static void EncodeLength(List<byte> result, int length, int offset)
        {
            byte[] lenBytes = BitHelper.GetBytes(length, ByteOrder.BigEndian);
            int len = 4;
            for (int i = 0; i < 4; i++)
                if (lenBytes[i] == 0)
                    len--;
                else
                    break;
            result.Add((byte)(offset + len));
            result.AddRange(lenBytes.Skip(4 - len));
        }

        private static int DecodeLength(byte[] data, int index, int length)
        {
            if (length > 4)
                throw new NotImplementedException();

            int result = 0;
            for (int i = 0; i < length; i++)
                result = (result << 8) | data[index + i];

            return result;
        }

        private static RlpItem DecodeOne(byte[] data, ref int index)
        {
            if (data[index] < 0x80)
            {
                var item = new RlpItem
                {
                    String = new[] { data[index] }
                };
                index++;
                return item;
            }
            else if (data[index] <= 0xBF)
            {
                int length;
                if (data[index] <= 0xB7)
                {
                    length = data[index++] - 0x80;
                }
                else
                {
                    int encLength = data[index++] - 0xB7;
                    length = DecodeLength(data, index, encLength);
                    index += encLength;
                }

                var item = new RlpItem
                {
                    String = ByteArray.SubArray(data, index, length)
                };
                index += length;
                return item;
            }
            else
            {
                int length;
                if (data[index] <= 0xF7)
                {
                    length = data[index++] - 0xC0;
                }
                else
                {
                    int encLength = data[index++] - 0xF7;
                    length = DecodeLength(data, index, encLength);
                    index += encLength;
                }
                int end = index + length;
                var list = new List<RlpItem>();
                while (index < end)
                    list.Add(DecodeOne(data, ref index));
                if (index != end)
                    throw new InvalidOperationException();
                return new RlpItem
                {
                    Items = list.ToArray()
                };
            }

            throw new InvalidOperationException();
        }
    }

    public class RlpItem
    {
        public byte[] String { get; set; }
        public RlpItem[] Items { get; set; }

        public RlpItem()
        {

        }

        public RlpItem(int value)
        {
            int index = 0;
            byte[] data = BitHelper.GetBytes(value, ByteOrder.BigEndian);
            while (index < data.Length && data[index] == 0)
                index++;
            String = ByteArray.SubArray(data, index, data.Length - index);
        }

        public RlpItem(long value)
        {
            int index = 0;
            byte[] data = BitHelper.GetBytes(value, ByteOrder.BigEndian);
            while (index < data.Length && data[index] == 0)
                index++;
            String = ByteArray.SubArray(data, index, data.Length - index);
        }

        public RlpItem(byte[] @string)
        {
            String = @string ?? new byte[0];
        }

        public void SanityCheck()
        {
            if (String != null && Items != null)
                throw new InvalidOperationException();
            if (String == null && Items == null)
                throw new InvalidOperationException();
        }
    }
}