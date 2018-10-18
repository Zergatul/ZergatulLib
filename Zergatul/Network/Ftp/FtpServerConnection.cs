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
        private readonly Stream _stream;
        private readonly FtpControlStreamWriter _writer;

        public FtpServerConnection(Stream stream)
        {
            this._stream = stream;
            this._writer = new FtpControlStreamWriter(stream);
        }

        public void WriteReply(FtpReplyCode code, string message = null)
        {
            _writer.WriteReply(new FtpServerReply(code, message));
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
        }
    }
}