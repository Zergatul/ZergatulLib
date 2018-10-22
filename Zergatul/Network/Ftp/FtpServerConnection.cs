using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Ftp
{
    public class FtpServerConnection
    {
        public TextWriter Log { get; set; }

        private readonly Stream _stream;

        public FtpServerConnection(Stream stream)
        {
            this._stream = stream;
        }

        public void WriteReply(FtpReplyCode reply, string message = null)
        {
            if (message != null)
            {
                for (int i = 0; i < message.Length; i++)
                {
                    int @char = message[i];
                    if (@char == 10 || @char == 13)
                        throw new NotImplementedException();
                }
            }

            int code = (int)reply;
            if (code < 100 || code >= 1000)
                throw new InvalidOperationException();

            string line = code + " " + (string.IsNullOrEmpty(message) ? reply.ToString() : message) + Constants.TelnetEndOfLine;
            byte[] buffer = Encoding.ASCII.GetBytes(line);
            _stream.Write(buffer, 0, buffer.Length);

            Log?.Write(line);
        }

        public void WriteFeatures(string[] features)
        {
            string message =
                "211-Extensions supported:" + Constants.TelnetEndOfLine +
                string.Join("", features.Select(f => " " + f + Constants.TelnetEndOfLine)) +
                "211 END" + Constants.TelnetEndOfLine;

            byte[] buffer = Encoding.ASCII.GetBytes(message);
            _stream.Write(buffer, 0, buffer.Length);

            Log?.Write(message);
        }

        public void ReadNextCommand(out string command, out string param)
        {
            List<char> data = new List<char>();
            while (true)
            {
                if (data.Count >= 2 && data[data.Count - 2] == Constants.TelnetEndOfLine[0] && data[data.Count - 1] == Constants.TelnetEndOfLine[1])
                    break;

                int @byte = _stream.ReadByte();
                if (@byte == -1)
                    throw new EndOfStreamException();

                data.Add((char)@byte);
            }

            int index = data.IndexOf(' ');
            char[] chars = data.ToArray();
            if (index == -1)
            {
                command = new string(chars, 0, chars.Length - 2);
                param = null;
            }
            else
            {
                command = new string(chars, 0, index);
                param = new string(chars, index + 1, chars.Length - index - 3);
            }

            Log?.Write(new string(chars));
        }
    }
}