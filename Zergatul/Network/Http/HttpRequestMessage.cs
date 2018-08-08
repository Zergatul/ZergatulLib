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
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
        public byte[] Body { get; set; }

        public byte[] ToBytes()
        {
            if (Method == null)
                throw new InvalidOperationException();

            List<byte> buffer = new List<byte>();
            buffer.AddRange(Encoding.ASCII.GetBytes($"{Method} {RequestUri} {Version}{Constants.TelnetEndOfLine}"));
            foreach (var kv in Headers)
                if (!string.IsNullOrEmpty(kv.Value))
                    buffer.AddRange(Encoding.ASCII.GetBytes($"{kv.Key}: {kv.Value}{Constants.TelnetEndOfLine}"));
            buffer.AddRange(Encoding.ASCII.GetBytes(Constants.TelnetEndOfLine));
            if (Body != null)
                buffer.AddRange(Body);

            return buffer.ToArray();
        }
    }
}