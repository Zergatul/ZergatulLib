using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zergatul.Net;

namespace Zergatul.Ftp
{
    public class FtpControlStreamReader
    {
        static Regex _multiLineReply = new Regex(@"^(?<code>\d{3})-.+$");
        static Regex _singleLineReply = new Regex(@"^(?<code>\d{3}) .+$");

        Stream _stream;
        byte[] _buffer;
        int _bufPos, _bufLen;
        List<byte> _line;

        public FtpControlStreamReader(Stream stream)
        {
            this._stream = stream;
            this._buffer = new byte[1024];
            this._line = new List<byte>();
            this._bufPos = 0;
            this._bufLen = 0;
        }

        private void ReadBuffer()
        {
            if (_bufPos + 1 < _bufLen)
                throw new InvalidOperationException("Buffer contain data, read it to the end.");

            _bufPos = 0;
            _bufLen = _stream.Read(_buffer, 0, _buffer.Length);
        }

        private bool EndOfLine()
        {
            return _line.Count >= 2 && _line[_line.Count - 2] == Constants.TelnetEndOfLine[0] && _line[_line.Count - 1] == Constants.TelnetEndOfLine[1];
        }

        public string ReadLine()
        {
            _line.Clear();

            while (true)
            {
                if (_bufPos + 1 >= _bufLen)
                    ReadBuffer();

                for (; _bufPos < _bufLen; _bufPos++)
                {
                    _line.Add(_buffer[_bufPos]);
                    if (EndOfLine())
                    {
                        _line.RemoveRange(_line.Count - 2, 2);
                        _bufPos++;
                        return Encoding.ASCII.GetString(_line.ToArray());
                    }
                }
            }
        }

        public FtpServerReply ReadServerReply()
        {
            string firstLine = ReadLine();
            Match m = _multiLineReply.Match(firstLine);

            if (m.Success)
            {
                int code = int.Parse(m.Groups["code"].Value);
                List<string> lines = new List<string>();
                lines.Add(firstLine);
                do
                {
                    string line = ReadLine();
                    lines.Add(line);
                    m = _singleLineReply.Match(line);
                }
                while (!m.Success);
                if (int.Parse(m.Groups["code"].Value) != code)
                    throw new InvalidDataException("Invalid reply");
                return new FtpServerReply((FtpReplyCode)code, string.Join(Environment.NewLine, lines));
            }
            else
            {
                m = _singleLineReply.Match(firstLine);
                if (m.Success)
                    return new FtpServerReply((FtpReplyCode)int.Parse(m.Groups["code"].Value), firstLine);
                else
                    throw new InvalidDataException("Incorrect reply format");
            }
        }
    }
}