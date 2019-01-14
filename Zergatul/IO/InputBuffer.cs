using System;

namespace Zergatul.IO
{
    public class InputBuffer
    {
        public int BufferSize { get; }
        public int TotalBits { get; private set; }

        private int _bitBuffer;
        private int _bitLength;

        private byte[] _readBuffer;
        private int _readBufferStart;
        private int _readBufferEnd;

        public InputBuffer(int bufferSize)
        {
            BufferSize = bufferSize;
            _readBuffer = new byte[bufferSize * 2];
        }

        public void Add(byte[] buffer, int offset, int count)
        {
            if (_readBuffer.Length - (_readBufferEnd - _readBufferStart) < count)
                throw new InvalidOperationException("No free space");

            if (_readBufferStart == _readBufferEnd)
            {
                _readBufferStart = 0;
                _readBufferEnd = 0;
            }
            else if (_readBufferStart + count > _readBuffer.Length)
            {
                Buffer.BlockCopy(_readBuffer, _readBufferStart, _readBuffer, 0, _readBufferEnd - _readBufferStart);
                _readBufferEnd -= _readBufferStart;
                _readBufferStart = 0;
            }

            Buffer.BlockCopy(buffer, offset, _readBuffer, _readBufferEnd, count);
            _readBufferEnd += count;
            TotalBits += count << 3;
        }

        public int Peek(int bits, out int read)
        {
            int result = _bitBuffer;
            int index = _readBufferStart;
            read = _bitLength;
            while (read < bits)
            {
                if (index < _readBufferEnd)
                {
                    result |= _readBuffer[index++] << read;
                    read += 8;
                }
                else
                    break;
            }

            if (read > bits)
            {
                read = bits;
                result = result & ((1 << bits) - 1);
            }

            return result;
        }

        public int ReadBits(int bits)
        {
            while (_bitLength < bits)
            {
                _bitBuffer |= _readBuffer[_readBufferStart++] << _bitLength;
                _bitLength += 8;
            }

            int result = _bitBuffer & ((1 << bits) - 1);
            _bitBuffer >>= bits;
            _bitLength -= bits;
            TotalBits -= bits;
            return result;
        }

        public byte ReadRawByte()
        {
            TotalBits -= 8;
            return _readBuffer[_readBufferStart++];
        }

        public int ReadRawInt16()
        {
            int result = _readBuffer[_readBufferStart++] | (_readBuffer[_readBufferStart++] << 8);
            TotalBits -= 16;
            return result;
        }

        public int ReadRawInt32()
        {
            int result =
                _readBuffer[_readBufferStart++] |
                (_readBuffer[_readBufferStart++] << 8) |
                (_readBuffer[_readBufferStart++] << 16) |
                (_readBuffer[_readBufferStart++] << 24);
            TotalBits -= 32;
            return result;
        }

        public void SkipTillByteBoundary()
        {
            TotalBits -= _bitLength;
            _bitBuffer = 0;
            _bitLength = 0;
        }

        public void SkipBits(int bits)
        {
            if (bits == 0)
                return;
            if (TotalBits < bits)
                throw new InvalidOperationException();

            TotalBits -= bits;

            if (_bitLength > 0)
            {
                if (bits >= _bitLength)
                {
                    bits -= _bitLength;
                    _bitBuffer = 0;
                    _bitLength = 0;
                }
                else
                {
                    _bitBuffer >>= bits;
                    _bitLength -= bits;
                    return;
                }
            }

            _readBufferStart += bits >> 3;
            bits &= 0x07;

            if (bits > 0)
            {
                _bitBuffer = _readBuffer[_readBufferStart++] >> bits;
                _bitLength = 8 - bits;
            }
        }
    }
}