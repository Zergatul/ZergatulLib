using System;
using System.IO;
using System.Threading.Tasks;

namespace Zergatul.IO.Compression
{
    // https://tools.ietf.org/html/rfc1951
    public class DeflateStream : Stream
    {
        public Stream BaseStream { get; private set; }

        private long _position;

        private CompressionMode _mode;
        private BitReader _reader;
        private bool _leaveOpen;
        private State _state;
        private bool _isFinalBlock;
        private int _blockLength;
        private byte[] _ringBuffer;
        private int _bufferPos;
        private int _copyDistance;
        private int _copyLength;
        private int[] _codeLen;
        private HuffmanTree _literalAlphabet;
        private HuffmanTree _distanceAlphabet;

        public DeflateStream(Stream stream, CompressionMode mode)
            : this(stream, new BitReader(stream), mode, false)
        {
            
        }

        public DeflateStream(Stream stream, BitReader reader, CompressionMode mode, bool leaveOpen)
        {
            if (mode == CompressionMode.Compress)
                throw new NotImplementedException();

            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));

            _mode = mode;
            _reader = reader;
            _leaveOpen = leaveOpen;
            _state = State.ReadBlockHeader;
            _position = 0;
            _ringBuffer = new byte[0x8000];
            _bufferPos = 0;
            _codeLen = new int[19];
        }

        #region Stream overrides

        public override bool CanRead => _mode == CompressionMode.Decompress;
        public override bool CanSeek => false;
        public override bool CanWrite => _mode == CompressionMode.Compress;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => _position;
            set => throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_leaveOpen)
                    BaseStream.Dispose();
            }
        }

        public override void Flush()
        {
            
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_mode != CompressionMode.Decompress)
                throw new InvalidOperationException();

            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            int read = 0;
            while (true)
            {
                switch (_state)
                {
                    case State.ReadBlockHeader:
                        _isFinalBlock = _reader.ReadBits(1) == 1;
                        switch (_reader.ReadBits(2))
                        {
                            case 0: // no compression
                                _reader.ReadTillByteBoundary();
                                _blockLength = _reader.ReadBits(16);
                                if (_blockLength != (~_reader.ReadBits(16) & 0xFFFF))
                                    throw new DeflateStreamException();
                                _state = State.ReadUncompressedBlock;
                                break;

                            case 1: // fixed Huffman codes
                                _state = State.ReadFixedBlock;
                                break;

                            case 2: // dynamic Huffman codes
                                int hLit = 257 + _reader.ReadBits(5);
                                int hDist = 1 + _reader.ReadBits(5);
                                int hCLen = 4 + _reader.ReadBits(4);
                                for (int i = 0; i < hCLen; i++)
                                    _codeLen[_codeLenIndex[i]] = _reader.ReadBits(3);

                                try
                                {
                                    var tree = new HuffmanTree(_codeLen);
                                    _literalAlphabet = ReadLengthsTree(tree, hLit);
                                    _distanceAlphabet = ReadLengthsTree(tree, hDist);
                                }
                                catch (HuffmanTreeException ex)
                                {
                                    throw new DeflateStreamException(ex);
                                }

                                _state = State.ReadDynamicBlock;
                                break;

                            case 3: // reserved (error)
                                throw new DeflateStreamException();

                            default:
                                throw new InvalidOperationException();
                        }
                        break;

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
                            int bufRead = System.Math.Min(_reader.GetBufferedBytesCount(), System.Math.Min(count, _blockLength));
                            for (int i = 0; i < bufRead; i++)
                            {
                                int literal = _reader.ReadBits(8);
                                buffer[offset++] = (byte)literal;
                                AddLiteral(literal);
                            }

                            count -= bufRead;
                            read += bufRead;
                            _blockLength -= bufRead;

                            count = System.Math.Min(count, _blockLength);
                            if (count > 0)
                            {
                                int rawRead = BaseStream.Read(buffer, offset, count);
                                if (rawRead == 0)
                                    throw new EndOfStreamException();

                                for (int i = 0; i < rawRead; i++)
                                    AddLiteral(buffer[offset + i]);

                                read += rawRead;
                                _blockLength -= rawRead;
                            }

                            return read;
                        }
                        break;

                    case State.ReadFixedBlock:
                        int value = FixedTree.ReadNextSymbol(_reader);
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

                            int distanceCode = BitHelper.ReverseBits(_reader.ReadBits(5), 5);
                            if (distanceCode >= 30)
                                throw new DeflateStreamException();

                            _copyDistance = FixedDistanceAlphabet[distanceCode];
                            bits = FixedDistanceAlphabetBits[distanceCode];
                            if (bits > 0)
                                _copyDistance += _reader.ReadBits(bits);

                            if (_copyDistance > _position)
                                throw new DeflateStreamException();

                            _state = State.CopyInsideFixedBlock;
                        }
                        break;

                    case State.CopyInsideFixedBlock:
                        while (count > 0 && _copyLength > 0)
                        {
                            byte literal = _ringBuffer[(0x8000 + _bufferPos - _copyDistance) & 0x7FFF];
                            buffer[offset++] = literal;
                            AddLiteral(literal);
                            count--;
                            read++;
                            _copyLength--;
                        }

                        if (_copyLength == 0)
                            _state = State.ReadFixedBlock;
                        return read;

                    case State.ReadDynamicBlock:
                        value = _literalAlphabet.ReadNextSymbol(_reader);
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

                            int distanceCode = _distanceAlphabet.ReadNextSymbol(_reader);
                            if (distanceCode < 0 || distanceCode >= 30)
                                throw new DeflateStreamException();

                            _copyDistance = FixedDistanceAlphabet[distanceCode];
                            bits = FixedDistanceAlphabetBits[distanceCode];
                            if (bits > 0)
                                _copyDistance += _reader.ReadBits(bits);

                            if (_copyDistance > _position)
                                throw new DeflateStreamException();

                            _state = State.CopyInsideDynamicBlock;
                        }
                        break;

                    case State.CopyInsideDynamicBlock:
                        while (count > 0 && _copyLength > 0)
                        {
                            byte literal = _ringBuffer[(0x8000 + _bufferPos - _copyDistance) & 0x7FFF];
                            buffer[offset++] = literal;
                            AddLiteral(literal);
                            count--;
                            read++;
                            _copyLength--;
                        }

                        if (_copyLength == 0)
                            _state = State.ReadDynamicBlock;
                        return read;

                    case State.End:
                        return read;

                    default:
                        throw new InvalidOperationException();
                }
            }

            throw new InvalidOperationException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_mode != CompressionMode.Compress)
                throw new InvalidOperationException();
        }

        #endregion

        #region Private methods

        private void AddLiteral(int value)
        {
            _ringBuffer[_bufferPos++] = (byte)value;
            _position++;
            if (_bufferPos == _ringBuffer.Length)
                _bufferPos = 0;
        }

        private HuffmanTree ReadLengthsTree(HuffmanTree codes, int count)
        {
            int[] bits = new int[count];
            int index = 0;
            while (index < count)
            {
                int symbol = codes.ReadNextSymbol(_reader);
                if (symbol < 16)
                    bits[index++] = symbol;
                else if (symbol == 16)
                {
                    if (index == 0)
                        throw new DeflateStreamException();
                    int repeat = _reader.ReadBits(2) + 3;
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
            return new HuffmanTree(bits);
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

        private static readonly int[] _codeLenIndex = new[]
        {
            16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15
        };

        private static readonly HuffmanTree FixedTree;

        static DeflateStream()
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
            ReadUncompressedBlock,
            ReadFixedBlock,
            CopyInsideFixedBlock,
            ReadDynamicBlock,
            CopyInsideDynamicBlock,
            End
        }

        #endregion
    }
}