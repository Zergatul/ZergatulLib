using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;

namespace Zergatul.Network.Http
{
    // https://tools.ietf.org/html/rfc7541
    public class Hpack
    {
        #region Static

        private static readonly Header[] StaticTable = new Header[]
        {
            new Header(":authority", null),
            new Header(":method", "GET"),
            new Header(":method", "POST"),
            new Header(":path", "/"),
            new Header(":path", "/index.html"),
            new Header(":scheme", "http"),
            new Header(":scheme", "https"),
            new Header(":status", "200"),
            new Header(":status", "204"),
            new Header(":status", "206"),
            new Header(":status", "304"),
            new Header(":status", "400"),
            new Header(":status", "404"),
            new Header(":status", "500"),
            new Header("accept-charset", null),
            new Header("accept-encoding", "gzip, deflate"),
            new Header("accept-language", null),
            new Header("accept-ranges", null),
            new Header("accept", null),
            new Header("access-control-allow-origin", null),
            new Header("age", null),
            new Header("allow", null),
            new Header("authorization", null),
            new Header("cache-control", null),
            new Header("content-disposition", null),
            new Header("content-encoding", null),
            new Header("content-language", null),
            new Header("content-length", null),
            new Header("content-location", null),
            new Header("content-range", null),
            new Header("content-type", null),
            new Header("cookie", null),
            new Header("date", null),
            new Header("etag", null),
            new Header("expect", null),
            new Header("expires", null),
            new Header("from", null),
            new Header("host", null),
            new Header("if-match", null),
            new Header("if-modified-since", null),
            new Header("if-none-match", null),
            new Header("if-range", null),
            new Header("if-unmodified-since", null),
            new Header("last-modified", null),
            new Header("link", null),
            new Header("location", null),
            new Header("max-forwards", null),
            new Header("proxy-authenticate", null),
            new Header("proxy-authorization", null),
            new Header("range", null),
            new Header("referer", null),
            new Header("refresh", null),
            new Header("retry-after", null),
            new Header("server", null),
            new Header("set-cookie", null),
            new Header("strict-transport-security", null),
            new Header("transfer-encoding", null),
            new Header("user-agent", null),
            new Header("vary", null),
            new Header("via", null),
            new Header("www-authenticate", null)
        };

