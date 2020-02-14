using System;

namespace Zergatul.IO
{
    public class OutputBuffer
    {
        private byte[] _writeBuffer;
        private int _writeBufferPos;
        private int _bitBuffer;
        private int _bitLength;

        public OutputBuffer(int bufferSize)
        {
            _writeBuffer = new byte[bufferSize];
        }

        public void WriteBits(int value, int bits)
        {
            if (bits > 24)
                throw new NotImplementedException();

            _bitBuffer |= (value & ((1 << bits) - 1)) << _bitLength;
            _bitLength += bits;

            while (_bitLength >= 8)
            {
                _writeBuffer[_writeBufferPos++] = (byte)_bitBuffer;
                _bitBuffer >>= 8;
                _bitLength -= 8;
            }
        }

        public byte[] GetBuffer()
        {
            bool extra = _bitLength > 0;
            byte[] buffer = new byte[_writeBufferPos + (extra ? 1 : 0)];
            Array.Copy(_writeBuffer, 0, buffer, 0, _writeBufferPos);
            if (extra)
                buffer[_writeBufferPos] = (byte)_bitBuffer;
            return buffer;
        }
    }
}