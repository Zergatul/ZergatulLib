using System;
using System.IO;
using System.IO.Compression;

namespace Zergatul.IO.Compression
{
    // https://tools.ietf.org/html/rfc7932
    public class BrotliStream : Stream
    {
        public Stream BaseStream { get; private set; }

        #region Private constants

        private const int LiteralAlphabetSize = 256;
        private const int InsertCopyAlphabetSize = 704;
        private const int BlockCountAlphabetSize = 26;
        private const int SpecialDistanceSymbols = 16;
        private static readonly int[] InsertLengthCodeOffset = new[]
        {
              0,   1,   2,   3,    4,    5,    6,     8,
             10,  14,  18,  26,   34,   50,   66,    98,
            130, 194, 322, 578, 1090, 2114, 6210, 22594
        };
        private static readonly int[] InsertLengthCodeBits = new[]
        {
            0, 0, 0, 0,  0,  0,  1,  1,
            2, 2, 3, 3,  4,  4,  5,  5,
            6, 7, 8, 9, 10, 12, 14, 24
        };
        private static readonly int[] CopyLengthCodeOffset = new[]
        {
             2,   3,   4,   5,   6,   7,    8,    9,
            10,  12,  14,  18,  22,  30,   38,   54,
            70, 102, 134, 198, 326, 582, 1094, 2118
        };
        private static readonly int[] CopyLengthCodeBits = new[]
        {
            0, 0, 0, 0, 0, 0,  0,  0,
            1, 1, 2, 2, 3, 3,  4,  4,
            5, 5, 6, 7, 8, 9, 10, 24
        };
        private static readonly int[] BlockCountCodeOffset = new[]
        {
              1,   5,   9,  13,  17,  25,  33,  41,   49,   65,   81,   97,   113,
            145, 177, 209, 241, 305, 369, 497, 753, 1265, 2289, 4337, 8433, 16625
        };
        private static readonly int[] BlockCountCodeBits = new[]
        {
            2, 2, 2, 2, 3, 3, 3, 3,  4,  4,  4,  4,  5,
            5, 5, 5, 6, 6, 7, 8, 9, 10, 11, 12, 13, 24
        };
        private static readonly int[] LookupTable0 = new[]
        {
            0,  0,  0,  0,  0,  0,  0,  0,  0,  4,  4,  0,  0,  4,  0,  0,
         0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
         8, 12, 16, 12, 12, 20, 12, 16, 24, 28, 12, 12, 32, 12, 36, 12,
        44, 44, 44, 44, 44, 44, 44, 44, 44, 44, 32, 32, 24, 40, 28, 12,
        12, 48, 52, 52, 52, 48, 52, 52, 52, 48, 52, 52, 52, 52, 52, 48,
        52, 52, 52, 52, 52, 48, 52, 52, 52, 52, 52, 24, 12, 28, 12, 12,
        12, 56, 60, 60, 60, 56, 60, 60, 60, 56, 60, 60, 60, 60, 60, 56,
        60, 60, 60, 60, 60, 56, 60, 60, 60, 60, 60, 24, 12, 28, 12,  0,
         0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1,
         0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1,
         0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1,
         0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1,
         2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3,
         2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3,
         2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3,
         2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3
        };
        private static readonly int[] LookupTable1 = new[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1,
         1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1,
         1, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
         3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2
        };
        private static readonly int[] LookupTable2 = new[]
        {
            0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
         3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
         3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
         3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
         3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
         4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
         4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
         4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
         4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
         5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
         5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
         5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
         6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7
        };
        private static readonly int[] CodeLengthCodeOrder = new[]
        {
            1, 2, 3, 4, 0, 5, 17, 6, 16, 7, 8, 9, 10, 11, 12, 13, 14, 15
        };

        #endregion

        private BitReader _reader;
        private byte[] _buffer;
        private int _bufferOffset;
        private int _bufferLength;
        private State _state;

        private int _maxBackwardDistance;

