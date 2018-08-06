using System;
using System.IO;

namespace Zergatul.IO
{
    class GenericMessageReadStream : Stream
    {
        private Stream _innerStream;
        private byte[] _buffer;
        private int _bufferIndex;

        public GenericMessageReadStream(Stream innerStream)
        {
            this._innerStream = innerStream;
        }

        public void IncrementalRead(byte[] buffer, int index, ref int length)
        {
            if (buffer.Length - index <= 0)
                throw new InvalidOperationException("Buffer too small");

            int read = Read(buffer, index, buffer.Length - length);
            if (read == 0)
                throw new EndOfStreamException();
            length += read;
        }

        public void SendBackBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException();
            if (offset < 0 || count < 0 || offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException();
            if (_buffer != null)
                throw new NotImplementedException();
            _bufferIndex = 0;
            if (count == 0)
            {
                _buffer = null;
                return;
            }
            else
            {
                _buffer = ByteArray.SubArray(buffer, offset, count);
            }
        }

        #region Stream overrides

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;

        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        public override void Flush() => _innerStream.Flush();

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_buffer != null)
            {
                int available = _buffer.Length - _bufferIndex;
                if (available <= 0)
                    throw new InvalidOperationException();
                int read = System.Math.Min(available, count);
                Array.Copy(_buffer, _bufferIndex, buffer, offset, read);
                _bufferIndex += read;
                if (_bufferIndex == _buffer.Length)
                {
                    _buffer = null;
                    _bufferIndex = 0;
                }
                return read;
            }
            else
                return _innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

        public override void SetLength(long value) => SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => Write(buffer, offset, count);

        #endregion
    }
}