using System;
using System.IO;

namespace Zergatul.Network.Http
{
    class HttpResponseStream : Stream
    {
        private Stream _innerStream;
        private long _position;
        private long _length;

        public HttpResponseStream(Stream innerStream, long length)
        {
            this._innerStream = innerStream;
            this._position = 0;
            this._length = length;
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
    }
}