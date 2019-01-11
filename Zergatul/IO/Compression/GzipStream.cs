using System;
using System.IO;
using System.Threading.Tasks;

namespace Zergatul.IO.Compression
{
    // https://tools.ietf.org/html/rfc1952
    public class GzipStream : Stream
    {
        public Stream BaseStream { get; private set; }

        private long _position;

        private CompressionMode _mode;
        private bool _leaveOpen;
        private BitReader _reader;
        private State _state;
        private DeflateStream _deflate;
        private CRC32 _crc32;
        private long _memberLength;
        private bool _begin;

        public GzipStream(Stream stream, CompressionMode mode, bool leaveOpen)
        {
            if (mode == CompressionMode.Compress)
                throw new NotImplementedException();

            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));

            _mode = mode;
            _leaveOpen = leaveOpen;
            _reader = new BitReader(stream);
            _state = State.ReadHeader;
            _crc32 = new CRC32(CRC32Parameters.IEEE8023);
            _begin = true;
        }

        public GzipStream(Stream stream, CompressionMode mode)
            : this(stream, mode, false)
        {
            
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
                    BaseStream?.Dispose();
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

            while (true)
            {
                switch (_state)
                {
                    case State.ReadHeader:
                        if (!_begin)
                        {
                            _reader.Peek(16, out int value, out int peekRead);
                            if (peekRead == 0)
                            {
                                _state = State.End;
                                goto case State.End;
                            }
                        }
                        int id = _reader.ReadBits(16);
                        if (id != 0x8B1F)
                            throw new GzipStreamException();
                        var cm = (CompressionMethod)_reader.ReadBits(8);
                        if (cm != CompressionMethod.Deflate)
                            throw new NotImplementedException();
                        var flags = (MemberFlags)_reader.ReadBits(8);
                        int mtime = _reader.ReadBits(32);
                        int xfl = _reader.ReadBits(8);
                        int os = _reader.ReadBits(8);
                        if (flags.HasFlag(MemberFlags.FEXTRA))
                            throw new NotImplementedException();
                        if (flags.HasFlag(MemberFlags.FNAME))
                            throw new NotImplementedException();
                        if (flags.HasFlag(MemberFlags.FCOMMENT))
                            throw new NotImplementedException();
                        if (flags.HasFlag(MemberFlags.FHCRC))
                            throw new NotImplementedException();
                        throw new NotImplementedException();
                        //_deflate = new DeflateStream(BaseStream, _reader, CompressionMode.Decompress, true);
                        _crc32.Reset();
                        _memberLength = 0;
                        _state = State.ReadDeflateStream;
                        break;

                    case State.ReadDeflateStream:
                        int read = _deflate.Read(buffer, offset, count);
                        if (read == 0)
                        {
                            _deflate.Dispose();
                            _deflate = null;
                            _reader.ReadTillByteBoundary();
                            uint sum = (uint)_reader.ReadBits(32);
                            int iSize = _reader.ReadBits(32);
                            if (_crc32.GetCheckSum() != sum)
                                throw new GzipStreamException();
                            if ((_memberLength & 0xFFFFFFFF) != iSize)
                                throw new GzipStreamException();
                            _begin = false;
                            _state = State.ReadHeader;
                            break;
                        }
                        _crc32.Update(buffer, offset, read);
                        _position += read;
                        _memberLength += read;
                        return read;

                    case State.End:
                        return 0;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_mode != CompressionMode.Compress)
                throw new InvalidOperationException();
        }

        #endregion

        #region Nested classes

        private enum State
        {
            ReadHeader,
            ReadDeflateStream,
            End
        }

        private enum CompressionMethod
        {
            Deflate = 8
        }

        [Flags]
        private enum MemberFlags
        {
            FTEXT = 0x01,
            FHCRC = 0x02,
            FEXTRA = 0x04,
            FNAME = 0x08,
            FCOMMENT = 0x10
        }

        #endregion
    }
}