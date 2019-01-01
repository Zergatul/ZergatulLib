using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zergatul.IO.Compression;

namespace Zergatul.Network.Http
{
    public class HttpResponse : IDisposable
    {
        public string Version { get; set; }
        public HttpStatusCode Status { get; set; }
        public string ReasonPhase { get; set; }
        public Stream Body { get; set; }

        public string this[string header]
        {
            get
            {
                for (int i = 0; i < _headers.Count; i++)
                {
                    var h = _headers[i];
                    if (string.Equals(header, h.Name, StringComparison.InvariantCultureIgnoreCase))
                        return h.Value;
                }
                return null;
            }
            set
            {
                if (header == null)
                    throw new ArgumentNullException(nameof(header));
                if (value == null)
                    throw new ArgumentNullException();
                if (string.Equals(header, HttpResponseHeaders.SetCookie, StringComparison.InvariantCultureIgnoreCase))
                    throw new InvalidOperationException("Use AddSetCookieHeader method");
                for (int i = 0; i < _headers.Count; i++)
                {
                    if (string.Equals(header, _headers[i].Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        _headers[i] = new Header(header, value);
                        return;
                    }
                }
                _headers.Add(new Header(header, value));
            }
        }

        private Stream _stream;
        private StringBuilder _sb;
        private List<Header> _headers;
        private byte[] _buffer;
        private bool _disposed = false;

        public HttpResponse()
        {
            _headers = new List<Header>();
        }

        public void ReadFrom(Stream stream)
        {
            _stream = stream;
            _sb = new StringBuilder();

            ParseStatusLine();
            ParseHeaders();

            ProcessHeaders();
        }

        public async Task ReadFromAsync(Stream stream, CancellationToken cancellationToken)
        {
            _stream = stream;
            _sb = new StringBuilder();

            await ParseStatusLineAsync(cancellationToken);
            await ParseHeadersAsync(cancellationToken);

            ProcessHeaders();
        }

        //private void Init()
        //{
        //    _readStream = new GenericMessageReadStream(_connection.Stream);

        //    _message = new HttpResponseMessage();
        //    _message.Read(_readStream, _buffer);

        //    string keepAlive = _message[HttpResponseHeaders.KeepAlive];
        //    int timeout = 0;
        //    if (keepAlive != null)
        //    {
        //        var match = _paramValueList.Match(keepAlive);
        //        if (!match.Success)
        //            throw new InvalidOperationException();
        //        int count = match.Groups["param"].Captures.Count;
        //        for (int i = 0; i < count; i++)
        //        {
        //            if (string.Equals(match.Groups["param"].Captures[i].Value, HttpHeaderValue.Timeout, StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                timeout = int.Parse(match.Groups["value"].Captures[i].Value);
        //                break;
        //            }
        //        }
        //    }

        //    var http1Connection = _connection as Http1Connection;
        //    if (http1Connection != null)
        //    {
        //        if (timeout != 0)
        //        {
        //            http1Connection.Timer.Restart();
        //            http1Connection.Timeout = timeout;
        //        }
        //        else
        //        {
        //            http1Connection.Timer.Reset();
        //            http1Connection.Timeout = 0;
        //        }
        //    }

        //    long length = -1;
        //    string contentLength = _message[HttpResponseHeaders.ContentLength];
        //    if (contentLength != null)
        //        length = long.Parse(contentLength);
        //    bool chunked = false;
        //    string transferEncoding = _message[HttpResponseHeaders.TransferEncoding];
        //    if (transferEncoding != null)
        //    {
        //        if (transferEncoding.IndexOf(',') >= 0)
        //            throw new NotImplementedException();
        //        if (string.Equals(transferEncoding, HttpHeaderValue.Chunked, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            length = -1;
        //            chunked = true;
        //        }
        //    }
        //    _httpResponseStream = new HttpResponseStream(_readStream, chunked, length);
        //    Stream = _httpResponseStream;
        //    string contentEncoding = _message[HttpResponseHeaders.ContentEncoding];
        //    if (contentEncoding != null)
        //    {
        //        if (string.Equals(contentEncoding, HttpHeaderValue.GZip, StringComparison.OrdinalIgnoreCase))
        //            Stream = new GZipStream(Stream, CompressionMode.Decompress, true);
        //        else if (string.Equals(contentEncoding, HttpHeaderValue.Brotli, StringComparison.OrdinalIgnoreCase))
        //            Stream = new BrotliStream(Stream, CompressionMode.Decompress);
        //        else
        //            throw new InvalidOperationException("Unsupported content encoding: " + contentEncoding);
        //    }
        //}

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            //var http1Connection = _connection as Http1Connection;
            //if (disposing)
            //{
            //    _httpResponseStream.ReadToEnd();
            //    if (http1Connection != null)
            //    {
            //        if (string.Equals(_message[HttpResponseHeaders.Connection], HttpHeaderValue.KeepAlive, StringComparison.InvariantCultureIgnoreCase))
            //            http1Connection.Close();
            //        else
            //            http1Connection.CloseUnderlyingStream();
            //    }
            //    _disposed = true;
            //}
            //else
            //{
            //    // if dispose wasn't called, we will not keep this connection for keep-alive reuse
            //    if (http1Connection != null)
            //        http1Connection.CloseUnderlyingStream();
            //    _disposed = true;
            //}
        }

