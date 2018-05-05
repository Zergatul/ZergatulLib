using System;
using System.Collections.Generic;
using System.IO;

namespace Zergatul.Tls.Tests
{
    public class DualMemoryStream : Stream
    {
        private List<byte> _readBuffer;
        private List<byte> _writeBuffer;

        public DualMemoryStream(List<byte> readBuffer, List<byte> writeBuffer)
        {
            this._readBuffer = readBuffer;
            this._writeBuffer = writeBuffer;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => -1;
        public override long Position { get; set; }

        public override void Flush()
        {
            
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_readBuffer)
            {
                if (_readBuffer.Count == 0)
                    return 0;

                int totalRead = System.Math.Min(_readBuffer.Count, count);
                for (int i = 0; i < totalRead; i++)
                    buffer[offset + i] = _readBuffer[i];
                _readBuffer.RemoveRange(0, totalRead);

                return totalRead;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_writeBuffer)
            {
                for (int i = 0; i < count; i++)
                    _writeBuffer.Add(buffer[offset + i]);
            }
        }
    }

    public class DualPeer
    {
        public Stream Peer1 { get; private set; }
        public Stream Peer2 { get; private set; }

        public DualPeer()
        {
            var buffer1 = new List<byte>(1024);
            var buffer2 = new List<byte>(1024);
            Peer1 = new DualMemoryStream(buffer1, buffer2);
            Peer2 = new DualMemoryStream(buffer2, buffer1);
        }
    }
}