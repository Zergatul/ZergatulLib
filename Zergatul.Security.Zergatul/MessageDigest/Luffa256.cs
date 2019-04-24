using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.LuffaConstants;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Luffa256 : AbstractMessageDigest
    {
        public override int BlockLength => 32;
        public override int DigestLength => 32;

        protected uint h00, h01, h02, h03, h04, h05, h06, h07;
        protected uint h10, h11, h12, h13, h14, h15, h16, h17;
        protected uint h20, h21, h22, h23, h24, h25, h26, h27;

        public Luffa256()
        {
            buffer = new byte[32];
            Reset();
        }

        public override void Reset()
        {
            h00 = 0x6D251E69;
            h01 = 0x44B051E0;
            h02 = 0x4EAA6FB4;
            h03 = 0xDBF78465;
            h04 = 0x6E292011;
            h05 = 0x90152DF4;
            h06 = 0xEE058139;
            h07 = 0xDEF610BB;
            h10 = 0xC3B44B95;
            h11 = 0xD9D2F256;
            h12 = 0x70EEE9A0;
            h13 = 0xDE099FA3;
            h14 = 0x5D9B0557;
            h15 = 0x8FC944B3;
            h16 = 0xCF1CCF0E;
            h17 = 0x746CD581;
            h20 = 0xF7EFC89D;
            h21 = 0x5DBA5781;
            h22 = 0x04016CE5;
            h23 = 0xAD659C05;
            h24 = 0x0306194F;
            h25 = 0x666D1836;
            h26 = 0x24AA230A;
            h27 = 0x8B264AE7;

            bufOffset = 0;
        }

        public override byte[] Digest()
        {
            buffer[bufOffset++] = 0x80;
            while (bufOffset < 32)
                buffer[bufOffset++] = 0;

            bufOffset = 0;
            ProcessBlock();

            while (bufOffset < 32)
                buffer[bufOffset++] = 0;

            ProcessBlock();

            return InternalStateToDigest();
        }

        protected virtual byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[32];
            GetBytes(h00 ^ h10 ^ h20, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h01 ^ h11 ^ h21, ByteOrder.BigEndian, digest, 0x04);
            GetBytes(h02 ^ h12 ^ h22, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h03 ^ h13 ^ h23, ByteOrder.BigEndian, digest, 0x0C);
            GetBytes(h04 ^ h14 ^ h24, ByteOrder.BigEndian, digest, 0x10);
            GetBytes(h05 ^ h15 ^ h25, ByteOrder.BigEndian, digest, 0x14);
            GetBytes(h06 ^ h16 ^ h26, ByteOrder.BigEndian, digest, 0x18);
            GetBytes(h07 ^ h17 ^ h27, ByteOrder.BigEndian, digest, 0x1C);
            return digest;
        }

        protected override void ProcessBlock()
        {
            uint v00 = h00;
            uint v01 = h01;
            uint v02 = h02;
            uint v03 = h03;
            uint v04 = h04;
            uint v05 = h05;
            uint v06 = h06;
            uint v07 = h07;
            uint v10 = h10;
            uint v11 = h11;
            uint v12 = h12;
            uint v13 = h13;
            uint v14 = h14;
            uint v15 = h15;
            uint v16 = h16;
            uint v17 = h17;
            uint v20 = h20;
            uint v21 = h21;
            uint v22 = h22;
            uint v23 = h23;
            uint v24 = h24;
            uint v25 = h25;
            uint v26 = h26;
            uint v27 = h27;

            #region MI3

            uint m0 = ToUInt32(buffer, 0x00, ByteOrder.BigEndian);
            uint m1 = ToUInt32(buffer, 0x04, ByteOrder.BigEndian);
            uint m2 = ToUInt32(buffer, 0x08, ByteOrder.BigEndian);
            uint m3 = ToUInt32(buffer, 0x0C, ByteOrder.BigEndian);
            uint m4 = ToUInt32(buffer, 0x10, ByteOrder.BigEndian);
            uint m5 = ToUInt32(buffer, 0x14, ByteOrder.BigEndian);
            uint m6 = ToUInt32(buffer, 0x18, ByteOrder.BigEndian);
            uint m7 = ToUInt32(buffer, 0x1C, ByteOrder.BigEndian);

            uint a0, a1, a2, a3, a4, a5, a6, a7;
            uint tmp;

            a0 = v00 ^ v10;
            a1 = v01 ^ v11;
            a2 = v02 ^ v12;
            a3 = v03 ^ v13;
            a4 = v04 ^ v14;
            a5 = v05 ^ v15;
            a6 = v06 ^ v16;
            a7 = v07 ^ v17;
            a0 = a0 ^ v20;
            a1 = a1 ^ v21;
            a2 = a2 ^ v22;
            a3 = a3 ^ v23;
            a4 = a4 ^ v24;
            a5 = a5 ^ v25;
            a6 = a6 ^ v26;
            a7 = a7 ^ v27;
            tmp = a7;
            a7 = a6;
            a6 = a5;
            a5 = a4;
            a4 = a3 ^ tmp;
            a3 = a2 ^ tmp;
            a2 = a1;
            a1 = a0 ^ tmp;
            a0 = tmp;
            v00 = a0 ^ v00;
            v01 = a1 ^ v01;
            v02 = a2 ^ v02;
            v03 = a3 ^ v03;
            v04 = a4 ^ v04;
            v05 = a5 ^ v05;
            v06 = a6 ^ v06;
            v07 = a7 ^ v07;
            v00 = m0 ^ v00;
            v01 = m1 ^ v01;
            v02 = m2 ^ v02;
            v03 = m3 ^ v03;
            v04 = m4 ^ v04;
            v05 = m5 ^ v05;
            v06 = m6 ^ v06;
            v07 = m7 ^ v07;
            tmp = m7;
            m7 = m6;
            m6 = m5;
            m5 = m4;
            m4 = m3 ^ tmp;
            m3 = m2 ^ tmp;
            m2 = m1;
            m1 = m0 ^ tmp;
            m0 = tmp;
            v10 = a0 ^ v10;
            v11 = a1 ^ v11;
            v12 = a2 ^ v12;
            v13 = a3 ^ v13;
            v14 = a4 ^ v14;
            v15 = a5 ^ v15;
            v16 = a6 ^ v16;
            v17 = a7 ^ v17;
            v10 = m0 ^ v10;
            v11 = m1 ^ v11;
            v12 = m2 ^ v12;
            v13 = m3 ^ v13;
            v14 = m4 ^ v14;
            v15 = m5 ^ v15;
            v16 = m6 ^ v16;
            v17 = m7 ^ v17;
            tmp = m7;
            m7 = m6;
            m6 = m5;
            m5 = m4;
            m4 = m3 ^ tmp;
            m3 = m2 ^ tmp;
            m2 = m1;
            m1 = m0 ^ tmp;
            m0 = tmp;
            v20 = a0 ^ v20;
            v21 = a1 ^ v21;
            v22 = a2 ^ v22;
            v23 = a3 ^ v23;
            v24 = a4 ^ v24;
            v25 = a5 ^ v25;
            v26 = a6 ^ v26;
            v27 = a7 ^ v27;
            v20 = m0 ^ v20;
            v21 = m1 ^ v21;
            v22 = m2 ^ v22;
            v23 = m3 ^ v23;
            v24 = m4 ^ v24;
            v25 = m5 ^ v25;
            v26 = m6 ^ v26;
            v27 = m7 ^ v27;

            #endregion

            #region P3

            ulong w0, w1, w2, w3, w4, w5, w6, w7;

            v14 = RotateLeft(v14, 1);
            v15 = RotateLeft(v15, 1);
            v16 = RotateLeft(v16, 1);
            v17 = RotateLeft(v17, 1);
            v24 = RotateLeft(v24, 2);
            v25 = RotateLeft(v25, 2);
            v26 = RotateLeft(v26, 2);
            v27 = RotateLeft(v27, 2);

            w0 = v00 | ((ulong)v10 << 32);
            w1 = v01 | ((ulong)v11 << 32);
            w2 = v02 | ((ulong)v12 << 32);
            w3 = v03 | ((ulong)v13 << 32);
            w4 = v04 | ((ulong)v14 << 32);
            w5 = v05 | ((ulong)v15 << 32);
            w6 = v06 | ((ulong)v16 << 32);
            w7 = v07 | ((ulong)v17 << 32);

            for (int r = 0; r < 8; r++)
            {
                ulong ltmp;
                uint ul, uh, vl, vh;
                ltmp = w0;
                w0 |= w1;
                w2 ^= w3;
                w1 = ~w1;
                w0 ^= w3;
                w3 &= ltmp;
                w1 ^= w3;
                w3 ^= w2;
                w2 &= w0;
                w0 = ~w0;
                w2 ^= w1;
                w1 |= w3;
                ltmp ^= w1;
                w3 ^= w2;
                w2 &= w1;
                w1 ^= w0;
                w0 = ltmp;
                ltmp = w5;
                w5 |= w6;
                w7 ^= w4;
                w6 = ~w6;
                w5 ^= w4;
                w4 &= ltmp;
                w6 ^= w4;
                w4 ^= w7;
                w7 &= w5;
                w5 = ~w5;
                w7 ^= w6;
                w6 |= w4;
                ltmp ^= w6;
                w4 ^= w7;
                w7 &= w6;
                w6 ^= w5;
                w5 = ltmp;
                w4 ^= w0;
                ul = (uint)w0;
                uh = (uint)(w0 >> 32);
                vl = (uint)w4;
                vh = (uint)(w4 >> 32);
                ul = RotateLeft(ul, 2) ^ vl;
                vl = RotateLeft(vl, 14) ^ ul;
                ul = RotateLeft(ul, 10) ^ vl;
                vl = RotateLeft(vl, 1);
                uh = RotateLeft(uh, 2) ^ vh;
                vh = RotateLeft(vh, 14) ^ uh;
                uh = RotateLeft(uh, 10) ^ vh;
                vh = RotateLeft(vh, 1);
                w0 = ul | ((ulong)uh << 32);
                w4 = vl | ((ulong)vh << 32);
                w5 ^= w1;
                ul = (uint)w1;
                uh = (uint)(w1 >> 32);
                vl = (uint)w5;
                vh = (uint)(w5 >> 32);
                ul = RotateLeft(ul, 2) ^ vl;
                vl = RotateLeft(vl, 14) ^ ul;
                ul = RotateLeft(ul, 10) ^ vl;
                vl = RotateLeft(vl, 1);
                uh = RotateLeft(uh, 2) ^ vh;
                vh = RotateLeft(vh, 14) ^ uh;
                uh = RotateLeft(uh, 10) ^ vh;
                vh = RotateLeft(vh, 1);
                w1 = ul | ((ulong)uh << 32);
                w5 = vl | ((ulong)vh << 32);
                w6 ^= w2;
                ul = (uint)w2;
                uh = (uint)(w2 >> 32);
                vl = (uint)w6;
                vh = (uint)(w6 >> 32);
                ul = RotateLeft(ul, 2) ^ vl;
                vl = RotateLeft(vl, 14) ^ ul;
                ul = RotateLeft(ul, 10) ^ vl;
                vl = RotateLeft(vl, 1);
                uh = RotateLeft(uh, 2) ^ vh;
                vh = RotateLeft(vh, 14) ^ uh;
                uh = RotateLeft(uh, 10) ^ vh;
                vh = RotateLeft(vh, 1);
                w2 = ul | ((ulong)uh << 32);
                w6 = vl | ((ulong)vh << 32);
                w7 ^= w3;
                ul = (uint)w3;
                uh = (uint)(w3 >> 32);
                vl = (uint)w7;
                vh = (uint)(w7 >> 32);
                ul = RotateLeft(ul, 2) ^ vl;
                vl = RotateLeft(vl, 14) ^ ul;
                ul = RotateLeft(ul, 10) ^ vl;
                vl = RotateLeft(vl, 1);
                uh = RotateLeft(uh, 2) ^ vh;
                vh = RotateLeft(vh, 14) ^ uh;
                uh = RotateLeft(uh, 10) ^ vh;
                vh = RotateLeft(vh, 1);
                w3 = ul | ((ulong)uh << 32);
                w7 = vl | ((ulong)vh << 32);
                w0 ^= RCW010[r];
                w4 ^= RCW014[r];
            }

            v00 = (uint)w0;
            v10 = (uint)(w0 >> 32);
            v01 = (uint)w1;
            v11 = (uint)(w1 >> 32);
            v02 = (uint)w2;
            v12 = (uint)(w2 >> 32);
            v03 = (uint)w3;
            v13 = (uint)(w3 >> 32);
            v04 = (uint)w4;
            v14 = (uint)(w4 >> 32);
            v05 = (uint)w5;
            v15 = (uint)(w5 >> 32);
            v06 = (uint)w6;
            v16 = (uint)(w6 >> 32);
            v07 = (uint)w7;
            v17 = (uint)(w7 >> 32);

            for (int r = 0; r < 8; r++)
            {
                tmp = v20;
                v20 |= v21;
                v22 ^= v23;
                v21 = ~v21;
                v20 ^= v23;
                v23 &= tmp;
                v21 ^= v23;
                v23 ^= v22;
                v22 &= v20;
                v20 = ~v20;
                v22 ^= v21;
                v21 |= v23;
                tmp ^= v21;
                v23 ^= v22;
                v22 &= v21;
                v21 ^= v20;
                v20 = tmp;
                tmp = v25;
                v25 |= v26;
                v27 ^= v24;
                v26 = ~v26;
                v25 ^= v24;
                v24 &= tmp;
                v26 ^= v24;
                v24 ^= v27;
                v27 &= v25;
                v25 = ~v25;
                v27 ^= v26;
                v26 |= v24;
                tmp ^= v26;
                v24 ^= v27;
                v27 &= v26;
                v26 ^= v25;
                v25 = tmp;
                v24 ^= v20;
                v20 = RotateLeft(v20, 2) ^ v24;
                v24 = RotateLeft(v24, 14) ^ v20;
                v20 = RotateLeft(v20, 10) ^ v24;
                v24 = RotateLeft(v24, 1);
                v25 ^= v21;
                v21 = RotateLeft(v21, 2) ^ v25;
                v25 = RotateLeft(v25, 14) ^ v21;
                v21 = RotateLeft(v21, 10) ^ v25;
                v25 = RotateLeft(v25, 1);
                v26 ^= v22;
                v22 = RotateLeft(v22, 2) ^ v26;
                v26 = RotateLeft(v26, 14) ^ v22;
                v22 = RotateLeft(v22, 10) ^ v26;
                v26 = RotateLeft(v26, 1);
                v27 ^= v23;
                v23 = RotateLeft(v23, 2) ^ v27;
                v27 = RotateLeft(v27, 14) ^ v23;
                v23 = RotateLeft(v23, 10) ^ v27;
                v27 = RotateLeft(v27, 1);
                v20 ^= RC20[r];
                v24 ^= RC24[r];
            }

            #endregion

            h00 = v00;
            h01 = v01;
            h02 = v02;
            h03 = v03;
            h04 = v04;
            h05 = v05;
            h06 = v06;
            h07 = v07;
            h10 = v10;
            h11 = v11;
            h12 = v12;
            h13 = v13;
            h14 = v14;
            h15 = v15;
            h16 = v16;
            h17 = v17;
            h20 = v20;
            h21 = v21;
            h22 = v22;
            h23 = v23;
            h24 = v24;
            h25 = v25;
            h26 = v26;
            h27 = v27;

        }
    }
}