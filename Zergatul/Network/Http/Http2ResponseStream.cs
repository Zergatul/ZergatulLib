using System;
using System.IO;
using System.IO.Compression;
using Zergatul.IO;
using Zergatul.IO.Compression;

namespace Zergatul.Network.Http
{
    class Http2ResponseStream : Stream
    {
        private readonly Http2Client _client;
        private readonly Http2Response _response;
        private readonly Stream _innerStream;

        public Http2ResponseStream(Http2Client client, Http2Response response)
        {
            this._client = client;
            this._response = response;

            var rawStream = new Http2ResponseRawDataStream(client, response);
            if (response.ContentEncoding != null)
            {
                switch (response.ContentEncoding)
                {
                    case HttpHeaderValue.GZip:
                        _innerStream = new GZipStream(rawStream, CompressionMode.Decompress, true);
                        break;

                    case HttpHeaderValue.Brotli:
                        _innerStream = new BrotliStream(rawStream, CompressionMode.Decompress);
                        break;

                    default:
                        throw new InvalidOperationException("Unknown content encoding: " + response.ContentEncoding);
                }
            }
            else
            {
                _innerStream = rawStream;
            }
        }

        #region Stream overrides

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new InvalidOperationException();

        public override long Position
        {
            get => throw new InvalidOperationException();
            set => throw new InvalidOperationException();
        }

        public override void Flush()
        {

        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            return _innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}