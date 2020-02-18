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
        public long Position { get; private set; } = 0;

        private Func<int> _nextByte;
        private int _byte;
        private StringBuilder _sb = new StringBuilder();
        private List<byte> _list = new List<byte>();
        private TokenBase _token1, _token2;

        public Parser(Func<int> nextByte)
        {
            if (nextByte == null)
                throw new ArgumentNullException(nameof(nextByte));

            _nextByte = nextByte;
        }

        public Parser(byte[] data, long offset, long endOffset)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _nextByte = () =>
            {
                if (offset >= endOffset)
                    throw new InvalidDataException("End of parser data.");

                return data[offset++];
            };
        }

        public void Reset(long position)
        {
            Position = position - 1;
            MoveNext();
            _token1 = null;
            _token2 = null;
        }

        public TokenBase NextToken(bool allowObj = false)
        {
            TokenBase token;
            if (_token1 != null)
            {
                token = _token1;
                _token1 = _token2;
                _token2 = null;
            }
            else
            {
                token = ReadSimpleToken();
            }

            // check for indirect object
            // or begin object
            var integer1 = token as IntegerToken;
            if (integer1?.Value >= 0)
            {
                if (_token1 == null)
                    _token1 = ReadSimpleToken();

                var integer2 = _token1 as IntegerToken;
                if (0 <= integer2?.Value && integer2?.Value <= MaxGeneration)
                {
                    if (_token2 == null)
                        _token2 = ReadSimpleToken();

                    var @static = _token2 as StaticToken;
                    if (@static?.Value == "R")
                    {
                        _token1 = null;
                        _token2 = null;
                        return new IndirectReferenceToken(integer1.Value, (int)integer2.Value);
                    }
                    if (allowObj && @static?.Value == "obj")
                    {
                        _token1 = null;
                        _token2 = null;
                        return new BeginObjectToken(integer1.Value, (int)integer2.Value);
                    }
                }
            }

            return token;
        }

        public void SkipStrongRuleEndOfLine()
        {
            if (_byte == 0x0A)
            {
                MoveNext();
                return;
            }

            if (_byte == 0x0D)
            {
                MoveNext();
                if (_byte == 0x0A)
                {
                    MoveNext();
                    return;
                }
            }

            throw new InvalidDataException();
        }

        private TokenBase ReadSimpleToken()
        {
            bool comment = false;
            bool readingComment = false;

            while (true)
            {
                if (comment)
                {
                    if (IsEndOfLine(_byte) || _byte == -1)
                    {
                        comment = false;
                        if (readingComment && _sb.ToString() == "%%EOF")
                            return EndOfFileToken.Marker;
                        if (_byte == -1)
                            return EndOfFileToken.Unexpected;
                    }
                    else if (_byte < 0x80 && _sb.Length < 5)
                    {
                        _sb.Append((char)_byte);
                    }
                    else
                    {
                        readingComment = false;
                    }
                }
                else
                {
                    if (_byte == '%')
                    {
                        comment = true;
                        readingComment = true;
                        _sb.Clear();
                        _sb.Append((char)_byte);
                    }
                    else
                    {
                        if (!IsWhiteSpace(_byte))
                            break;
                    }
                }

                MoveNext();
            }

            switch (_byte)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '.':
                case '+':
                case '-':
                    return ReadNumber();

                case '(':
                    MoveNext();
                    return ReadString();

                case '/':
                    MoveNext();
                    return ReadName();

                case '[':
                    MoveNext();
                    return ReadArray();

                case ']':
                    MoveNext();
                    return EndOfArrayToken.Instance;

                case '<':
                    MoveNext();
                    if (_byte == '<')
                    {
                        MoveNext();
                        return ReadDictionary();
                    }
                    else
                    {
                        return ReadHexString();
                    }

                case '>':
                    MoveNext();
                    if (_byte == '>')
                    {
                        MoveNext();
                        return EndOfDictionaryToken.Instance;
                    }
                    else
                    {
                        return EndOfHexStringToken.Instance;
                    }

                default:
                    return ReadStaticToken();
            }
        }

        private TokenBase ReadArray()
        {
            var list = new List<TokenBase>();
            while (true)
            {
                var token = NextToken();
                if (token == EndOfArrayToken.Instance)
                    break;

                if (!token.IsBasic)
                    throw new InvalidDataException("Unexpected special token in array.");

                list.Add(token);
            }
            return new ArrayToken(list);
        }

        private TokenBase ReadDictionary()
        {
            var dictionary = new Dictionary<string, TokenBase>();
            TokenBase token;
            while (true)
            {
                token = NextToken();

                var name = token as NameToken;
                if (name == null)
                {
                    if (token == EndOfDictionaryToken.Instance)
                        return new DictionaryToken(dictionary);

                    throw new InvalidDataException("Dictionary parsing error. Name token expected.");
                }

                token = NextToken();
                if (!token.IsBasic)
                    throw new InvalidDataException("Dictionary parsing error. Unexpected special token.");

                if (dictionary.ContainsKey(name.Value))
                {
                    if (token == NullToken.Instance)
                        dictionary.Remove(name.Value);
                    else
                        dictionary[name.Value] = token;
                }
                else
                {
                    if (token != NullToken.Instance)
                        dictionary.Add(name.Value, token);
                }
            }
        }

        private TokenBase ReadHexString()
        {
            var list = new List<byte>();
            int index = 0;
            int value = 0;
            while (true)
            {
                if (IsHexDigit(_byte))
                {
                    if ((index & 0x01) == 0)
                    {
                        value = ParseHex(_byte) << 4;
                    }
                    else
                    {
                        value |= ParseHex(_byte);
                        list.Add((byte)value);
                    }
                    index++;
                    MoveNext();
                    continue;
                }

                if (_byte == '>')
                {
                    MoveNext();
                    break;
                }

                throw new InvalidDataException("Unexpected symbol in hex string.");
            }

            if ((index & 0x01) == 1)
                list.Add((byte)value);

            return new HexStringToken(list);
        }

        private TokenBase ReadName()
        {
            _list.Clear();
            while (true)
            {
                if (IsWhiteSpace(_byte) || IsSpecial(_byte))
                    break;

                if (_byte == '#')
                {
                    MoveNext();
                    if (!IsHexDigit(_byte))
                        throw new InvalidDataException("Hex symbol expected in name token");
                    int value = ParseHex(_byte) << 4;

                    MoveNext();
                    if (!IsHexDigit(_byte))
                        throw new InvalidDataException("Hex symbol expected in name token");
                    value |= ParseHex(_byte);

                    _list.Add((byte)value);
                }
                else
                {
                    _list.Add((byte)_byte);
                }

                if (_list.Count >= 128)
                    throw new ImplementationLimitException("Name token too long");

                MoveNext();
            }

            return new NameToken(Encoding.UTF8.GetString(_list.ToArray()));
        }

        private TokenBase ReadNumber()
        {
            int sign = 1;

            if (_byte == '+')
                MoveNext();

            if (_byte == '-')
            {
                sign = -1;
                MoveNext();
            }

            bool point = false;

            _sb.Clear();
            while (true)
            {
                if (IsWhiteSpace(_byte))
                {
                    MoveNext();
                    break;
                }

                if (_byte == '.')
                {
                    point = true;
                    MoveNext();
                    break;
                }

                if (IsSpecial(_byte))
                    break;

                if (!IsDigit(_byte))
                    throw new InvalidDataException("Invalid symbol in number token.");

                _sb.Append((char)_byte);

                MoveNext();

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
                        MoveNext();
                        break;
                    }

                    if (IsSpecial(_byte))
                        break;

                    if (!IsDigit(_byte))
                        throw new InvalidDataException("Invalid symbol in number token.");

                    _sb.Append((char)_byte);

                    MoveNext();

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

                MoveNext();

                if (IsWhiteSpace(_byte) || IsSpecial(_byte))
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

        private void MoveNext()
        {
            _byte = _nextByte();
            Position++;
        }
    }
}