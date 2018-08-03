using System;
using System.Collections.Generic;
using System.IO;

namespace Zergatul.Network.Http
{
    public class HttpRequest
    {
        #region Headers-related properties

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

        private Stream _stream;
        private Uri _uri;
        private HttpRequestMessage _reqMsg = new HttpRequestMessage();

        public HttpRequest(Uri uri)
        {
            this._uri = uri;
            SetDefaultHeaders();
        }

        public HttpRequest(string uri)
            : this(new Uri(uri))
        {

        }

        public HttpResponse GetResponse()
        {
            _reqMsg.ToBytes();
            return null;
        }

        private void SetDefaultHeaders()
        {
            AcceptEncoding = "gzip";
            KeepAlive = true;
            Host = _uri.Host;
        }


    }
}