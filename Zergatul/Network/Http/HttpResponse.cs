using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Zergatul.IO;
using Zergatul.IO.Compression;

namespace Zergatul.Network.Http
{
    public class HttpResponse : IDisposable
    {
        private static readonly Regex _paramValueList = new Regex(@"^(?<param>\w+)=(?<value>\w+)(,\s*(?<param>\w+)=(?<value>\w+))*$");

        public Stream Stream { get; private set; }
        public string Version => _message.Version;
        public HttpStatusCode Status => (HttpStatusCode)_message.StatusCode;
        public string ReasonPhase => _message.ReasonPhrase;
        public string this[string header] => _message[header];

        private HttpConnection _connection;
        private GenericMessageReadStream _readStream;
        private HttpResponseMessage _message;
        private byte[] _buffer = new byte[1024 * 1024];
        private HttpResponseStream _httpResponseStream;
        private bool _disposed = false;

        public HttpResponse(HttpConnection connection)
        {
            this._connection = connection;
            Init();
        }

        public IReadOnlyList<string> AllHeaders() => _message.AllHeaders();

        private void Init()
        {
            _readStream = new GenericMessageReadStream(_connection.Stream);

            _message = new HttpResponseMessage();
            _message.Read(_readStream, _buffer);

            string keepAlive = _message[HttpResponseHeaders.KeepAlive];
            int timeout = 0;
            if (keepAlive != null)
            {
                var match = _paramValueList.Match(keepAlive);
                if (!match.Success)
                    throw new InvalidOperationException();
                int count = match.Groups["param"].Captures.Count;
                for (int i = 0; i < count; i++)
                {
                    if (string.Equals(match.Groups["param"].Captures[i].Value, HttpHeaderValue.Timeout, StringComparison.InvariantCultureIgnoreCase))
                    {
                        timeout = int.Parse(match.Groups["value"].Captures[i].Value);
                        break;
                    }
                }
            }
            if (timeout != 0)
            {
                _connection.Timer.Restart();
                _connection.Timeout = timeout;
            }
            else
            {
                _connection.Timer.Reset();
                _connection.Timeout = 0;
            }

            long length = -1;
            string contentLength = _message[HttpResponseHeaders.ContentLength];
            if (contentLength != null)
                length = long.Parse(contentLength);
            bool chunked = false;
            string transferEncoding = _message[HttpResponseHeaders.TransferEncoding];
            if (transferEncoding != null)
            {
                if (transferEncoding.IndexOf(',') >= 0)
                    throw new NotImplementedException();
                if (string.Equals(transferEncoding, HttpHeaderValue.Chunked, StringComparison.InvariantCultureIgnoreCase))
                {
                    length = -1;
                    chunked = true;
                }
            }
            _httpResponseStream = new HttpResponseStream(_readStream, chunked, length);
            Stream = _httpResponseStream;
            string contentEncoding = _message[HttpResponseHeaders.ContentEncoding];
            if (contentEncoding != null)
            {
                if (string.Equals(contentEncoding, HttpHeaderValue.GZip, StringComparison.OrdinalIgnoreCase))
                    Stream = new GZipStream(Stream, CompressionMode.Decompress, true);
                else if (string.Equals(contentEncoding, HttpHeaderValue.Brotli, StringComparison.OrdinalIgnoreCase))
                    Stream = new BrotliStream(Stream, CompressionMode.Decompress);
                else
                    throw new InvalidOperationException("Unsupported content encoding: " + contentEncoding);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _httpResponseStream.ReadToEnd();
                if (string.Equals(_message[HttpResponseHeaders.Connection], HttpHeaderValue.KeepAlive, StringComparison.InvariantCultureIgnoreCase))
                    _connection.Close();
                else
                    _connection.CloseUnderlyingStream();
                _disposed = true;
            }
            else
            {
                // if dispose wasn't called, we will not keep this connection for keep-alive reuse
                _connection.CloseUnderlyingStream();
                _disposed = true;
            }
        }

        ~HttpResponse()
        {
            Dispose(false);
        }
    }
}