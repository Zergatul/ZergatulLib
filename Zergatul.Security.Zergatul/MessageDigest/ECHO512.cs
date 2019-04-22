using System;
using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.Common.AESHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class ECHO512 : Security.MessageDigest
    {
        public override int BlockLength => 128;
        public override int DigestLength => 64;

        protected byte[] buffer;
        protected int bufOffset;
        protected ulong v0, v1, v2, v3, v4, v5, v6, v7, v8, v9, va, vb, vc, vd, ve, vf;
        protected uint c0, c1, c2, c3;

        public ECHO512()
        {
            buffer = new byte[128];
            Reset();
        }

        public override void Reset()
        {
            v0 = v2 = v4 = v6 = v8 = va = vc = ve = (ulong)DigestLength << 3;
            v1 = v3 = v5 = v7 = v9 = vb = vd = vf = 0;

            c0 = c1 = c2 = c3 = 0;

            bufOffset = 0;
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
                    IncreaseLength(1024);
                    bufOffset = 0;
                    ProcessBlock();
                }
            }
        }

        public override byte[] Digest()
        {
            IncreaseLength(bufOffset << 3);

            uint t0 = c0;
            uint t1 = c1;
            uint t2 = c2;
            uint t3 = c3;

            if (bufOffset == 0)
                c0 = c1 = c2 = c3 = 0;

            buffer[bufOffset++] = 0x80;

            if (bufOffset > 110)
            {
                while (bufOffset < 128)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
                c0 = c1 = c2 = c3 = 0;
            }

            while (bufOffset < 110)
                buffer[bufOffset++] = 0;

            buffer[110] = (byte)((DigestLength << 3) & 0xFF);
            buffer[111] = (byte)(DigestLength >> 5);

            GetBytes(t0, ByteOrder.LittleEndian, buffer, 112);
            GetBytes(t1, ByteOrder.LittleEndian, buffer, 116);
            GetBytes(t2, ByteOrder.LittleEndian, buffer, 120);
            GetBytes(t3, ByteOrder.LittleEndian, buffer, 124);

            ProcessBlock();

            return InternalStateToDigest();
        }

        private void IncreaseLength(int value)
        {
            if ((c0 += (uint)value) < value)
            {
                c1++;
                if (c1 == 0)
                {
                    c2++;
                    if (c2 == 0)
                        c3++;
                }
            }
        }

        protected virtual byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[64];
            GetBytes(v0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(v1, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(v2, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(v3, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(v4, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(v5, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(v6, ByteOrder.LittleEndian, digest, 0x30);
            GetBytes(v7, ByteOrder.LittleEndian, digest, 0x38);
            return digest;
        }

        private void ProcessBlock()
        {
            ulong w00 = v0;
            ulong w01 = v1;
            ulong w02 = v2;
            ulong w03 = v3;
            ulong w04 = v4;
            ulong w05 = v5;
            ulong w06 = v6;
            ulong w07 = v7;
            ulong w08 = v8;
            ulong w09 = v9;
            ulong w0a = va;
            ulong w0b = vb;
            ulong w0c = vc;
            ulong w0d = vd;
            ulong w0e = ve;
            ulong w0f = vf;

            ulong w10 = ToUInt64(buffer, 0x00, ByteOrder.LittleEndian);
            ulong w11 = ToUInt64(buffer, 0x08, ByteOrder.LittleEndian);
            ulong w12 = ToUInt64(buffer, 0x10, ByteOrder.LittleEndian);
            ulong w13 = ToUInt64(buffer, 0x18, ByteOrder.LittleEndian);
            ulong w14 = ToUInt64(buffer, 0x20, ByteOrder.LittleEndian);
            ulong w15 = ToUInt64(buffer, 0x28, ByteOrder.LittleEndian);
            ulong w16 = ToUInt64(buffer, 0x30, ByteOrder.LittleEndian);
            ulong w17 = ToUInt64(buffer, 0x38, ByteOrder.LittleEndian);
            ulong w18 = ToUInt64(buffer, 0x40, ByteOrder.LittleEndian);
            ulong w19 = ToUInt64(buffer, 0x48, ByteOrder.LittleEndian);
            ulong w1a = ToUInt64(buffer, 0x50, ByteOrder.LittleEndian);
            ulong w1b = ToUInt64(buffer, 0x58, ByteOrder.LittleEndian);
            ulong w1c = ToUInt64(buffer, 0x60, ByteOrder.LittleEndian);
            ulong w1d = ToUInt64(buffer, 0x68, ByteOrder.LittleEndian);
            ulong w1e = ToUInt64(buffer, 0x70, ByteOrder.LittleEndian);
            ulong w1f = ToUInt64(buffer, 0x78, ByteOrder.LittleEndian);

            uint k0 = c0;
            uint k1 = c1;
            uint k2 = c2;
            uint k3 = c3;

            uint x0, x1, x2, x3;
            uint y0, y1, y2, y3;
            ulong tmp;
            ulong a, b, c, d, ab, bc, cd, abx, bcx, cdx;

            for (int r = 0; r < 10; r++)
            {
                #region Sub Words
                x0 = (uint)w00;
                x1 = (uint)(w00 >> 32);
                x2 = (uint)w01;
                x3 = (uint)(w01 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w00 = (ulong)x0 | ((ulong)x1 << 32);
                w01 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w02;
                x1 = (uint)(w02 >> 32);
                x2 = (uint)w03;
                x3 = (uint)(w03 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w02 = (ulong)x0 | ((ulong)x1 << 32);
                w03 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w04;
                x1 = (uint)(w04 >> 32);
                x2 = (uint)w05;
                x3 = (uint)(w05 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w04 = (ulong)x0 | ((ulong)x1 << 32);
                w05 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w06;
                x1 = (uint)(w06 >> 32);
                x2 = (uint)w07;
                x3 = (uint)(w07 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w06 = (ulong)x0 | ((ulong)x1 << 32);
                w07 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w08;
                x1 = (uint)(w08 >> 32);
                x2 = (uint)w09;
                x3 = (uint)(w09 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w08 = (ulong)x0 | ((ulong)x1 << 32);
                w09 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w0a;
                x1 = (uint)(w0a >> 32);
                x2 = (uint)w0b;
                x3 = (uint)(w0b >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w0a = (ulong)x0 | ((ulong)x1 << 32);
                w0b = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w0c;
                x1 = (uint)(w0c >> 32);
                x2 = (uint)w0d;
                x3 = (uint)(w0d >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w0c = (ulong)x0 | ((ulong)x1 << 32);
                w0d = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w0e;
                x1 = (uint)(w0e >> 32);
                x2 = (uint)w0f;
                x3 = (uint)(w0f >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w0e = (ulong)x0 | ((ulong)x1 << 32);
                w0f = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w10;
                x1 = (uint)(w10 >> 32);
                x2 = (uint)w11;
                x3 = (uint)(w11 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w10 = (ulong)x0 | ((ulong)x1 << 32);
                w11 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w12;
                x1 = (uint)(w12 >> 32);
                x2 = (uint)w13;
                x3 = (uint)(w13 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w12 = (ulong)x0 | ((ulong)x1 << 32);
                w13 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w14;
                x1 = (uint)(w14 >> 32);
                x2 = (uint)w15;
                x3 = (uint)(w15 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w14 = (ulong)x0 | ((ulong)x1 << 32);
                w15 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w16;
                x1 = (uint)(w16 >> 32);
                x2 = (uint)w17;
                x3 = (uint)(w17 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w16 = (ulong)x0 | ((ulong)x1 << 32);
                w17 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w18;
                x1 = (uint)(w18 >> 32);
                x2 = (uint)w19;
                x3 = (uint)(w19 >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w18 = (ulong)x0 | ((ulong)x1 << 32);
                w19 = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w1a;
                x1 = (uint)(w1a >> 32);
                x2 = (uint)w1b;
                x3 = (uint)(w1b >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w1a = (ulong)x0 | ((ulong)x1 << 32);
                w1b = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w1c;
                x1 = (uint)(w1c >> 32);
                x2 = (uint)w1d;
                x3 = (uint)(w1d >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w1c = (ulong)x0 | ((ulong)x1 << 32);
                w1d = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                x0 = (uint)w1e;
                x1 = (uint)(w1e >> 32);
                x2 = (uint)w1f;
                x3 = (uint)(w1f >> 32);
                y0 = AES0[x0 & 0xFF] ^ AES1[(x1 >> 8) & 0xFF] ^ AES2[(x2 >> 16) & 0xFF] ^ AES3[(x3 >> 24) & 0xFF] ^ k0;
                y1 = AES0[x1 & 0xFF] ^ AES1[(x2 >> 8) & 0xFF] ^ AES2[(x3 >> 16) & 0xFF] ^ AES3[(x0 >> 24) & 0xFF] ^ k1;
                y2 = AES0[x2 & 0xFF] ^ AES1[(x3 >> 8) & 0xFF] ^ AES2[(x0 >> 16) & 0xFF] ^ AES3[(x1 >> 24) & 0xFF] ^ k2;
                y3 = AES0[x3 & 0xFF] ^ AES1[(x0 >> 8) & 0xFF] ^ AES2[(x1 >> 16) & 0xFF] ^ AES3[(x2 >> 24) & 0xFF] ^ k3;
                x0 = AES0[y0 & 0xFF] ^ AES1[(y1 >> 8) & 0xFF] ^ AES2[(y2 >> 16) & 0xFF] ^ AES3[(y3 >> 24) & 0xFF];
                x1 = AES0[y1 & 0xFF] ^ AES1[(y2 >> 8) & 0xFF] ^ AES2[(y3 >> 16) & 0xFF] ^ AES3[(y0 >> 24) & 0xFF];
                x2 = AES0[y2 & 0xFF] ^ AES1[(y3 >> 8) & 0xFF] ^ AES2[(y0 >> 16) & 0xFF] ^ AES3[(y1 >> 24) & 0xFF];
                x3 = AES0[y3 & 0xFF] ^ AES1[(y0 >> 8) & 0xFF] ^ AES2[(y1 >> 16) & 0xFF] ^ AES3[(y2 >> 24) & 0xFF];
                w1e = (ulong)x0 | ((ulong)x1 << 32);
                w1f = (ulong)x2 | ((ulong)x3 << 32);
                if (++k0 == 0)
                    if (++k1 == 0)
                        if (++k2 == 0)
                            k3++;
                #endregion

                #region Shift Rows
                tmp = w02;
                w02 = w0a;
                w0a = w12;
                w12 = w1a;
                w1a = tmp;
                tmp = w03;
                w03 = w0b;
                w0b = w13;
                w13 = w1b;
                w1b = tmp;
                tmp = w04;
                w04 = w14;
                w14 = tmp;
                tmp = w0c;
                w0c = w1c;
                w1c = tmp;
                tmp = w05;
                w05 = w15;
                w15 = tmp;
                tmp = w0d;
                w0d = w1d;
                w1d = tmp;
                tmp = w1e;
                w1e = w16;
                w16 = w0e;
                w0e = w06;
                w06 = tmp;
                tmp = w1f;
                w1f = w17;
                w17 = w0f;
                w0f = w07;
                w07 = tmp;
                #endregion

                #region Mix Columns
                a = w00;
                b = w02;
                c = w04;
                d = w06;
                ab = a ^ b;
                bc = b ^ c;
                cd = c ^ d;
                abx = ((ab & 0x8080808080808080) >> 7) * 27 ^ ((ab & 0x7F7F7F7F7F7F7F7F) << 1);
                bcx = ((bc & 0x8080808080808080) >> 7) * 27 ^ ((bc & 0x7F7F7F7F7F7F7F7F) << 1);
                cdx = ((cd & 0x8080808080808080) >> 7) * 27 ^ ((cd & 0x7F7F7F7F7F7F7F7F) << 1);
                w00 = abx ^ bc ^ d;
                w02 = bcx ^ a ^ cd;
                w04 = cdx ^ ab ^ d;
                w06 = abx ^ bcx ^ cdx ^ ab ^ c;
                a = w01;
                b = w03;
                c = w05;
                d = w07;
                ab = a ^ b;
                bc = b ^ c;
                cd = c ^ d;
                abx = ((ab & 0x8080808080808080) >> 7) * 27 ^ ((ab & 0x7F7F7F7F7F7F7F7F) << 1);
                bcx = ((bc & 0x8080808080808080) >> 7) * 27 ^ ((bc & 0x7F7F7F7F7F7F7F7F) << 1);
                cdx = ((cd & 0x8080808080808080) >> 7) * 27 ^ ((cd & 0x7F7F7F7F7F7F7F7F) << 1);
                w01 = abx ^ bc ^ d;
                w03 = bcx ^ a ^ cd;
                w05 = cdx ^ ab ^ d;
                w07 = abx ^ bcx ^ cdx ^ ab ^ c;
                a = w08;
                b = w0a;
                c = w0c;
                d = w0e;
                ab = a ^ b;
                bc = b ^ c;
                cd = c ^ d;
                abx = ((ab & 0x8080808080808080) >> 7) * 27 ^ ((ab & 0x7F7F7F7F7F7F7F7F) << 1);
                bcx = ((bc & 0x8080808080808080) >> 7) * 27 ^ ((bc & 0x7F7F7F7F7F7F7F7F) << 1);
                cdx = ((cd & 0x8080808080808080) >> 7) * 27 ^ ((cd & 0x7F7F7F7F7F7F7F7F) << 1);
                w08 = abx ^ bc ^ d;
                w0a = bcx ^ a ^ cd;
                w0c = cdx ^ ab ^ d;
                w0e = abx ^ bcx ^ cdx ^ ab ^ c;
                a = w09;
                b = w0b;
                c = w0d;
                d = w0f;
                ab = a ^ b;
                bc = b ^ c;
                cd = c ^ d;
                abx = ((ab & 0x8080808080808080) >> 7) * 27 ^ ((ab & 0x7F7F7F7F7F7F7F7F) << 1);
                bcx = ((bc & 0x8080808080808080) >> 7) * 27 ^ ((bc & 0x7F7F7F7F7F7F7F7F) << 1);
                cdx = ((cd & 0x8080808080808080) >> 7) * 27 ^ ((cd & 0x7F7F7F7F7F7F7F7F) << 1);
                w09 = abx ^ bc ^ d;
                w0b = bcx ^ a ^ cd;
                w0d = cdx ^ ab ^ d;
                w0f = abx ^ bcx ^ cdx ^ ab ^ c;
                a = w10;
                b = w12;
                c = w14;
                d = w16;
                ab = a ^ b;
                bc = b ^ c;
                cd = c ^ d;
                abx = ((ab & 0x8080808080808080) >> 7) * 27 ^ ((ab & 0x7F7F7F7F7F7F7F7F) << 1);
                bcx = ((bc & 0x8080808080808080) >> 7) * 27 ^ ((bc & 0x7F7F7F7F7F7F7F7F) << 1);
                cdx = ((cd & 0x8080808080808080) >> 7) * 27 ^ ((cd & 0x7F7F7F7F7F7F7F7F) << 1);
                w10 = abx ^ bc ^ d;
                w12 = bcx ^ a ^ cd;
                w14 = cdx ^ ab ^ d;
                w16 = abx ^ bcx ^ cdx ^ ab ^ c;
                a = w11;
                b = w13;
                c = w15;
                d = w17;
                ab = a ^ b;
                bc = b ^ c;
                cd = c ^ d;
                abx = ((ab & 0x8080808080808080) >> 7) * 27 ^ ((ab & 0x7F7F7F7F7F7F7F7F) << 1);
                bcx = ((bc & 0x8080808080808080) >> 7) * 27 ^ ((bc & 0x7F7F7F7F7F7F7F7F) << 1);
                cdx = ((cd & 0x8080808080808080) >> 7) * 27 ^ ((cd & 0x7F7F7F7F7F7F7F7F) << 1);
                w11 = abx ^ bc ^ d;
                w13 = bcx ^ a ^ cd;
                w15 = cdx ^ ab ^ d;
                w17 = abx ^ bcx ^ cdx ^ ab ^ c;
                a = w18;
                b = w1a;
                c = w1c;
                d = w1e;
                ab = a ^ b;
                bc = b ^ c;
                cd = c ^ d;
                abx = ((ab & 0x8080808080808080) >> 7) * 27 ^ ((ab & 0x7F7F7F7F7F7F7F7F) << 1);
                bcx = ((bc & 0x8080808080808080) >> 7) * 27 ^ ((bc & 0x7F7F7F7F7F7F7F7F) << 1);
                cdx = ((cd & 0x8080808080808080) >> 7) * 27 ^ ((cd & 0x7F7F7F7F7F7F7F7F) << 1);
                w18 = abx ^ bc ^ d;
                w1a = bcx ^ a ^ cd;
                w1c = cdx ^ ab ^ d;
                w1e = abx ^ bcx ^ cdx ^ ab ^ c;
                a = w19;
                b = w1b;
                c = w1d;
                d = w1f;
                ab = a ^ b;
                bc = b ^ c;
                cd = c ^ d;
                abx = ((ab & 0x8080808080808080) >> 7) * 27 ^ ((ab & 0x7F7F7F7F7F7F7F7F) << 1);
                bcx = ((bc & 0x8080808080808080) >> 7) * 27 ^ ((bc & 0x7F7F7F7F7F7F7F7F) << 1);
                cdx = ((cd & 0x8080808080808080) >> 7) * 27 ^ ((cd & 0x7F7F7F7F7F7F7F7F) << 1);
                w19 = abx ^ bc ^ d;
                w1b = bcx ^ a ^ cd;
                w1d = cdx ^ ab ^ d;
                w1f = abx ^ bcx ^ cdx ^ ab ^ c;
                #endregion
            }

            v0 ^= ToUInt64(buffer, 0x00, ByteOrder.LittleEndian) ^ w00 ^ w10;
            v1 ^= ToUInt64(buffer, 0x08, ByteOrder.LittleEndian) ^ w01 ^ w11;
            v2 ^= ToUInt64(buffer, 0x10, ByteOrder.LittleEndian) ^ w02 ^ w12;
            v3 ^= ToUInt64(buffer, 0x18, ByteOrder.LittleEndian) ^ w03 ^ w13;
            v4 ^= ToUInt64(buffer, 0x20, ByteOrder.LittleEndian) ^ w04 ^ w14;
            v5 ^= ToUInt64(buffer, 0x28, ByteOrder.LittleEndian) ^ w05 ^ w15;
            v6 ^= ToUInt64(buffer, 0x30, ByteOrder.LittleEndian) ^ w06 ^ w16;
            v7 ^= ToUInt64(buffer, 0x38, ByteOrder.LittleEndian) ^ w07 ^ w17;
            v8 ^= ToUInt64(buffer, 0x40, ByteOrder.LittleEndian) ^ w08 ^ w18;
            v9 ^= ToUInt64(buffer, 0x48, ByteOrder.LittleEndian) ^ w09 ^ w19;
            va ^= ToUInt64(buffer, 0x50, ByteOrder.LittleEndian) ^ w0a ^ w1a;
            vb ^= ToUInt64(buffer, 0x58, ByteOrder.LittleEndian) ^ w0b ^ w1b;
            vc ^= ToUInt64(buffer, 0x60, ByteOrder.LittleEndian) ^ w0c ^ w1c;
            vd ^= ToUInt64(buffer, 0x68, ByteOrder.LittleEndian) ^ w0d ^ w1d;
            ve ^= ToUInt64(buffer, 0x70, ByteOrder.LittleEndian) ^ w0e ^ w1e;
            vf ^= ToUInt64(buffer, 0x78, ByteOrder.LittleEndian) ^ w0f ^ w1f;
        }
    }
}