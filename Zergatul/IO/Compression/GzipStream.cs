﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.IO.Compression
{
    // https://tools.ietf.org/html/rfc1952
    public class GzipStream : Stream
    {
        public Stream BaseStream { get; private set; }

        private CompressionMode _mode;
        private bool _leaveOpen;
        private long _position;
        private byte[] _readBuffer;
        private GzipDecompressor _decompressor;

        public GzipStream(Stream stream, CompressionMode mode, bool leaveOpen)
        {
            if (mode == CompressionMode.Compress)
                throw new NotImplementedException();

            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));

            _mode = mode;
            _leaveOpen = leaveOpen;

            _readBuffer = new byte[0x1000];
            _decompressor = new GzipDecompressor(_readBuffer.Length);
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
                {
                    BaseStream?.Dispose();
                    BaseStream = null;
                }
            }
        }

        public override void Flush()
        {

        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_mode != CompressionMode.Decompress)
                throw new InvalidOperationException();

            EnsureNotDisposed();

            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            while (true)
            {
                if (_decompressor.Available == 0)
                    _decompressor.Decode();
                if (_decompressor.Available > 0)
                {
                    count = System.Math.Min(_decompressor.Available, count);
                    _decompressor.Get(buffer, offset, count);
                    _position += count;
                    return count;
                }

                int read = BaseStream.Read(_readBuffer, 0, _readBuffer.Length);
                if (read == 0)
                {
                    if (_decompressor.CanFinishNow)
                        return 0;
                    else
                        throw new EndOfStreamException();
                }

                _decompressor.Add(_readBuffer, 0, read);
            }
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (_mode != CompressionMode.Decompress)
                throw new InvalidOperationException();

            EnsureNotDisposed();

            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            while (true)
            {
                if (_decompressor.Available == 0)
                    _decompressor.Decode();
                if (_decompressor.Available > 0)
                {
                    count = System.Math.Min(_decompressor.Available, count);
                    _decompressor.Get(buffer, offset, count);
                    _position += count;
                    return count;
                }

                int read = await BaseStream.ReadAsync(_readBuffer, 0, _readBuffer.Length, cancellationToken).ConfigureAwait(false);
                if (read == 0)
                {
                    if (_decompressor.CanFinishNow)
                        return 0;
                    else
                        throw new EndOfStreamException();
                }

                _decompressor.Add(_readBuffer, 0, read);
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

        #region Private methods

        private void EnsureNotDisposed()
        {
            if (BaseStream == null)
                throw new ObjectDisposedException(nameof(GzipStream));
        }

        #endregion
    }
}