        private static readonly int[] HuffmanCode = new int[]
        {
            0x00001FF8, 0x007FFFD8, 0x0FFFFFE2, 0x0FFFFFE3, 0x0FFFFFE4, 0x0FFFFFE5, 0x0FFFFFE6, 0x0FFFFFE7, 0x0FFFFFE8,
            0x00FFFFEA, 0x3FFFFFFC, 0x0FFFFFE9, 0x0FFFFFEA, 0x3FFFFFFD, 0x0FFFFFEB, 0x0FFFFFEC, 0x0FFFFFED, 0x0FFFFFEE,
            0x0FFFFFEF, 0x0FFFFFF0, 0x0FFFFFF1, 0x0FFFFFF2, 0x3FFFFFFE, 0x0FFFFFF3, 0x0FFFFFF4, 0x0FFFFFF5, 0x0FFFFFF6,
            0x0FFFFFF7, 0x0FFFFFF8, 0x0FFFFFF9, 0x0FFFFFFA, 0x0FFFFFFB, 0x00000014, 0x000003F8, 0x000003F9, 0x00000FFA,
            0x00001FF9, 0x00000015, 0x000000F8, 0x000007FA, 0x000003FA, 0x000003FB, 0x000000F9, 0x000007FB, 0x000000FA,
            0x00000016, 0x00000017, 0x00000018, 0x00000000, 0x00000001, 0x00000002, 0x00000019, 0x0000001A, 0x0000001B,
            0x0000001C, 0x0000001D, 0x0000001E, 0x0000001F, 0x0000005C, 0x000000FB, 0x00007FFC, 0x00000020, 0x00000FFB,
            0x000003FC, 0x00001FFA, 0x00000021, 0x0000005D, 0x0000005E, 0x0000005F, 0x00000060, 0x00000061, 0x00000062,
            0x00000063, 0x00000064, 0x00000065, 0x00000066, 0x00000067, 0x00000068, 0x00000069, 0x0000006A, 0x0000006B,
            0x0000006C, 0x0000006D, 0x0000006E, 0x0000006F, 0x00000070, 0x00000071, 0x00000072, 0x000000FC, 0x00000073,
            0x000000FD, 0x00001FFB, 0x0007FFF0, 0x00001FFC, 0x00003FFC, 0x00000022, 0x00007FFD, 0x00000003, 0x00000023,
            0x00000004, 0x00000024, 0x00000005, 0x00000025, 0x00000026, 0x00000027, 0x00000006, 0x00000074, 0x00000075,
            0x00000028, 0x00000029, 0x0000002A, 0x00000007, 0x0000002B, 0x00000076, 0x0000002C, 0x00000008, 0x00000009,
            0x0000002D, 0x00000077, 0x00000078, 0x00000079, 0x0000007A, 0x0000007B, 0x00007FFE, 0x000007FC, 0x00003FFD,
            0x00001FFD, 0x0FFFFFFC, 0x000FFFE6, 0x003FFFD2, 0x000FFFE7, 0x000FFFE8, 0x003FFFD3, 0x003FFFD4, 0x003FFFD5,
            0x007FFFD9, 0x003FFFD6, 0x007FFFDA, 0x007FFFDB, 0x007FFFDC, 0x007FFFDD, 0x007FFFDE, 0x00FFFFEB, 0x007FFFDF,
            0x00FFFFEC, 0x00FFFFED, 0x003FFFD7, 0x007FFFE0, 0x00FFFFEE, 0x007FFFE1, 0x007FFFE2, 0x007FFFE3, 0x007FFFE4,
            0x001FFFDC, 0x003FFFD8, 0x007FFFE5, 0x003FFFD9, 0x007FFFE6, 0x007FFFE7, 0x00FFFFEF, 0x003FFFDA, 0x001FFFDD,
            0x000FFFE9, 0x003FFFDB, 0x003FFFDC, 0x007FFFE8, 0x007FFFE9, 0x001FFFDE, 0x007FFFEA, 0x003FFFDD, 0x003FFFDE,
            0x00FFFFF0, 0x001FFFDF, 0x003FFFDF, 0x007FFFEB, 0x007FFFEC, 0x001FFFE0, 0x001FFFE1, 0x003FFFE0, 0x001FFFE2,
            0x007FFFED, 0x003FFFE1, 0x007FFFEE, 0x007FFFEF, 0x000FFFEA, 0x003FFFE2, 0x003FFFE3, 0x003FFFE4, 0x007FFFF0,
            0x003FFFE5, 0x003FFFE6, 0x007FFFF1, 0x03FFFFE0, 0x03FFFFE1, 0x000FFFEB, 0x0007FFF1, 0x003FFFE7, 0x007FFFF2,
            0x003FFFE8, 0x01FFFFEC, 0x03FFFFE2, 0x03FFFFE3, 0x03FFFFE4, 0x07FFFFDE, 0x07FFFFDF, 0x03FFFFE5, 0x00FFFFF1,
            0x01FFFFED, 0x0007FFF2, 0x001FFFE3, 0x03FFFFE6, 0x07FFFFE0, 0x07FFFFE1, 0x03FFFFE7, 0x07FFFFE2, 0x00FFFFF2,
            0x001FFFE4, 0x001FFFE5, 0x03FFFFE8, 0x03FFFFE9, 0x0FFFFFFD, 0x07FFFFE3, 0x07FFFFE4, 0x07FFFFE5, 0x000FFFEC,
            0x00FFFFF3, 0x000FFFED, 0x001FFFE6, 0x003FFFE9, 0x001FFFE7, 0x001FFFE8, 0x007FFFF3, 0x003FFFEA, 0x003FFFEB,
            0x01FFFFEE, 0x01FFFFEF, 0x00FFFFF4, 0x00FFFFF5, 0x03FFFFEA, 0x007FFFF4, 0x03FFFFEB, 0x07FFFFE6, 0x03FFFFEC,
            0x03FFFFED, 0x07FFFFE7, 0x07FFFFE8, 0x07FFFFE9, 0x07FFFFEA, 0x07FFFFEB, 0x0FFFFFFE, 0x07FFFFEC, 0x07FFFFED,
            0x07FFFFEE, 0x07FFFFEF, 0x07FFFFF0, 0x03FFFFEE, 0x3FFFFFFF
        };

