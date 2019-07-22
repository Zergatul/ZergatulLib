using System;
using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Haval256 : Security.MessageDigest
    {
        public override int BlockLength => 128;
        public override int DigestLength => 32;

        private const int Passes = 5;

        protected byte[] buffer;
        protected int bufOffset;
        protected uint s0, s1, s2, s3, s4, s5, s6, s7;
        protected ulong count;

        public Haval256()
        {
            buffer = new byte[128];
            Reset();
        }

        public override void Reset()
        {
            s0 = 0x243F6A88;
            s1 = 0x85A308D3;
            s2 = 0x13198A2E;
            s3 = 0x03707344;
            s4 = 0xA4093822;
            s5 = 0x299F31D0;
            s6 = 0x082EFA98;
            s7 = 0xEC4E6C89;

            bufOffset = 0;
            count = 0;
        }

        public override void Update(byte[] data, int offset, int length)
        {
            throw new NotImplementedException();
        }

        public override byte[] Digest()
        {
            buffer[bufOffset++] = 0x01;

            if (bufOffset > 118)
            {
                throw new NotImplementedException();
            }

            while (bufOffset < 118)
                buffer[bufOffset++] = 0;

            buffer[118] = 0x01 | (Passes << 3);
            buffer[119] = 0x40;

            GetBytes(count, ByteOrder.LittleEndian, buffer, 120);

            ProcessBlock();
        }

        private void ProcessBlock()
        {

        }
    }
}