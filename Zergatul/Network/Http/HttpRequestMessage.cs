using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Zergatul.Network.Http
{
    public class HttpRequestMessage
    {
        #region Static

        private readonly Dictionary<HttpVersion, string> VersionStrings = new Dictionary<HttpVersion, string>
        {
            [HttpVersion.V1_0] = "HTTP/1.0",
            [HttpVersion.V1_1] = "HTTP/1.1",
        };

        #endregion

        public string Method { get; set; }
        public string RequestUri { get; set; }
        public HttpVersion Version { get; set; } = HttpVersion.V1_1;

        private byte[] _bodyBytes;
        public byte[] BodyBytes
        {
            get => _bodyBytes;
            set
            {
                _bodyStream = null;
                _bodyBytes = value;
            }
        }

        private Stream _bodyStream;
        public Stream BodyStream
        {
            get => _bodyStream;
            set
            {
                _bodyBytes = null;
                _bodyStream = value;
            }
        }

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

        public void Write(HttpConnection connection)
        {
            connection.WriteHeader(GetHeaderBytes());

            if (_bodyBytes != null)
                connection.WriteBody(_bodyBytes);
            if (_bodyStream != null)
                connection.WriteBody(_bodyStream);
        }

        public void Write(Stream stream, int bufferSize)
        {
            var bufferedStream = new BufferedStream(stream, bufferSize);
            byte[] buffer = GetHeaderBytes();
            bufferedStream.Write(buffer, 0, buffer.Length);

            if (_bodyBytes != null)
                bufferedStream.Write(_bodyBytes, 0, _bodyBytes.Length);
            if (_bodyStream != null)
                BodyStream.CopyTo(bufferedStream);

            bufferedStream.Flush();
        }

        private byte[] GetHeaderBytes()
        {
            if (Method == null)
                throw new InvalidOperationException();

            List<byte> buffer = new List<byte>();
            buffer.AddRange(Encoding.ASCII.GetBytes($"{Method} {RequestUri} {VersionStrings[Version]}{Constants.TelnetEndOfLine}"));
            foreach (string header in _orderedHeaders)
                buffer.AddRange(Encoding.ASCII.GetBytes($"{header}: {_headers[header.ToLower()]}{Constants.TelnetEndOfLine}"));
            buffer.AddRange(Encoding.ASCII.GetBytes(Constants.TelnetEndOfLine));
            return buffer.ToArray();
        }
    }
}