using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.IO
{
    public class Adler32
    {
        private uint _s1, _s2;

        public Adler32()
        {
            Reset();
        }

        public void Reset()
        {
            _s1 = 1;
            _s2 = 0;
        }
        public void Update(byte[] buffer, int offset, int count)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return;

            while (count > 0)
            {
                _s1 = (_s1 + buffer[offset]) % 65521;
                _s2 = (_s2 + _s1) % 65521;
                offset++;
                count--;
            }
        }

        public void Update(byte[] buffer) => Update(buffer, 0, buffer.Length);

        public uint GetCheckSum() => (_s2 << 16) + _s1;
    }
}