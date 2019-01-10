using System;
using System.IO;

namespace Zergatul.IO
{
    public class BitReader
    {
        private Stream _stream;
        private byte[] _readBuffer;
        private long _buffer;
        private int _bufferLengthBits;

        public BitReader(Stream stream)
        {
            _stream = stream;
            _readBuffer = new byte[4];
            _buffer = 0;
            _bufferLengthBits = 0;
        }

        public int ReadBits(int bits)
        {
            if (bits < 0 || bits > 32)
                throw new ArgumentOutOfRangeException(nameof(bits), "Bits must be >= 0 and <= 32");

            while (_bufferLengthBits < bits)
            {
                int read = _stream.Read(_readBuffer, 0, _readBuffer.Length);
                if (read == 0)
                    throw new EndOfStreamException();

                for (int i = 0; i < read; i++)
                {
                    _buffer |= (long)_readBuffer[i] << _bufferLengthBits;
                    _bufferLengthBits += 8;
                }
            }

            int result = (int)(_buffer & ((1L << bits) - 1));
            _buffer >>= bits;
            _bufferLengthBits -= bits;

            return result;
        }

        public void Peek(int bits, out int value, out int read)
        {
            if (bits < 0 || bits > 32)
                throw new ArgumentOutOfRangeException(nameof(bits), "Bits must be >= 0 and <= 32");

            while (_bufferLengthBits < bits)
            {
                int readBytes = _stream.Read(_readBuffer, 0, _readBuffer.Length);
                if (readBytes == 0)
                    break;

                for (int i = 0; i < readBytes; i++)
                {
                    _buffer |= (long)_readBuffer[i] << _bufferLengthBits;
                    _bufferLengthBits += 8;
                }
            }

            value = (int)(_buffer & ((1L << bits) - 1));
            read = System.Math.Min(_bufferLengthBits, bits);
        }

        public void RemoveBits(int bits)
        {
            if (bits < 0 || bits > _bufferLengthBits)
                throw new ArgumentOutOfRangeException(nameof(bits));

            _buffer >>= bits;
            _bufferLengthBits -= bits;
        }

        /// <summary>
        /// Reads [0..8) bits
        /// </summary>
        public int ReadTillByteBoundary()
        {
            int bits = _bufferLengthBits & 0x07;
            return ReadBits(bits);
        }

        public int GetBufferedBytesCount()
        {
            if ((_bufferLengthBits & 0x07) != 0)
                throw new InvalidOperationException();
            return _bufferLengthBits >> 3;
        }

        public int ReadBytes(byte[] buffer, int offset, int count)
        {
            if ((_bufferLengthBits & 0x07) != 0)
                throw new InvalidOperationException();

            int bufferCopy = System.Math.Min(_bufferLengthBits >> 3, count);
            for (int i = 0; i < bufferCopy; i++)
            {
                buffer[offset++] = (byte)(_buffer & 0xFF);
                _buffer >>= 8;
            }
            _bufferLengthBits -= bufferCopy << 3;
            count -= bufferCopy;
            if (count == 0)
                return bufferCopy;

            int read = _stream.Read(buffer, offset, count);
            return bufferCopy + read;
        }
    }
}