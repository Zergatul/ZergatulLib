using System;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class MD2 : Security.MessageDigest
    {
        public override int BlockLength => 16;
        public override int DigestLength => 16;

        private readonly byte[] Subst = new byte[]
        {
            0x29, 0x2e, 0x43, 0xc9, 0xa2, 0xd8, 0x7c, 0x01, 0x3d, 0x36, 0x54, 0xa1, 0xec, 0xf0, 0x06, 0x13,
            0x62, 0xa7, 0x05, 0xf3, 0xc0, 0xc7, 0x73, 0x8c, 0x98, 0x93, 0x2b, 0xd9, 0xbc, 0x4c, 0x82, 0xca,
            0x1e, 0x9b, 0x57, 0x3c, 0xfd, 0xd4, 0xe0, 0x16, 0x67, 0x42, 0x6f, 0x18, 0x8a, 0x17, 0xe5, 0x12,
            0xbe, 0x4e, 0xc4, 0xd6, 0xda, 0x9e, 0xde, 0x49, 0xa0, 0xfb, 0xf5, 0x8e, 0xbb, 0x2f, 0xee, 0x7a,
            0xa9, 0x68, 0x79, 0x91, 0x15, 0xb2, 0x07, 0x3f, 0x94, 0xc2, 0x10, 0x89, 0x0b, 0x22, 0x5f, 0x21,
            0x80, 0x7f, 0x5d, 0x9a, 0x5a, 0x90, 0x32, 0x27, 0x35, 0x3e, 0xcc, 0xe7, 0xbf, 0xf7, 0x97, 0x03,
            0xff, 0x19, 0x30, 0xb3, 0x48, 0xa5, 0xb5, 0xd1, 0xd7, 0x5e, 0x92, 0x2a, 0xac, 0x56, 0xaa, 0xc6,
            0x4f, 0xb8, 0x38, 0xd2, 0x96, 0xa4, 0x7d, 0xb6, 0x76, 0xfc, 0x6b, 0xe2, 0x9c, 0x74, 0x04, 0xf1,
            0x45, 0x9d, 0x70, 0x59, 0x64, 0x71, 0x87, 0x20, 0x86, 0x5b, 0xcf, 0x65, 0xe6, 0x2d, 0xa8, 0x02,
            0x1b, 0x60, 0x25, 0xad, 0xae, 0xb0, 0xb9, 0xf6, 0x1c, 0x46, 0x61, 0x69, 0x34, 0x40, 0x7e, 0x0f,
            0x55, 0x47, 0xa3, 0x23, 0xdd, 0x51, 0xaf, 0x3a, 0xc3, 0x5c, 0xf9, 0xce, 0xba, 0xc5, 0xea, 0x26,
            0x2c, 0x53, 0x0d, 0x6e, 0x85, 0x28, 0x84, 0x09, 0xd3, 0xdf, 0xcd, 0xf4, 0x41, 0x81, 0x4d, 0x52,
            0x6a, 0xdc, 0x37, 0xc8, 0x6c, 0xc1, 0xab, 0xfa, 0x24, 0xe1, 0x7b, 0x08, 0x0c, 0xbd, 0xb1, 0x4a,
            0x78, 0x88, 0x95, 0x8b, 0xe3, 0x63, 0xe8, 0x6d, 0xe9, 0xcb, 0xd5, 0xfe, 0x3b, 0x00, 0x1d, 0x39,
            0xf2, 0xef, 0xb7, 0x0e, 0x66, 0x58, 0xd0, 0xe4, 0xa6, 0x77, 0x72, 0xf8, 0xeb, 0x75, 0x4b, 0x0a,
            0x31, 0x44, 0x50, 0xb4, 0x8f, 0xed, 0x1f, 0x1a, 0xdb, 0x99, 0x8d, 0x33, 0x9f, 0x11, 0x83, 0x14,
        };

        byte[] buffer;
        int bufOffset;
        byte s0, s1, s2, s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf;
        byte c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, ca, cb, cc, cd, ce, cf;

        public MD2()
        {
            buffer = new byte[16];
            Reset();
        }

        public override void Reset()
        {
            s0 = s1 = s2 = s3 = s4 = s5 = s6 = s7 = s8 = s9 = sa = sb = sc = sd = se = sf = 0;
            c0 = c1 = c2 = c3 = c4 = c5 = c6 = c7 = c8 = c9 = ca = cb = cc = cd = ce = cf = 0;
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
                    bufOffset = 0;
                    ProcessBlock();
                }
            }
        }

        public override byte[] Digest()
        {
            byte pad = (byte)(16 - bufOffset);
            while (bufOffset < 16)
                buffer[bufOffset++] = pad;
            ProcessBlock();

            buffer[0x0] = c0;
            buffer[0x1] = c1;
            buffer[0x2] = c2;
            buffer[0x3] = c3;
            buffer[0x4] = c4;
            buffer[0x5] = c5;
            buffer[0x6] = c6;
            buffer[0x7] = c7;
            buffer[0x8] = c8;
            buffer[0x9] = c9;
            buffer[0xa] = ca;
            buffer[0xb] = cb;
            buffer[0xc] = cc;
            buffer[0xd] = cd;
            buffer[0xe] = ce;
            buffer[0xf] = cf;
            ProcessBlock();

            byte[] digest = new byte[16];
            digest[0x0] = s0;
            digest[0x1] = s1;
            digest[0x2] = s2;
            digest[0x3] = s3;
            digest[0x4] = s4;
            digest[0x5] = s5;
            digest[0x6] = s6;
            digest[0x7] = s7;
            digest[0x8] = s8;
            digest[0x9] = s9;
            digest[0xa] = sa;
            digest[0xb] = sb;
            digest[0xc] = sc;
            digest[0xd] = sd;
            digest[0xe] = se;
            digest[0xf] = sf;
            return digest;
        }

        private void ProcessBlock()
        {
            byte x00 = s0;
            byte x01 = s1;
            byte x02 = s2;
            byte x03 = s3;
            byte x04 = s4;
            byte x05 = s5;
            byte x06 = s6;
            byte x07 = s7;
            byte x08 = s8;
            byte x09 = s9;
            byte x0a = sa;
            byte x0b = sb;
            byte x0c = sc;
            byte x0d = sd;
            byte x0e = se;
            byte x0f = sf;
            byte x10 = buffer[0x0];
            byte x11 = buffer[0x1];
            byte x12 = buffer[0x2];
            byte x13 = buffer[0x3];
            byte x14 = buffer[0x4];
            byte x15 = buffer[0x5];
            byte x16 = buffer[0x6];
            byte x17 = buffer[0x7];
            byte x18 = buffer[0x8];
            byte x19 = buffer[0x9];
            byte x1a = buffer[0xa];
            byte x1b = buffer[0xb];
            byte x1c = buffer[0xc];
            byte x1d = buffer[0xd];
            byte x1e = buffer[0xe];
            byte x1f = buffer[0xf];
            byte x20 = (byte)(x00 ^ x10);
            byte x21 = (byte)(x01 ^ x11);
            byte x22 = (byte)(x02 ^ x12);
            byte x23 = (byte)(x03 ^ x13);
            byte x24 = (byte)(x04 ^ x14);
            byte x25 = (byte)(x05 ^ x15);
            byte x26 = (byte)(x06 ^ x16);
            byte x27 = (byte)(x07 ^ x17);
            byte x28 = (byte)(x08 ^ x18);
            byte x29 = (byte)(x09 ^ x19);
            byte x2a = (byte)(x0a ^ x1a);
            byte x2b = (byte)(x0b ^ x1b);
            byte x2c = (byte)(x0c ^ x1c);
            byte x2d = (byte)(x0d ^ x1d);
            byte x2e = (byte)(x0e ^ x1e);
            byte x2f = (byte)(x0f ^ x1f);

            // Update checksum
            int t = cf;
            t = c0 ^= Subst[x10 ^ t];
            t = c1 ^= Subst[x11 ^ t];
            t = c2 ^= Subst[x12 ^ t];
            t = c3 ^= Subst[x13 ^ t];
            t = c4 ^= Subst[x14 ^ t];
            t = c5 ^= Subst[x15 ^ t];
            t = c6 ^= Subst[x16 ^ t];
            t = c7 ^= Subst[x17 ^ t];
            t = c8 ^= Subst[x18 ^ t];
            t = c9 ^= Subst[x19 ^ t];
            t = ca ^= Subst[x1a ^ t];
            t = cb ^= Subst[x1b ^ t];
            t = cc ^= Subst[x1c ^ t];
            t = cd ^= Subst[x1d ^ t];
            t = ce ^= Subst[x1e ^ t];
            t = cf ^= Subst[x1f ^ t];

            // Compress block
            t = 0;
            for (int i = 0; i < 18; i++)
            {
                t = x00 ^= Subst[t];
                t = x01 ^= Subst[t];
                t = x02 ^= Subst[t];
                t = x03 ^= Subst[t];
                t = x04 ^= Subst[t];
                t = x05 ^= Subst[t];
                t = x06 ^= Subst[t];
                t = x07 ^= Subst[t];
                t = x08 ^= Subst[t];
                t = x09 ^= Subst[t];
                t = x0a ^= Subst[t];
                t = x0b ^= Subst[t];
                t = x0c ^= Subst[t];
                t = x0d ^= Subst[t];
                t = x0e ^= Subst[t];
                t = x0f ^= Subst[t];
                t = x10 ^= Subst[t];
                t = x11 ^= Subst[t];
                t = x12 ^= Subst[t];
                t = x13 ^= Subst[t];
                t = x14 ^= Subst[t];
                t = x15 ^= Subst[t];
                t = x16 ^= Subst[t];
                t = x17 ^= Subst[t];
                t = x18 ^= Subst[t];
                t = x19 ^= Subst[t];
                t = x1a ^= Subst[t];
                t = x1b ^= Subst[t];
                t = x1c ^= Subst[t];
                t = x1d ^= Subst[t];
                t = x1e ^= Subst[t];
                t = x1f ^= Subst[t];
                t = x20 ^= Subst[t];
                t = x21 ^= Subst[t];
                t = x22 ^= Subst[t];
                t = x23 ^= Subst[t];
                t = x24 ^= Subst[t];
                t = x25 ^= Subst[t];
                t = x26 ^= Subst[t];
                t = x27 ^= Subst[t];
                t = x28 ^= Subst[t];
                t = x29 ^= Subst[t];
                t = x2a ^= Subst[t];
                t = x2b ^= Subst[t];
                t = x2c ^= Subst[t];
                t = x2d ^= Subst[t];
                t = x2e ^= Subst[t];
                t = x2f ^= Subst[t];
                t = (t + i) & 0xFF;
            }

            s0 = x00;
            s1 = x01;
            s2 = x02;
            s3 = x03;
            s4 = x04;
            s5 = x05;
            s6 = x06;
            s7 = x07;
            s8 = x08;
            s9 = x09;
            sa = x0a;
            sb = x0b;
            sc = x0c;
            sd = x0d;
            se = x0e;
            sf = x0f;
        }
    }
}