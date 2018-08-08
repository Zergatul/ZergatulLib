using System;

namespace Zergatul.Network.Http
{
    public class HttpRequest
    {
        #region Headers-related properties

        public string Method
        {
            get => _reqMsg.Method;
            set => _reqMsg.Method = value;
        }

        public string AcceptEncoding
        {
            get
            {
                return _reqMsg.Headers[HttpRequestHeader.AcceptEncoding];
            }
            set
            {
                _reqMsg.Headers[HttpRequestHeader.AcceptEncoding] = value;
            }
        }

        public bool KeepAlive
        {
            get
            {
                if (string.Equals(_reqMsg.Headers[HttpRequestHeader.Connection], HttpHeaderValue.KeepAlive, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(_reqMsg.Headers[HttpRequestHeader.Connection], HttpHeaderValue.Close, StringComparison.OrdinalIgnoreCase))
                    return false;
                throw new InvalidOperationException();
            }
            set
            {
                _reqMsg.Headers[HttpRequestHeader.Connection] = value ? HttpHeaderValue.KeepAlive : HttpHeaderValue.Close;
            }
        }

        public string Host
        {
            get
            {
                return _reqMsg.Headers[HttpRequestHeader.Host];
            }
            set
            {
                _reqMsg.Headers[HttpRequestHeader.Host] = value;
            }
        }

        #endregion

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

            var connection = _connectionProvider.GetConnection(_uri);

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