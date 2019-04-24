using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Luffa512 : AbstractMessageDigest
    {
        public override int BlockLength => 32;
        public override int DigestLength => 64;

        #region Constants

        private static readonly ulong[] RCW010 = new ulong[]
        {
            0xB6DE10ED303994A6, 0x70F47AAEC0E65299,
            0x0707A3D46CC33A12, 0x1C1E8F51DC56983E,
            0x707A3D451E00108F, 0xAEB285627800423D,
            0xBACA15898F5B7882, 0x40A46F3E96E1DB12
        };

        private static readonly ulong[] RCW014 = new ulong[]
        {
            0x01685F3DE0337818, 0x05A17CF4441BA90D,
            0xBD09CACA7F34D442, 0xF4272B289389217F,
            0x144AE5CCE5A8BCE6, 0xFAA7AE2B5274BAF4,
            0x2E48F1C126889BA7, 0xB923C7049A226E9D
        };

        private static readonly ulong[] RCW230 = new ulong[]
        {
            0xB213AFA5FC20D9D2, 0xC84EBE9534552E25,
            0x4E608A227AD8818F, 0x56D858FE8438764A,
            0x343B138FBB6DE032, 0xD0EC4E3DEDB780C8,
            0x2CEB4882D9847356, 0xB3AD2208A2C78434
        };


        private static readonly ulong[] RCW234 = new ulong[]
        {
            0xE028C9BFE25E72C1, 0x44756F91E623BB72,
            0x7E8FCE325C58A4A4, 0x956548BE1E38E2E7,
            0xFE191BE278E38B9D, 0x3CB226E527586719,
            0x5944A28E36EDA57F, 0xA1C4C355703AACE7
        };

        private static readonly uint[] RC40 = new uint[]
        {
            0xF0D2E9E3, 0xAC11D7FA,
            0x1BCB66F2, 0x6F2D9BC9,
            0x78602649, 0x8EDAE952,
            0x3B6BA548, 0xEDAE9520
        };

        private static readonly uint[] RC44 = new uint[]
        {
            0x5090D577, 0x2D1925AB,
            0xB46496AC, 0xD1925AB0,
            0x29131AB6, 0x0FC053C3,
            0x3F014F0C, 0xFC053C31
        };

        #endregion

        uint h00, h01, h02, h03, h04, h05, h06, h07;
        uint h10, h11, h12, h13, h14, h15, h16, h17;
        uint h20, h21, h22, h23, h24, h25, h26, h27;
        uint h30, h31, h32, h33, h34, h35, h36, h37;
        uint h40, h41, h42, h43, h44, h45, h46, h47;

        public Luffa512()
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
            h30 = 0x858075D5;
            h31 = 0x36D79CCE;
            h32 = 0xE571F7D7;
            h33 = 0x204B1F67;
            h34 = 0x35870C6A;
            h35 = 0x57E9E923;
            h36 = 0x14BCB808;
            h37 = 0x7CDE72CE;
            h40 = 0x6C68E9BE;
            h41 = 0x5EC41E22;
            h42 = 0xC825B7C7;
            h43 = 0xAFFB4363;
            h44 = 0xF5DF3999;
            h45 = 0x0FC688F1;
            h46 = 0xB07224CC;
            h47 = 0x03E86CEA;

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

            byte[] digest = new byte[64];
            GetBytes(h00 ^ h10 ^ h20 ^ h30 ^ h40, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h01 ^ h11 ^ h21 ^ h31 ^ h41, ByteOrder.BigEndian, digest, 0x04);
            GetBytes(h02 ^ h12 ^ h22 ^ h32 ^ h42, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h03 ^ h13 ^ h23 ^ h33 ^ h43, ByteOrder.BigEndian, digest, 0x0C);
            GetBytes(h04 ^ h14 ^ h24 ^ h34 ^ h44, ByteOrder.BigEndian, digest, 0x10);
            GetBytes(h05 ^ h15 ^ h25 ^ h35 ^ h45, ByteOrder.BigEndian, digest, 0x14);
            GetBytes(h06 ^ h16 ^ h26 ^ h36 ^ h46, ByteOrder.BigEndian, digest, 0x18);
            GetBytes(h07 ^ h17 ^ h27 ^ h37 ^ h47, ByteOrder.BigEndian, digest, 0x1C);

            ProcessBlock();

            GetBytes(h00 ^ h10 ^ h20 ^ h30 ^ h40, ByteOrder.BigEndian, digest, 0x20);
            GetBytes(h01 ^ h11 ^ h21 ^ h31 ^ h41, ByteOrder.BigEndian, digest, 0x24);
            GetBytes(h02 ^ h12 ^ h22 ^ h32 ^ h42, ByteOrder.BigEndian, digest, 0x28);
            GetBytes(h03 ^ h13 ^ h23 ^ h33 ^ h43, ByteOrder.BigEndian, digest, 0x2C);
            GetBytes(h04 ^ h14 ^ h24 ^ h34 ^ h44, ByteOrder.BigEndian, digest, 0x30);
            GetBytes(h05 ^ h15 ^ h25 ^ h35 ^ h45, ByteOrder.BigEndian, digest, 0x34);
            GetBytes(h06 ^ h16 ^ h26 ^ h36 ^ h46, ByteOrder.BigEndian, digest, 0x38);
            GetBytes(h07 ^ h17 ^ h27 ^ h37 ^ h47, ByteOrder.BigEndian, digest, 0x3C);

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
            uint v30 = h30;
            uint v31 = h31;
            uint v32 = h32;
            uint v33 = h33;
            uint v34 = h34;
            uint v35 = h35;
            uint v36 = h36;
            uint v37 = h37;
            uint v40 = h40;
            uint v41 = h41;
            uint v42 = h42;
            uint v43 = h43;
            uint v44 = h44;
            uint v45 = h45;
            uint v46 = h46;
            uint v47 = h47;

            #region MI5

            uint m0 = ToUInt32(buffer, 0x00, ByteOrder.BigEndian);
            uint m1 = ToUInt32(buffer, 0x04, ByteOrder.BigEndian);
            uint m2 = ToUInt32(buffer, 0x08, ByteOrder.BigEndian);
            uint m3 = ToUInt32(buffer, 0x0C, ByteOrder.BigEndian);
            uint m4 = ToUInt32(buffer, 0x10, ByteOrder.BigEndian);
            uint m5 = ToUInt32(buffer, 0x14, ByteOrder.BigEndian);
            uint m6 = ToUInt32(buffer, 0x18, ByteOrder.BigEndian);
            uint m7 = ToUInt32(buffer, 0x1C, ByteOrder.BigEndian);

            uint a0, a1, a2, a3, a4, a5, a6, a7;
            uint b0, b1, b2, b3, b4, b5, b6, b7;
            uint tmp;

            a0 = v00 ^ v10;
            a1 = v01 ^ v11;
            a2 = v02 ^ v12;
            a3 = v03 ^ v13;
            a4 = v04 ^ v14;
            a5 = v05 ^ v15;
            a6 = v06 ^ v16;
            a7 = v07 ^ v17;
            b0 = v20 ^ v30;
            b1 = v21 ^ v31;
            b2 = v22 ^ v32;
            b3 = v23 ^ v33;
            b4 = v24 ^ v34;
            b5 = v25 ^ v35;
            b6 = v26 ^ v36;
            b7 = v27 ^ v37;
            a0 = a0 ^ b0;
            a1 = a1 ^ b1;
            a2 = a2 ^ b2;
            a3 = a3 ^ b3;
            a4 = a4 ^ b4;
            a5 = a5 ^ b5;
            a6 = a6 ^ b6;
            a7 = a7 ^ b7;
            a0 = a0 ^ v40;
            a1 = a1 ^ v41;
            a2 = a2 ^ v42;
            a3 = a3 ^ v43;
            a4 = a4 ^ v44;
            a5 = a5 ^ v45;
            a6 = a6 ^ v46;
            a7 = a7 ^ v47;
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
            v10 = a0 ^ v10;
            v11 = a1 ^ v11;
            v12 = a2 ^ v12;
            v13 = a3 ^ v13;
            v14 = a4 ^ v14;
            v15 = a5 ^ v15;
            v16 = a6 ^ v16;
            v17 = a7 ^ v17;
            v20 = a0 ^ v20;
            v21 = a1 ^ v21;
            v22 = a2 ^ v22;
            v23 = a3 ^ v23;
            v24 = a4 ^ v24;
            v25 = a5 ^ v25;
            v26 = a6 ^ v26;
            v27 = a7 ^ v27;
            v30 = a0 ^ v30;
            v31 = a1 ^ v31;
            v32 = a2 ^ v32;
            v33 = a3 ^ v33;
            v34 = a4 ^ v34;
            v35 = a5 ^ v35;
            v36 = a6 ^ v36;
            v37 = a7 ^ v37;
            v40 = a0 ^ v40;
            v41 = a1 ^ v41;
            v42 = a2 ^ v42;
            v43 = a3 ^ v43;
            v44 = a4 ^ v44;
            v45 = a5 ^ v45;
            v46 = a6 ^ v46;
            v47 = a7 ^ v47;
            tmp = v07;
            b7 = v06;
            b6 = v05;
            b5 = v04;
            b4 = v03 ^ tmp;
            b3 = v02 ^ tmp;
            b2 = v01;
            b1 = v00 ^ tmp;
            b0 = tmp;
            b0 = b0 ^ v10;
            b1 = b1 ^ v11;
            b2 = b2 ^ v12;
            b3 = b3 ^ v13;
            b4 = b4 ^ v14;
            b5 = b5 ^ v15;
            b6 = b6 ^ v16;
            b7 = b7 ^ v17;
            tmp = v17;
            v17 = v16;
            v16 = v15;
            v15 = v14;
            v14 = v13 ^ tmp;
            v13 = v12 ^ tmp;
            v12 = v11;
            v11 = v10 ^ tmp;
            v10 = tmp;
            v10 = v10 ^ v20;
            v11 = v11 ^ v21;
            v12 = v12 ^ v22;
            v13 = v13 ^ v23;
            v14 = v14 ^ v24;
            v15 = v15 ^ v25;
            v16 = v16 ^ v26;
            v17 = v17 ^ v27;
            tmp = v27;
            v27 = v26;
            v26 = v25;
            v25 = v24;
            v24 = v23 ^ tmp;
            v23 = v22 ^ tmp;
            v22 = v21;
            v21 = v20 ^ tmp;
            v20 = tmp;
            v20 = v20 ^ v30;
            v21 = v21 ^ v31;
            v22 = v22 ^ v32;
            v23 = v23 ^ v33;
            v24 = v24 ^ v34;
            v25 = v25 ^ v35;
            v26 = v26 ^ v36;
            v27 = v27 ^ v37;
            tmp = v37;
            v37 = v36;
            v36 = v35;
            v35 = v34;
            v34 = v33 ^ tmp;
            v33 = v32 ^ tmp;
            v32 = v31;
            v31 = v30 ^ tmp;
            v30 = tmp;
            v30 = v30 ^ v40;
            v31 = v31 ^ v41;
            v32 = v32 ^ v42;
            v33 = v33 ^ v43;
            v34 = v34 ^ v44;
            v35 = v35 ^ v45;
            v36 = v36 ^ v46;
            v37 = v37 ^ v47;
            tmp = v47;
            v47 = v46;
            v46 = v45;
            v45 = v44;
            v44 = v43 ^ tmp;
            v43 = v42 ^ tmp;
            v42 = v41;
            v41 = v40 ^ tmp;
            v40 = tmp;
            v40 = v40 ^ v00;
            v41 = v41 ^ v01;
            v42 = v42 ^ v02;
            v43 = v43 ^ v03;
            v44 = v44 ^ v04;
            v45 = v45 ^ v05;
            v46 = v46 ^ v06;
            v47 = v47 ^ v07;
            tmp = b7;
            v07 = b6;
            v06 = b5;
            v05 = b4;
            v04 = b3 ^ tmp;
            v03 = b2 ^ tmp;
            v02 = b1;
            v01 = b0 ^ tmp;
            v00 = tmp;
            v00 = v00 ^ v40;
            v01 = v01 ^ v41;
            v02 = v02 ^ v42;
            v03 = v03 ^ v43;
            v04 = v04 ^ v44;
            v05 = v05 ^ v45;
            v06 = v06 ^ v46;
            v07 = v07 ^ v47;
            tmp = v47;
            v47 = v46;
            v46 = v45;
            v45 = v44;
            v44 = v43 ^ tmp;
            v43 = v42 ^ tmp;
            v42 = v41;
            v41 = v40 ^ tmp;
            v40 = tmp;
            v40 = v40 ^ v30;
            v41 = v41 ^ v31;
            v42 = v42 ^ v32;
            v43 = v43 ^ v33;
            v44 = v44 ^ v34;
            v45 = v45 ^ v35;
            v46 = v46 ^ v36;
            v47 = v47 ^ v37;
            tmp = v37;
            v37 = v36;
            v36 = v35;
            v35 = v34;
            v34 = v33 ^ tmp;
            v33 = v32 ^ tmp;
            v32 = v31;
            v31 = v30 ^ tmp;
            v30 = tmp;
            v30 = v30 ^ v20;
            v31 = v31 ^ v21;
            v32 = v32 ^ v22;
            v33 = v33 ^ v23;
            v34 = v34 ^ v24;
            v35 = v35 ^ v25;
            v36 = v36 ^ v26;
            v37 = v37 ^ v27;
            tmp = v27;
            v27 = v26;
            v26 = v25;
            v25 = v24;
            v24 = v23 ^ tmp;
            v23 = v22 ^ tmp;
            v22 = v21;
            v21 = v20 ^ tmp;
            v20 = tmp;
            v20 = v20 ^ v10;
            v21 = v21 ^ v11;
            v22 = v22 ^ v12;
            v23 = v23 ^ v13;
            v24 = v24 ^ v14;
            v25 = v25 ^ v15;
            v26 = v26 ^ v16;
            v27 = v27 ^ v17;
            tmp = v17;
            v17 = v16;
            v16 = v15;
            v15 = v14;
            v14 = v13 ^ tmp;
            v13 = v12 ^ tmp;
            v12 = v11;
            v11 = v10 ^ tmp;
            v10 = tmp;
            v10 = v10 ^ b0;
            v11 = v11 ^ b1;
            v12 = v12 ^ b2;
            v13 = v13 ^ b3;
            v14 = v14 ^ b4;
            v15 = v15 ^ b5;
            v16 = v16 ^ b6;
            v17 = v17 ^ b7;
            v00 = v00 ^ m0;
            v01 = v01 ^ m1;
            v02 = v02 ^ m2;
            v03 = v03 ^ m3;
            v04 = v04 ^ m4;
            v05 = v05 ^ m5;
            v06 = v06 ^ m6;
            v07 = v07 ^ m7;
            tmp = m7;
            m7 = m6;
            m6 = m5;
            m5 = m4;
            m4 = m3 ^ tmp;
            m3 = m2 ^ tmp;
            m2 = m1;
            m1 = m0 ^ tmp;
            m0 = tmp;
            v10 = v10 ^ m0;
            v11 = v11 ^ m1;
            v12 = v12 ^ m2;
            v13 = v13 ^ m3;
            v14 = v14 ^ m4;
            v15 = v15 ^ m5;
            v16 = v16 ^ m6;
            v17 = v17 ^ m7;
            tmp = m7;
            m7 = m6;
            m6 = m5;
            m5 = m4;
            m4 = m3 ^ tmp;
            m3 = m2 ^ tmp;
            m2 = m1;
            m1 = m0 ^ tmp;
            m0 = tmp;
            v20 = v20 ^ m0;
            v21 = v21 ^ m1;
            v22 = v22 ^ m2;
            v23 = v23 ^ m3;
            v24 = v24 ^ m4;
            v25 = v25 ^ m5;
            v26 = v26 ^ m6;
            v27 = v27 ^ m7;
            tmp = m7;
            m7 = m6;
            m6 = m5;
            m5 = m4;
            m4 = m3 ^ tmp;
            m3 = m2 ^ tmp;
            m2 = m1;
            m1 = m0 ^ tmp;
            m0 = tmp;
            v30 = v30 ^ m0;
            v31 = v31 ^ m1;
            v32 = v32 ^ m2;
            v33 = v33 ^ m3;
            v34 = v34 ^ m4;
            v35 = v35 ^ m5;
            v36 = v36 ^ m6;
            v37 = v37 ^ m7;
            tmp = m7;
            m7 = m6;
            m6 = m5;
            m5 = m4;
            m4 = m3 ^ tmp;
            m3 = m2 ^ tmp;
            m2 = m1;
            m1 = m0 ^ tmp;
            m0 = tmp;
            v40 = v40 ^ m0;
            v41 = v41 ^ m1;
            v42 = v42 ^ m2;
            v43 = v43 ^ m3;
            v44 = v44 ^ m4;
            v45 = v45 ^ m5;
            v46 = v46 ^ m6;
            v47 = v47 ^ m7;

            #endregion

            #region P5

            ulong w0, w1, w2, w3, w4, w5, w6, w7;

            v14 = RotateLeft(v14, 1);
            v15 = RotateLeft(v15, 1);
            v16 = RotateLeft(v16, 1);
            v17 = RotateLeft(v17, 1);
            v24 = RotateLeft(v24, 2);
            v25 = RotateLeft(v25, 2);
            v26 = RotateLeft(v26, 2);
            v27 = RotateLeft(v27, 2);
            v34 = RotateLeft(v34, 3);
            v35 = RotateLeft(v35, 3);
            v36 = RotateLeft(v36, 3);
            v37 = RotateLeft(v37, 3);
            v44 = RotateLeft(v44, 4);
            v45 = RotateLeft(v45, 4);
            v46 = RotateLeft(v46, 4);
            v47 = RotateLeft(v47, 4);

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
                ulong ul, uh, vl, vh;
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

            w0 = v20 | ((ulong)v30 << 32);
            w1 = v21 | ((ulong)v31 << 32);
            w2 = v22 | ((ulong)v32 << 32);
            w3 = v23 | ((ulong)v33 << 32);
            w4 = v24 | ((ulong)v34 << 32);
            w5 = v25 | ((ulong)v35 << 32);
            w6 = v26 | ((ulong)v36 << 32);
            w7 = v27 | ((ulong)v37 << 32);

            for (int r = 0; r < 8; r++)
            {
                ulong ltmp;
                ulong ul, uh, vl, vh;
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
                w0 ^= RCW230[r];
                w4 ^= RCW234[r];
            }

            v20 = (uint)w0;
            v30 = (uint)(w0 >> 32);
            v21 = (uint)w1;
            v31 = (uint)(w1 >> 32);
            v22 = (uint)w2;
            v32 = (uint)(w2 >> 32);
            v23 = (uint)w3;
            v33 = (uint)(w3 >> 32);
            v24 = (uint)w4;
            v34 = (uint)(w4 >> 32);
            v25 = (uint)w5;
            v35 = (uint)(w5 >> 32);
            v26 = (uint)w6;
            v36 = (uint)(w6 >> 32);
            v27 = (uint)w7;
            v37 = (uint)(w7 >> 32);

            for (int r = 0; r < 8; r++)
            {
                tmp = v40;
                v40 |= v41;
                v42 ^= v43;
                v41 = ~v41;
                v40 ^= v43;
                v43 &= tmp;
                v41 ^= v43;
                v43 ^= v42;
                v42 &= v40;
                v40 = ~v40;
                v42 ^= v41;
                v41 |= v43;
                tmp ^= v41;
                v43 ^= v42;
                v42 &= v41;
                v41 ^= v40;
                v40 = tmp;
                tmp = v45;
                v45 |= v46;
                v47 ^= v44;
                v46 = ~v46;
                v45 ^= v44;
                v44 &= tmp;
                v46 ^= v44;
                v44 ^= v47;
                v47 &= v45;
                v45 = ~v45;
                v47 ^= v46;
                v46 |= v44;
                tmp ^= v46;
                v44 ^= v47;
                v47 &= v46;
                v46 ^= v45;
                v45 = tmp;
                v44 ^= v40;
                v40 = RotateLeft(v40, 2) ^ v44;
                v44 = RotateLeft(v44, 14) ^ v40;
                v40 = RotateLeft(v40, 10) ^ v44;
                v44 = RotateLeft(v44, 1);
                v45 ^= v41;
                v41 = RotateLeft(v41, 2) ^ v45;
                v45 = RotateLeft(v45, 14) ^ v41;
                v41 = RotateLeft(v41, 10) ^ v45;
                v45 = RotateLeft(v45, 1);
                v46 ^= v42;
                v42 = RotateLeft(v42, 2) ^ v46;
                v46 = RotateLeft(v46, 14) ^ v42;
                v42 = RotateLeft(v42, 10) ^ v46;
                v46 = RotateLeft(v46, 1);
                v47 ^= v43;
                v43 = RotateLeft(v43, 2) ^ v47;
                v47 = RotateLeft(v47, 14) ^ v43;
                v43 = RotateLeft(v43, 10) ^ v47;
                v47 = RotateLeft(v47, 1);
                v40 ^= RC40[r];
                v44 ^= RC44[r];
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
            h30 = v30;
            h31 = v31;
            h32 = v32;
            h33 = v33;
            h34 = v34;
            h35 = v35;
            h36 = v36;
            h37 = v37;
            h40 = v40;
            h41 = v41;
            h42 = v42;
            h43 = v43;
            h44 = v44;
            h45 = v45;
            h46 = v46;
            h47 = v47;
        }
    }
}