        ~HttpResponse()
        {
            Dispose(false);
        }

        #endregion

        #region Error messages

        private const string InvalidHttpVersion = "Invalid HTTP version";
        private const string SpaceExpected = "Space symbol expected";
        private const string InvalidStatusCode = "Invalid status code";
        private const string CRLFExpected = "CRLF expected";
        private const string InvalidReasonPhrase = "Invalid reason phrase";
        private const string InvalidHeaderName = "Invalid header name";
        private const string InvalidHeaderValue = "Invalid header value";
        private const string EndOfStream = "Unexpected end of stream";

        #endregion

        #region Private methods

        private void ParseStatusLine()
        {
            // status-line = HTTP-version SP status-code SP reason-phrase CRLF
            // https://tools.ietf.org/html/rfc7230#section-3.1.2

            // parse http version
            // HTTP-version  = HTTP-name "/" DIGIT "." DIGIT
            // HTTP-name     = %x48.54.54.50 ; "HTTP", case-sensitive
            // https://tools.ietf.org/html/rfc7230#section-2.6
            if (ReadNextByte() != 'H')
                throw new HttpParseException(InvalidHttpVersion);
            if (ReadNextByte() != 'T')
                throw new HttpParseException(InvalidHttpVersion);
            if (ReadNextByte() != 'T')
                throw new HttpParseException(InvalidHttpVersion);
            if (ReadNextByte() != 'P')
                throw new HttpParseException(InvalidHttpVersion);
            if (ReadNextByte() != '/')
                throw new HttpParseException(InvalidHttpVersion);
            int major = ReadNextByte();
            if (!CharHelper.IsDigit(major))
                throw new HttpParseException(InvalidHttpVersion);
            if (ReadNextByte() != '.')
                throw new HttpParseException(InvalidHttpVersion);
            int minor = ReadNextByte();
            if (!CharHelper.IsDigit(minor))
                throw new HttpParseException(InvalidHttpVersion);
            if (ReadNextByte() != ' ')
                throw new HttpParseException(SpaceExpected);

            Version = $"HTTP/{(char)major}.{(char)minor}";

            // parse status code
            // status-code = 3DIGIT
            int d1 = ReadNextByte();
            if (!CharHelper.IsDigit(d1))
                throw new HttpParseException(InvalidStatusCode);
            int d2 = ReadNextByte();
            if (!CharHelper.IsDigit(d2))
                throw new HttpParseException(InvalidStatusCode);
            int d3 = ReadNextByte();
            if (!CharHelper.IsDigit(d3))
                throw new HttpParseException(InvalidStatusCode);
            if (ReadNextByte() != ' ')
                throw new HttpParseException(SpaceExpected);

            Status = (HttpStatusCode)int.Parse($"{(char)d1}{(char)d2}{(char)d3}");

            // parse reason phrase
            // reason-phrase = *( HTAB / SP / VCHAR / obs-text )
            _sb.Clear();
            while (true)
            {
                int ch = ReadNextByte();
                if (CharHelper.IsCR(ch))
                {
                    ch = ReadNextByte();
                    if (CharHelper.IsLF(ch))
                        break;
                    else
                        throw new HttpParseException(CRLFExpected);
                }
                if (CharHelper.IsTab(ch) || CharHelper.IsVChar(ch) || CharHelper.IsObsText(ch))
                    _sb.Append((char)ch);
                else
                    throw new HttpParseException(InvalidReasonPhrase);
            }

            ReasonPhase = _sb.ToString();
        }

