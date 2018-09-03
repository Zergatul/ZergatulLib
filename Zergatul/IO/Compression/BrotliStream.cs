﻿using System;
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
        private int _r1, _r2, _r3, _r4;
        private Block[] _blocks;
        private Block _literalBlock;
        private Block _insertCopyBlock;
        private Block _distanceBlock;
        private ContextMode[] _cMode;

        private int _distanceCode;
        private int _insertLength;
        private int _copyLength;
        private int _distance;
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

            _r1 = 4;
            _r2 = 11;
            _r3 = 15;
            _r4 = 16;

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

                    for (int i = 0; i < _mLen; i++)
                        AddLiteral(_reader.ReadBits(8));

                    _state = State.BlockStart;
                    return;
                }
            }

            // Literals - 0
            // Inser-and-Copy - 1
            // Distance - 2
            _literalBlock = new Block
            {
                AlphabetSize = LiteralAlphabetSize
            };
            _insertCopyBlock = new Block
            {
                AlphabetSize = InsertCopyAlphabetSize,
                nTrees = 1
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
                    _blocks[i].bLen = ReadBlockLength(_blocks[i].BlockLenTree);
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

            // read only literal and distance trees
            for (int i = 0; i < 3; i += 2)
            {
                _blocks[i].nTrees = ReadVariableLength256Code();
                _blocks[i].cMap = new int[_blocks[i].nBlTypes];
                if (_blocks[i].nTrees >= 2)
                    throw new NotImplementedException();
                else
                {
                    // fill with zeros
                }
            }

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
                throw new NotImplementedException();
            }

            _insertCopyBlock.bLen--;
            int insertCode, copyCode;
            DecodeInsertCopyLengthCode(ReadHuffmanValue(_insertCopyBlock.Trees[0]), out insertCode, out copyCode, out _distanceCode);
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
                    throw new NotImplementedException();
                }
                _literalBlock.bLen--;

                int cidl = 0; // GetContextId(_cMode[_literalBlock.bType]); TODO
                int literal = ReadHuffmanValue(_literalBlock.Trees[64 * _literalBlock.bType + cidl]);
                AddLiteral(literal);

                _insertLength--;
                _mLen--;
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
                    _distance = _r1;
                }
                else
                {
                    if (_distanceBlock.bLen == 0)
                        throw new NotImplementedException();
                    _distanceBlock.bLen--;

                    _distanceCode = ReadHuffmanValue(_distanceBlock.Trees[0]); // TODO
                    if (_distanceCode >= SpecialDistanceSymbols + _nDirect)
                    {
                        int nDistBits = 1 + ((_distanceCode - _nDirect - SpecialDistanceSymbols) >> (_nPostfix + 1));
                        int hcode = (_distanceCode - _nDirect - SpecialDistanceSymbols) >> _nPostfix;
                        int lcode = (_distanceCode - _nDirect - SpecialDistanceSymbols) & _postfixMask;
                        int offset = ((2 + (hcode & 1)) << nDistBits) - 4;
                        int dextra = _reader.ReadBits(nDistBits);
                        _distance = ((offset + dextra) << _nPostfix) + lcode + _nDirect + 1;
                        AddDistance();
                    }
                    else
                        throw new NotImplementedException();
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

                _mLen -= _copyLength;
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
                    return new Tree
                    {
                        Zero = new Tree { Value = symbols[0] },
                        One = new Tree { Value = symbols[1] }
                    };

                case 3:
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

        private Tree ReadComplexPrefixCodes(int hSkip, int alphabetSize)
        {
            int[] codeLengthCodeLengths = new int[18]; // TODO why 18?
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
            while (symbolCount < alphabetSize && space > 0)
            {
                int codeLen = ReadHuffmanValue(tree);
                codeLength[symbolCount] = codeLen;
                symbolCount++;
                if (codeLen < 16)
                {
                    if (codeLen != 0)
                    {
                        space -= 0x8000 >> codeLen;
                    }
                }
                else
                    throw new NotImplementedException();
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
        }

        private void AddDistance()
        {
            _r4 = _r3;
            _r3 = _r2;
            _r2 = _r1;
            _r1 = _distance;
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

        private int ReadBlockLength(Tree tree)
        {
            int code = ReadHuffmanValue(tree);
            return BlockCountCodeOffset[code] + _reader.ReadBits(BlockCountCodeBits[code]);
        }

        #region Stream overrides

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
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

            public int nTrees;

            public int[] cMap;

            public int AlphabetSize;

            public Tree BlockTypeTree;
            public Tree BlockLenTree;
            public Tree[] Trees;
        }
    }
}