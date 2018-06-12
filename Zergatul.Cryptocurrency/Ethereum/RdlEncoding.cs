using System;
using System.Collections.Generic;
using System.Linq;

namespace Zergatul.Cryptocurrency.Ethereum
{
    public class RdlEncoding
    {
        public static byte[] Encode(RdlItem rdl)
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

        public static RdlItem Decode(byte[] data)
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

        private static RdlItem DecodeOne(byte[] data, ref int index)
        {
            if (data[index] < 0x80)
            {
                var item = new RdlItem
                {
                    String = new[] { data[index] }
                };
                index++;
                return item;
            }
            else if (data[index] <= 0xB7)
            {
                int length = data[index] - 0x80;
                index++;
                var item = new RdlItem
                {
                    String = ByteArray.SubArray(data, index, length)
                };
                index += length;
                return item;
            }
            else if (data[index] <= 0xBF)
            {

            }
            else
                throw new NotImplementedException();
        }
    }

    public class RdlItem
    {
        public byte[] String { get; set; }
        public RdlItem[] Items { get; set; }

        public void SanityCheck()
        {
            if (String != null && Items != null)
                throw new InvalidOperationException();
            if (String == null && Items == null)
                throw new InvalidOperationException();
        }
    }
}