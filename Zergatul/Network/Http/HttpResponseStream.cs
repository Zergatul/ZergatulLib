using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.Network.Http
{
    public class HttpResponseStream : Stream
    {
        private Stream _innerStream;
        private long _position;
        private long _length;
        private bool _chunked;
        private ChunkReadState _chunkReadState;
        private long _chunkLength;
        private byte[] _buffer;

        public HttpResponseStream(Stream innerStream, bool chunked, long length)
        {
            _innerStream = innerStream;
            _position = 0;
            _length = length;
            _chunked = chunked;
            _chunkReadState = ChunkReadState.ReadChunkLength;
        }

        public bool EndOfStream
        {
            get
            {
                if (_chunked)
                    return _chunkReadState == ChunkReadState.End;
                if (_length >= 0)
                    return _length == _position;
                return false;
            }
        }

        #region Stream overrides

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                if (_length < 0)
                    throw new NotSupportedException();
                return _length;
            }
        }

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_chunked)
            {
                switch (_chunkReadState)
                {
                    case ChunkReadState.ReadChunkLength:
                        int ch = ReadNextByte();
                        if (CharHelper.IsHex(ch))
                            _chunkLength = CharHelper.ParseHex(ch);
                        else
                            throw new HttpParseException(InvalidChunkLength);
                        while (true)
                        {
                            ch = ReadNextByte();
                            if (CharHelper.IsCR(ch))
                            {
                                ch = ReadNextByte();
                                if (CharHelper.IsLF(ch))
                                    break;
                                else
                                    throw new HttpParseException(InvalidChunkLength);
                            }
                            if (CharHelper.IsHex(ch))
                                _chunkLength = (_chunkLength << 4) | (long)CharHelper.ParseHex(ch);
                            else
                                throw new HttpParseException(InvalidChunkLength);
                        }
                        if (_chunkLength == 0)
                        {
                            ch = ReadNextByte();
                            if (!CharHelper.IsCR(ch))
                                throw new HttpParseException(InvalidEnding);
                            ch = ReadNextByte();
                            if (!CharHelper.IsLF(ch))
                                throw new HttpParseException(InvalidEnding);
                            _chunkReadState = ChunkReadState.End;
                            goto case ChunkReadState.End;
                        }
                        else
                        {
                            _chunkReadState = ChunkReadState.ReadChunkData;
                            goto case ChunkReadState.ReadChunkData;
                        }

                    case ChunkReadState.ReadChunkData:
                        count = checked((int)System.Math.Min(count, _chunkLength));
                        int read = _innerStream.Read(buffer, offset, count);
                        _chunkLength -= read;
                        if (_chunkLength == 0)
                        {
                            ch = ReadNextByte();
                            if (!CharHelper.IsCR(ch))
                                throw new HttpParseException(InvalidChunkEnding);
                            ch = ReadNextByte();
                            if (!CharHelper.IsLF(ch))
                                throw new HttpParseException(InvalidChunkEnding);
                            _chunkReadState = ChunkReadState.ReadChunkLength;
                        }
                        return read;

                    case ChunkReadState.End:
                        return 0;

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
            {
                if (_length > 0)
                {
                    if (_position >= _length)
                        return 0;
                    count = checked((int)System.Math.Min(count, _length - _position));
                }
                int read = _innerStream.Read(buffer, offset, count);
                _position += read;
                return read;
            }
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_chunked)
            {
                switch (_chunkReadState)
                {
                    case ChunkReadState.ReadChunkLength:
                        int ch = await ReadNextByteAsync(cancellationToken);
                        if (CharHelper.IsHex(ch))
                            _chunkLength = CharHelper.ParseHex(ch);
                        else
                            throw new HttpParseException(InvalidChunkLength);
                        while (true)
                        {
                            ch = await ReadNextByteAsync(cancellationToken);
                            if (CharHelper.IsCR(ch))
                            {
                                ch = await ReadNextByteAsync(cancellationToken);
                                if (CharHelper.IsLF(ch))
                                    break;
                                else
                                    throw new HttpParseException(InvalidChunkLength);
                            }
                            if (CharHelper.IsHex(ch))
                                _chunkLength = (_chunkLength << 4) | (long)CharHelper.ParseHex(ch);
                            else
                                throw new HttpParseException(InvalidChunkLength);
                        }
                        if (_chunkLength == 0)
                        {
                            ch = await ReadNextByteAsync(cancellationToken);
                            if (!CharHelper.IsCR(ch))
                                throw new HttpParseException(InvalidEnding);
                            ch = await ReadNextByteAsync(cancellationToken);
                            if (!CharHelper.IsLF(ch))
                                throw new HttpParseException(InvalidEnding);
                            _chunkReadState = ChunkReadState.End;
                            goto case ChunkReadState.End;
                        }
                        else
                        {
                            _chunkReadState = ChunkReadState.ReadChunkData;
                            goto case ChunkReadState.ReadChunkData;
                        }

                    case ChunkReadState.ReadChunkData:
                        count = checked((int)System.Math.Min(count, _chunkLength));
                        int read = await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
                        _chunkLength -= read;
                        if (_chunkLength == 0)
                        {
                            ch = await ReadNextByteAsync(cancellationToken);
                            if (!CharHelper.IsCR(ch))
                                throw new HttpParseException(InvalidChunkEnding);
                            ch = await ReadNextByteAsync(cancellationToken);
                            if (!CharHelper.IsLF(ch))
                                throw new HttpParseException(InvalidChunkEnding);
                            _chunkReadState = ChunkReadState.ReadChunkLength;
                        }
                        return read;

                    case ChunkReadState.End:
                        return 0;

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
            {
                if (_length > 0)
                {
                    if (_position >= _length)
                        return 0;
                    count = checked((int)System.Math.Min(count, _length - _position));
                }
                int read = await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
                _position += read;
                return read;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Private constants

        private const string UnexpectedEndOfStream = "Unexpected end of stream";
        private const string InvalidChunkLength = "Invalid chunk length";
        private const string InvalidChunkEnding = "Chunk must end with CRLF";
        private const string InvalidEnding = "Expected CRLF after chunk with length 0";

        #endregion

        #region Private methods

        private int ReadNextByte()
        {
            int result = _innerStream.ReadByte();
            if (result == -1)
                throw new HttpParseException(UnexpectedEndOfStream);
            return result;
        }

        private async Task<int> ReadNextByteAsync(CancellationToken cancellationToken)
        {
            if (_buffer == null)
                _buffer = new byte[1];

            int read = await _innerStream.ReadAsync(_buffer, 0, 1, cancellationToken);
            if (read == 0)
                throw new HttpParseException(UnexpectedEndOfStream);
            return _buffer[0];
        }

        #endregion

        #region Nested classes

        private enum ChunkReadState
        {
            ReadChunkLength,
            ReadChunkData,
            End
        }

        #endregion
    }
}