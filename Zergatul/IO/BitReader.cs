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
            if (bits < 0 || bits > 24)
                throw new ArgumentOutOfRangeException(nameof(bits), "Bits must be >= 0 and <= 24");

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

            int result = (int)(_buffer & ((1 << bits) - 1));
            _buffer >>= bits;
            _bufferLengthBits -= bits;

            return result;
        }

        /// <summary>
        /// Reads [0..8) bits
        /// </summary>
        public int ReadTillByteBoundary()
        {
            int bits = _bufferLengthBits & 0x07;
            return ReadBits(bits);
        }
    }
}