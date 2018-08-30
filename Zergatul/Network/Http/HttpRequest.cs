using System;

namespace Zergatul.Network.Http
{
    public class HttpRequest
    {
        public Proxy.ProxyBase Proxy { get; set; }

        #region Headers-related properties

        public string Method
        {
            get => _reqMsg.Method;
            set => _reqMsg.Method = value;
        }

        public string Version
        {
            get => _reqMsg.Version;
            set => _reqMsg.Version = value;
        }

        public string AcceptEncoding
        {
            get
            {
                return _reqMsg.GetHeader(HttpRequestHeaders.AcceptEncoding);
            }
            set
            {
                _reqMsg.SetHeader(HttpRequestHeaders.AcceptEncoding, value);
            }
        }

        public bool KeepAlive
        {
            get
            {
                string connection = _reqMsg.GetHeader(HttpRequestHeaders.Connection);
                if (string.Equals(connection, HttpHeaderValue.KeepAlive, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(connection, HttpHeaderValue.Close, StringComparison.OrdinalIgnoreCase))
                    return false;
                throw new InvalidOperationException();
            }
            set
            {
                _reqMsg.SetHeader(HttpRequestHeaders.Connection, value ? HttpHeaderValue.KeepAlive : HttpHeaderValue.Close);
            }
        }

        public string Host
        {
            get
            {
                return _reqMsg.GetHeader(HttpRequestHeaders.Host);
            }
            set
            {
                _reqMsg.SetHeader(HttpRequestHeaders.Host, value);
            }
        }

        #endregion

        public string this[string header]
        {
            get
            {
                return _reqMsg.GetHeader(header);
            }
            set
            {
                _reqMsg.SetHeader(header, value);
            }
        }

        private Uri _uri;
        private KeepAliveConnectionProvider _connectionProvider;
        private HttpRequestMessage _reqMsg = new HttpRequestMessage();

        public HttpRequest(Uri uri)
            : this(uri, DefaultKeepAliveConnectionProvider.Instance)
        {
            
        }

        public HttpRequest(Uri uri, KeepAliveConnectionProvider provider)
        {
            this._uri = uri;
            this._connectionProvider = provider;
            SetDefaultHeaders();
        }

        public HttpRequest(string uri)
            : this(new Uri(uri))
        {

        }

        public HttpResponse GetResponse()
        {
            if (_connectionProvider == null)
                throw new NotImplementedException();

            var connection = _connectionProvider.GetConnection(_uri, Proxy);

            byte[] requestBytes = _reqMsg.ToBytes();
            connection.Stream.Write(requestBytes, 0, requestBytes.Length);

            return new HttpResponse(connection);
        }

        private void SetDefaultHeaders()
        {
            Method = HttpMethod.Get;
            _reqMsg.RequestUri = _uri.PathAndQuery;

            AcceptEncoding = "gzip";
            KeepAlive = true;
            Host = _uri.Host;
        }


    }
}