using System;
using System.IO;
using System.Text;
using Zergatul.IO;

namespace Zergatul.Network.Http
{
    class HttpResponseStream : Stream
    {
        private Stream _innerStream;
        private long _position;
        private long _length;
        private bool _chunked;
        private ChunkReadState _chunkReadState;
        private long _chunkLength;
        private GenericMessageReadStream _readStream;
        private byte[] _chunkLengthReadBuffer;

        public HttpResponseStream(Stream innerStream, bool chunked, long length)
        {
            this._innerStream = innerStream;
            this._position = 0;
            this._length = length;
            this._chunked = chunked;

            if (chunked)
            {
                this._chunkReadState = ChunkReadState.ReadChunkLength;
                this._readStream = innerStream as GenericMessageReadStream;
                if (this._readStream == null)
                    throw new NotImplementedException();
                this._chunkLengthReadBuffer = new byte[18];
            }
        }

        public void ReadToEnd()
        {
            if (_length >= 0)
            {
                if (_position < _length)
                {
                    byte[] buffer = new byte[8 * 1024];
                    while (_position < _length)
                        Read(buffer, 0, buffer.Length);
                }
                return;
            }

            if (_chunked)
            {
                if (_chunkReadState != ChunkReadState.End)
                {
                    byte[] buffer = new byte[8 * 1024];
                    while (_chunkReadState != ChunkReadState.End)
                        Read(buffer, 0, buffer.Length);
                }
                return;
            }

            throw new InvalidOperationException();
        }

        private bool ReadEndOfLine()
        {
            int length = 0;
            do
            {
                length += _readStream.Read(_chunkLengthReadBuffer, length, 2 - length);
            } while (length < 2);
            return _chunkLengthReadBuffer[0] == Constants.TelnetEndOfLineBytes[0] && _chunkLengthReadBuffer[1] == Constants.TelnetEndOfLineBytes[1];
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
                        int length = 0;
                        int index;
                        do
                        {
                            _readStream.IncrementalRead(_chunkLengthReadBuffer, ref length);
                        }
                        while ((index = ByteArray.IndexOf(_chunkLengthReadBuffer, 0, length, Constants.TelnetEndOfLineBytes)) == -1);
                        _chunkLength = Convert.ToInt64(Encoding.ASCII.GetString(_chunkLengthReadBuffer, 0, index), 16);
                        index += 2;
                        if (index < length)
                            _readStream.SendBackBuffer(_chunkLengthReadBuffer, index, length - index);
                        if (_chunkLength == 0)
                        {
                            if (!ReadEndOfLine())
                                throw new ChunkedEncodingException("Last chunk must end with CRLF");
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
                        int read = _readStream.Read(buffer, offset, count);
                        _chunkLength -= read;
                        if (_chunkLength == 0)
                        {
                            if (!ReadEndOfLine())
                                throw new ChunkedEncodingException("Chunk must end with CRLF");
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