        private bool _isLast;
        private int _mLen;
        private int _nPostfix;
        private int _nDirect;
        private int _postfixMask;
        private int _d1, _d2, _d3, _d4;
        private Block[] _blocks;
        private Block _literalBlock;
        private Block _insertCopyBlock;
        private Block _distanceBlock;
        private ContextMode[] _cMode;

        private int _distanceCode;
        private int _insertLength;
        private int _copyLength;
        private int _distance;
        private int _maxDistance;
        private int _p1, _p2;

        public BrotliStream(Stream stream, CompressionMode mode)
        {
            if (mode == CompressionMode.Compress)
                throw new NotImplementedException();

            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));

            _reader = new BitReader(stream);
            _buffer = new byte[1024];
            _bufferOffset = 0;
            _bufferLength = 0;
            _blocks = new Block[3];
            _state = State.Uninitialized;
        }

        private void ReadHeader()
        {
            if (_state != State.Uninitialized)
                throw new InvalidOperationException();

            int wbits = DecodeWBits();

            _maxBackwardDistance = (1 << wbits) - 16;

            _p1 = 0;
            _p2 = 0;

            _d1 = 4;
            _d2 = 11;
            _d3 = 15;
            _d4 = 16;

            _state = State.BlockStart;
        }

        private void ReadMetaBlockHeader()
        {
            if (_state != State.BlockStart)
                throw new InvalidOperationException();

            _isLast = _reader.ReadBits(1) == 1;
            if (_isLast)
            {
                bool isLastEmpty = _reader.ReadBits(1) == 1;
                if (isLastEmpty)
                {
                    _state = State.Finished;
                    return;
                }
            }

            int mNibbles;
            switch (_reader.ReadBits(2))
            {
                case 0: mNibbles = 4; break;
                case 1: mNibbles = 5; break;
                case 2: mNibbles = 6; break;
                case 3: mNibbles = 0; break;
                default:
                    throw new InvalidOperationException();
            }

            if (mNibbles == 0)
                throw new NotImplementedException();

            _mLen = _reader.ReadBits(mNibbles * 4);
            if (mNibbles == 5 && _mLen < 0x10000)
                throw new BrotliStreamException();
            if (mNibbles == 6 && _mLen < 0x100000)
                throw new BrotliStreamException();
            _mLen++;

            if (!_isLast)
            {
                bool isUncompressed = _reader.ReadBits(1) == 1;
                if (isUncompressed)
                {
                    if (_reader.ReadTillByteBoundary() != 0)
                        throw new BrotliStreamException();

                    while (_mLen > 0)
                        AddLiteral(_reader.ReadBits(8));

                    _state = State.BlockStart;
                    return;
                }
            }

            _literalBlock = new Block
            {
                AlphabetSize = LiteralAlphabetSize
            };
            _insertCopyBlock = new Block
            {
                AlphabetSize = InsertCopyAlphabetSize
            };
            _distanceBlock = new Block();
            _blocks[0] = _literalBlock;
            _blocks[1] = _insertCopyBlock;
            _blocks[2] = _distanceBlock;

            for (int i = 0; i < 3; i++)
            {
                _blocks[i].nBlTypes = ReadVariableLength256Code();
                if (_blocks[i].nBlTypes >= 2)
                {
                    _blocks[i].BlockTypeTree = ReadHuffmanTree(_blocks[i].nBlTypes + 2);
                    _blocks[i].BlockLenTree = ReadHuffmanTree(BlockCountAlphabetSize);
                    _blocks[i].PrevBlockType = 1;
                    _blocks[i].bType = 0;
                    ReadBlockLength(_blocks[i]);
                }
                else
                {
                    _blocks[i].bType = 0;
                    _blocks[i].bLen = 16777216;
                }
            }

            _nPostfix = _reader.ReadBits(2);
            _nDirect = _reader.ReadBits(4) << _nPostfix;
            _postfixMask = (1 << _nPostfix) - 1;

            // Distance Alphabet Size
            _distanceBlock.AlphabetSize = SpecialDistanceSymbols + _nDirect + (48 << _nPostfix);

            _cMode = new ContextMode[_literalBlock.nBlTypes];
            for (int i = 0; i < _cMode.Length; i++)
                _cMode[i] = (ContextMode)_reader.ReadBits(2);

            ReadContextMap(_literalBlock, _literalBlock.nBlTypes << 6);
            ReadContextMap(_distanceBlock, _distanceBlock.nBlTypes << 2);
            _insertCopyBlock.nTrees = _insertCopyBlock.nBlTypes;

            // read prefix codes
            for (int i = 0; i < 3; i++)
            {
                _blocks[i].Trees = new Tree[_blocks[i].nTrees];
                for (int j = 0; j < _blocks[i].nTrees; j++)
                    _blocks[i].Trees[j] = ReadHuffmanTree(_blocks[i].AlphabetSize);
            }

            _state = State.MainLoop;
        }

        private void DecodeMetaBlockData()
        {
            if (_state != State.MainLoop)
                throw new InvalidOperationException();

            if (_insertCopyBlock.bLen == 0)
            {
                ReadBlockType(_insertCopyBlock);
                ReadBlockLength(_insertCopyBlock);
            }
            _insertCopyBlock.bLen--;

            int insertCode, copyCode;
            DecodeInsertCopyLengthCode(ReadHuffmanValue(_insertCopyBlock.Trees[_insertCopyBlock.bType]), out insertCode, out copyCode, out _distanceCode);
            _insertLength = InsertLengthCodeOffset[insertCode] + _reader.ReadBits(InsertLengthCodeBits[insertCode]);
            _copyLength = CopyLengthCodeOffset[copyCode] + _reader.ReadBits(CopyLengthCodeBits[copyCode]);

            _state = State.InsertLoop;
        }

        private void ProcessInsertLoop()
        {
            if (_state != State.InsertLoop)
                throw new InvalidOperationException();

            if (_insertLength > 0)
            {
                if (_literalBlock.bLen == 0)
                {
                    ReadBlockType(_literalBlock);
                    ReadBlockLength(_literalBlock);
                }
                _literalBlock.bLen--;

                int cidl = GetContextId(_cMode[_literalBlock.bType]);
                int literal = ReadHuffmanValue(_literalBlock.Trees[_literalBlock.cMap[64 * _literalBlock.bType + cidl]]);
                AddLiteral(literal);

                _insertLength--;
            }

            if (_insertLength == 0)
            {
                if (_mLen == 0)
                {
                    if (_isLast)
                        _state = State.Finished;
                    else
                        _state = State.BlockStart;
                    return;
                }

                if (_distanceCode == 0)
                {
                    _distance = _d1;
                }
                else
                {
                    if (_distanceBlock.bLen == 0)
                    {
                        ReadBlockType(_distanceBlock);
                        ReadBlockLength(_distanceBlock);
                    }
                    _distanceBlock.bLen--;

                    int cidd = _copyLength > 4 ? 3 : _copyLength - 2;
                    int index = (_distanceBlock.bType << 2) + cidd;
                    _distanceCode = ReadHuffmanValue(_distanceBlock.Trees[_distanceBlock.cMap[index]]);
                    if (_distanceCode >= SpecialDistanceSymbols + _nDirect)
                    {
                        int nDistBits = 1 + ((_distanceCode - _nDirect - SpecialDistanceSymbols) >> (_nPostfix + 1));
                        int hcode = (_distanceCode - _nDirect - SpecialDistanceSymbols) >> _nPostfix;
                        int lcode = (_distanceCode - _nDirect - SpecialDistanceSymbols) & _postfixMask;
                        int offset = ((2 + (hcode & 1)) << nDistBits) - 4;
                        int dextra = _reader.ReadBits(nDistBits);
                        _distance = ((offset + dextra) << _nPostfix) + lcode + _nDirect + 1;
                    }
                    else
                    {
                        switch (_distanceCode)
                        {
                            case 0: _distance = _d1; break;
                            case 1: _distance = _d2; break;
                            case 2: _distance = _d3; break;
                            case 3: _distance = _d4; break;
                            case 4: _distance = _d1 - 1; break;
                            case 5: _distance = _d1 + 1; break;
                            case 6: _distance = _d1 - 2; break;
                            case 7: _distance = _d1 + 2; break;
                            case 8: _distance = _d1 - 3; break;
                            case 9: _distance = _d1 + 3; break;
                            case 10: _distance = _d2 - 1; break;
                            case 11: _distance = _d2 + 1; break;
                            case 12: _distance = _d2 - 2; break;
                            case 13: _distance = _d2 + 2; break;
                            case 14: _distance = _d2 - 3; break;
                            case 15: _distance = _d2 + 3; break;
                            default:
                                throw new InvalidOperationException();
                        }
                    }

                    _maxDistance = System.Math.Min(_bufferLength, _maxBackwardDistance);
                    if (_distance > _maxDistance)
                    {
                        _state = State.Transform;
                        return;
                    }
                    else
                    {
                        if (_distanceCode != 0)
                            AddDistance();
                    }
                }

                _state = State.Copy;
            }
        }

        private void ProcessCopy()
        {
            if (_state != State.Copy)
                throw new InvalidOperationException();

            if (_distance <= _maxBackwardDistance)
            {
                for (int i = 0; i < _copyLength; i++)
                    AddLiteral(_buffer[_bufferLength - _distance]);
            }
            else
                throw new NotImplementedException();

            if (_mLen == 0)
            {
                if (_isLast)
                    _state = State.Finished;
                else
                    _state = State.BlockStart;
            }
            else
                _state = State.MainLoop;
        }

        private void ProcessStaticDictionaryTransform()
        {
            if (_state != State.Transform)
                throw new InvalidOperationException();

            if (_copyLength < 4 || _copyLength > 24)
                throw new BrotliStreamException();

            int wordId = _distance - _maxDistance - 1;
            int index = wordId % BrotliStaticDictionary.NWords[_copyLength];
            int dictOffset = BrotliStaticDictionary.DOffset[_copyLength] + index * _copyLength;
            int transformId = wordId >> BrotliStaticDictionary.NDBits[_copyLength];

            if (transformId > 120)
                throw new BrotliStreamException();

            Transform(transformId, dictOffset);

            _state = State.MainLoop;
        }

        private int DecodeWBits()
        {
            if (_reader.ReadBits(1) == 0)
                return 16;
            int n = _reader.ReadBits(3);
            if (n != 0)
                return 17 + n;
            n = _reader.ReadBits(3);
            if (n == 0)
                return 17;
            if (n == 1)
                throw new BrotliStreamException();
            return 8 + n;
        }

        private int ReadVariableLength256Code()
        {
            if (_reader.ReadBits(1) == 0)
                return 1;
            else
                switch (_reader.ReadBits(3))
                {
                    case 0: return 2;
                    case 1: return 3 + _reader.ReadBits(1);
                    case 2: return 5 + _reader.ReadBits(2);
                    case 3: return 9 + _reader.ReadBits(3);
                    case 4: return 17 + _reader.ReadBits(4);
                    case 5: return 33 + _reader.ReadBits(5);
                    case 6: return 65 + _reader.ReadBits(6);
                    case 7: return 129 + _reader.ReadBits(7);
                    default:
                        throw new InvalidOperationException();
                }
        }

        private Tree ReadHuffmanTree(int alphabetSize)
        {
            int prefixCodeType = _reader.ReadBits(2);
            if (prefixCodeType == 1)
                return ReadSimplePrefixCodes(alphabetSize);
            else
                return ReadComplexPrefixCodes(prefixCodeType, alphabetSize);
        }

        private Tree ReadSimplePrefixCodes(int alphabetSize)
        {
            int nSymb = _reader.ReadBits(2) + 1;

            int alphabetBits = 0;
            int alphabetMaxValue = alphabetSize - 1;
            while (alphabetMaxValue > 0)
            {
                alphabetMaxValue >>= 1;
                alphabetBits++;
            }

            int[] symbols = new int[nSymb];
            for (int i = 0; i < nSymb; i++)
            {
                symbols[i] = _reader.ReadBits(alphabetBits);
                if (symbols[i] >= alphabetSize)
                    throw new BrotliStreamException();
                for (int j = 0; j < i; j++)
                    if (symbols[j] == symbols[i])
                        throw new BrotliStreamException();
            }

            switch (nSymb)
            {
                case 1:
                    return new Tree { Value = symbols[0] };

                case 2:
                    Sort(symbols, 0, 2);
                    return new Tree
                    {
                        Zero = new Tree { Value = symbols[0] },
                        One = new Tree { Value = symbols[1] }
                    };

                case 3:
                    Sort(symbols, 1, 2);
                    return new Tree
                    {
                        Zero = new Tree { Value = symbols[0] },
                        One = new Tree
                        {
                            Zero = new Tree { Value = symbols[1] },
                            One = new Tree { Value = symbols[2] }
                        }
                    };

                case 4:
                    // tree-select
                    if (_reader.ReadBits(1) == 0)
                    {
                        Sort(symbols, 0, 4);
                        return new Tree
                        {
                            Zero = new Tree
                            {
                                Zero = new Tree { Value = symbols[0] },
                                One = new Tree { Value = symbols[1] }
                            },
                            One = new Tree
                            {
                                Zero = new Tree { Value = symbols[2] },
                                One = new Tree { Value = symbols[3] }
                            }
                        };
                    }
                    else
                    {
                        Sort(symbols, 2, 2);
                        return new Tree
                        {
                            Zero = new Tree { Value = symbols[0] },
                            One = new Tree
                            {
                                Zero = new Tree { Value = symbols[1] },
                                One = new Tree
                                {
                                    Zero = new Tree { Value = symbols[2] },
                                    One = new Tree { Value = symbols[3] }
                                }
                            }
                        };
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        private void Sort(int[] array, int from, int length)
        {
            if (length == 2)
            {
                if (array[from] > array[from + 1])
                {
                    int buf = array[from];
                    array[from] = array[from + 1];
                    array[from + 1] = buf;
                }
                return;
            }

            if (length == 4)
            {
                int min, index;
                for (int i = 0; i < 3; i++)
                {
                    index = from + i;
                    min = array[index];
                    for (int j = from + i + 1; j < from + 4; j++)
                        if (array[j] < min)
                        {
                            index = j;
                            min = array[j];
                        }

                    if (index != from + i)
                    {
                        int buf = array[from + i];
                        array[from + i] = min;
                        array[index] = buf;
                    }
                }
            }
        }

        private Tree ReadComplexPrefixCodes(int hSkip, int alphabetSize)
        {
            int[] codeLengthCodeLengths = new int[18];
            int space = 32;
            int nonZeroCodes = 0;
            for (int i = hSkip; i < 18; i++)
            {
                int length;
                switch (_reader.ReadBits(2))
                {
                    case 0:
                        length = 0;
                        break;

                    case 1:
                        length = 4;
                        break;

                    case 2:
                        length = 3;
                        break;

                    case 3:
                        if (_reader.ReadBits(1) == 0)
                            length = 2;
                        else
                        {
                            if (_reader.ReadBits(1) == 0)
                                length = 1;
                            else
                                length = 5;
                        }
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                codeLengthCodeLengths[CodeLengthCodeOrder[i]] = length;
                if (length != 0)
                {
                    space -= 32 >> length;
                    nonZeroCodes++;

                    if (space == 0)
                        break;
                }
            }

            if (nonZeroCodes != 1 && space != 0)
                throw new BrotliStreamException();

            var tree = BuildTree(codeLengthCodeLengths);
            int symbolCount = 0;
            int[] codeLength = new int[alphabetSize];
            space = 0x8000;
            int prevCodeLength = 8;
            int prevRepeatCount16 = 0;
            int prevRepeatCount17 = 0;
            while (symbolCount < alphabetSize && space > 0)
            {
                int codeLen = ReadHuffmanValue(tree);
                if (codeLen < 16)
                {
                    codeLength[symbolCount] = codeLen;
                    symbolCount++;
                    prevRepeatCount16 = prevRepeatCount17 = 0;
                    if (codeLen != 0)
                    {
                        space -= 0x8000 >> codeLen;
                        prevCodeLength = codeLen;
                    }
                }
                else
                {
                    int repeatValue;
                    int repeatCount;
                    if (codeLen == 16)
                    {
                        repeatValue = prevCodeLength;
                        repeatCount = 3 + _reader.ReadBits(2);
                        prevRepeatCount17 = 0;
                        if (prevRepeatCount16 == 0)
                        {
                            prevRepeatCount16 = repeatCount;
                        }
                        else
                        {
                            repeatCount += ((prevRepeatCount16 - 2) << 2) - prevRepeatCount16;
                            prevRepeatCount16 += repeatCount;
                        }
                    }
                    else if (codeLen == 17)
                    {
                        repeatValue = 0;
                        repeatCount = 3 + _reader.ReadBits(3);
                        prevRepeatCount16 = 0;
                        if (prevRepeatCount17 == 0)
                        {
                            prevRepeatCount17 = repeatCount;
                        }
                        else
                        {
                            repeatCount += ((prevRepeatCount17 - 2) << 3) - prevRepeatCount17;
                            prevRepeatCount17 += repeatCount;
                        }
                    }
                    else
                        throw new BrotliStreamException();

                    if (symbolCount + repeatCount > alphabetSize)
                        throw new BrotliStreamException();

                    for (int i = 0; i < repeatCount; i++)
                    {
                        codeLength[symbolCount] = repeatValue;
                        symbolCount++;
                    }

                    if (repeatValue != 0)
                        space -= repeatCount << (15 - repeatValue);
                }
            }

            if (space != 0)
                throw new BrotliStreamException();

            return BuildTree(codeLength);
        }

        private int ReadHuffmanValue(Tree tree)
        {
            while (tree.Value == null)
                if (_reader.ReadBits(1) == 0)
                    tree = tree.Zero;
                else
                    tree = tree.One;

            return tree.Value.Value;
        }

        private void DecodeInsertCopyLengthCode(int value, out int insertCode, out int copyCode, out int distanceCode)
        {
            if (value < 64)
            {
                insertCode = 0;
                copyCode = 0;
            }
            else if (value < 128)
            {
                insertCode = 0;
                copyCode = 8;
            }
            else if (value < 192)
            {
                insertCode = 0;
                copyCode = 0;
            }
            else if (value < 256)
            {
                insertCode = 0;
                copyCode = 8;
            }
            else if (value < 320)
            {
                insertCode = 8;
                copyCode = 0;
            }
            else if (value < 384)
            {
                insertCode = 8;
                copyCode = 8;
            }
            else if (value < 448)
            {
                insertCode = 0;
                copyCode = 16;
            }
            else if (value < 512)
            {
                insertCode = 16;
                copyCode = 0;
            }
            else if (value < 576)
            {
                insertCode = 8;
                copyCode = 16;
            }
            else if (value < 640)
            {
                insertCode = 16;
                copyCode = 8;
            }
            else if (value < 704)
            {
                insertCode = 16;
                copyCode = 16;
            }
            else
                throw new InvalidOperationException();

            copyCode |= value & 0x07;
            insertCode |= (value >> 3) & 0x07;

            if (value < 128)
                distanceCode = 0;
            else
                distanceCode = -1;
        }

        private int GetContextId(ContextMode mode)
        {
            switch (mode)
            {
                case ContextMode.Lsb6: return _p1 & 0x3F;
                case ContextMode.Msb6: return (_p1 >> 2) & 0x3F;
                case ContextMode.Utf8: return LookupTable0[_p1] | LookupTable1[_p2];
                case ContextMode.Signed: return (LookupTable2[_p1] << 3) | LookupTable2[_p2];
                default:
                    throw new InvalidOperationException();
            }
        }

        private void AddLiteral(int value)
        {
            if (value < 0 || value >= 256)
                throw new InvalidOperationException();

            _p2 = _p1;
            _p1 = value;

            if (_bufferLength == _buffer.Length)
            {
                if (_bufferLength >= (_maxBackwardDistance << 1))
                {
                    if (_bufferOffset < _maxBackwardDistance)
                        throw new InvalidOperationException();

                    for (int i = 0; i < _maxBackwardDistance; i++)
                        _buffer[i] = _buffer[_maxBackwardDistance + i];

                    _bufferOffset -= _maxBackwardDistance;
                    _bufferLength -= _maxBackwardDistance;
                }
                else
                {
                    var oldBuffer = _buffer;
                    _buffer = new byte[_bufferLength << 1];
                    Array.Copy(oldBuffer, 0, _buffer, 0, _bufferLength);
                }
            }

            _buffer[_bufferLength] = (byte)value;
            _bufferLength++;
            _mLen--;
        }

        private void AddDistance()
        {
            _d4 = _d3;
            _d3 = _d2;
            _d2 = _d1;
            _d1 = _distance;
        }

        private Tree BuildTree(int[] lengths)
        {
            var tree = new Tree();

            int maxLen = 0;
            for (int i = 0; i < lengths.Length; i++)
                if (lengths[i] > maxLen)
                    maxLen = lengths[i];

            int[] blCount = new int[maxLen + 1];
            for (int i = 0; i < lengths.Length; i++)
                blCount[lengths[i]]++;

            if (maxLen == 1 && blCount[1] == 1)
            {
                for (int i = 0; i < lengths.Length; i++)
                    if (lengths[i] == 1)
                    {
                        tree.Value = i;
                        break;
                    }
                return tree;
            }

            int code = 0;
            blCount[0] = 0;
            int[] nextCode = new int[maxLen + 1];
            for (int bits = 1; bits <= maxLen; bits++)
            {
                code = (code + blCount[bits - 1]) << 1;
                nextCode[bits] = code;
            }

            for (int i = 0; i < lengths.Length; i++)
            {
                int len = lengths[i];
                if (len != 0)
                {
                    AddToTree(tree, i, nextCode[len], len);
                    nextCode[len]++;
                }
            }

            return tree;
        }

        private void AddToTree(Tree tree, int value, int code, int len)
        {
            for (int i = len - 1; i >= 0; i--)
            {
                int bit = code & (1 << i);
                if (bit == 0)
                {
                    if (tree.Zero == null)
                        tree.Zero = new Tree();
                    tree = tree.Zero;
                }
                else
                {
                    if (tree.One == null)
                        tree.One = new Tree();
                    tree = tree.One;
                }
            }

            tree.Value = value;
        }

        private void ReadBlockType(Block block)
        {
            int code = ReadHuffmanValue(block.BlockTypeTree);
            int newBlockType;
            if (code == 0)
                newBlockType = block.PrevBlockType;
            else if (code == 1)
                newBlockType = (block.bType + 1) % block.nBlTypes;
            else
                newBlockType = code - 2;

            block.PrevBlockType = block.bType;
            block.bType = newBlockType;
        }

        private void ReadBlockLength(Block block)
        {
            int code = ReadHuffmanValue(block.BlockLenTree);
            block.bLen = BlockCountCodeOffset[code] + _reader.ReadBits(BlockCountCodeBits[code]);
        }

        private void ReadContextMap(Block block, int size)
        {
            block.nTrees = ReadVariableLength256Code();
            block.cMap = new int[size];
            if (block.nTrees >= 2)
            {
                int rleMax = _reader.ReadBits(1);
                if (rleMax == 1)
                    rleMax = 1 + _reader.ReadBits(4);
                var tree = ReadHuffmanTree(block.nTrees + rleMax);

                int index = 0;
                while (index < size)
                {
                    int code = ReadHuffmanValue(tree);
                    if (code == 0)
                    {
                        block.cMap[index] = 0;
                        index++;
                        continue;
                    }
                    if (code <= rleMax)
                    {
                        int count = (1 << code) + _reader.ReadBits(code);
                        if (index + count > size)
                            throw new BrotliStreamException();
                        while (count > 0)
                        {
                            block.cMap[index] = 0;
                            index++;
                            count--;
                        }
                    }
                    else
                    {
                        block.cMap[index] = code - rleMax;
                        index++;
                    }
                }
                bool imtf = _reader.ReadBits(1) == 1;
                if (imtf)
                    InverseMoveToFrontTransform(block.cMap);
            }
        }

        private void InverseMoveToFrontTransform(int[] map)
        {
            int[] mtf = new int[256];
            for (int i = 0; i < 256; i++)
                mtf[i] = i;
            for (int i = 0; i < map.Length; i++)
            {
                int index = map[i];
                int value = mtf[index];
                map[i] = value;
                while (index > 0)
                {
                    mtf[index] = mtf[index - 1];
                    index--;
                }
                mtf[0] = value;
            }
        }

        private void Transform(int id, int offset)
        {
            var transform = BrotliStaticDictionary.Transforms[id];

            for (int i = 0; i < transform.Suffix.Length; i++)
                AddLiteral(transform.Suffix[i]);

            switch (transform.Type)
            {
                case BrotliStaticDictionary.TransformType.Identity:
                    for (int i = 0; i < _copyLength; i++)
                        AddLiteral(BrotliStaticDictionary.Dict[offset + i]);
                    break;

                case BrotliStaticDictionary.TransformType.OmitLast9:
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException();
            }

            for (int i = 0; i < transform.Prefix.Length; i++)
                AddLiteral(transform.Prefix[i]);
        }

        #region Stream overrides

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return 0;

            if (_maxBackwardDistance != 0)
                count = System.Math.Min(_maxBackwardDistance, count);

            while (_state != State.Finished && _bufferLength - _bufferOffset < count)
            {
                switch (_state)
                {
                    case State.Uninitialized:
                        ReadHeader();
                        count = System.Math.Min(_maxBackwardDistance, count);
                        break;

                    case State.BlockStart:
                        ReadMetaBlockHeader();
                        break;

                    case State.MainLoop:
                        DecodeMetaBlockData();
                        break;

                    case State.InsertLoop:
                        ProcessInsertLoop();
                        break;

                    case State.Copy:
                        ProcessCopy();
                        break;

                    case State.Transform:
                        ProcessStaticDictionaryTransform();
                        break;
                }
            }

            int read = System.Math.Min(count, _bufferLength - _bufferOffset);
            Array.Copy(_buffer, _bufferOffset, buffer, offset, read);
            _bufferOffset += read;
            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        #endregion

        private enum State
        {
            Uninitialized,
            BlockStart,
            MainLoop,
            InsertLoop,
            Copy,
            Transform,
            Finished
        }

        private enum ContextMode
        {
            Lsb6 = 0,
            Msb6 = 1,
            Utf8 = 2,
            Signed = 3
        }

        private class Tree
        {
            public int? Value;
            public Tree Zero;
            public Tree One;
        }

        private class Block
        {
            public int nBlTypes;

            /// <summary>
            /// Prefix code for block types
            /// </summary>
            public int hTreeBType;

            /// <summary>
            /// Prefix code for block counts
            /// </summary>
            public int hTreeBLen;

            /// <summary>
            /// Block count
            /// </summary>
            public int bLen;

            /// <summary>
            /// Block type
            /// </summary>
            public int bType;
            public int PrevBlockType;

            public int nTrees;

            public int[] cMap;

            public int AlphabetSize;

            public Tree BlockTypeTree;
            public Tree BlockLenTree;
            public Tree[] Trees;
        }
    }
}