        private static readonly int[] HuffmanCodeLen = new int[]
        {
            13, 23, 28, 28, 28, 28, 28, 28, 28, 24, 30, 28, 28, 30, 28, 28, 28, 28, 28, 28, 28, 28, 30, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 06, 10, 10, 12, 13, 06, 08, 11, 10, 10, 08, 11, 08, 06, 06, 06, 05, 05, 05, 06, 06, 06,
            06, 06, 06, 06, 07, 08, 15, 06, 12, 10, 13, 06, 07, 07, 07, 07, 07, 07, 07, 07, 07, 07, 07, 07, 07, 07, 07,
            07, 07, 07, 07, 07, 07, 07, 08, 07, 08, 13, 19, 13, 14, 06, 15, 05, 06, 05, 06, 05, 06, 06, 06, 05, 07, 07,
            06, 06, 06, 05, 06, 07, 06, 05, 05, 06, 07, 07, 07, 07, 07, 15, 11, 14, 13, 28, 20, 22, 20, 20, 22, 22, 22,
            23, 22, 23, 23, 23, 23, 23, 24, 23, 24, 24, 22, 23, 24, 23, 23, 23, 23, 21, 22, 23, 22, 23, 23, 24, 22, 21,
            20, 22, 22, 23, 23, 21, 23, 22, 22, 24, 21, 22, 23, 23, 21, 21, 22, 21, 23, 22, 23, 23, 20, 22, 22, 22, 23,
            22, 22, 23, 26, 26, 20, 19, 22, 23, 22, 25, 26, 26, 26, 27, 27, 26, 24, 25, 19, 21, 26, 27, 27, 26, 27, 24,
            21, 21, 26, 26, 28, 27, 27, 27, 20, 24, 20, 21, 22, 21, 21, 23, 22, 22, 25, 25, 24, 24, 26, 23, 26, 27, 26,
            26, 27, 27, 27, 27, 27, 28, 27, 27, 27, 27, 27, 26, 30
        };

        private static readonly Tree HuffmanTree;

        #endregion

        public int DynamicTableSize { get; private set; }

        private int _maxTableSize;
        private List<Header> _dynamicTable;
        private int _dynamicTableCount;

        public Hpack(int maxTableSize)
        {
            this._maxTableSize = maxTableSize;
            this._dynamicTable = new List<Header>();
        }

        public void Encode(Stream stream, IEnumerable<Header> headers)
        {
            foreach (var header in headers)
            {
                int nameIndex = -1;
                int fullIndex = -1;

                // search in static table
                for (int i = 0; i < StaticTable.Length; i++)
                {
                    if (StaticTable[i].Name == header.Name)
                    {
                        if (nameIndex == -1)
                            nameIndex = i;
                        if (StaticTable[i].Value == header.Value)
                        {
                            fullIndex = i;
                            break;
                        }
                    }
                }

                // search in dynamic table
                if (fullIndex == -1)
                {
                    for (int i = 0; i < _dynamicTableCount; i++)
                    {
                        if (_dynamicTable[_dynamicTable.Count - i - 1].Name == header.Name)
                        {
                            if (nameIndex == -1)
                                nameIndex = StaticTable.Length + i;
                            if (_dynamicTable[_dynamicTable.Count - i - 1].Value == header.Value)
                            {
                                fullIndex = StaticTable.Length + i;
                                break;
                            }
                        }
                    }
                }

                if (fullIndex >= 0)
                {
                    // Indexed Header Field
                    EncodeInteger(stream, 7, 0x80, fullIndex + 1);
                    continue;
                }

                // Literal Header Field with Incremental Indexing
                if (nameIndex >= 0)
                {
                    EncodeInteger(stream, 6, 0x40, nameIndex + 1);
                }
                else
                {
                    stream.WriteByte(0x40);
                    EncodeString(stream, header.Name);
                }

                EncodeString(stream, header.Value);

                AddToDynamicTable(header);
            }
        }

