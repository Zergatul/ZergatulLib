using System;

namespace Zergatul.IO
{
    public class CRC32
    {
        private CRC32Parameters _parameters;
        private uint _sum;

        public CRC32(CRC32Parameters parameters)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            Reset();
        }

        public void Reset()
        {
            _sum = _parameters.Init;
        }

        public void Update(byte[] buffer, int offset, int count)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return;

            while (count > 0)
            {
                _sum = _parameters.Table[(_sum ^ buffer[offset]) & 0xFF] ^ (_sum >> 8);
                offset++;
                count--;
            }
        }

        public void Update(byte[] buffer) => Update(buffer, 0, buffer.Length);

        public uint GetCheckSum() => _sum ^ _parameters.Final;
    }

    public class CRC32Parameters
    {
        public uint[] Table { get; }
        public uint Init { get; }
        public uint Final { get; }

        public CRC32Parameters(uint polynom, uint init, uint final)
        {
            Table = new uint[256];
            Init = init;
            Final = final;

            for (uint n = 0; n < 256; n++)
            {
                uint c = n;
                for (int k = 0; k < 8; k++)
                {
                    if ((c & 1) != 0)
                        c = polynom ^ (c >> 1);
                    else
                        c >>= 1;
                }
                Table[n] = c;
            }
        }

        public static CRC32Parameters IEEE8023 { get; }

        static CRC32Parameters()
        {
            IEEE8023 = new CRC32Parameters(0xEDB88320, 0xFFFFFFFF, 0xFFFFFFFF);
        }
    }
}