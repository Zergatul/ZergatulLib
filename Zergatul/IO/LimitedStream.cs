using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.IO
{
    public class LimitedReadStream : Stream
    {
        private Stream _stream;
        private int _limit;
        private int _totalRead;

        public LimitedReadStream(Stream stream, int limit)
        {
            this._stream = stream;
            this._limit = limit;
            this._totalRead = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = System.Math.Min(count, _limit - _totalRead);
            if (count == 0)
                return 0;
            int read = _stream.Read(buffer, offset, count);
            _totalRead += read;
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            count = System.Math.Min(count, _limit - _totalRead);
            if (count == 0)
                return 0;
            int read = await _stream.ReadAsync(buffer, offset, count, cancellationToken);
            _totalRead += read;
            return read;
        }

        public override bool CanRead => _stream.CanRead && _totalRead < _limit;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;
        public override long Length => _limit;

        public override long Position
        {
            get
            {
                return _totalRead;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override void Flush() => _stream.Flush();
        public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);
        public override void SetLength(long value) => _stream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);
    }
}