        // copy of ParseStatusLine method
        private async Task ParseStatusLineAsync(CancellationToken cancellationToken)
        {
            if (await ReadNextByteAsync(cancellationToken) != 'H')
                throw new HttpParseException(InvalidHttpVersion);
            if (await ReadNextByteAsync(cancellationToken) != 'T')
                throw new HttpParseException(InvalidHttpVersion);
            if (await ReadNextByteAsync(cancellationToken) != 'T')
                throw new HttpParseException(InvalidHttpVersion);
            if (await ReadNextByteAsync(cancellationToken) != 'P')
                throw new HttpParseException(InvalidHttpVersion);
            if (await ReadNextByteAsync(cancellationToken) != '/')
                throw new HttpParseException(InvalidHttpVersion);
            int major = await ReadNextByteAsync(cancellationToken);
            if (!CharHelper.IsDigit(major))
                throw new HttpParseException(InvalidHttpVersion);
            if (await ReadNextByteAsync(cancellationToken) != '.')
                throw new HttpParseException(InvalidHttpVersion);
            int minor = await ReadNextByteAsync(cancellationToken);
            if (!CharHelper.IsDigit(minor))
                throw new HttpParseException(InvalidHttpVersion);
            if (await ReadNextByteAsync(cancellationToken) != ' ')
                throw new HttpParseException(SpaceExpected);

            Version = $"HTTP/{(char)major}.{(char)minor}";

            int d1 = await ReadNextByteAsync(cancellationToken);
            if (!CharHelper.IsDigit(d1))
                throw new HttpParseException(InvalidStatusCode);
            int d2 = await ReadNextByteAsync(cancellationToken);
            if (!CharHelper.IsDigit(d2))
                throw new HttpParseException(InvalidStatusCode);
            int d3 = await ReadNextByteAsync(cancellationToken);
            if (!CharHelper.IsDigit(d3))
                throw new HttpParseException(InvalidStatusCode);
            if (await ReadNextByteAsync(cancellationToken) != ' ')
                throw new HttpParseException(SpaceExpected);

            Status = (HttpStatusCode)int.Parse($"{(char)d1}{(char)d2}{(char)d3}");

            _sb.Clear();
            while (true)
            {
                int ch = await ReadNextByteAsync(cancellationToken);
                if (CharHelper.IsCR(ch))
                {
                    ch = await ReadNextByteAsync(cancellationToken);
                    if (CharHelper.IsLF(ch))
                        break;
                    else
                        throw new HttpParseException(CRLFExpected);
                }
                if (CharHelper.IsTab(ch) || CharHelper.IsVChar(ch) || CharHelper.IsObsText(ch))
                    _sb.Append((char)ch);
                else
                    throw new HttpParseException(InvalidReasonPhrase);
            }

            ReasonPhase = _sb.ToString();
        }

        private void ParseHeaders()
        {
            // header-field   = field-name ":" OWS field-value OWS
            // field-name     = token
            // field-value    = *( field-content / obs-fold )
            // field-content  = field-vchar [ 1*( SP / HTAB ) field-vchar ]
            // field-vchar    = VCHAR / obs-text
            // https://tools.ietf.org/html/rfc7230#section-3.2
            while (true)
            {
                // parse field name
                _sb.Clear();
                int ch = ReadNextByte();
                if (CharHelper.IsCR(ch))
                {
                    ch = ReadNextByte();
                    if (CharHelper.IsLF(ch))
                        break;
                    else
                        throw new HttpParseException(CRLFExpected);
                }
                if (CharHelper.IsTChar(ch))
                {
                    _sb.Append((char)ch);
                    while (true)
                    {
                        ch = ReadNextByte();
                        if (ch == ':')
                            break;
                        if (!CharHelper.IsTChar(ch))
                            throw new HttpParseException(InvalidHeaderName);
                        _sb.Append((char)ch);
                    }
                }
                else
                {
                    throw new HttpParseException(InvalidHeaderName);
                }

                string headerName = _sb.ToString();

                // parse field value
                _sb.Clear();
                while (true)
                {
                    ch = ReadNextByte();
                    if (!CharHelper.IsWhitespace(ch))
                        break;
                }
                if (!CharHelper.IsVChar(ch) && !CharHelper.IsObsText(ch))
                    throw new HttpParseException(InvalidHeaderValue);
                _sb.Append((char)ch);
                while (true)
                {
                    ch = ReadNextByte();
                    if (CharHelper.IsCR(ch))
                    {
                        ch = ReadNextByte();
                        if (CharHelper.IsLF(ch))
                            break;
                        else
                            throw new HttpParseException(CRLFExpected);
                    }
                    if (!CharHelper.IsVChar(ch) && !CharHelper.IsObsText(ch) && CharHelper.IsTab(ch))
                        throw new HttpParseException(InvalidHeaderValue);
                    _sb.Append((char)ch);
                }

                string headerValue = _sb.ToString();

                // add to collection

                _headers.Add(new Header(headerName, headerValue));
            }
        }

