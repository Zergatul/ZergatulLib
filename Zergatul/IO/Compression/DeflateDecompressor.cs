using System;

namespace Zergatul.IO.Compression
{
    class DeflateDecompressor
    {
        public int Available { get; private set; }
        public bool IsFinished { get; private set; }
        public int ReadBufferSize { get; }

        private State _state;
        private bool _isFinalBlock;
        private BlockType _blockType;
        private int _blockLength;
        private byte[] _ringBuffer;
        private int _ringBufferLastReadPos;
        private int _ringBufferPos;
        private int _position;
        private int[] _codeLen;

        private InputBuffer _input;

        private int hLit;
        private int hDist;
        private int hCLen;
        private int _arrayIndex;
        private HuffmanTree _codeLenTree;
        private int[] _treeBits;

        public DeflateDecompressor(int readBufferSize)
        {
            ReadBufferSize = readBufferSize;
            _state = State.ReadBlockHeader;
            _ringBuffer = new byte[0x8000];
            _codeLen = new int[19];
            _input = new InputBuffer(readBufferSize);
        }

        public void Get(byte[] buffer, int offset, int count)
        {
            if (count > Available)
                throw new InvalidOperationException();
        }

        public void Process(byte[] buffer, int offset, int count)
        {
            if (Available != 0)
                throw new InvalidOperationException();

            _input.Add(buffer, offset, count);

            Decode();
        }

        #region Private methods

