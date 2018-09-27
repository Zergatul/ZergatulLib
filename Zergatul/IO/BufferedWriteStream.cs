using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }
        }

        public override long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);

        public override void SetLength(long value) => _stream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            EnsureBufferSize(count);

            throw new NotImplementedException();
        }

        #endregion

        #region Private methods

        private void EnsureBufferSize(int count)
        {
            if (_buffer.Length == _bufferSize)
                return;

            if (_bufferPosition + count > _buffer.Length)
            {
                // extend buffer 2 times more
                // but not greater than _bufferSize
                int newLength = System.Math.Min(2 * _buffer.Length);
            }
        }

        #endregion
    }
}