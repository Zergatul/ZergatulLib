using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Network.Http
{
    public class HttpRequestMessage
    {
        public string Method { get; set; }
        public string RequestUri { get; set; }
        public string Version { get; set; } = HttpVersion.V11;
        public byte[] Body { get; set; }

        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        private List<string> _orderedHeaders = new List<string>();

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

        public string GetHeader(string header)
        {
            if (_headers.TryGetValue(header.ToLower(), out string value))
                return value;
            else
                return null;
        }

        public byte[] ToBytes()
        {
            if (Method == null)
                throw new InvalidOperationException();

            List<byte> buffer = new List<byte>();
            buffer.AddRange(Encoding.ASCII.GetBytes($"{Method} {RequestUri} {Version}{Constants.TelnetEndOfLine}"));
            foreach (string header in _orderedHeaders)
                    buffer.AddRange(Encoding.ASCII.GetBytes($"{header}: {_headers[header.ToLower()]}{Constants.TelnetEndOfLine}"));
            buffer.AddRange(Encoding.ASCII.GetBytes(Constants.TelnetEndOfLine));
            if (Body != null)
                buffer.AddRange(Body);

            return buffer.ToArray();
        }
    }
}