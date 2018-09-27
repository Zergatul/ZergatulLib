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

        public HttpVersion Version
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
        private HttpConnectionProvider _connectionProvider;
        private HttpRequestMessage _reqMsg;

        public HttpRequest(string uri)
            : this(new Uri(uri), HttpVersion.V1_1)
        {

        }

        public HttpRequest(Uri uri, HttpVersion version)
            : this(uri, version, DefaultHttpConnectionProvider.Instance)
        {
            
        }

        public HttpRequest(string uri, HttpVersion version)
            : this(new Uri(uri), version)
        {

        }

        public HttpRequest(Uri uri, HttpVersion version, HttpConnectionProvider provider)
        {
            this._uri = uri;
            this._connectionProvider = provider;
            this._reqMsg = new HttpRequestMessage();
            this._reqMsg.Version = version;
            SetDefaultHeaders();
        }

        public HttpResponse GetResponse()
        {
            if (_connectionProvider == null)
                throw new NotImplementedException();

            HttpConnection connection;
            switch (Version)
            {
                case HttpVersion.V1_0:
                case HttpVersion.V1_1:
                    connection = _connectionProvider.GetHttp1Connection(_uri, Proxy);
                    break;

                default:
                    throw new InvalidOperationException("Unknown HTTP version");
            }

            _reqMsg.Write(connection);

            return new HttpResponse(connection);
        }

        private void SetDefaultHeaders()
        {
            Method = HttpMethod.Get;
            _reqMsg.RequestUri = _uri.PathAndQuery;
            Host = _uri.Host;

            switch (Version)
            {
                case HttpVersion.V1_0:
                case HttpVersion.V1_1:
                    AcceptEncoding = "gzip, br";
                    KeepAlive = true;
                    break;
            }
        }
    }
}