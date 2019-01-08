using System;
using System.IO;

namespace Zergatul.IO.Compression
{
    public class HuffmanTree
    {
        private int _maxBits;
        private int[] _treeSymbols;
        private int[] _treeBits;

        public HuffmanTree(int[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            _maxBits = -1;
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] < 0)
                    throw new HuffmanTreeException();
                if (bits[i] > _maxBits)
                    _maxBits = bits[i];
            }

            int[] blCount = new int[_maxBits + 1];
            int[] nextCode = new int[_maxBits + 1];
            for (int i = 0; i < bits.Length; i++)
                blCount[bits[i]]++;

            for (int i = 1; i <= _maxBits; i++)
                if (blCount[i] > (1 << i))
                    throw new HuffmanTreeException();

            int code = 0;
            blCount[0] = 0;
            for (int b = 1; b <= _maxBits; b++)
            {
                code = (code + blCount[b - 1]) << 1;
                nextCode[b] = code;
            }
            _treeSymbols = new int[1 << _maxBits];
            _treeBits = new int[1 << _maxBits];
            for (int i = 0; i < _treeSymbols.Length; i++)
            {
                _treeSymbols[i] = -1;
                _treeBits[i] = -1;
            }

            for (int i = 0; i < bits.Length; i++)
            {
                int len = bits[i];
                if (len != 0)
                {
                    code = BitHelper.ReverseBits(nextCode[len], len);
                    nextCode[len]++;
                    int records = 1 << (_maxBits - len);
                    for (int j = 0; j < records; j++)
                    {
                        int index = (j << len) | code;
                        _treeSymbols[index] = i;
                        _treeBits[index] = len;
                    }
                }
            }
        }

        public int ReadNextSymbol(BitReader reader)
        {
            reader.Peek(_maxBits, out int value, out int read);

            int symbolBits = _treeBits[value];
            if (symbolBits == -1)
                return -1;
            if (symbolBits > read)
                throw new EndOfStreamException();

            reader.RemoveBits(symbolBits);
            return _treeSymbols[value];
        }
    }
}