        public List<Header> Decode(Stream stream)
        {
            var headers = new List<Header>();

            while (true)
            {
                int octet = stream.ReadByte();
                if (octet == -1)
                    return headers;

                if ((octet & 0x80) != 0)
                {
                    // Indexed Header Field
                    int index = octet & 0x7F;
                    index = DecodeInteger(stream, 7, index);
                    if (index == 0)
                        throw new HpackDecodingException();
                    headers.Add(GetHeaderByIndex(index));
                }
                else if ((octet & 0x40) != 0)
                {
                    // Literal Header Field with Incremental Indexing
                    int index = octet & 0x3F;
                    index = DecodeInteger(stream, 6, index);

                    Header header;
                    if (index > 0)
                    {
                        // Indexed Name
                        header = new Header(GetHeaderByIndex(index).Name, DecodeString(stream));
                    }
                    else
                    {
                        // New Name
                        header = new Header(DecodeString(stream), DecodeString(stream));
                    }

                    headers.Add(header);
                    AddToDynamicTable(header);
                }
                else if ((octet & 0x20) != 0)
                {
                    int index = octet & 0x1F;
                    index = DecodeInteger(stream, 5, index);
                    if (index > _maxTableSize)
                        throw new HpackDecodingException();

                    _dynamicTableCount = index;
                    ShrinkDynamicTableIfNecessary();
                }
                else if ((octet & 0x10) != 0)
                {
                    // Literal Header Field Never Indexed
                    int index = octet & 0x0F;
                    index = DecodeInteger(stream, 4, index);

                    Header header;
                    if (index > 0)
                    {
                        // Indexed Name
                        header = new Header(GetHeaderByIndex(index).Name, DecodeString(stream));
                    }
                    else
                    {
                        // New Name
                        header = new Header(DecodeString(stream), DecodeString(stream));
                    }

                    headers.Add(header);
                }
                else
                {
                    // Literal Header Field without Indexing
                    int index = octet & 0x0F;
                    index = DecodeInteger(stream, 4, index);

                    Header header;
                    if (index > 0)
                    {
                        // Indexed Name
                        header = new Header(GetHeaderByIndex(index).Name, DecodeString(stream));
                    }
                    else
                    {
                        // New Name
                        header = new Header(DecodeString(stream), DecodeString(stream));
                    }

                    headers.Add(header);
                }
            }
        }

        private void EncodeInteger(Stream stream, int n, int high, int value)
        {
            if (value < (1 << n) - 1)
            {
                stream.WriteByte((byte)(high | value));
                return;
            }

            throw new NotImplementedException();
        }

        private int DecodeInteger(Stream stream, int n, int value)
        {
            if (value < (1 << n) - 1)
                return value;

            long result = value;
            n = 0;
            while (true)
            {
                int octet = stream.ReadByte();
                if (octet == -1)
                    throw new HpackDecodingException("Unexpected end of stream");

                result += (octet & 0x7F) << n;
                n += 7;
                if (n > 32 || result > int.MaxValue)
                    throw new HpackDecodingException();
                if ((octet & 0x80) == 0)
                    return (int)result;
            }
        }

