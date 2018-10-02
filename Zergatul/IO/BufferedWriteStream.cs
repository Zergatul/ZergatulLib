using System;
using System.IO;

namespace Zergatul.IO
{
    public class BufferedWriteStream : Stream
    {
        private const int InitialBufferSize = 0x2000;

        private Stream _stream;
        private int _bufferSize;
        private byte[] _buffer;
        private int _bufferPosition;

        public BufferedWriteStream(Stream stream, int bufferSize)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            this._stream = stream;
            this._bufferSize = bufferSize;

            this._buffer = new byte[System.Math.Min(bufferSize, InitialBufferSize)];
        }

        #region Stream overrides

        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;

        public override void Flush()
        {
            if (_bufferPosition != 0)
            {
                _stream.Write(_buffer, 0, _bufferPosition);
                _stream.Flush();
                _bufferPosition = 0;
            }
        }

        public override long Length => _stream.Length;

        public override long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public override int ReadByte() => _stream.ReadByte();

        public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);

        public override void SetLength(long value) => _stream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (StreamHelper.ValidateWriteParameters(buffer, offset, count))
                return;

            IncreaseBufferIfNeeded(count);

            while (count > 0)
            {
                int write = System.Math.Min(_buffer.Length - _bufferPosition, count);
                Array.Copy(buffer, offset, _buffer, _bufferPosition, write);

                _bufferPosition += write;
                offset += write;
                count -= write;

                ReleaseBufferIfNeeded();
            }
        }

        public override void WriteByte(byte value)
        {
            IncreaseBufferIfNeeded(1);

            _buffer[_bufferPosition] = value;
            _bufferPosition++;

            ReleaseBufferIfNeeded();
        }

        #endregion

        #region Private methods

        private void IncreaseBufferIfNeeded(int count)
        {
            if (_buffer.Length == _bufferSize)
                return;

            if (_bufferPosition + count > _buffer.Length)
            {
                // extend buffer 2 times more
                // but not greater than _bufferSize
                int newLength = System.Math.Max(2 * _buffer.Length, _bufferPosition + count);
                newLength = System.Math.Min(newLength, _bufferSize);

                byte[] newBuffer = new byte[newLength];
                Array.Copy(_buffer, 0, newBuffer, 0, _bufferPosition);
                _buffer = newBuffer;
            }
        }

        private void ReleaseBufferIfNeeded()
        {
            if (_bufferPosition == _buffer.Length)
            {
                _stream.Write(_buffer, 0, _bufferPosition);
                _stream.Flush();
                _bufferPosition = 0;
            }
        }

        #endregion
    }
}