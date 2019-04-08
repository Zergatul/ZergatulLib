using System;
using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    abstract class CubeHash : Security.MessageDigest
    {
        public override int BlockLength => 32;

        protected byte[] buffer;
        protected int bufOffset;

        protected uint
            s00, s01, s02, s03, s04, s05, s06, s07, s08, s09, s0a, s0b, s0c, s0d, s0e, s0f,
            s10, s11, s12, s13, s14, s15, s16, s17, s18, s19, s1a, s1b, s1c, s1d, s1e, s1f;

        public CubeHash()
        {
            buffer = new byte[32];
            Reset();
        }

        public override void Update(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            while (length > 0)
            {
                int copy = System.Math.Min(buffer.Length - bufOffset, length);
                Buffer.BlockCopy(data, offset, buffer, bufOffset, copy);

                offset += copy;
                length -= copy;
                bufOffset += copy;

                if (bufOffset == buffer.Length)
                {
                    bufOffset = 0;
                    ProcessBlock();
                }
            }
        }

        private void ProcessBlock()
        {
            s00 ^= ToUInt32(buffer, 0x00, ByteOrder.LittleEndian);
            s01 ^= ToUInt32(buffer, 0x04, ByteOrder.LittleEndian);
            s02 ^= ToUInt32(buffer, 0x08, ByteOrder.LittleEndian);
            s03 ^= ToUInt32(buffer, 0x0C, ByteOrder.LittleEndian);
            s04 ^= ToUInt32(buffer, 0x10, ByteOrder.LittleEndian);
            s05 ^= ToUInt32(buffer, 0x14, ByteOrder.LittleEndian);
            s06 ^= ToUInt32(buffer, 0x18, ByteOrder.LittleEndian);
            s07 ^= ToUInt32(buffer, 0x1C, ByteOrder.LittleEndian);

            for (int i = 0; i < 8; i++)
                TwoRounds();
        }

        protected void FinalProcess()
        {
            buffer[bufOffset++] = 0x80;
            while (bufOffset < 32)
                buffer[bufOffset++] = 0;

            ProcessBlock();

            s1f ^= 1;
            for (int i = 0; i < 80; i++)
                TwoRounds();
        }

        private void TwoRounds()
        {
            s10 += s00;
            s11 += s01;
            s12 += s02;
            s13 += s03;
            s14 += s04;
            s15 += s05;
            s16 += s06;
            s17 += s07;
            s18 += s08;
            s19 += s09;
            s1a += s0a;
            s1b += s0b;
            s1c += s0c;
            s1d += s0d;
            s1e += s0e;
            s1f += s0f;
            s00 = RotateLeft(s00, 7);
            s01 = RotateLeft(s01, 7);
            s02 = RotateLeft(s02, 7);
            s03 = RotateLeft(s03, 7);
            s04 = RotateLeft(s04, 7);
            s05 = RotateLeft(s05, 7);
            s06 = RotateLeft(s06, 7);
            s07 = RotateLeft(s07, 7);
            s08 = RotateLeft(s08, 7);
            s09 = RotateLeft(s09, 7);
            s0a = RotateLeft(s0a, 7);
            s0b = RotateLeft(s0b, 7);
            s0c = RotateLeft(s0c, 7);
            s0d = RotateLeft(s0d, 7);
            s0e = RotateLeft(s0e, 7);
            s0f = RotateLeft(s0f, 7);
            s08 ^= s10;
            s09 ^= s11;
            s0a ^= s12;
            s0b ^= s13;
            s0c ^= s14;
            s0d ^= s15;
            s0e ^= s16;
            s0f ^= s17;
            s00 ^= s18;
            s01 ^= s19;
            s02 ^= s1a;
            s03 ^= s1b;
            s04 ^= s1c;
            s05 ^= s1d;
            s06 ^= s1e;
            s07 ^= s1f;
            s12 += s08;
            s13 += s09;
            s10 += s0a;
            s11 += s0b;
            s16 += s0c;
            s17 += s0d;
            s14 += s0e;
            s15 += s0f;
            s1a += s00;
            s1b += s01;
            s18 += s02;
            s19 += s03;
            s1e += s04;
            s1f += s05;
            s1c += s06;
            s1d += s07;
            s08 = RotateLeft(s08, 11);
            s09 = RotateLeft(s09, 11);
            s0a = RotateLeft(s0a, 11);
            s0b = RotateLeft(s0b, 11);
            s0c = RotateLeft(s0c, 11);
            s0d = RotateLeft(s0d, 11);
            s0e = RotateLeft(s0e, 11);
            s0f = RotateLeft(s0f, 11);
            s00 = RotateLeft(s00, 11);
            s01 = RotateLeft(s01, 11);
            s02 = RotateLeft(s02, 11);
            s03 = RotateLeft(s03, 11);
            s04 = RotateLeft(s04, 11);
            s05 = RotateLeft(s05, 11);
            s06 = RotateLeft(s06, 11);
            s07 = RotateLeft(s07, 11);
            s0c ^= s12;
            s0d ^= s13;
            s0e ^= s10;
            s0f ^= s11;
            s08 ^= s16;
            s09 ^= s17;
            s0a ^= s14;
            s0b ^= s15;
            s04 ^= s1a;
            s05 ^= s1b;
            s06 ^= s18;
            s07 ^= s19;
            s00 ^= s1e;
            s01 ^= s1f;
            s02 ^= s1c;
            s03 ^= s1d;
            s13 += s0c;
            s12 += s0d;
            s11 += s0e;
            s10 += s0f;
            s17 += s08;
            s16 += s09;
            s15 += s0a;
            s14 += s0b;
            s1b += s04;
            s1a += s05;
            s19 += s06;
            s18 += s07;
            s1f += s00;
            s1e += s01;
            s1d += s02;
            s1c += s03;
            s0c = RotateLeft(s0c, 7);
            s0d = RotateLeft(s0d, 7);
            s0e = RotateLeft(s0e, 7);
            s0f = RotateLeft(s0f, 7);
            s08 = RotateLeft(s08, 7);
            s09 = RotateLeft(s09, 7);
            s0a = RotateLeft(s0a, 7);
            s0b = RotateLeft(s0b, 7);
            s04 = RotateLeft(s04, 7);
            s05 = RotateLeft(s05, 7);
            s06 = RotateLeft(s06, 7);
            s07 = RotateLeft(s07, 7);
            s00 = RotateLeft(s00, 7);
            s01 = RotateLeft(s01, 7);
            s02 = RotateLeft(s02, 7);
            s03 = RotateLeft(s03, 7);
            s04 ^= s13;
            s05 ^= s12;
            s06 ^= s11;
            s07 ^= s10;
            s00 ^= s17;
            s01 ^= s16;
            s02 ^= s15;
            s03 ^= s14;
            s0c ^= s1b;
            s0d ^= s1a;
            s0e ^= s19;
            s0f ^= s18;
            s08 ^= s1f;
            s09 ^= s1e;
            s0a ^= s1d;
            s0b ^= s1c;
            s11 += s04;
            s10 += s05;
            s13 += s06;
            s12 += s07;
            s15 += s00;
            s14 += s01;
            s17 += s02;
            s16 += s03;
            s19 += s0c;
            s18 += s0d;
            s1b += s0e;
            s1a += s0f;
            s1d += s08;
            s1c += s09;
            s1f += s0a;
            s1e += s0b;
            s04 = RotateLeft(s04, 11);
            s05 = RotateLeft(s05, 11);
            s06 = RotateLeft(s06, 11);
            s07 = RotateLeft(s07, 11);
            s00 = RotateLeft(s00, 11);
            s01 = RotateLeft(s01, 11);
            s02 = RotateLeft(s02, 11);
            s03 = RotateLeft(s03, 11);
            s0c = RotateLeft(s0c, 11);
            s0d = RotateLeft(s0d, 11);
            s0e = RotateLeft(s0e, 11);
            s0f = RotateLeft(s0f, 11);
            s08 = RotateLeft(s08, 11);
            s09 = RotateLeft(s09, 11);
            s0a = RotateLeft(s0a, 11);
            s0b = RotateLeft(s0b, 11);
            s00 ^= s11;
            s01 ^= s10;
            s02 ^= s13;
            s03 ^= s12;
            s04 ^= s15;
            s05 ^= s14;
            s06 ^= s17;
            s07 ^= s16;
            s08 ^= s19;
            s09 ^= s18;
            s0a ^= s1b;
            s0b ^= s1a;
            s0c ^= s1d;
            s0d ^= s1c;
            s0e ^= s1f;
            s0f ^= s1e;
        }
    }
}