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
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            count += (ulong)length;

            while (offset < length)
            {
                int copy = System.Math.Min(128 - bufOffset, length);
                Buffer.BlockCopy(data, offset, buffer, bufOffset, copy);

                offset += copy;
                length -= copy;
                bufOffset += copy;

                if (bufOffset == 128)
                {
                    bufOffset = 0;
                    ProcessBlock();
                }
            }
        }

        public override byte[] Digest()
        {
            buffer[bufOffset++] = 0x01;

            if (bufOffset > 118)
            {
                while (bufOffset < 128)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
            }

            while (bufOffset < 118)
                buffer[bufOffset++] = 0;

            buffer[118] = 0x01 | (Passes << 3);
            buffer[119] = 0x40;

            GetBytes(count << 3, ByteOrder.LittleEndian, buffer, 120);

            ProcessBlock();

            byte[] digest = new byte[32];
            GetBytes(s0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s1, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(s2, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s3, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(s4, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(s5, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(s6, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(s7, ByteOrder.LittleEndian, digest, 0x1C);
            return digest;
        }

        private void ProcessBlock()
        {
            uint m00 = ToUInt32(buffer, 0x00, ByteOrder.LittleEndian);
            uint m01 = ToUInt32(buffer, 0x04, ByteOrder.LittleEndian);
            uint m02 = ToUInt32(buffer, 0x08, ByteOrder.LittleEndian);
            uint m03 = ToUInt32(buffer, 0x0C, ByteOrder.LittleEndian);
            uint m04 = ToUInt32(buffer, 0x10, ByteOrder.LittleEndian);
            uint m05 = ToUInt32(buffer, 0x14, ByteOrder.LittleEndian);
            uint m06 = ToUInt32(buffer, 0x18, ByteOrder.LittleEndian);
            uint m07 = ToUInt32(buffer, 0x1C, ByteOrder.LittleEndian);
            uint m08 = ToUInt32(buffer, 0x20, ByteOrder.LittleEndian);
            uint m09 = ToUInt32(buffer, 0x24, ByteOrder.LittleEndian);
            uint m0a = ToUInt32(buffer, 0x28, ByteOrder.LittleEndian);
            uint m0b = ToUInt32(buffer, 0x2C, ByteOrder.LittleEndian);
            uint m0c = ToUInt32(buffer, 0x30, ByteOrder.LittleEndian);
            uint m0d = ToUInt32(buffer, 0x34, ByteOrder.LittleEndian);
            uint m0e = ToUInt32(buffer, 0x38, ByteOrder.LittleEndian);
            uint m0f = ToUInt32(buffer, 0x3C, ByteOrder.LittleEndian);
            uint m10 = ToUInt32(buffer, 0x40, ByteOrder.LittleEndian);
            uint m11 = ToUInt32(buffer, 0x44, ByteOrder.LittleEndian);
            uint m12 = ToUInt32(buffer, 0x48, ByteOrder.LittleEndian);
            uint m13 = ToUInt32(buffer, 0x4C, ByteOrder.LittleEndian);
            uint m14 = ToUInt32(buffer, 0x50, ByteOrder.LittleEndian);
            uint m15 = ToUInt32(buffer, 0x54, ByteOrder.LittleEndian);
            uint m16 = ToUInt32(buffer, 0x58, ByteOrder.LittleEndian);
            uint m17 = ToUInt32(buffer, 0x5C, ByteOrder.LittleEndian);
            uint m18 = ToUInt32(buffer, 0x60, ByteOrder.LittleEndian);
            uint m19 = ToUInt32(buffer, 0x64, ByteOrder.LittleEndian);
            uint m1a = ToUInt32(buffer, 0x68, ByteOrder.LittleEndian);
            uint m1b = ToUInt32(buffer, 0x6C, ByteOrder.LittleEndian);
            uint m1c = ToUInt32(buffer, 0x70, ByteOrder.LittleEndian);
            uint m1d = ToUInt32(buffer, 0x74, ByteOrder.LittleEndian);
            uint m1e = ToUInt32(buffer, 0x78, ByteOrder.LittleEndian);
            uint m1f = ToUInt32(buffer, 0x7C, ByteOrder.LittleEndian);

            uint s0 = this.s0;
            uint s1 = this.s1;
            uint s2 = this.s2;
            uint s3 = this.s3;
            uint s4 = this.s4;
            uint s5 = this.s5;
            uint s6 = this.s6;
            uint s7 = this.s7;

            #region Pass 1
            s7 = RotateRight((s2 & (s6 ^ s1)) ^ (s5 & s4) ^ (s0 & s3) ^ s6, 7) + RotateRight(s7, 11) + m00 + 0x00000000;
            s6 = RotateRight((s1 & (s5 ^ s0)) ^ (s4 & s3) ^ (s7 & s2) ^ s5, 7) + RotateRight(s6, 11) + m01 + 0x00000000;
            s5 = RotateRight((s0 & (s4 ^ s7)) ^ (s3 & s2) ^ (s6 & s1) ^ s4, 7) + RotateRight(s5, 11) + m02 + 0x00000000;
            s4 = RotateRight((s7 & (s3 ^ s6)) ^ (s2 & s1) ^ (s5 & s0) ^ s3, 7) + RotateRight(s4, 11) + m03 + 0x00000000;
            s3 = RotateRight((s6 & (s2 ^ s5)) ^ (s1 & s0) ^ (s4 & s7) ^ s2, 7) + RotateRight(s3, 11) + m04 + 0x00000000;
            s2 = RotateRight((s5 & (s1 ^ s4)) ^ (s0 & s7) ^ (s3 & s6) ^ s1, 7) + RotateRight(s2, 11) + m05 + 0x00000000;
            s1 = RotateRight((s4 & (s0 ^ s3)) ^ (s7 & s6) ^ (s2 & s5) ^ s0, 7) + RotateRight(s1, 11) + m06 + 0x00000000;
            s0 = RotateRight((s3 & (s7 ^ s2)) ^ (s6 & s5) ^ (s1 & s4) ^ s7, 7) + RotateRight(s0, 11) + m07 + 0x00000000;
            s7 = RotateRight((s2 & (s6 ^ s1)) ^ (s5 & s4) ^ (s0 & s3) ^ s6, 7) + RotateRight(s7, 11) + m08 + 0x00000000;
            s6 = RotateRight((s1 & (s5 ^ s0)) ^ (s4 & s3) ^ (s7 & s2) ^ s5, 7) + RotateRight(s6, 11) + m09 + 0x00000000;
            s5 = RotateRight((s0 & (s4 ^ s7)) ^ (s3 & s2) ^ (s6 & s1) ^ s4, 7) + RotateRight(s5, 11) + m0a + 0x00000000;
            s4 = RotateRight((s7 & (s3 ^ s6)) ^ (s2 & s1) ^ (s5 & s0) ^ s3, 7) + RotateRight(s4, 11) + m0b + 0x00000000;
            s3 = RotateRight((s6 & (s2 ^ s5)) ^ (s1 & s0) ^ (s4 & s7) ^ s2, 7) + RotateRight(s3, 11) + m0c + 0x00000000;
            s2 = RotateRight((s5 & (s1 ^ s4)) ^ (s0 & s7) ^ (s3 & s6) ^ s1, 7) + RotateRight(s2, 11) + m0d + 0x00000000;
            s1 = RotateRight((s4 & (s0 ^ s3)) ^ (s7 & s6) ^ (s2 & s5) ^ s0, 7) + RotateRight(s1, 11) + m0e + 0x00000000;
            s0 = RotateRight((s3 & (s7 ^ s2)) ^ (s6 & s5) ^ (s1 & s4) ^ s7, 7) + RotateRight(s0, 11) + m0f + 0x00000000;
            s7 = RotateRight((s2 & (s6 ^ s1)) ^ (s5 & s4) ^ (s0 & s3) ^ s6, 7) + RotateRight(s7, 11) + m10 + 0x00000000;
            s6 = RotateRight((s1 & (s5 ^ s0)) ^ (s4 & s3) ^ (s7 & s2) ^ s5, 7) + RotateRight(s6, 11) + m11 + 0x00000000;
            s5 = RotateRight((s0 & (s4 ^ s7)) ^ (s3 & s2) ^ (s6 & s1) ^ s4, 7) + RotateRight(s5, 11) + m12 + 0x00000000;
            s4 = RotateRight((s7 & (s3 ^ s6)) ^ (s2 & s1) ^ (s5 & s0) ^ s3, 7) + RotateRight(s4, 11) + m13 + 0x00000000;
            s3 = RotateRight((s6 & (s2 ^ s5)) ^ (s1 & s0) ^ (s4 & s7) ^ s2, 7) + RotateRight(s3, 11) + m14 + 0x00000000;
            s2 = RotateRight((s5 & (s1 ^ s4)) ^ (s0 & s7) ^ (s3 & s6) ^ s1, 7) + RotateRight(s2, 11) + m15 + 0x00000000;
            s1 = RotateRight((s4 & (s0 ^ s3)) ^ (s7 & s6) ^ (s2 & s5) ^ s0, 7) + RotateRight(s1, 11) + m16 + 0x00000000;
            s0 = RotateRight((s3 & (s7 ^ s2)) ^ (s6 & s5) ^ (s1 & s4) ^ s7, 7) + RotateRight(s0, 11) + m17 + 0x00000000;
            s7 = RotateRight((s2 & (s6 ^ s1)) ^ (s5 & s4) ^ (s0 & s3) ^ s6, 7) + RotateRight(s7, 11) + m18 + 0x00000000;
            s6 = RotateRight((s1 & (s5 ^ s0)) ^ (s4 & s3) ^ (s7 & s2) ^ s5, 7) + RotateRight(s6, 11) + m19 + 0x00000000;
            s5 = RotateRight((s0 & (s4 ^ s7)) ^ (s3 & s2) ^ (s6 & s1) ^ s4, 7) + RotateRight(s5, 11) + m1a + 0x00000000;
            s4 = RotateRight((s7 & (s3 ^ s6)) ^ (s2 & s1) ^ (s5 & s0) ^ s3, 7) + RotateRight(s4, 11) + m1b + 0x00000000;
            s3 = RotateRight((s6 & (s2 ^ s5)) ^ (s1 & s0) ^ (s4 & s7) ^ s2, 7) + RotateRight(s3, 11) + m1c + 0x00000000;
            s2 = RotateRight((s5 & (s1 ^ s4)) ^ (s0 & s7) ^ (s3 & s6) ^ s1, 7) + RotateRight(s2, 11) + m1d + 0x00000000;
            s1 = RotateRight((s4 & (s0 ^ s3)) ^ (s7 & s6) ^ (s2 & s5) ^ s0, 7) + RotateRight(s1, 11) + m1e + 0x00000000;
            s0 = RotateRight((s3 & (s7 ^ s2)) ^ (s6 & s5) ^ (s1 & s4) ^ s7, 7) + RotateRight(s0, 11) + m1f + 0x00000000;
            #endregion

            #region Pass 2
            s7 = RotateRight((s3 & ((s4 & ~s0) ^ (s1 & s2) ^ s6 ^ s5)) ^ (s1 & (s4 ^ s2)) ^ (s0 & s2) ^ s5, 7) + RotateRight(s7, 11) + m05 + 0x452821E6;
            s6 = RotateRight((s2 & ((s3 & ~s7) ^ (s0 & s1) ^ s5 ^ s4)) ^ (s0 & (s3 ^ s1)) ^ (s7 & s1) ^ s4, 7) + RotateRight(s6, 11) + m0e + 0x38D01377;
            s5 = RotateRight((s1 & ((s2 & ~s6) ^ (s7 & s0) ^ s4 ^ s3)) ^ (s7 & (s2 ^ s0)) ^ (s6 & s0) ^ s3, 7) + RotateRight(s5, 11) + m1a + 0xBE5466CF;
            s4 = RotateRight((s0 & ((s1 & ~s5) ^ (s6 & s7) ^ s3 ^ s2)) ^ (s6 & (s1 ^ s7)) ^ (s5 & s7) ^ s2, 7) + RotateRight(s4, 11) + m12 + 0x34E90C6C;
            s3 = RotateRight((s7 & ((s0 & ~s4) ^ (s5 & s6) ^ s2 ^ s1)) ^ (s5 & (s0 ^ s6)) ^ (s4 & s6) ^ s1, 7) + RotateRight(s3, 11) + m0b + 0xC0AC29B7;
            s2 = RotateRight((s6 & ((s7 & ~s3) ^ (s4 & s5) ^ s1 ^ s0)) ^ (s4 & (s7 ^ s5)) ^ (s3 & s5) ^ s0, 7) + RotateRight(s2, 11) + m1c + 0xC97C50DD;
            s1 = RotateRight((s5 & ((s6 & ~s2) ^ (s3 & s4) ^ s0 ^ s7)) ^ (s3 & (s6 ^ s4)) ^ (s2 & s4) ^ s7, 7) + RotateRight(s1, 11) + m07 + 0x3F84D5B5;
            s0 = RotateRight((s4 & ((s5 & ~s1) ^ (s2 & s3) ^ s7 ^ s6)) ^ (s2 & (s5 ^ s3)) ^ (s1 & s3) ^ s6, 7) + RotateRight(s0, 11) + m10 + 0xB5470917;
            s7 = RotateRight((s3 & ((s4 & ~s0) ^ (s1 & s2) ^ s6 ^ s5)) ^ (s1 & (s4 ^ s2)) ^ (s0 & s2) ^ s5, 7) + RotateRight(s7, 11) + m00 + 0x9216D5D9;
            s6 = RotateRight((s2 & ((s3 & ~s7) ^ (s0 & s1) ^ s5 ^ s4)) ^ (s0 & (s3 ^ s1)) ^ (s7 & s1) ^ s4, 7) + RotateRight(s6, 11) + m17 + 0x8979FB1B;
            s5 = RotateRight((s1 & ((s2 & ~s6) ^ (s7 & s0) ^ s4 ^ s3)) ^ (s7 & (s2 ^ s0)) ^ (s6 & s0) ^ s3, 7) + RotateRight(s5, 11) + m14 + 0xD1310BA6;
            s4 = RotateRight((s0 & ((s1 & ~s5) ^ (s6 & s7) ^ s3 ^ s2)) ^ (s6 & (s1 ^ s7)) ^ (s5 & s7) ^ s2, 7) + RotateRight(s4, 11) + m16 + 0x98DFB5AC;
            s3 = RotateRight((s7 & ((s0 & ~s4) ^ (s5 & s6) ^ s2 ^ s1)) ^ (s5 & (s0 ^ s6)) ^ (s4 & s6) ^ s1, 7) + RotateRight(s3, 11) + m01 + 0x2FFD72DB;
            s2 = RotateRight((s6 & ((s7 & ~s3) ^ (s4 & s5) ^ s1 ^ s0)) ^ (s4 & (s7 ^ s5)) ^ (s3 & s5) ^ s0, 7) + RotateRight(s2, 11) + m0a + 0xD01ADFB7;
            s1 = RotateRight((s5 & ((s6 & ~s2) ^ (s3 & s4) ^ s0 ^ s7)) ^ (s3 & (s6 ^ s4)) ^ (s2 & s4) ^ s7, 7) + RotateRight(s1, 11) + m04 + 0xB8E1AFED;
            s0 = RotateRight((s4 & ((s5 & ~s1) ^ (s2 & s3) ^ s7 ^ s6)) ^ (s2 & (s5 ^ s3)) ^ (s1 & s3) ^ s6, 7) + RotateRight(s0, 11) + m08 + 0x6A267E96;
            s7 = RotateRight((s3 & ((s4 & ~s0) ^ (s1 & s2) ^ s6 ^ s5)) ^ (s1 & (s4 ^ s2)) ^ (s0 & s2) ^ s5, 7) + RotateRight(s7, 11) + m1e + 0xBA7C9045;
            s6 = RotateRight((s2 & ((s3 & ~s7) ^ (s0 & s1) ^ s5 ^ s4)) ^ (s0 & (s3 ^ s1)) ^ (s7 & s1) ^ s4, 7) + RotateRight(s6, 11) + m03 + 0xF12C7F99;
            s5 = RotateRight((s1 & ((s2 & ~s6) ^ (s7 & s0) ^ s4 ^ s3)) ^ (s7 & (s2 ^ s0)) ^ (s6 & s0) ^ s3, 7) + RotateRight(s5, 11) + m15 + 0x24A19947;
            s4 = RotateRight((s0 & ((s1 & ~s5) ^ (s6 & s7) ^ s3 ^ s2)) ^ (s6 & (s1 ^ s7)) ^ (s5 & s7) ^ s2, 7) + RotateRight(s4, 11) + m09 + 0xB3916CF7;
            s3 = RotateRight((s7 & ((s0 & ~s4) ^ (s5 & s6) ^ s2 ^ s1)) ^ (s5 & (s0 ^ s6)) ^ (s4 & s6) ^ s1, 7) + RotateRight(s3, 11) + m11 + 0x0801F2E2;
            s2 = RotateRight((s6 & ((s7 & ~s3) ^ (s4 & s5) ^ s1 ^ s0)) ^ (s4 & (s7 ^ s5)) ^ (s3 & s5) ^ s0, 7) + RotateRight(s2, 11) + m18 + 0x858EFC16;
            s1 = RotateRight((s5 & ((s6 & ~s2) ^ (s3 & s4) ^ s0 ^ s7)) ^ (s3 & (s6 ^ s4)) ^ (s2 & s4) ^ s7, 7) + RotateRight(s1, 11) + m1d + 0x636920D8;
            s0 = RotateRight((s4 & ((s5 & ~s1) ^ (s2 & s3) ^ s7 ^ s6)) ^ (s2 & (s5 ^ s3)) ^ (s1 & s3) ^ s6, 7) + RotateRight(s0, 11) + m06 + 0x71574E69;
            s7 = RotateRight((s3 & ((s4 & ~s0) ^ (s1 & s2) ^ s6 ^ s5)) ^ (s1 & (s4 ^ s2)) ^ (s0 & s2) ^ s5, 7) + RotateRight(s7, 11) + m13 + 0xA458FEA3;
            s6 = RotateRight((s2 & ((s3 & ~s7) ^ (s0 & s1) ^ s5 ^ s4)) ^ (s0 & (s3 ^ s1)) ^ (s7 & s1) ^ s4, 7) + RotateRight(s6, 11) + m0c + 0xF4933D7E;
            s5 = RotateRight((s1 & ((s2 & ~s6) ^ (s7 & s0) ^ s4 ^ s3)) ^ (s7 & (s2 ^ s0)) ^ (s6 & s0) ^ s3, 7) + RotateRight(s5, 11) + m0f + 0x0D95748F;
            s4 = RotateRight((s0 & ((s1 & ~s5) ^ (s6 & s7) ^ s3 ^ s2)) ^ (s6 & (s1 ^ s7)) ^ (s5 & s7) ^ s2, 7) + RotateRight(s4, 11) + m0d + 0x728EB658;
            s3 = RotateRight((s7 & ((s0 & ~s4) ^ (s5 & s6) ^ s2 ^ s1)) ^ (s5 & (s0 ^ s6)) ^ (s4 & s6) ^ s1, 7) + RotateRight(s3, 11) + m02 + 0x718BCD58;
            s2 = RotateRight((s6 & ((s7 & ~s3) ^ (s4 & s5) ^ s1 ^ s0)) ^ (s4 & (s7 ^ s5)) ^ (s3 & s5) ^ s0, 7) + RotateRight(s2, 11) + m19 + 0x82154AEE;
            s1 = RotateRight((s5 & ((s6 & ~s2) ^ (s3 & s4) ^ s0 ^ s7)) ^ (s3 & (s6 ^ s4)) ^ (s2 & s4) ^ s7, 7) + RotateRight(s1, 11) + m1f + 0x7B54A41D;
            s0 = RotateRight((s4 & ((s5 & ~s1) ^ (s2 & s3) ^ s7 ^ s6)) ^ (s2 & (s5 ^ s3)) ^ (s1 & s3) ^ s6, 7) + RotateRight(s0, 11) + m1b + 0xC25A59B5;
            #endregion

            #region Pass 3
            s7 = RotateRight((s4 & ((s1 & s3) ^ s2 ^ s5)) ^ (s1 & s0) ^ (s3 & s6) ^ s5, 7) + RotateRight(s7, 11) + m13 + 0x9C30D539;
            s6 = RotateRight((s3 & ((s0 & s2) ^ s1 ^ s4)) ^ (s0 & s7) ^ (s2 & s5) ^ s4, 7) + RotateRight(s6, 11) + m09 + 0x2AF26013;
            s5 = RotateRight((s2 & ((s7 & s1) ^ s0 ^ s3)) ^ (s7 & s6) ^ (s1 & s4) ^ s3, 7) + RotateRight(s5, 11) + m04 + 0xC5D1B023;
            s4 = RotateRight((s1 & ((s6 & s0) ^ s7 ^ s2)) ^ (s6 & s5) ^ (s0 & s3) ^ s2, 7) + RotateRight(s4, 11) + m14 + 0x286085F0;
            s3 = RotateRight((s0 & ((s5 & s7) ^ s6 ^ s1)) ^ (s5 & s4) ^ (s7 & s2) ^ s1, 7) + RotateRight(s3, 11) + m1c + 0xCA417918;
            s2 = RotateRight((s7 & ((s4 & s6) ^ s5 ^ s0)) ^ (s4 & s3) ^ (s6 & s1) ^ s0, 7) + RotateRight(s2, 11) + m11 + 0xB8DB38EF;
            s1 = RotateRight((s6 & ((s3 & s5) ^ s4 ^ s7)) ^ (s3 & s2) ^ (s5 & s0) ^ s7, 7) + RotateRight(s1, 11) + m08 + 0x8E79DCB0;
            s0 = RotateRight((s5 & ((s2 & s4) ^ s3 ^ s6)) ^ (s2 & s1) ^ (s4 & s7) ^ s6, 7) + RotateRight(s0, 11) + m16 + 0x603A180E;
            s7 = RotateRight((s4 & ((s1 & s3) ^ s2 ^ s5)) ^ (s1 & s0) ^ (s3 & s6) ^ s5, 7) + RotateRight(s7, 11) + m1d + 0x6C9E0E8B;
            s6 = RotateRight((s3 & ((s0 & s2) ^ s1 ^ s4)) ^ (s0 & s7) ^ (s2 & s5) ^ s4, 7) + RotateRight(s6, 11) + m0e + 0xB01E8A3E;
            s5 = RotateRight((s2 & ((s7 & s1) ^ s0 ^ s3)) ^ (s7 & s6) ^ (s1 & s4) ^ s3, 7) + RotateRight(s5, 11) + m19 + 0xD71577C1;
            s4 = RotateRight((s1 & ((s6 & s0) ^ s7 ^ s2)) ^ (s6 & s5) ^ (s0 & s3) ^ s2, 7) + RotateRight(s4, 11) + m0c + 0xBD314B27;
            s3 = RotateRight((s0 & ((s5 & s7) ^ s6 ^ s1)) ^ (s5 & s4) ^ (s7 & s2) ^ s1, 7) + RotateRight(s3, 11) + m18 + 0x78AF2FDA;
            s2 = RotateRight((s7 & ((s4 & s6) ^ s5 ^ s0)) ^ (s4 & s3) ^ (s6 & s1) ^ s0, 7) + RotateRight(s2, 11) + m1e + 0x55605C60;
            s1 = RotateRight((s6 & ((s3 & s5) ^ s4 ^ s7)) ^ (s3 & s2) ^ (s5 & s0) ^ s7, 7) + RotateRight(s1, 11) + m10 + 0xE65525F3;
            s0 = RotateRight((s5 & ((s2 & s4) ^ s3 ^ s6)) ^ (s2 & s1) ^ (s4 & s7) ^ s6, 7) + RotateRight(s0, 11) + m1a + 0xAA55AB94;
            s7 = RotateRight((s4 & ((s1 & s3) ^ s2 ^ s5)) ^ (s1 & s0) ^ (s3 & s6) ^ s5, 7) + RotateRight(s7, 11) + m1f + 0x57489862;
            s6 = RotateRight((s3 & ((s0 & s2) ^ s1 ^ s4)) ^ (s0 & s7) ^ (s2 & s5) ^ s4, 7) + RotateRight(s6, 11) + m0f + 0x63E81440;
            s5 = RotateRight((s2 & ((s7 & s1) ^ s0 ^ s3)) ^ (s7 & s6) ^ (s1 & s4) ^ s3, 7) + RotateRight(s5, 11) + m07 + 0x55CA396A;
            s4 = RotateRight((s1 & ((s6 & s0) ^ s7 ^ s2)) ^ (s6 & s5) ^ (s0 & s3) ^ s2, 7) + RotateRight(s4, 11) + m03 + 0x2AAB10B6;
            s3 = RotateRight((s0 & ((s5 & s7) ^ s6 ^ s1)) ^ (s5 & s4) ^ (s7 & s2) ^ s1, 7) + RotateRight(s3, 11) + m01 + 0xB4CC5C34;
            s2 = RotateRight((s7 & ((s4 & s6) ^ s5 ^ s0)) ^ (s4 & s3) ^ (s6 & s1) ^ s0, 7) + RotateRight(s2, 11) + m00 + 0x1141E8CE;
            s1 = RotateRight((s6 & ((s3 & s5) ^ s4 ^ s7)) ^ (s3 & s2) ^ (s5 & s0) ^ s7, 7) + RotateRight(s1, 11) + m12 + 0xA15486AF;
            s0 = RotateRight((s5 & ((s2 & s4) ^ s3 ^ s6)) ^ (s2 & s1) ^ (s4 & s7) ^ s6, 7) + RotateRight(s0, 11) + m1b + 0x7C72E993;
            s7 = RotateRight((s4 & ((s1 & s3) ^ s2 ^ s5)) ^ (s1 & s0) ^ (s3 & s6) ^ s5, 7) + RotateRight(s7, 11) + m0d + 0xB3EE1411;
            s6 = RotateRight((s3 & ((s0 & s2) ^ s1 ^ s4)) ^ (s0 & s7) ^ (s2 & s5) ^ s4, 7) + RotateRight(s6, 11) + m06 + 0x636FBC2A;
            s5 = RotateRight((s2 & ((s7 & s1) ^ s0 ^ s3)) ^ (s7 & s6) ^ (s1 & s4) ^ s3, 7) + RotateRight(s5, 11) + m15 + 0x2BA9C55D;
            s4 = RotateRight((s1 & ((s6 & s0) ^ s7 ^ s2)) ^ (s6 & s5) ^ (s0 & s3) ^ s2, 7) + RotateRight(s4, 11) + m0a + 0x741831F6;
            s3 = RotateRight((s0 & ((s5 & s7) ^ s6 ^ s1)) ^ (s5 & s4) ^ (s7 & s2) ^ s1, 7) + RotateRight(s3, 11) + m17 + 0xCE5C3E16;
            s2 = RotateRight((s7 & ((s4 & s6) ^ s5 ^ s0)) ^ (s4 & s3) ^ (s6 & s1) ^ s0, 7) + RotateRight(s2, 11) + m0b + 0x9B87931E;
            s1 = RotateRight((s6 & ((s3 & s5) ^ s4 ^ s7)) ^ (s3 & s2) ^ (s5 & s0) ^ s7, 7) + RotateRight(s1, 11) + m05 + 0xAFD6BA33;
            s0 = RotateRight((s5 & ((s2 & s4) ^ s3 ^ s6)) ^ (s2 & s1) ^ (s4 & s7) ^ s6, 7) + RotateRight(s0, 11) + m02 + 0x6C24CF5C;
            #endregion

            #region Pass 4
            s7 = RotateRight((s2 & ((s4 & s0) ^ (s3 | s1) ^ s5)) ^ (s3 & ((~s0 & s5) ^ s4 ^ s1 ^ s6)) ^ (s0 & s1) ^ s6, 7) + RotateRight(s7, 11) + m18 + 0x7A325381;
            s6 = RotateRight((s1 & ((s3 & s7) ^ (s2 | s0) ^ s4)) ^ (s2 & ((~s7 & s4) ^ s3 ^ s0 ^ s5)) ^ (s7 & s0) ^ s5, 7) + RotateRight(s6, 11) + m04 + 0x28958677;
            s5 = RotateRight((s0 & ((s2 & s6) ^ (s1 | s7) ^ s3)) ^ (s1 & ((~s6 & s3) ^ s2 ^ s7 ^ s4)) ^ (s6 & s7) ^ s4, 7) + RotateRight(s5, 11) + m00 + 0x3B8F4898;
            s4 = RotateRight((s7 & ((s1 & s5) ^ (s0 | s6) ^ s2)) ^ (s0 & ((~s5 & s2) ^ s1 ^ s6 ^ s3)) ^ (s5 & s6) ^ s3, 7) + RotateRight(s4, 11) + m0e + 0x6B4BB9AF;
            s3 = RotateRight((s6 & ((s0 & s4) ^ (s7 | s5) ^ s1)) ^ (s7 & ((~s4 & s1) ^ s0 ^ s5 ^ s2)) ^ (s4 & s5) ^ s2, 7) + RotateRight(s3, 11) + m02 + 0xC4BFE81B;
            s2 = RotateRight((s5 & ((s7 & s3) ^ (s6 | s4) ^ s0)) ^ (s6 & ((~s3 & s0) ^ s7 ^ s4 ^ s1)) ^ (s3 & s4) ^ s1, 7) + RotateRight(s2, 11) + m07 + 0x66282193;
            s1 = RotateRight((s4 & ((s6 & s2) ^ (s5 | s3) ^ s7)) ^ (s5 & ((~s2 & s7) ^ s6 ^ s3 ^ s0)) ^ (s2 & s3) ^ s0, 7) + RotateRight(s1, 11) + m1c + 0x61D809CC;
            s0 = RotateRight((s3 & ((s5 & s1) ^ (s4 | s2) ^ s6)) ^ (s4 & ((~s1 & s6) ^ s5 ^ s2 ^ s7)) ^ (s1 & s2) ^ s7, 7) + RotateRight(s0, 11) + m17 + 0xFB21A991;
            s7 = RotateRight((s2 & ((s4 & s0) ^ (s3 | s1) ^ s5)) ^ (s3 & ((~s0 & s5) ^ s4 ^ s1 ^ s6)) ^ (s0 & s1) ^ s6, 7) + RotateRight(s7, 11) + m1a + 0x487CAC60;
            s6 = RotateRight((s1 & ((s3 & s7) ^ (s2 | s0) ^ s4)) ^ (s2 & ((~s7 & s4) ^ s3 ^ s0 ^ s5)) ^ (s7 & s0) ^ s5, 7) + RotateRight(s6, 11) + m06 + 0x5DEC8032;
            s5 = RotateRight((s0 & ((s2 & s6) ^ (s1 | s7) ^ s3)) ^ (s1 & ((~s6 & s3) ^ s2 ^ s7 ^ s4)) ^ (s6 & s7) ^ s4, 7) + RotateRight(s5, 11) + m1e + 0xEF845D5D;
            s4 = RotateRight((s7 & ((s1 & s5) ^ (s0 | s6) ^ s2)) ^ (s0 & ((~s5 & s2) ^ s1 ^ s6 ^ s3)) ^ (s5 & s6) ^ s3, 7) + RotateRight(s4, 11) + m14 + 0xE98575B1;
            s3 = RotateRight((s6 & ((s0 & s4) ^ (s7 | s5) ^ s1)) ^ (s7 & ((~s4 & s1) ^ s0 ^ s5 ^ s2)) ^ (s4 & s5) ^ s2, 7) + RotateRight(s3, 11) + m12 + 0xDC262302;
            s2 = RotateRight((s5 & ((s7 & s3) ^ (s6 | s4) ^ s0)) ^ (s6 & ((~s3 & s0) ^ s7 ^ s4 ^ s1)) ^ (s3 & s4) ^ s1, 7) + RotateRight(s2, 11) + m19 + 0xEB651B88;
            s1 = RotateRight((s4 & ((s6 & s2) ^ (s5 | s3) ^ s7)) ^ (s5 & ((~s2 & s7) ^ s6 ^ s3 ^ s0)) ^ (s2 & s3) ^ s0, 7) + RotateRight(s1, 11) + m13 + 0x23893E81;
            s0 = RotateRight((s3 & ((s5 & s1) ^ (s4 | s2) ^ s6)) ^ (s4 & ((~s1 & s6) ^ s5 ^ s2 ^ s7)) ^ (s1 & s2) ^ s7, 7) + RotateRight(s0, 11) + m03 + 0xD396ACC5;
            s7 = RotateRight((s2 & ((s4 & s0) ^ (s3 | s1) ^ s5)) ^ (s3 & ((~s0 & s5) ^ s4 ^ s1 ^ s6)) ^ (s0 & s1) ^ s6, 7) + RotateRight(s7, 11) + m16 + 0x0F6D6FF3;
            s6 = RotateRight((s1 & ((s3 & s7) ^ (s2 | s0) ^ s4)) ^ (s2 & ((~s7 & s4) ^ s3 ^ s0 ^ s5)) ^ (s7 & s0) ^ s5, 7) + RotateRight(s6, 11) + m0b + 0x83F44239;
            s5 = RotateRight((s0 & ((s2 & s6) ^ (s1 | s7) ^ s3)) ^ (s1 & ((~s6 & s3) ^ s2 ^ s7 ^ s4)) ^ (s6 & s7) ^ s4, 7) + RotateRight(s5, 11) + m1f + 0x2E0B4482;
            s4 = RotateRight((s7 & ((s1 & s5) ^ (s0 | s6) ^ s2)) ^ (s0 & ((~s5 & s2) ^ s1 ^ s6 ^ s3)) ^ (s5 & s6) ^ s3, 7) + RotateRight(s4, 11) + m15 + 0xA4842004;
            s3 = RotateRight((s6 & ((s0 & s4) ^ (s7 | s5) ^ s1)) ^ (s7 & ((~s4 & s1) ^ s0 ^ s5 ^ s2)) ^ (s4 & s5) ^ s2, 7) + RotateRight(s3, 11) + m08 + 0x69C8F04A;
            s2 = RotateRight((s5 & ((s7 & s3) ^ (s6 | s4) ^ s0)) ^ (s6 & ((~s3 & s0) ^ s7 ^ s4 ^ s1)) ^ (s3 & s4) ^ s1, 7) + RotateRight(s2, 11) + m1b + 0x9E1F9B5E;
            s1 = RotateRight((s4 & ((s6 & s2) ^ (s5 | s3) ^ s7)) ^ (s5 & ((~s2 & s7) ^ s6 ^ s3 ^ s0)) ^ (s2 & s3) ^ s0, 7) + RotateRight(s1, 11) + m0c + 0x21C66842;
            s0 = RotateRight((s3 & ((s5 & s1) ^ (s4 | s2) ^ s6)) ^ (s4 & ((~s1 & s6) ^ s5 ^ s2 ^ s7)) ^ (s1 & s2) ^ s7, 7) + RotateRight(s0, 11) + m09 + 0xF6E96C9A;
            s7 = RotateRight((s2 & ((s4 & s0) ^ (s3 | s1) ^ s5)) ^ (s3 & ((~s0 & s5) ^ s4 ^ s1 ^ s6)) ^ (s0 & s1) ^ s6, 7) + RotateRight(s7, 11) + m01 + 0x670C9C61;
            s6 = RotateRight((s1 & ((s3 & s7) ^ (s2 | s0) ^ s4)) ^ (s2 & ((~s7 & s4) ^ s3 ^ s0 ^ s5)) ^ (s7 & s0) ^ s5, 7) + RotateRight(s6, 11) + m1d + 0xABD388F0;
            s5 = RotateRight((s0 & ((s2 & s6) ^ (s1 | s7) ^ s3)) ^ (s1 & ((~s6 & s3) ^ s2 ^ s7 ^ s4)) ^ (s6 & s7) ^ s4, 7) + RotateRight(s5, 11) + m05 + 0x6A51A0D2;
            s4 = RotateRight((s7 & ((s1 & s5) ^ (s0 | s6) ^ s2)) ^ (s0 & ((~s5 & s2) ^ s1 ^ s6 ^ s3)) ^ (s5 & s6) ^ s3, 7) + RotateRight(s4, 11) + m0f + 0xD8542F68;
            s3 = RotateRight((s6 & ((s0 & s4) ^ (s7 | s5) ^ s1)) ^ (s7 & ((~s4 & s1) ^ s0 ^ s5 ^ s2)) ^ (s4 & s5) ^ s2, 7) + RotateRight(s3, 11) + m11 + 0x960FA728;
            s2 = RotateRight((s5 & ((s7 & s3) ^ (s6 | s4) ^ s0)) ^ (s6 & ((~s3 & s0) ^ s7 ^ s4 ^ s1)) ^ (s3 & s4) ^ s1, 7) + RotateRight(s2, 11) + m0a + 0xAB5133A3;
            s1 = RotateRight((s4 & ((s6 & s2) ^ (s5 | s3) ^ s7)) ^ (s5 & ((~s2 & s7) ^ s6 ^ s3 ^ s0)) ^ (s2 & s3) ^ s0, 7) + RotateRight(s1, 11) + m10 + 0x6EEF0B6C;
            s0 = RotateRight((s3 & ((s5 & s1) ^ (s4 | s2) ^ s6)) ^ (s4 & ((~s1 & s6) ^ s5 ^ s2 ^ s7)) ^ (s1 & s2) ^ s7, 7) + RotateRight(s0, 11) + m0d + 0x137A3BE4;
            #endregion

            #region Pass 5
            s7 = RotateRight((s1 & ~((s3 & s4 & s6) ^ s5)) ^ (s3 & s0) ^ (s4 & s5) ^ (s6 & s2), 7) + RotateRight(s7, 11) + m1b + 0xBA3BF050;
            s6 = RotateRight((s0 & ~((s2 & s3 & s5) ^ s4)) ^ (s2 & s7) ^ (s3 & s4) ^ (s5 & s1), 7) + RotateRight(s6, 11) + m03 + 0x7EFB2A98;
            s5 = RotateRight((s7 & ~((s1 & s2 & s4) ^ s3)) ^ (s1 & s6) ^ (s2 & s3) ^ (s4 & s0), 7) + RotateRight(s5, 11) + m15 + 0xA1F1651D;
            s4 = RotateRight((s6 & ~((s0 & s1 & s3) ^ s2)) ^ (s0 & s5) ^ (s1 & s2) ^ (s3 & s7), 7) + RotateRight(s4, 11) + m1a + 0x39AF0176;
            s3 = RotateRight((s5 & ~((s7 & s0 & s2) ^ s1)) ^ (s7 & s4) ^ (s0 & s1) ^ (s2 & s6), 7) + RotateRight(s3, 11) + m11 + 0x66CA593E;
            s2 = RotateRight((s4 & ~((s6 & s7 & s1) ^ s0)) ^ (s6 & s3) ^ (s7 & s0) ^ (s1 & s5), 7) + RotateRight(s2, 11) + m0b + 0x82430E88;
            s1 = RotateRight((s3 & ~((s5 & s6 & s0) ^ s7)) ^ (s5 & s2) ^ (s6 & s7) ^ (s0 & s4), 7) + RotateRight(s1, 11) + m14 + 0x8CEE8619;
            s0 = RotateRight((s2 & ~((s4 & s5 & s7) ^ s6)) ^ (s4 & s1) ^ (s5 & s6) ^ (s7 & s3), 7) + RotateRight(s0, 11) + m1d + 0x456F9FB4;
            s7 = RotateRight((s1 & ~((s3 & s4 & s6) ^ s5)) ^ (s3 & s0) ^ (s4 & s5) ^ (s6 & s2), 7) + RotateRight(s7, 11) + m13 + 0x7D84A5C3;
            s6 = RotateRight((s0 & ~((s2 & s3 & s5) ^ s4)) ^ (s2 & s7) ^ (s3 & s4) ^ (s5 & s1), 7) + RotateRight(s6, 11) + m00 + 0x3B8B5EBE;
            s5 = RotateRight((s7 & ~((s1 & s2 & s4) ^ s3)) ^ (s1 & s6) ^ (s2 & s3) ^ (s4 & s0), 7) + RotateRight(s5, 11) + m0c + 0xE06F75D8;
            s4 = RotateRight((s6 & ~((s0 & s1 & s3) ^ s2)) ^ (s0 & s5) ^ (s1 & s2) ^ (s3 & s7), 7) + RotateRight(s4, 11) + m07 + 0x85C12073;
            s3 = RotateRight((s5 & ~((s7 & s0 & s2) ^ s1)) ^ (s7 & s4) ^ (s0 & s1) ^ (s2 & s6), 7) + RotateRight(s3, 11) + m0d + 0x401A449F;
            s2 = RotateRight((s4 & ~((s6 & s7 & s1) ^ s0)) ^ (s6 & s3) ^ (s7 & s0) ^ (s1 & s5), 7) + RotateRight(s2, 11) + m08 + 0x56C16AA6;
            s1 = RotateRight((s3 & ~((s5 & s6 & s0) ^ s7)) ^ (s5 & s2) ^ (s6 & s7) ^ (s0 & s4), 7) + RotateRight(s1, 11) + m1f + 0x4ED3AA62;
            s0 = RotateRight((s2 & ~((s4 & s5 & s7) ^ s6)) ^ (s4 & s1) ^ (s5 & s6) ^ (s7 & s3), 7) + RotateRight(s0, 11) + m0a + 0x363F7706;
            s7 = RotateRight((s1 & ~((s3 & s4 & s6) ^ s5)) ^ (s3 & s0) ^ (s4 & s5) ^ (s6 & s2), 7) + RotateRight(s7, 11) + m05 + 0x1BFEDF72;
            s6 = RotateRight((s0 & ~((s2 & s3 & s5) ^ s4)) ^ (s2 & s7) ^ (s3 & s4) ^ (s5 & s1), 7) + RotateRight(s6, 11) + m09 + 0x429B023D;
            s5 = RotateRight((s7 & ~((s1 & s2 & s4) ^ s3)) ^ (s1 & s6) ^ (s2 & s3) ^ (s4 & s0), 7) + RotateRight(s5, 11) + m0e + 0x37D0D724;
            s4 = RotateRight((s6 & ~((s0 & s1 & s3) ^ s2)) ^ (s0 & s5) ^ (s1 & s2) ^ (s3 & s7), 7) + RotateRight(s4, 11) + m1e + 0xD00A1248;
            s3 = RotateRight((s5 & ~((s7 & s0 & s2) ^ s1)) ^ (s7 & s4) ^ (s0 & s1) ^ (s2 & s6), 7) + RotateRight(s3, 11) + m12 + 0xDB0FEAD3;
            s2 = RotateRight((s4 & ~((s6 & s7 & s1) ^ s0)) ^ (s6 & s3) ^ (s7 & s0) ^ (s1 & s5), 7) + RotateRight(s2, 11) + m06 + 0x49F1C09B;
            s1 = RotateRight((s3 & ~((s5 & s6 & s0) ^ s7)) ^ (s5 & s2) ^ (s6 & s7) ^ (s0 & s4), 7) + RotateRight(s1, 11) + m1c + 0x075372C9;
            s0 = RotateRight((s2 & ~((s4 & s5 & s7) ^ s6)) ^ (s4 & s1) ^ (s5 & s6) ^ (s7 & s3), 7) + RotateRight(s0, 11) + m18 + 0x80991B7B;
            s7 = RotateRight((s1 & ~((s3 & s4 & s6) ^ s5)) ^ (s3 & s0) ^ (s4 & s5) ^ (s6 & s2), 7) + RotateRight(s7, 11) + m02 + 0x25D479D8;
            s6 = RotateRight((s0 & ~((s2 & s3 & s5) ^ s4)) ^ (s2 & s7) ^ (s3 & s4) ^ (s5 & s1), 7) + RotateRight(s6, 11) + m17 + 0xF6E8DEF7;
            s5 = RotateRight((s7 & ~((s1 & s2 & s4) ^ s3)) ^ (s1 & s6) ^ (s2 & s3) ^ (s4 & s0), 7) + RotateRight(s5, 11) + m10 + 0xE3FE501A;
            s4 = RotateRight((s6 & ~((s0 & s1 & s3) ^ s2)) ^ (s0 & s5) ^ (s1 & s2) ^ (s3 & s7), 7) + RotateRight(s4, 11) + m16 + 0xB6794C3B;
            s3 = RotateRight((s5 & ~((s7 & s0 & s2) ^ s1)) ^ (s7 & s4) ^ (s0 & s1) ^ (s2 & s6), 7) + RotateRight(s3, 11) + m04 + 0x976CE0BD;
            s2 = RotateRight((s4 & ~((s6 & s7 & s1) ^ s0)) ^ (s6 & s3) ^ (s7 & s0) ^ (s1 & s5), 7) + RotateRight(s2, 11) + m01 + 0x04C006BA;
            s1 = RotateRight((s3 & ~((s5 & s6 & s0) ^ s7)) ^ (s5 & s2) ^ (s6 & s7) ^ (s0 & s4), 7) + RotateRight(s1, 11) + m19 + 0xC1A94FB6;
            s0 = RotateRight((s2 & ~((s4 & s5 & s7) ^ s6)) ^ (s4 & s1) ^ (s5 & s6) ^ (s7 & s3), 7) + RotateRight(s0, 11) + m0f + 0x409F60C4;
            #endregion

            this.s0 += s0;
            this.s1 += s1;
            this.s2 += s2;
            this.s3 += s3;
            this.s4 += s4;
            this.s5 += s5;
            this.s6 += s6;
            this.s7 += s7;
        }
    }
}