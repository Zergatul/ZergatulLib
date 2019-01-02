using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Http.Frames;

namespace Zergatul.Network.Http
{
    class Http2ResponseRawDataStream : Stream
    {
        private readonly Http2Client _client;
        private readonly Http2Response _response;

        private byte[] _buffer;
        private int _bufferOffset;
        private bool _endOfStream;

        public Http2ResponseRawDataStream(Http2Client client, Http2Response response)
        {
            this._client = client;
            this._response = response;
        }

        #region Stream overrides

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotImplementedException();
        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // this is internal class, we assume parameters are valid, and count != 0

            if (_buffer == null || _buffer.Length == _bufferOffset)
            {
                if (_endOfStream)
                    return 0;

                var frame = _client.GetNextFrameForStream(_response.StreamId);
                switch (frame)
                {
                    case DataFrame dataFrame:
                        _buffer = dataFrame.Data;
                        _bufferOffset = 0;
                        if (dataFrame.END_STREAM)
                            _endOfStream = true;
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            if (_buffer == null)
                return 0;

            count = System.Math.Min(count, _buffer.Length - _bufferOffset);
            if (count == 0)
            {
                _buffer = null;
                return 0;
            }

            Array.Copy(_buffer, _bufferOffset, buffer, offset, count);
            _bufferOffset += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}