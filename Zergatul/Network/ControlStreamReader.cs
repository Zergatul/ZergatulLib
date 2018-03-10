using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Zergatul.Network
{
    public abstract class ControlStreamReader
    {
        static Regex _multiLineReply = new Regex(@"^(?<code>\d{3})-(?<msg>.+)$");
        static Regex _singleLineReply = new Regex(@"^(?<code>\d{3})( (?<msg>.+))?$");

        Stream _stream;
        byte[] _buffer;
        int _bufPos, _bufLen;
        List<byte> _line;

        public Stopwatch Timer;
        public int Timeout;

        public ControlStreamReader(Stream stream)
        {
            this._stream = stream;
            this._buffer = new byte[1024];
            this._line = new List<byte>();
            this._bufPos = 0;
            this._bufLen = 0;
        }

        public void CheckTimeout()
        {
            if (Timeout > 0)
            {
                if (Timer.ElapsedMilliseconds > Timeout)
                    throw new TimeoutException();
            }
        }

        private void ReadBuffer()
        {
            if (_bufPos + 1 < _bufLen)
                throw new InvalidOperationException("Buffer contain data, read it to the end.");

            _bufPos = 0;
            _bufLen = _stream.Read(_buffer, 0, _buffer.Length);

            CheckTimeout();
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

        protected Reply ReadServerReplyRaw()
        {
            string firstLine = ReadLine();
            Match m = _multiLineReply.Match(firstLine);

            if (m.Success)
            {
                int code = int.Parse(m.Groups["code"].Value);

                List<string> raw = new List<string>();
                List<string> parsed = new List<string>();

                raw.Add(firstLine);
                if (m.Groups["msg"] != null)
                    parsed.Add(m.Groups["msg"].Value);

                while (true)
                {
                    string line = ReadLine();
                    raw.Add(line);
                    m = _singleLineReply.Match(line);
                    if (m.Success)
                    {
                        if (m.Groups["msg"] != null)
                            parsed.Add(m.Groups["msg"].Value);
                        if (int.Parse(m.Groups["code"].Value) == code)
                            break;
                    }
                    else
                    {
                        m = _multiLineReply.Match(line);
                        if (!m.Success)
                            throw new InvalidOperationException("Invalid reply");
                        if (m.Groups["code"].Value != code.ToString())
                            throw new InvalidOperationException("Invalid code in multiline reply");
                        if (m.Groups["msg"] != null)
                            parsed.Add(m.Groups["msg"].Value);
                    }
                }
                return new Reply
                {
                    Code = code,
                    Raw = string.Join(Environment.NewLine, raw),
                    Parsed = string.Join(Environment.NewLine, parsed)
                };
            }
            else
            {
                m = _singleLineReply.Match(firstLine);
                if (m.Success)
                    return new Reply
                    {
                        Code = int.Parse(m.Groups["code"].Value),
                        Raw = firstLine,
                        Parsed = m.Groups["msg"]?.Value
                    };
                else
                    throw new InvalidDataException("Incorrect reply format");
            }
        }

        protected class Reply
        {
            public int Code;
            public string Raw;
            public string Parsed;
        }
    }
}