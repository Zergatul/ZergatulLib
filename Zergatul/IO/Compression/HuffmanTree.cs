using System;
using System.IO;

namespace Zergatul.IO.Compression
{
    public class HuffmanTree
    {
        private int MaxBits { get; }
        private int[] Symbols { get; }
        private int[] Bits { get; }

        public HuffmanTree(int[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            MaxBits = -1;
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] < 0)
                    throw new HuffmanTreeException();
                if (bits[i] > MaxBits)
                    MaxBits = bits[i];
            }

            int[] blCount = new int[MaxBits + 1];
            int[] nextCode = new int[MaxBits + 1];
            for (int i = 0; i < bits.Length; i++)
                blCount[bits[i]]++;

            for (int i = 1; i <= MaxBits; i++)
                if (blCount[i] > (1 << i))
                    throw new HuffmanTreeException();

            int code = 0;
            blCount[0] = 0;
            for (int b = 1; b <= MaxBits; b++)
            {
                code = (code + blCount[b - 1]) << 1;
                nextCode[b] = code;
            }
            Symbols = new int[1 << MaxBits];
            Bits = new int[1 << MaxBits];
            for (int i = 0; i < Symbols.Length; i++)
            {
                Symbols[i] = -1;
                Bits[i] = -1;
            }

            for (int i = 0; i < bits.Length; i++)
            {
                int len = bits[i];
                if (len != 0)
                {
                    code = BitHelper.ReverseBits(nextCode[len], len);
                    nextCode[len]++;
                    int records = 1 << (MaxBits - len);
                    for (int j = 0; j < records; j++)
                    {
                        int index = (j << len) | code;
                        Symbols[index] = i;
                        Bits[index] = len;
                    }
                }
            }
        }

        public int ReadNextSymbol(BitReader reader)
        {
            reader.Peek(MaxBits, out int value, out int read);

            int symbolBits = Bits[value];
            if (symbolBits == -1)
                return -1;
            if (symbolBits > read)
                throw new EndOfStreamException();

            reader.RemoveBits(symbolBits);
            return Symbols[value];
        }

        public int ReadNextSymbol(InputBuffer buffer)
        {
            int value = buffer.Peek(MaxBits, out int read);

            int symbolBits = Bits[value];
            if (symbolBits == -1)
            {
                if (read == MaxBits)
                    throw new HuffmanTreeException();
                else
                    return -1;
            }
            if (symbolBits > read)
                return -1;

            buffer.SkipBits(symbolBits);
            return Symbols[value];
        }
    }
}