        // copy of ParseHeaders method
        private async Task ParseHeadersAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                _sb.Clear();
                int ch = await ReadNextByteAsync(cancellationToken);
                if (CharHelper.IsCR(ch))
                {
                    ch = await ReadNextByteAsync(cancellationToken);
                    if (CharHelper.IsLF(ch))
                        break;
                    else
                        throw new HttpParseException(CRLFExpected);
                }
                if (CharHelper.IsTChar(ch))
                {
                    _sb.Append((char)ch);
                    while (true)
                    {
                        ch = await ReadNextByteAsync(cancellationToken);
                        if (ch == ':')
                            break;
                        if (!CharHelper.IsTChar(ch))
                            throw new HttpParseException(InvalidHeaderName);
                        _sb.Append((char)ch);
                    }
                }
                else
                {
                    throw new HttpParseException(InvalidHeaderName);
                }

                string headerName = _sb.ToString();

                _sb.Clear();
                while (true)
                {
                    ch = await ReadNextByteAsync(cancellationToken);
                    if (!CharHelper.IsWhitespace(ch))
                        break;
                }
                if (!CharHelper.IsVChar(ch) && !CharHelper.IsObsText(ch))
                    throw new HttpParseException(InvalidHeaderValue);
                _sb.Append((char)ch);
                while (true)
                {
                    ch = await ReadNextByteAsync(cancellationToken);
                    if (CharHelper.IsCR(ch))
                    {
                        ch = await ReadNextByteAsync(cancellationToken);
                        if (CharHelper.IsLF(ch))
                            break;
                        else
                            throw new HttpParseException(CRLFExpected);
                    }
                    if (!CharHelper.IsVChar(ch) && !CharHelper.IsObsText(ch) && CharHelper.IsTab(ch))
                        throw new HttpParseException(InvalidHeaderValue);
                    _sb.Append((char)ch);
                }

                string headerValue = _sb.ToString();

                _headers.Add(new Header(headerName, headerValue));
            }
        }

        private void ProcessHeaders()
        {
            long length = -1;
            string contentLength = this[HttpResponseHeaders.ContentLength];
            if (contentLength != null)
                length = long.Parse(contentLength);
            bool chunked = false;
            string transferEncoding = this[HttpResponseHeaders.TransferEncoding];
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

            Stream stream = new HttpResponseStream(_stream, chunked, length);
            string contentEncoding = this[HttpResponseHeaders.ContentEncoding];
            if (contentEncoding != null)
            {
                if (string.Equals(contentEncoding, HttpHeaderValue.GZip, StringComparison.OrdinalIgnoreCase))
                    stream = new GZipStream(stream, CompressionMode.Decompress, true);
                else if (string.Equals(contentEncoding, HttpHeaderValue.Brotli, StringComparison.OrdinalIgnoreCase))
                    stream = new BrotliStream(stream, CompressionMode.Decompress);
                else
                    throw new InvalidOperationException("Unsupported content encoding: " + contentEncoding);
            }

            Body = stream;
        }

        private int ReadNextByte()
        {
            int result = _stream.ReadByte();
            if (result == -1)
                throw new HttpParseException(EndOfStream);
            return result;
        }

        private async Task<int> ReadNextByteAsync(CancellationToken cancellationToken)
        {
            if (_buffer == null)
                _buffer = new byte[1];

            int read = await _stream.ReadAsync(_buffer, 0, 1, cancellationToken);
            if (read == 0)
                throw new HttpParseException(EndOfStream);
            return _buffer[0];
        }

        #endregion
    }
}