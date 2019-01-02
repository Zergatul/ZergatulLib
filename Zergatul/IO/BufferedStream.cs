using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.IO
{
    public class BufferedStream : Stream
    {
        private Stream _stream;
        private byte[] _readBuffer;
        private byte[] _writeBuffer;
        private int _readBufferPos;
        private int _readBufferLength;
        private int _writeBufferPos;

        public BufferedStream(Stream stream)
            : this(stream, 0x1000)
        {

        }

        public BufferedStream(Stream stream, int bufferSize)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            _stream = stream;
            _readBuffer = new byte[bufferSize];
            _writeBuffer = new byte[bufferSize];
            _readBufferPos = 0;
            _readBufferLength = 0;
            _writeBufferPos = 0;
        }

        #region Stream overrides

        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _stream.Dispose();
        }

        public override void Flush()
        {
            if (_writeBufferPos != 0)
            {
                _stream.Write(_writeBuffer, 0, _writeBufferPos);
                _writeBufferPos = 0;
                _stream.Flush();
            }
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            if (_writeBufferPos != 0)
            {
                _stream.Write(_writeBuffer, 0, _writeBufferPos);
                _writeBufferPos = 0;
                await _stream.FlushAsync(cancellationToken);
            }
        }

        public override long Length => _stream.Length;

        public override long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            FillReadBufferIfRequired();

            int readBufferCount = _readBufferLength - _readBufferPos;
            if (readBufferCount != 0)
            {
                int read = System.Math.Min(readBufferCount, count);
                Buffer.BlockCopy(_readBuffer, _readBufferPos, buffer, offset, read);
                _readBufferPos += read;
                return read;
            }
            else
            {
                return 0;
            }
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            await FillReadBufferIfRequiredAsync(cancellationToken);

            int readBufferCount = _readBufferLength - _readBufferPos;
            if (readBufferCount != 0)
            {
                int read = System.Math.Min(readBufferCount, count);
                Buffer.BlockCopy(_readBuffer, _readBufferPos, buffer, offset, read);
                _readBufferPos += read;
                return read;
            }
            else
            {
                return 0;
            }
        }

        public override int ReadByte()
        {
            FillReadBufferIfRequired();
            if (_readBufferLength - _readBufferPos != 0)
                return _readBuffer[_readBufferPos++];
            else
                return -1;
        }

        public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);

        public override void SetLength(long value) => _stream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return;

            while (count > 0)
            {
                int write = System.Math.Min(_writeBuffer.Length - _writeBufferPos, count);
                Buffer.BlockCopy(buffer, offset, _writeBuffer, _writeBufferPos, write);

                _writeBufferPos += write;
                offset += write;
                count -= write;

                FlushWriteBufferIfFull();
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return;

            while (count > 0)
            {
                int write = System.Math.Min(_writeBuffer.Length - _writeBufferPos, count);
                Buffer.BlockCopy(buffer, offset, _writeBuffer, _writeBufferPos, write);

                _writeBufferPos += write;
                offset += write;
                count -= write;

                await FlushWriteBufferIfFullAsync(cancellationToken);
            }
        }

        public override void WriteByte(byte value)
        {
            _writeBuffer[_writeBufferPos++] = value;
            FlushWriteBufferIfFull();
        }

        #endregion

        #region Private methods

        private void FillReadBufferIfRequired()
        {
            if (_readBufferLength - _readBufferPos == 0)
            {
                _readBufferPos = 0;
                _readBufferLength = _stream.Read(_readBuffer, 0, _readBuffer.Length);
            }
        }

        private async Task FillReadBufferIfRequiredAsync(CancellationToken cancellationToken)
        {
            if (_readBufferLength - _readBufferPos == 0)
            {
                _readBufferPos = 0;
                _readBufferLength = await _stream.ReadAsync(_readBuffer, 0, _readBuffer.Length, cancellationToken);
            }
        }

        private void FlushWriteBufferIfFull()
        {
            if (_writeBufferPos == _writeBuffer.Length)
            {
                _stream.Write(_writeBuffer, 0, _writeBufferPos);
                _writeBufferPos = 0;
                _stream.Flush();
            }
        }

        private async Task FlushWriteBufferIfFullAsync(CancellationToken cancellationToken)
        {
            if (_writeBufferPos == _writeBuffer.Length)
            {
                await _stream.WriteAsync(_writeBuffer, 0, _writeBufferPos, cancellationToken);
                _writeBufferPos = 0;
                await _stream.FlushAsync(cancellationToken);
            }
        }

        #endregion
    }
}