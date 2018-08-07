using System;
using System.IO;
using System.IO.Compression;
using Zergatul.IO;

namespace Zergatul.Network.Http
{
    public class HttpResponse : IDisposable
    {
        public Stream Stream { get; private set; }

        private HttpConnection _connection;
        private GenericMessageReadStream _readStream;
        private HttpResponseMessage _message;
        private byte[] _buffer = new byte[1024 * 1024];

        public HttpResponse(HttpConnection connection)
        {
            this._connection = connection;
            Init();
        }

        private void Init()
        {
            _readStream = new GenericMessageReadStream(_connection.Stream);

            _message = new HttpResponseMessage();
            _message.Read(_readStream, _buffer);

            long length = -1;
            string contentLength = _message[HttpResponseHeader.ContentLength];
            if (contentLength != null)
                length = long.Parse(contentLength);
            Stream = new HttpResponseStream(_readStream, length);
            if (string.Equals(_message[HttpResponseHeader.ContentEncoding], "gzip", StringComparison.OrdinalIgnoreCase))
                Stream = new GZipStream(Stream, CompressionMode.Decompress, true);
        }

        public void Dispose()
        {
            if (string.Equals(_message[HttpResponseHeader.Connection], HttpHeaderValue.KeepAlive, StringComparison.InvariantCultureIgnoreCase))
                _connection.Close();
            else
                _connection.CloseUnderlyingStream();
        }
    }
}