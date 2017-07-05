using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal class InterceptionStream : Stream
    {
        private Stream _innerStream;

        public delegate void ReadDataEventHandler(object sender, ReadDataEventArgs e);
        public delegate void WriteDataEventHandler(object sender, WriteDataEventArgs e);

        public event ReadDataEventHandler OnReadData;
        public event WriteDataEventHandler OnWriteData;

        public InterceptionStream(Stream innerStream)
        {
            this._innerStream = innerStream;
        }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;

        public override long Position
        {
            get
            {
                return _innerStream.Position;
            }

            set
            {
                _innerStream.Position = value;
            }
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _innerStream.Read(buffer, offset, count);
            if (bytesRead > 0)
                OnReadData?.Invoke(this, new ReadDataEventArgs
                {
                    Data = buffer.Skip(offset).Take(bytesRead).ToArray()
                });

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
            if (count > 0)
                OnWriteData?.Invoke(this, new WriteDataEventArgs
                {
                    Data = buffer.Skip(offset).Take(count).ToArray()
                });
        }
    }
}