        private void Decode()
        {
            while (true)
            {
                switch (_state)
                {
                    case State.ReadBlockHeader:
                        if (_input.TotalBits < 3)
                            return;
                        _isFinalBlock = _input.ReadBits(1) == 1;
                        _blockType = (BlockType)_input.ReadBits(2);
                        switch (_blockType)
                        {
                            case BlockType.Uncompressed:
                                _input.SkipTillByteBoundary();
                                _state = State.ReadUncompressedBlockHeader;
                                break;
                            case BlockType.Fixed:
                                _state = State.ReadBlock;
                                break;
                            case BlockType.Dynamic:
                                _state = State.ReadDynamicBlockHeader;
                                break;
                            case BlockType.Reserved:
                                throw new DeflateStreamException();
                        }
                        break;

                    case State.ReadUncompressedBlockHeader:
                        if (_input.TotalBits < 32)
                            return;
                        _blockLength = _input.ReadRawInt16();
                        int nLen = _input.ReadRawInt16();
                        if (_blockLength != (~nLen & 0xFFFF))
                            throw new DeflateStreamException();
                        _state = State.ReadUncompressedBlock;
                        goto case State.ReadUncompressedBlock;

                    case State.ReadDynamicBlockHeader:
                        if (_input.TotalBits < 14)
                            return;
                        hLit = 257 + _input.ReadBits(5);
                        hDist = 1 + _input.ReadBits(5);
                        hCLen = 4 + _input.ReadBits(4);
                        _arrayIndex = 0;
                        _state = State.ReadCodeLen;
                        goto case State.ReadCodeLen;

                    case State.ReadCodeLen:
                        while (_arrayIndex < hCLen && _input.TotalBits >= 3)
                            _codeLen[CodeLenIndex[_arrayIndex++]] = _input.ReadBits(3);
                        if (_arrayIndex == hCLen)
                        {
                            _codeLenTree = new HuffmanTree(_codeLen);
                            _arrayIndex = 0;
                            _treeBits = new int[hLit];
                            _state = State.ReadLiteralTree;
                            goto case State.ReadLiteralTree;
                        }
                        else
                            return;

                    case State.ReadLiteralTree:
                        while (_arrayIndex < hLit && _input.TotalBits >= 1)
                        {
                            int symbol = _codeLenTree.ReadNextSymbol(_input);
                            if (symbol < 16)
                                _treeBits[_arrayIndex++] = symbol;
                            else if (symbol == 16)
                            {
                                if (_arrayIndex == 0)
                                    throw new DeflateStreamException();
                                int repeat = _input.ReadBits(2) + 3;
                                for (int i = 0; i < repeat; i++)
                                    bits[index++] = bits[index - 1];
                            }
                            else if (symbol == 17)
                            {
                                int repeat = _reader.ReadBits(3) + 3;
                                if (index + repeat > count)
                                    throw new DeflateStreamException();
                                for (int i = 0; i < repeat; i++)
                                    bits[index++] = 0;
                            }
                            else if (symbol == 18)
                            {
                                int repeat = _reader.ReadBits(7) + 11;
                                if (index + repeat > count)
                                    throw new DeflateStreamException();
                                for (int i = 0; i < repeat; i++)
                                    bits[index++] = 0;
                            }
                        }
                        break;

                        //for (int i = 0; i < hCLen; i++)
                        //    _codeLen[_codeLenIndex[i]] = _reader.ReadBits(3);

                        //try
                        //{
                        //    var tree = new HuffmanTree(_codeLen);
                        //    _literalAlphabet = ReadLengthsTree(tree, hLit);
                        //    _distanceAlphabet = ReadLengthsTree(tree, hDist);
                        //}
                        //catch (HuffmanTreeException ex)
                        //{
                        //    throw new DeflateStreamException(ex);
                        //}

                        //_state = State.ReadBlock;
                        //break;

                    case State.ReadUncompressedBlock:
                        if (_blockLength == 0)
                        {
                            if (_isFinalBlock)
                            {
                                _state = State.End;
                                return read;
                            }
                            else
                                _state = State.ReadBlockHeader;
                        }
                        else
                        {
                            int rawRead = _reader.ReadBytes(buffer, offset, System.Math.Min(count, _blockLength));
                            if (rawRead == 0)
                                throw new EndOfStreamException();
                            for (int i = 0; i < rawRead; i++)
                                AddLiteral(buffer[offset + i]);
                            read += rawRead;
                            _blockLength -= rawRead;
                            return read;
                        }
                        break;

                    case State.ReadBlock:
                        int value = (_isFixedBlock ? FixedTree : _literalAlphabet).ReadNextSymbol(_reader);
                        if (value < 256)
                        {
                            buffer[offset++] = (byte)value;
                            AddLiteral(value);
                            count--;
                            read++;
                        }
                        else if (value == 256)
                        {
                            if (_isFinalBlock)
                                _state = State.End;
                            else
                                _state = State.ReadBlockHeader;
                        }
                        else
                        {
                            if (value >= 286)
                                throw new DeflateStreamException();

                            value -= 257;
                            _copyLength = FixedLengthAlphabet[value];
                            int bits = FixedLengthAlphabetBits[value];
                            if (bits > 0)
                                _copyLength += _reader.ReadBits(bits);

                            int distanceCode;
                            if (_isFixedBlock)
                                distanceCode = BitHelper.ReverseBits(_reader.ReadBits(5), 5);
                            else
                                distanceCode = _distanceAlphabet.ReadNextSymbol(_reader);
                            if (distanceCode < 0 || distanceCode >= 30)
                                throw new DeflateStreamException();

                            _copyDistance = FixedDistanceAlphabet[distanceCode];
                            bits = FixedDistanceAlphabetBits[distanceCode];
                            if (bits > 0)
                                _copyDistance += _reader.ReadBits(bits);

                            if (_copyDistance > _position)
                                throw new DeflateStreamException();

                            _state = State.Copy;
                        }
                        break;

                    case State.Copy:
                        int copied = ProcessCopy(buffer, offset, count);
                        read += copied;
                        offset += copied;
                        count -= copied;
                        if (count == 0)
                            return read;
                        break;

                    case State.End:
                        return read;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        #endregion

        #region Constants

        private static readonly int[] FixedLengthAlphabetBits = new[]
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            1, 1, 1, 1, 2, 2, 2, 2,
            3, 3, 3, 3, 4, 4, 4, 4,
            5, 5, 5, 5, 0
        };

        private static readonly int[] FixedLengthAlphabet = new[]
        {
            3, 4, 5, 6, 7, 8, 9, 10, 11, 13,
            15, 17, 19, 23, 27, 31, 35, 43,
            51, 69, 67, 83, 99, 115, 131, 163, 195, 227, 258
        };

        private static readonly int[] FixedDistanceAlphabetBits = new[]
        {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3,
            4, 4, 5, 5, 6, 6, 7, 7, 8, 8,
            9, 9, 10, 10, 11, 11, 12, 12,
            13, 13,
        };

        private static readonly int[] FixedDistanceAlphabet = new[]
        {
            1, 2, 3, 4, 5, 7, 9, 13, 17,
            25, 33, 49, 65, 97, 129, 193,
            257, 385, 513, 769, 1025, 1537,
            2049, 3073, 4097, 6145, 8193,
            12289, 16385, 24577,
        };

        private static readonly int[] CodeLenIndex = new[]
        {
            16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
        };

        private static readonly HuffmanTree FixedTree;

        static DeflateDecompressor()
        {
            int[] lengths = new int[288];
            int index = 0;
            while (index < 144)
                lengths[index++] = 8;
            while (index < 256)
                lengths[index++] = 9;
            while (index < 280)
                lengths[index++] = 7;
            while (index < 288)
                lengths[index++] = 8;

            FixedTree = new HuffmanTree(lengths);
        }

        #endregion

        #region Nested classes

        private enum State
        {
            ReadBlockHeader,
            ReadUncompressedBlockHeader,
            ReadUncompressedBlock,
            ReadDynamicBlockHeader,
            ReadCodeLen,
            ReadLiteralTree,
            ReadDistanceTree,
            ReadBlock,
            Copy,
            End
        }

        private enum BlockType
        {
            Uncompressed = 0,
            Fixed = 1,
            Dynamic = 2,
            Reserved = 3
        }

        #endregion
    }
}