using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Zergatul.Tests
{
    public class DualMemoryStream : Stream
    {
        private List<byte> _readBuffer;
        private EventWaitHandle _readHandle;
        private List<byte> _writeBuffer;
        private EventWaitHandle _writeHandle;
        private EventWaitHandle _closeHandle;
        private EventWaitHandle _otherCloseHandle;

        public DualMemoryStream(
            List<byte> readBuffer,
            EventWaitHandle readHandle,
            List<byte> writeBuffer,
            EventWaitHandle writeHandle,
            EventWaitHandle closeHandle,
            EventWaitHandle otherCloseHandle)
        {
            _readBuffer = readBuffer;
            _readHandle = readHandle;
            _writeBuffer = writeBuffer;
            _writeHandle = writeHandle;
            _closeHandle = closeHandle;
            _otherCloseHandle = otherCloseHandle;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();
        public override long Position { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _closeHandle.Set();
        }

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
            while (true)
            {
                lock (_readBuffer)
                {
                    if (_readBuffer.Count > 0)
                    {
                        int totalRead = System.Math.Min(_readBuffer.Count, count);
                        for (int i = 0; i < totalRead; i++)
                            buffer[offset + i] = _readBuffer[i];
                        _readBuffer.RemoveRange(0, totalRead);

                        return totalRead;
                    }
                }

                if (_otherCloseHandle.WaitOne(0))
                    return 0;

                WaitHandle.WaitAny(new[] { _readHandle, _otherCloseHandle });
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_writeBuffer)
            {
                for (int i = 0; i < count; i++)
                    _writeBuffer.Add(buffer[offset + i]);
            }
            _writeHandle.Set();
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
            var evt1 = new AutoResetEvent(false);
            var evt2 = new AutoResetEvent(false);
            var cevt1 = new ManualResetEvent(false);
            var cevt2 = new ManualResetEvent(false);
            Peer1 = new DualMemoryStream(buffer1, evt1, buffer2, evt2, cevt1, cevt2);
            Peer2 = new DualMemoryStream(buffer2, evt2, buffer1, evt1, cevt2, cevt1);
        }
    }
}