using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zergatul.IO;

namespace Zergatul.Network.Http
{
    public class HttpRequest
    {
        #region Properties

        public Uri Uri { get; }

        private string _method;
        public string Method
        {
            get => _method;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value == "")
                    throw new ArgumentException();
                _method = value;
            }
        }

        private string _version;
        public string Version
        {
            get => _version;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (value != "1.0" && value != "1.1")
                    throw new ArgumentException();
                _version = value;
            }
        }

        public string AcceptEncoding
        {
            get => GetHeader(HttpRequestHeaders.AcceptEncoding);
            set => SetHeader(HttpRequestHeaders.AcceptEncoding, value);
        }

        public bool KeepAlive
        {
            get
            {
                string connection = GetHeader(HttpRequestHeaders.Connection);
                if (string.Equals(connection, HttpHeaderValue.KeepAlive, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(connection, HttpHeaderValue.Close, StringComparison.OrdinalIgnoreCase))
                    return false;
                throw new InvalidOperationException();
            }
            set
            {
                SetHeader(HttpRequestHeaders.Connection, value ? HttpHeaderValue.KeepAlive : HttpHeaderValue.Close);
            }
        }

        public string Host
        {
            get => GetHeader(HttpRequestHeaders.Host);
            set => SetHeader(HttpRequestHeaders.Host, value);
        }

        public string this[string header]
        {
            get => GetHeader(header);
            set => SetHeader(header, value);
        }

        public Stream Body { get; set; }

        #endregion

        #region Private fields

        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        private List<string> _orderedHeaders = new List<string>();

        #endregion

        public HttpRequest(string uri)
            : this(new Uri(uri))
        {

        }

        public HttpRequest(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            Uri = uri;
            SetDefaultHeaders();
        }

        #region Public methods

        public string GetHeader(string header)
        {
            if (_headers.TryGetValue(header.ToLower(), out string value))
                return value;
            else
                return null;
        }

        public void SetHeader(string header, string value)
        {
            string lower = header.ToLower();
            bool remove = string.IsNullOrEmpty(value);
            if (_headers.ContainsKey(lower))
            {
                if (remove)
                    _headers.Remove(lower);
                else
                    _headers[lower] = value;

                int index = _orderedHeaders.FindIndex(h => string.Equals(h, lower, StringComparison.InvariantCultureIgnoreCase));
                if (index == -1)
                    throw new InvalidOperationException();
                _orderedHeaders.RemoveAt(index);

                if (!remove)
                    _orderedHeaders.Add(header);
            }
            else
            {
                if (!remove)
                {
                    _headers.Add(lower, value);
                    _orderedHeaders.Add(header);
                }
            }
        }

        public void RemoveHeader(string header)
        {
            header = header.ToLower();
            if (_headers.ContainsKey(header))
            {
                _headers.Remove(header);

                int index = _orderedHeaders.FindIndex(h => string.Equals(h, header, StringComparison.InvariantCultureIgnoreCase));
                if (index == -1)
                    throw new InvalidOperationException();
                _orderedHeaders.RemoveAt(index);
            }
        }

        /// <summary>
        /// Writes request content to stream. For best performance stream should have internal buffer and Flush() method implementation.
        /// </summary>
        public void WriteTo(Stream stream)
        {
            byte[] buffer = Encoding.ASCII.GetBytes($"{Method} {Uri.PathAndQuery} HTTP/{Version}{Constants.TelnetEndOfLine}");
            stream.Write(buffer, 0, buffer.Length);
            foreach (string header in _orderedHeaders)
            {
                buffer = Encoding.ASCII.GetBytes($"{header}: {_headers[header.ToLower()]}{Constants.TelnetEndOfLine}");
                stream.Write(buffer, 0, buffer.Length);
            }
            stream.Write(Constants.TelnetEndOfLineBytes, 0, Constants.TelnetEndOfLineBytes.Length);

            if (Body != null)
                Body.CopyTo(stream);
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            byte[] buffer = Encoding.ASCII.GetBytes($"{Method} {Uri.PathAndQuery} HTTP/{Version}{Constants.TelnetEndOfLine}");
            await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            foreach (string header in _orderedHeaders)
            {
                buffer = Encoding.ASCII.GetBytes($"{header}: {_headers[header.ToLower()]}{Constants.TelnetEndOfLine}");
                await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            }
            await stream.WriteAsync(Constants.TelnetEndOfLineBytes, 0, Constants.TelnetEndOfLineBytes.Length, cancellationToken);

            if (Body != null)
                await StreamHelper.CopyToAsync(Body, stream, cancellationToken);
        }

        #endregion

        #region Private methods

        private void SetDefaultHeaders()
        {
            Method = HttpMethods.Get;
            Version = "1.1";

            Host = Uri.Host;
            AcceptEncoding = "gzip, br";
            KeepAlive = true;
        }

        #endregion
    }
}