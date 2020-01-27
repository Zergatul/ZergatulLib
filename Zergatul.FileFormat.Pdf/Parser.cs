using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf
{
    using static Common;

    internal class Parser
    {
        private Func<byte> _nextByte;
        private byte _byte;
        private StringBuilder _sb;
        private List<byte> _list;

        public Parser(Func<byte> nextByte)
        {
            if (nextByte == null)
                throw new ArgumentNullException(nameof(nextByte));

            _sb = new StringBuilder();
            _list = new List<byte>();

            _nextByte = nextByte;
        }

        public void Reset()
        {
            _byte = _nextByte();
        }

        public TokenBase NextToken()
        {
            bool comment = false;

            while (true)
            {
                if (comment)
                {
                    if (IsEndOfLine(_byte))
                        comment = false;
                }
                else
                {
                    if (_byte == '%')
                        comment = true;
                    if (!IsWhiteSpace(_byte))
                        break;
                }

                _byte = _nextByte();
            }

            switch (_byte)
            {
                case (byte)'0':
                case (byte)'1':
                case (byte)'2':
                case (byte)'3':
                case (byte)'4':
                case (byte)'5':
                case (byte)'6':
                case (byte)'7':
                case (byte)'8':
                case (byte)'9':
                case (byte)'.':
                case (byte)'+':
                case (byte)'-':
                    return ReadNumber();

                case (byte)'(':
                    return ReadString();

                case (byte)'/':
                    return ReadName();

                case (byte)'[':
                    return ReadArray();

                case (byte)'<':
                    _byte = _nextByte();
                    if (_byte == '<')
                        return ReadDictionary();
                    else
                        return ReadHexString();

                default:
                    return ReadStaticToken();
            }
        }

        private TokenBase ReadArray()
        {
            throw new NotImplementedException();
        }

        private TokenBase ReadDictionary()
        {
            _byte = _nextByte();

            var dictionary = new Dictionary<string, TokenBase>();
            TokenBase token, nextToken = null, nextNextToken = null;
            while (true)
            {
                token = NextToken();
                var name = token as NameToken;
                if (name == null)
                    throw new InvalidDataException("Dictionary parsing error. Name token expected.");

                token = NextToken();
                var 
                bool isNull = token == NullToken.Instance;

                if (dictionary.ContainsKey(name.Value))
                {
                    if (isNull)
                        dictionary.Remove(name.Value);
                    else
                        dictionary[name.Value] = token;
                }
                else
                {
                    if (!isNull)
                        dictionary.Add(name.Value, token);
                }
            }
        }

        private TokenBase ReadHexString()
        {
            throw new NotImplementedException();
        }

        private TokenBase ReadName()
        {
            _list.Clear();
            while (true)
            {
                _byte = _nextByte();
                if (IsWhiteSpace(_byte))
                    break;

                if (_byte == '#')
                {
                    _byte = _nextByte();
                    if (!IsHexDigit(_byte))
                        throw new InvalidDataException("Hex symbol expected in name token");
                    int value = ParseHex(_byte) << 4;

                    _byte = _nextByte();
                    if (!IsHexDigit(_byte))
                        throw new InvalidDataException("Hex symbol expected in name token");
                    value |= ParseHex(_byte);

                    _list.Add((byte)value);
                }
                else
                {
                    _list.Add(_byte);
                }

                if (_list.Count >= 128)
                    throw new ImplementationLimitException("Name token too long");
            }

            return new NameToken(Encoding.UTF8.GetString(_list.ToArray()));
        }

        private TokenBase ReadNumber()
        {
            int sign = 1;

            if (_byte == '+')
                _byte = _nextByte();

            if (_byte == '-')
            {
                sign = -1;
                _byte = _nextByte();
            }

            bool point = false;

            _sb.Clear();
            while (true)
            {
                if (IsWhiteSpace(_byte))
                {
                    _byte = _nextByte();
                    break;
                }

                if (_byte == '.')
                {
                    point = true;
                    _byte = _nextByte();
                    break;
                }

                if (!IsDigit(_byte))
                    throw new InvalidDataException("Invalid symbol in number token.");

                _sb.Append((char)_byte);

                _byte = _nextByte();

                if (_sb.Length > 16)
                    throw new InvalidDataException("Integer part of number token too long.");
            }

            string integer = _sb.ToString();

            _sb.Clear();
            if (point)
            {
                while (true)
                {
                    if (IsWhiteSpace(_byte))
                    {
                        _byte = _nextByte();
                        break;
                    }

                    if (!IsDigit(_byte))
                        throw new InvalidDataException("Invalid symbol in number token.");

                    _sb.Append((char)_byte);

                    _byte = _nextByte();

                    if (_sb.Length > 16)
                        throw new InvalidDataException("Decimals part of number token too long.");
                }
            }

            string decimals = _sb.ToString();

            if (IsZeroString(decimals))
                return new IntegerToken(sign * long.Parse(integer));
            else
                return new RealToken(sign * decimal.Parse($"{integer}.{decimals}"));
        }

        private TokenBase ReadStaticToken()
        {
            _sb.Clear();

            while (true)
            {
                if (_byte >= 0x80)
                    throw new InvalidDataException("Non-ASCII character in static token.");

                _sb.Append((char)_byte);

                _byte = _nextByte();

                if (IsWhiteSpace(_byte))
                    break;

                if (_sb.Length > 128)
                    throw new InvalidDataException("Static token too long.");
            }

            string value = _sb.ToString();
            switch (value)
            {
                case "false":
                    return BooleanToken.False;

                case "true":
                    return BooleanToken.True;

                case "null":
                    return NullToken.Instance;

                default:
                    return new StaticToken(value);
            }
        }

        private TokenBase ReadString()
        {
            throw new NotImplementedException();
        }
    }
}