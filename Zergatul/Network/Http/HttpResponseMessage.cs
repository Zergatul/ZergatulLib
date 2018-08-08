using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Zergatul.IO;

namespace Zergatul.Network.Http
{
    class HttpResponseMessage
    {
        private static readonly Regex StatusLineRegex = new Regex(@"^(?<version>\S+) (?<status>\d{3}) (?<reason>.+)$");
        private static readonly Regex HeaderRegex = new Regex(@"^(?<header>[a-zA-Z0-9\-]+):( (?<value>.+))?$");

        public string Version { get; set; }
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }

        private Dictionary<string, string> _headers = new Dictionary<string, string>();

        public string this[string header]
        {
            get
            {
                if (_headers.TryGetValue(header.ToLower(), out string value))
                    return value;
                else
                    return null;
            }
            set
            {
                header = header.ToLower();
                if (_headers.ContainsKey(header))
                    _headers[header] = value;
                else
                    _headers.Add(header, value);
            }
        }

        public void Read(GenericMessageReadStream stream, byte[] buffer)
        {
            int length = 0;
            int index = 0;

            int eolIndex;
            while ((eolIndex = ByteArray.IndexOf(buffer, index, length, Constants.TelnetEndOfLineBytes)) == -1)
                stream.IncrementalRead(buffer, ref length);

            ParseStatusLine(Encoding.ASCII.GetString(buffer, index, eolIndex - index));
            index = eolIndex + 2;

            while (true)
            {
                while ((eolIndex = ByteArray.IndexOf(buffer, index, length, Constants.TelnetEndOfLineBytes)) == -1)
                    stream.IncrementalRead(buffer, ref length);
                if (index == eolIndex)
                {
                    index += 2;
                    break;
                }

                ParseHeader(Encoding.ASCII.GetString(buffer, index, eolIndex - index));
                index = eolIndex + 2;
            }

            if (index < length)
                stream.SendBackBuffer(buffer, index, length - index);
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

            this[match.Groups["header"].Value] = match.Groups["value"].Value;
        }
    }
}