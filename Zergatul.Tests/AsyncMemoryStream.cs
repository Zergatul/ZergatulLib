using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.Tests
{
    public class AsyncMemoryStream : Stream
    {
        private MemoryStream _ms;
        private int _delay;

        public AsyncMemoryStream(int delay)
        {
            _ms = new MemoryStream();
            _delay = delay;
        }

        public AsyncMemoryStream(byte[] buffer, int delay)
        {
            _ms = new MemoryStream(buffer);
            _delay = delay;
        }

        public byte[] GetBuffer() => _ms.GetBuffer();

        #region Stream overrides

        public override bool CanRead => _ms.CanRead;
        public override bool CanSeek => _ms.CanSeek;
        public override bool CanWrite => _ms.CanWrite;
        public override long Length => _ms.Length;

        public override long Position
        {
            get => _ms.Position;
            set => _ms.Position = value;
        }

        public override void Flush() => throw new NotSupportedException();

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(_delay);
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await Task.Delay(_delay);
            return _ms.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) => _ms.Seek(offset, origin);

        public override void SetLength(long value) => _ms.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await Task.Delay(_delay);
            _ms.Write(buffer, offset, count);
        }

        #endregion
    }
}