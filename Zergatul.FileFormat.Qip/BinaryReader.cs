using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Zergatul.FileFormat.Qip
{
    internal class BinaryReader
    {
        private Stream _stream;
        private byte[] _buffer = new byte[1024];
        private StringBuilder _sb = new StringBuilder();
        private int _bufLen;
        private int _bufPos;

        public BinaryReader(Stream stream)
        {
            _stream = stream;
            _bufLen = 0;
            _bufPos = 0;
        }

        public string ReadAnsiString(int length)
        {
            if (length == 0)
                return string.Empty;

            _sb.Clear();
            while (_sb.Length < length)
            {
                if (_bufPos >= _bufLen)
                    ReadNext();

                _sb.Append((char)_buffer[_bufPos++]);
            }

            return _sb.ToString();
        }

        public string ReadUtf8String(int length)
        {
            if (length == 0)
                return string.Empty;

            return Encoding.UTF8.GetString(ReadBytes(length));
        }

        public int ReadByte()
        {
            if (_bufPos >= _bufLen)
                ReadNext();

            return _buffer[_bufPos++];
        }

        public int ReadWord()
        {
            int result = 0;
            for (int i = 0; i < 2; i++)
            {
                if (_bufPos >= _bufLen)
                    ReadNext();
                result = (result << 8) | _buffer[_bufPos++];
            }
            return result;
        }

        public uint ReadDWord()
        {
            uint result = 0;
            for (int i = 0; i < 4; i++)
            {
                if (_bufPos >= _bufLen)
                    ReadNext();
                result = (result << 8) | _buffer[_bufPos++];
            }
            return result;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] result = new byte[length];
            int pos = 0;
            while (pos < length)
            {
                if (_bufPos >= _bufLen)
                    ReadNext();
                int copy = Math.Min(length - pos, _bufLen - _bufPos);
                Array.Copy(_buffer, _bufPos, result, pos, copy);
                pos += copy;
                _bufPos += copy;
            }
            return result;
        }

        private void ReadNext(bool required = true)
        {
            _bufPos = 0;
            _bufLen = _stream.Read(_buffer, 0, _buffer.Length);

            if (required)
                ThrowIfEmpty();
        }

        private void ThrowIfEmpty()
        {
            if (_bufLen == 0)
                throw new EndOfStreamException();
        }
    }
}