        private void EncodeString(Stream stream, string value)
        {
            int huffmanLength = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] >= 256)
                    throw new HpackDecodingException("Non-ASCII character");
                huffmanLength += HuffmanCodeLen[value[i]];
            }

            huffmanLength = (huffmanLength + 7) >> 3;
            if (huffmanLength < value.Length)
            {
                // Use huffman
                EncodeInteger(stream, 7, 0x80, huffmanLength);
                long data = 0;
                int bits = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    int len = HuffmanCodeLen[value[i]];
                    data = (data << len) | (long)HuffmanCode[value[i]];
                    bits += len;

                    while (bits >= 8)
                    {
                        bits -= 8;
                        stream.WriteByte((byte)(data >> bits));
                        data &= (1 << bits) - 1;
                    }
                }
                if (bits > 0)
                {
                    data = (data << (8 - bits)) | (0xFFL >> bits);
                    stream.WriteByte((byte)data);
                }
            }
            else
            {
                // Use raw
                EncodeInteger(stream, 7, 0x00, value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    stream.WriteByte((byte)value[i]);
                }
            }
        }

        private string DecodeString(Stream stream)
        {
            int octet = stream.ReadByte();
            if (octet == -1)
                throw new HpackDecodingException("Unexpected end of stream");

            bool huffman = (octet & 0x80) != 0;
            int length = DecodeInteger(stream, 7, octet & 0x7F);
            if (huffman)
            {
                var sb = new StringBuilder();
                int totalRead = 0;
                int buffer = 0;
                int bufIndex = 0;
                while (totalRead < length || bufIndex > 0)
                {
                    var tree = HuffmanTree;
                    while (tree.Value == null)
                    {
                        if (bufIndex == 0)
                        {
                            if (totalRead >= length)
                                goto endHuffman;
                            bufIndex = 8;
                            octet = stream.ReadByte();
                            if (octet == -1)
                                throw new HpackDecodingException("Unexpected end of stream");
                            totalRead++;
                            buffer = octet;
                        }
                        bool bit = (buffer & 0x80) != 0;
                        buffer <<= 1;
                        bufIndex--;
                        tree = bit ? tree.One : tree.Zero;
                    }
                    if (tree.Value == 256)
                        throw new NotImplementedException();
                    sb.Append((char)tree.Value.Value);
                }
                endHuffman:
                return sb.ToString();
            }
            else
            {
                byte[] buffer = new byte[length];
                try
                {
                    StreamHelper.ReadArray(stream, buffer);
                }
                catch (EndOfStreamException ex)
                {
                    throw new HpackDecodingException("Unexpected end of stream", ex);
                }
                return Encoding.ASCII.GetString(buffer);
            }
        }

        private void AddToDynamicTable(Header header)
        {
            _dynamicTable.Add(header);
            DynamicTableSize += GetHeaderSize(header);
            _dynamicTableCount++;

            while (DynamicTableSize > _maxTableSize)
            {
                DynamicTableSize -= GetHeaderSize(_dynamicTable[_dynamicTable.Count - _dynamicTableCount]);
                _dynamicTable[_dynamicTable.Count - _dynamicTableCount] = null;
                _dynamicTableCount--;
            }

            ShrinkDynamicTableIfNecessary();
        }

        private void ShrinkDynamicTableIfNecessary()
        {
            if (_dynamicTable.Count - _dynamicTableCount >= 256)
                _dynamicTable.RemoveRange(0, 256);
        }

        private Header GetHeaderByIndex(int index)
        {
            index--;
            if (index < StaticTable.Length)
            {
                return StaticTable[index];
            }
            else
            {
                index -= StaticTable.Length;
                if (index >= _dynamicTableCount || index >= _maxTableSize)
                    throw new HpackDecodingException("Index too big");
                return _dynamicTable[_dynamicTable.Count - 1 - index];
            }
        }

        private int GetHeaderSize(Header header)
        {
            return (header.Name?.Length ?? 0) + (header.Value?.Length ?? 0) + 32;
        }

        #region Private static constructor

        static Hpack()
        {
            // Build huffman tree
            HuffmanTree = new Tree();
            for (int i = 0; i <= 256; i++)
            {
                int value = HuffmanCode[i];
                int len = HuffmanCodeLen[i];
                Tree tree = HuffmanTree;

                while (len > 0)
                {
                    len--;
                    bool one = (value & (1 << len)) != 0;
                    if (tree.Value != null)
                        throw new InvalidOperationException();
                    if (one)
                    {
                        if (tree.One == null)
                            tree.One = new Tree();
                        tree = tree.One;
                    }
                    else
                    {
                        if (tree.Zero == null)
                            tree.Zero = new Tree();
                        tree = tree.Zero;
                    }
                }

                if (tree.Value != null || tree.Zero != null || tree.One != null)
                    throw new InvalidOperationException();

                tree.Value = i;
            }
        }

        #endregion

        #region Nested classes

        private class Tree
        {
            public int? Value;
            public Tree Zero;
            public Tree One;
        }

        #endregion
    }
}