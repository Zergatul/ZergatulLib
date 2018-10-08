using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Http
{
    public class Http2Request
    {
        public string Method
        {
            get => GetPseudoHeader(":method");
            set => SetPseudoHeader(":method", value);
        }

        public string Scheme
        {
            get => GetPseudoHeader(":scheme");
            set => SetPseudoHeader(":scheme", value);
        }

        public string Authority
        {
            get => GetPseudoHeader(":authority");
            set => SetPseudoHeader(":authority", value);
        }

        public string Path
        {
            get => GetPseudoHeader(":path");
            set => SetPseudoHeader(":path", value);
        }

        public IEnumerable<Header> Headers => _pseudoHeaders.Concat(_headers);

        private List<Header> _pseudoHeaders;
        private List<Header> _headers;

        public Http2Request()
        {
            this._pseudoHeaders = new List<Header>();
            this._headers = new List<Header>();

            SetDefaultHeaders();
        }

        private void SetDefaultHeaders()
        {
            Method = "GET";
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
    }
}