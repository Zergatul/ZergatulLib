using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Http
{
    public class Http2Request
    {
        #region Header properties

        #region Pseudo-headers

        public string Method
        {
            get => GetPseudoHeader(Http2Headers.Pseudo.Method);
            set => SetPseudoHeader(Http2Headers.Pseudo.Method, value);
        }

        public string Scheme
        {
            get => GetPseudoHeader(Http2Headers.Pseudo.Scheme);
            set => SetPseudoHeader(Http2Headers.Pseudo.Scheme, value);
        }

        public string Authority
        {
            get => GetPseudoHeader(Http2Headers.Pseudo.Authority);
            set => SetPseudoHeader(Http2Headers.Pseudo.Authority, value);
        }

        public string Path
        {
            get => GetPseudoHeader(Http2Headers.Pseudo.Path);
            set => SetPseudoHeader(Http2Headers.Pseudo.Path, value);
        }

        #endregion

        #region Headers

        public string AcceptEncoding
        {
            get => GetHeader(Http2Headers.AcceptEncoding);
            set => SetHeader(Http2Headers.AcceptEncoding, value);
        }

        public string this[string header]
        {
            get
            {
                ValidateHeaderCharacters(header);
                if (header == Http2Headers.Cookie)
                    throw new InvalidOperationException("Please use Headers property");

                return GetHeader(header);
            }
            set
            {
                ValidateHeaderCharacters(header);
                if (header == Http2Headers.Cookie)
                {
                    AddCookie(value);
                    return;
                }

                SetHeader(header, value);
            }
        }

        public void AddCookie(string value)
        {
            _headers.Add(new Header(Http2Headers.Cookie, value));
        }

        public void RemoveHeader(string header)
        {
            ValidateHeaderCharacters(header);
            _headers.RemoveAll(h => h.Name == header);
        }

        #endregion

        #endregion

        public IEnumerable<Header> Headers => _pseudoHeaders.Concat(_headers);

        private readonly List<Header> _pseudoHeaders;
        private readonly List<Header> _headers;

        public Http2Request()
        {
            this._pseudoHeaders = new List<Header>();
            this._headers = new List<Header>();

            SetDefaultHeaders();
        }

        private void SetDefaultHeaders()
        {
            Method = "GET";
            AcceptEncoding = "gzip, br";
        }

        private string GetPseudoHeader(string name)
        {
            for (int i = 0; i < _pseudoHeaders.Count; i++)
                if (_pseudoHeaders[i].Name == name)
                    return _pseudoHeaders[i].Value;
            return null;
        }

        private void SetPseudoHeader(string name, string value)
        {
            for (int i = 0; i < _pseudoHeaders.Count; i++)
                if (_pseudoHeaders[i].Name == name)
                {
                    _pseudoHeaders[i] = new Header(name, value);
                    return;
                }

            _pseudoHeaders.Add(new Header(name, value));
        }

        private string GetHeader(string name)
        {
            for (int i = 0; i < _headers.Count; i++)
                if (_headers[i].Name == name)
                    return _headers[i].Value;
            return null;
        }

        private void SetHeader(string name, string value)
        {
            for (int i = 0; i < _headers.Count; i++)
                if (_headers[i].Name == name)
                {
                    _headers[i] = new Header(name, value);
                    return;
                }

            _headers.Add(new Header(name, value));
        }

        private void ValidateHeaderCharacters(string name)
        {
            if (name == null)
                throw new ArgumentNullException();
            if (name.Length == 0)
                throw new InvalidOperationException("Header length cannot be 0");
            for (int i = 0; i < name.Length; i++)
            {
                int @char = name[i];
                if (@char >= 128)
                    throw new InvalidOperationException("Non-ASCII characters not allowed");
                if ('A' <= @char && @char <= 'Z')
                    throw new InvalidOperationException("Upper case characters not allowed");
            }
        }
    }
}