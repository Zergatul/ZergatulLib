using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Zergatul.Network.Http
{
    class HttpResponseMessage
    {
        private static readonly Regex StatusLineRegex = new Regex(@"^(?<version>\S+) (?<status>\d{3}) (?<reason>.+)$");
        private static readonly Regex HeaderRegex = new Regex(@"^(?<header>[a-zA-Z0-9\-]+):( (?<value>.+))?$");

        public string Version { get; set; }
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        public static HttpResponseMessage Parse(byte[] buffer, int length, ref int messageLength)
        {
            if (length == 0)
                return null;

            int offset = 0;
            byte[] eol = Encoding.ASCII.GetBytes(Constants.TelnetEndOfLine);

            int index = ByteArray.IndexOf(buffer, offset, length, eol);
            if (index == -1)
                return null;

            var result = new HttpResponseMessage();
            result.ParseStatusLine(Encoding.ASCII.GetString(buffer, offset, index - offset));
            length -= index - offset - eol.Length;
            offset = index + eol.Length;
            while (true)
            {
                index = ByteArray.IndexOf(buffer, offset, length, eol);
                if (index == -1)
                    return null;
                if (index == offset)
                {
                    length -= index - offset - eol.Length;
                    offset = index + eol.Length;
                    break;
                }
                result.ParseHeader(Encoding.ASCII.GetString(buffer, offset, index - offset));
                length -= index - offset - eol.Length;
                offset = index + eol.Length;
            }

            messageLength = offset;
            return result;
        }

        private void ParseStatusLine(string line)
        {
            var match = StatusLineRegex.Match(line);
            if (!match.Success)
                throw new InvalidOperationException();

            Version = match.Groups["version"].Value;
            StatusCode = int.Parse(match.Groups["status"].Value);
            ReasonPhrase = match.Groups["reason"].Value;
        }

        private void ParseHeader(string line)
        {
            var match = HeaderRegex.Match(line);
            if (!match.Success)
                throw new InvalidOperationException();

            Headers.Add(match.Groups["header"].Value, match.Groups["value"].Value);
        }
    }
}