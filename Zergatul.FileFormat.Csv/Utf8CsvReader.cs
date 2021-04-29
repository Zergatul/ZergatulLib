using System;
using System.IO;

namespace Zergatul.FileFormat.Csv
{
    public class Utf8CsvReader
    {
        public long BytesConsumed => _consumed + _bufPos;
        public long LineNumber => _lineNumber + 1;
        public long CharNumber => _charNumber + 1 + _bufPos - _lineStart;
        public CsvTokenType TokenType { get; private set; }
        public Memory<byte> Value { get; private set; }
        public bool IsFinished { get; private set; }

        private long _consumed;
        private long _lineNumber;
        private long _charNumber;
        private long _lineStart;
        private Memory<byte> _buffer;
        private int _bufPos;
        private State _state;
        private byte[] _resBuf;
        private int _resBufPos;

        #region Public members

        public void AddData(Memory<byte> buffer)
        {
            if (_bufPos < _buffer.Length)
                throw new InvalidOperationException("Buffer not consumed.");

            _consumed += _bufPos;
            _charNumber += _bufPos - _lineStart;

            _buffer = buffer;
            _bufPos = 0;
            _lineStart = 0;
        }

        public bool End()
        {
            IsFinished = true;

            switch (_state)
            {
                case State.DataExpected:
                    return false;

                case State.CarriageReturn:
                    TokenType = CsvTokenType.EndOfLine;
                    Value = _resBuf.AsMemory(0, 1);
                    return true;

                case State.EndOfLine:
                    throw new InvalidOperationException();

                case State.NonQuotedData:
                    TokenType = CsvTokenType.Data;
                    Value = _resBuf.AsMemory(0, _resBufPos);
                    return true;

                case State.QuotedData:
                    throw new InvalidDataException();

                case State.InnerQuote:
                    TokenType = CsvTokenType.Data;
                    Value = _resBuf.AsMemory(0, _resBufPos);
                    return true;

                default:
                    throw new NotImplementedException();
            }
        }

        public bool Read()
        {
            var span = _buffer.Span;
            while (true)
            {
                if (_bufPos == span.Length)
                    return false;

                byte ch;
                switch (_state)
                {
                    case State.DataExpected:
                        ch = span[_bufPos++];
                        if (ch == 0x0D)
                        {
                            _state = State.CarriageReturn;
                            EnsureResultBufferCreated();
                            _resBuf[0] = ch;
                            _resBufPos = 1;
                            continue;
                        }
                        if (ch == 0x0A)
                        {
                            _state = State.DataExpected;
                            TokenType = CsvTokenType.EndOfLine;
                            EnsureResultBufferCreated();
                            _resBuf[0] = ch;
                            Value = _resBuf.AsMemory(0, 1);
                            _lineNumber++;
                            _charNumber = 0;
                            _lineStart = _bufPos;
                            return true;
                        }
                        if (ch == ',')
                        {
                            _state = State.DataExpected;
                            TokenType = CsvTokenType.Data;
                            Value = Memory<byte>.Empty;
                            return true;
                        }
                        if (ch == '"')
                        {
                            _state = State.QuotedData;
                            EnsureResultBufferCreated();
                            _resBufPos = 0;
                        }
                        else
                        {
                            _state = State.NonQuotedData;
                            EnsureResultBufferCreated();
                            _resBufPos = 0;
                            _bufPos--;
                        }
                        break;

                    case State.CarriageReturn:
                        ch = span[_bufPos++];
                        if (ch == 0x0A)
                            _resBuf[_resBufPos++] = ch;
                        else
                            _bufPos--;

                        _state = State.DataExpected;
                        TokenType = CsvTokenType.EndOfLine;
                        Value = _resBuf.AsMemory(0, _resBufPos);
                        _lineNumber++;
                        _charNumber = 0;
                        _lineStart = _bufPos;
                        return true;

                    case State.NonQuotedData:
                        while (_bufPos < span.Length)
                        {
                            ch = span[_bufPos++];
                            if (ch == 0x0A || ch == 0x0D)
                            {
                                _bufPos--;
                                _state = State.EndOfLine;
                                TokenType = CsvTokenType.Data;
                                Value = _resBuf.AsMemory(0, _resBufPos);
                                return true;
                            }
                            if (ch == ',')
                            {
                                _state = State.DataExpected;
                                TokenType = CsvTokenType.Data;
                                Value = _resBuf.AsMemory(0, _resBufPos);
                                return true;
                            }
                            AppendResultBuffer(ch);
                        }
                        break;

                    case State.QuotedData:
                        while (_bufPos < span.Length)
                        {
                            ch = span[_bufPos++];
                            if (ch == '"')
                            {
                                _state = State.InnerQuote;
                                break;
                            }
                            else
                            {
                                AppendResultBuffer(ch);
                            }
                        }
                        break;

                    case State.InnerQuote:
                        ch = span[_bufPos++];
                        if (ch == '"')
                        {
                            _state = State.QuotedData;
                            AppendResultBuffer((byte)'"');
                            goto case State.QuotedData;
                        }
                        else
                        {
                            if (ch == 0x0A || ch == 0x0D)
                            {
                                _bufPos--;
                                _state = State.EndOfLine;
                                TokenType = CsvTokenType.Data;
                                Value = _resBuf.AsMemory(0, _resBufPos);
                                return true;
                            }
                            if (ch == ',')
                            {
                                _state = State.DataExpected;
                                TokenType = CsvTokenType.Data;
                                Value = _resBuf.AsMemory(0, _resBufPos);
                                return true;
                            }
                            throw new InvalidDataException();
                        }

                    case State.EndOfLine:
                        ch = span[_bufPos++];
                        if (ch == 0x0D)
                        {
                            _state = State.CarriageReturn;
                            EnsureResultBufferCreated();
                            _resBuf[0] = ch;
                            _resBufPos = 1;
                            continue;
                        }
                        if (ch == 0x0A)
                        {
                            _state = State.DataExpected;
                            TokenType = CsvTokenType.EndOfLine;
                            EnsureResultBufferCreated();
                            _resBuf[0] = ch;
                            _resBufPos = 1;
                            Value = _resBuf.AsMemory(0, 1);
                            _lineNumber++;
                            _charNumber = 0;
                            _lineStart = _bufPos;
                            return true;
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        #region Private members

        private void EnsureResultBufferCreated()
        {
            if (_resBuf == null)
                _resBuf = new byte[256];
        }

        private void AppendResultBuffer(byte value)
        {
            if (_resBufPos == _resBuf.Length)
            {
                var buffer = new byte[_resBuf.Length * 2];
                Array.Copy(_resBuf, buffer, _resBufPos);
                _resBuf = buffer;
            }

            _resBuf[_resBufPos++] = value;
        }

        #endregion

        #region Nested classes

        private enum State
        {
            DataExpected,
            EndOfLine,
            CarriageReturn,
            QuotedData,
            NonQuotedData,
            InnerQuote
        }

        #endregion
    }
}