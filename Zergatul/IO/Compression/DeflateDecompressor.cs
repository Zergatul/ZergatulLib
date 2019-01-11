using System;

namespace Zergatul.IO.Compression
{
    // https://tools.ietf.org/html/rfc1951
    class DeflateDecompressor : IByteProcessor
    {
        public int Available { get; private set; }
        public bool IsFinished { get; private set; }
        public int ReadBufferSize { get; }

        private State _state;
        private bool _isFinalBlock;
        private bool _isFixedBlock;
        private BlockType _blockType;
        private int _blockLength;
        private byte[] _ringBuffer;
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
        private int _lastCommandSymbol;
        private HuffmanTree _literalAlphabet;
        private HuffmanTree _distanceAlphabet;
        private int _copyLength;
        private int _distanceCode;
        private int _copyDistance;

        public DeflateDecompressor(int readBufferSize)
        {
            ReadBufferSize = readBufferSize;
            _state = State.ReadBlockHeader;
            _ringBuffer = new byte[0x8000];
            _codeLen = new int[19];
            _input = new InputBuffer(readBufferSize);
        }

        public void Decode()
        {
            try
            {
                DecodeInternal();
            }
            catch (HuffmanTreeException ex)
            {
                throw new DeflateDataFormatException(ex);
            }
        }

        public void Get(byte[] buffer, int offset, int count)
        {
            if (count > Available)
                throw new InvalidOperationException();

            int start = (0x8000 + _ringBufferPos - Available) & 0x7FFF;
            for (int i = 0; i < count; i++)
                buffer[offset + i] = _ringBuffer[(start + i) & 0x7FFF];

            Available -= count;
        }

        public void Process(byte[] buffer, int offset, int count)
        {
            if (Available != 0)
                throw new InvalidOperationException();

            _input.Add(buffer, offset, count);
        }

        #region Private methods

        private void DecodeInternal()
        {
            if (Available == _ringBuffer.Length)
                return;

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
                                _literalAlphabet = FixedAlphabetTree;
                                _isFixedBlock = true;
                                _state = State.ReadBlock;
                                break;
                            case BlockType.Dynamic:
                                _isFixedBlock = false;
                                _state = State.ReadDynamicBlockHeader;
                                break;
                            case BlockType.Reserved:
                                throw new DeflateDataFormatException();
                        }
                        break;

                    case State.ReadUncompressedBlockHeader:
                        if (_input.TotalBits < 32)
                            return;
                        _blockLength = _input.ReadRawInt16();
                        int nLen = _input.ReadRawInt16();
                        if (_blockLength != (~nLen & 0xFFFF))
                            throw new DeflateDataFormatException();
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
                            _literalAlphabet = null;
                            _distanceAlphabet = null;
                            _state = State.ReadTree;
                            goto case State.ReadTree;
                        }
                        else
                            return;

                    case State.ReadTree:
                        while (_arrayIndex < _treeBits.Length && _input.TotalBits >= 1)
                        {
                            int symbol = _codeLenTree.ReadNextSymbol(_input);
                            if (symbol == int.MinValue)
                                return;
                            if (symbol < 16)
                                _treeBits[_arrayIndex++] = symbol;
                            else
                            {
                                _lastCommandSymbol = symbol;
                                _state = State.ReadCommandSymbol;
                                goto case State.ReadCommandSymbol;
                            }
                        }
                        if (_arrayIndex == _treeBits.Length)
                        {
                            if (_literalAlphabet == null)
                            {
                                _literalAlphabet = new HuffmanTree(_treeBits);
                                _arrayIndex = 0;
                                _treeBits = new int[hDist];
                                goto case State.ReadTree;
                            }
                            else
                            {
                                _distanceAlphabet = new HuffmanTree(_treeBits);
                                _state = State.ReadBlock;
                                goto case State.ReadBlock;
                            }
                        }
                        else
                            return;

