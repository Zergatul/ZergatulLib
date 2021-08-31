using System;
using System.IO;
using System.Text;

namespace Zergatul.FileFormat.Zip
{
    internal class BinaryReader
    {
        const int BufferSize = 0x1000;

        private Stream _stream;
        private byte[] _buffer;
        private int _bufPos;
        private int _bufLen;

        public BinaryReader(Stream stream)
        {
            _stream = stream;
            _buffer = new byte[BufferSize];
        }

        public int ReadInt16()
        {
            EnsureBuffer(2);
            return
                (_buffer[_bufPos++] << 00) |
                (_buffer[_bufPos++] << 08);
        }

        public int ReadInt32()
        {
            EnsureBuffer(4);
            return
                (_buffer[_bufPos++] << 00) |
                (_buffer[_bufPos++] << 08) |
                (_buffer[_bufPos++] << 16) |
                (_buffer[_bufPos++] << 24);
        }

        public string ReadString(int length)
        {
            var sb = new StringBuilder(length);
            while (sb.Length < length)
            {
                int use = Math.Min(BufferSize, length - sb.Length);
                EnsureBuffer(use);
                sb.Append(Encoding.ASCII.GetString(_buffer, _bufPos, use));
                _bufPos += use;
            }
            return sb.ToString();
        }

        public byte[] ReadBytes(int length)
        {
            byte[] bytes = new byte[length];
            int pos = 0;

            int copy = Math.Min(length, _bufLen - _bufPos);
            if (copy > 0)
            {
                Array.Copy(_buffer, _bufPos, bytes, pos, copy);
                pos += copy;
                _bufPos += copy;
            }

            while (pos < length)
            {
                copy = length - pos;
                int read = _stream.Read(bytes, pos, copy);
                if (read == 0)
                    throw new EndOfStreamException();
                pos += read;
            }

            return bytes;
        }

        private void EnsureBuffer(int size)
        {
            if (_bufPos + size < _bufLen)
                return;

            if (_bufPos == _bufLen)
            {
                _bufPos = _bufLen = 0;
            }
            else
            {
                Array.Copy(_buffer, _bufPos, _buffer, 0, _bufLen - _bufPos);
                _bufLen = _bufLen - _bufPos;
                _bufPos = 0;
            }

            while (_bufLen < size)
            {
                int read = _stream.Read(_buffer, 0, Math.Max(size - _bufLen, _buffer.Length));
                if (read == 0)
                    throw new EndOfStreamException();
                _bufLen += read;
            }
        }
    }
}