                    case State.ReadCommandSymbol:
                        switch (_lastCommandSymbol)
                        {
                            case 16:
                                if (_arrayIndex == 0)
                                    throw new DeflateDataFormatException();
                                if (_input.TotalBits < 2)
                                    return;
                                int repeat = _input.ReadBits(2) + 3;
                                if (_arrayIndex + repeat > _treeBits.Length)
                                    throw new DeflateDataFormatException();
                                for (int i = 0; i < repeat; i++)
                                    _treeBits[_arrayIndex++] = _treeBits[_arrayIndex - 1];
                                _state = State.ReadTree;
                                break;

                            case 17:
                                if (_input.TotalBits < 3)
                                    return;
                                repeat = _input.ReadBits(3) + 3;
                                if (_arrayIndex + repeat > _treeBits.Length)
                                    throw new DeflateDataFormatException();
                                for (int i = 0; i < repeat; i++)
                                    _treeBits[_arrayIndex++] = 0;
                                _state = State.ReadTree;
                                break;

                            case 18:
                                if (_input.TotalBits < 7)
                                    return;
                                repeat = _input.ReadBits(7) + 11;
                                if (_arrayIndex + repeat > _treeBits.Length)
                                    throw new DeflateDataFormatException();
                                for (int i = 0; i < repeat; i++)
                                    _treeBits[_arrayIndex++] = 0;
                                _state = State.ReadTree;
                                break;

                            default:
                                throw new InvalidOperationException();
                        }
                        break;

                    case State.ReadUncompressedBlock:
                        while (_blockLength > 0)
                        {
                            if (_input.TotalBits < 8)
                                return;
                            byte literal = _input.ReadRawByte();
                            _blockLength--;
                            if (AddLiteral(literal))
                                return;
                        }
                        _state = State.EndBlock;
                        goto case State.EndBlock;

                    case State.ReadBlock:
                        if (_input.TotalBits < 1)
                            return;
                        int value = _literalAlphabet.ReadNextSymbol(_input);
                        if (value == int.MinValue)
                            return;
                        if (value < 256)
                        {
                            if (AddLiteral((byte)value))
                                return;
                            goto case State.ReadBlock;
                        }
                        else if (value == 256)
                        {
                            _state = State.EndBlock;
                            goto case State.EndBlock;
                        }
                        else
                        {
                            if (value >= 286)
                                throw new DeflateDataFormatException();
                            _lastCommandSymbol = value - 257;
                            _copyLength = FixedLengthAlphabet[_lastCommandSymbol];
                            _state = State.ReadCopyLength;
                            goto case State.ReadCopyLength;
                        }

                    case State.ReadCopyLength:
                        int bits = FixedLengthAlphabetBits[_lastCommandSymbol];
                        if (bits > 0)
                        {
                            if (_input.TotalBits < bits)
                                return;
                            else
                                _copyLength += _input.ReadBits(bits);
                        }
                        _state = State.ReadDistanceCode;
                        goto case State.ReadDistanceCode;

                    case State.ReadDistanceCode:
                        if (_isFixedBlock)
                        {
                            if (_input.TotalBits < 5)
                                return;
                            _distanceCode = BitHelper.ReverseBits(_input.ReadBits(5), 5);
                        }
                        else
                        {
                            _distanceCode = _distanceAlphabet.ReadNextSymbol(_input);
                            if (_distanceCode == int.MinValue)
                                return;
                        }
                        if (_distanceCode < 0 || _distanceCode >= 30)
                            throw new DeflateDataFormatException();
                        _state = State.ReadDistance;
                        goto case State.ReadDistance;

                    case State.ReadDistance:
                        _copyDistance = FixedDistanceAlphabet[_distanceCode];
                        bits = FixedDistanceAlphabetBits[_distanceCode];
                        if (bits > 0)
                        {
                            if (_input.TotalBits < bits)
                                return;
                            _copyDistance += _input.ReadBits(bits);
                        }
                        if (_copyDistance > _position)
                            throw new DeflateDataFormatException();
                        _state = State.Copy;
                        goto case State.Copy;

                    case State.Copy:
                        while (_copyLength > 0)
                        {
                            byte literal = _ringBuffer[(0x8000 + _ringBufferPos - _copyDistance) & 0x7FFF];
                            _copyLength--;
                            if (AddLiteral(literal))
                                return;
                        }
                        _state = State.ReadBlock;
                        goto case State.ReadBlock;

                    case State.EndBlock:
                        if (_isFinalBlock)
                        {
                            _state = State.End;
                            goto case State.End;
                        }
                        else
                        {
                            _state = State.ReadBlockHeader;
                            goto case State.ReadBlockHeader;
                        }

                    case State.End:
                        IsFinished = true;
                        return;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private bool AddLiteral(byte value)
        {
            _ringBuffer[_ringBufferPos++] = value;
            _position++;
            Available++;
            if (_ringBufferPos == _ringBuffer.Length)
                _ringBufferPos = 0;
            return Available == _ringBuffer.Length;
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

        private static readonly HuffmanTree FixedAlphabetTree;

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

            FixedAlphabetTree = new HuffmanTree(lengths);
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
            ReadTree,
            ReadCommandSymbol,
            ReadBlock,
            ReadCopyLength,
            ReadDistanceCode,
            ReadDistance,
            Copy,
            EndBlock,
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