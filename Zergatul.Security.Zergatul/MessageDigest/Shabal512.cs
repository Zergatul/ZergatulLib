using System;
using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Shabal512 : Security.MessageDigest
    {
        public override int BlockLength => 64;
        public override int DigestLength => 64;

        private byte[] buffer;
        protected int bufOffset;
        protected uint wLow, wHi;
        protected uint a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, aa, ab;
        protected uint b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, ba, bb, bc, bd, be, bf;
        protected uint c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, ca, cb, cc, cd, ce, cf;

        public Shabal512()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            a0 = 0x20728DFD;
            a1 = 0x46C0BD53;
            a2 = 0xE782B699;
            a3 = 0x55304632;
            a4 = 0x71B4EF90;
            a5 = 0x0EA9E82C;
            a6 = 0xDBB930F1;
            a7 = 0xFAD06B8B;
            a8 = 0xBE0CAE40;
            a9 = 0x8BD14410;
            aa = 0x76D2ADAC;
            ab = 0x28ACAB7F;

            b0 = 0xC1099CB7;
            b1 = 0x07B385F3;
            b2 = 0xE7442C26;
            b3 = 0xCC8AD640;
            b4 = 0xEB6F56C7;
            b5 = 0x1EA81AA9;
            b6 = 0x73B9D314;
            b7 = 0x1DE85D08;
            b8 = 0x48910A5A;
            b9 = 0x893B22DB;
            ba = 0xC5A0DF44;
            bb = 0xBBC4324E;
            bc = 0x72D2F240;
            bd = 0x75941D99;
            be = 0x6D8BDE82;
            bf = 0xA1A7502B;

            c0 = 0xD9BF68D1;
            c1 = 0x58BAD750;
            c2 = 0x56028CB2;
            c3 = 0x8134F359;
            c4 = 0xB5D469D8;
            c5 = 0x941A8CC2;
            c6 = 0x418B2A6E;
            c7 = 0x04052780;
            c8 = 0x7F07D787;
            c9 = 0x5194358F;
            ca = 0x3C60D665;
            cb = 0xBE97D79A;
            cc = 0x950C3434;
            cd = 0xAED9A06D;
            ce = 0x2537DC8D;
            cf = 0x7CDB5969;

            bufOffset = 0;
            wLow = 0;
            wHi = 0;
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
                int copy = System.Math.Min(64 - bufOffset, length);
                Buffer.BlockCopy(data, offset, buffer, bufOffset, copy);

                offset += copy;
                length -= copy;
                bufOffset += copy;

                if (bufOffset == 64)
                {
                    bufOffset = 0;
                    ProcessBlock();
                }
            }
        }

        public override byte[] Digest()
        {
            buffer[bufOffset++] = 0x80;
            while (bufOffset < 64)
                buffer[bufOffset++] = 0;

            if (++wLow == 0)
                wHi++;

            uint m0 = ToUInt32(buffer, 0x00, ByteOrder.LittleEndian);
            uint m1 = ToUInt32(buffer, 0x04, ByteOrder.LittleEndian);
            uint m2 = ToUInt32(buffer, 0x08, ByteOrder.LittleEndian);
            uint m3 = ToUInt32(buffer, 0x0C, ByteOrder.LittleEndian);
            uint m4 = ToUInt32(buffer, 0x10, ByteOrder.LittleEndian);
            uint m5 = ToUInt32(buffer, 0x14, ByteOrder.LittleEndian);
            uint m6 = ToUInt32(buffer, 0x18, ByteOrder.LittleEndian);
            uint m7 = ToUInt32(buffer, 0x1C, ByteOrder.LittleEndian);
            uint m8 = ToUInt32(buffer, 0x20, ByteOrder.LittleEndian);
            uint m9 = ToUInt32(buffer, 0x24, ByteOrder.LittleEndian);
            uint ma = ToUInt32(buffer, 0x28, ByteOrder.LittleEndian);
            uint mb = ToUInt32(buffer, 0x2C, ByteOrder.LittleEndian);
            uint mc = ToUInt32(buffer, 0x30, ByteOrder.LittleEndian);
            uint md = ToUInt32(buffer, 0x34, ByteOrder.LittleEndian);
            uint me = ToUInt32(buffer, 0x38, ByteOrder.LittleEndian);
            uint mf = ToUInt32(buffer, 0x3C, ByteOrder.LittleEndian);

            uint a0 = this.a0 ^ wLow;
            uint a1 = this.a1 ^ wHi;
            uint a2 = this.a2;
            uint a3 = this.a3;
            uint a4 = this.a4;
            uint a5 = this.a5;
            uint a6 = this.a6;
            uint a7 = this.a7;
            uint a8 = this.a8;
            uint a9 = this.a9;
            uint aa = this.aa;
            uint ab = this.ab;

            uint b0 = this.b0 + m0;
            uint b1 = this.b1 + m1;
            uint b2 = this.b2 + m2;
            uint b3 = this.b3 + m3;
            uint b4 = this.b4 + m4;
            uint b5 = this.b5 + m5;
            uint b6 = this.b6 + m6;
            uint b7 = this.b7 + m7;
            uint b8 = this.b8 + m8;
            uint b9 = this.b9 + m9;
            uint ba = this.ba + ma;
            uint bb = this.bb + mb;
            uint bc = this.bc + mc;
            uint bd = this.bd + md;
            uint be = this.be + me;
            uint bf = this.bf + mf;

            uint c0 = this.c0;
            uint c1 = this.c1;
            uint c2 = this.c2;
            uint c3 = this.c3;
            uint c4 = this.c4;
            uint c5 = this.c5;
            uint c6 = this.c6;
            uint c7 = this.c7;
            uint c8 = this.c8;
            uint c9 = this.c9;
            uint ca = this.ca;
            uint cb = this.cb;
            uint cc = this.cc;
            uint cd = this.cd;
            uint ce = this.ce;
            uint cf = this.cf;

            #region P

            b0 = RotateLeft(b0, 17);
            b1 = RotateLeft(b1, 17);
            b2 = RotateLeft(b2, 17);
            b3 = RotateLeft(b3, 17);
            b4 = RotateLeft(b4, 17);
            b5 = RotateLeft(b5, 17);
            b6 = RotateLeft(b6, 17);
            b7 = RotateLeft(b7, 17);
            b8 = RotateLeft(b8, 17);
            b9 = RotateLeft(b9, 17);
            ba = RotateLeft(ba, 17);
            bb = RotateLeft(bb, 17);
            bc = RotateLeft(bc, 17);
            bd = RotateLeft(bd, 17);
            be = RotateLeft(be, 17);
            bf = RotateLeft(bf, 17);

            #region Step 0
            a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ c8) * 3) ^ bd ^ (b9 & ~b6) ^ m0;
            b0 = ~(RotateLeft(b0, 1) ^ a0);
            a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ c7) * 3) ^ be ^ (ba & ~b7) ^ m1;
            b1 = ~(RotateLeft(b1, 1) ^ a1);
            a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ c6) * 3) ^ bf ^ (bb & ~b8) ^ m2;
            b2 = ~(RotateLeft(b2, 1) ^ a2);
            a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ c5) * 3) ^ b0 ^ (bc & ~b9) ^ m3;
            b3 = ~(RotateLeft(b3, 1) ^ a3);
            a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ c4) * 3) ^ b1 ^ (bd & ~ba) ^ m4;
            b4 = ~(RotateLeft(b4, 1) ^ a4);
            a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ c3) * 3) ^ b2 ^ (be & ~bb) ^ m5;
            b5 = ~(RotateLeft(b5, 1) ^ a5);
            a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ c2) * 3) ^ b3 ^ (bf & ~bc) ^ m6;
            b6 = ~(RotateLeft(b6, 1) ^ a6);
            a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ c1) * 3) ^ b4 ^ (b0 & ~bd) ^ m7;
            b7 = ~(RotateLeft(b7, 1) ^ a7);
            a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ c0) * 3) ^ b5 ^ (b1 & ~be) ^ m8;
            b8 = ~(RotateLeft(b8, 1) ^ a8);
            a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ cf) * 3) ^ b6 ^ (b2 & ~bf) ^ m9;
            b9 = ~(RotateLeft(b9, 1) ^ a9);
            aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ ce) * 3) ^ b7 ^ (b3 & ~b0) ^ ma;
            ba = ~(RotateLeft(ba, 1) ^ aa);
            ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ cd) * 3) ^ b8 ^ (b4 & ~b1) ^ mb;
            bb = ~(RotateLeft(bb, 1) ^ ab);
            a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ cc) * 3) ^ b9 ^ (b5 & ~b2) ^ mc;
            bc = ~(RotateLeft(bc, 1) ^ a0);
            a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ cb) * 3) ^ ba ^ (b6 & ~b3) ^ md;
            bd = ~(RotateLeft(bd, 1) ^ a1);
            a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ ca) * 3) ^ bb ^ (b7 & ~b4) ^ me;
            be = ~(RotateLeft(be, 1) ^ a2);
            a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ c9) * 3) ^ bc ^ (b8 & ~b5) ^ mf;
            bf = ~(RotateLeft(bf, 1) ^ a3);
            #endregion

            #region Step 1
            a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ c8) * 3) ^ bd ^ (b9 & ~b6) ^ m0;
            b0 = ~(RotateLeft(b0, 1) ^ a4);
            a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ c7) * 3) ^ be ^ (ba & ~b7) ^ m1;
            b1 = ~(RotateLeft(b1, 1) ^ a5);
            a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ c6) * 3) ^ bf ^ (bb & ~b8) ^ m2;
            b2 = ~(RotateLeft(b2, 1) ^ a6);
            a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ c5) * 3) ^ b0 ^ (bc & ~b9) ^ m3;
            b3 = ~(RotateLeft(b3, 1) ^ a7);
            a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ c4) * 3) ^ b1 ^ (bd & ~ba) ^ m4;
            b4 = ~(RotateLeft(b4, 1) ^ a8);
            a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ c3) * 3) ^ b2 ^ (be & ~bb) ^ m5;
            b5 = ~(RotateLeft(b5, 1) ^ a9);
            aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ c2) * 3) ^ b3 ^ (bf & ~bc) ^ m6;
            b6 = ~(RotateLeft(b6, 1) ^ aa);
            ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ c1) * 3) ^ b4 ^ (b0 & ~bd) ^ m7;
            b7 = ~(RotateLeft(b7, 1) ^ ab);
            a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ c0) * 3) ^ b5 ^ (b1 & ~be) ^ m8;
            b8 = ~(RotateLeft(b8, 1) ^ a0);
            a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ cf) * 3) ^ b6 ^ (b2 & ~bf) ^ m9;
            b9 = ~(RotateLeft(b9, 1) ^ a1);
            a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ ce) * 3) ^ b7 ^ (b3 & ~b0) ^ ma;
            ba = ~(RotateLeft(ba, 1) ^ a2);
            a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ cd) * 3) ^ b8 ^ (b4 & ~b1) ^ mb;
            bb = ~(RotateLeft(bb, 1) ^ a3);
            a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ cc) * 3) ^ b9 ^ (b5 & ~b2) ^ mc;
            bc = ~(RotateLeft(bc, 1) ^ a4);
            a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ cb) * 3) ^ ba ^ (b6 & ~b3) ^ md;
            bd = ~(RotateLeft(bd, 1) ^ a5);
            a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ ca) * 3) ^ bb ^ (b7 & ~b4) ^ me;
            be = ~(RotateLeft(be, 1) ^ a6);
            a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ c9) * 3) ^ bc ^ (b8 & ~b5) ^ mf;
            bf = ~(RotateLeft(bf, 1) ^ a7);
            #endregion

            #region Step 2
            a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ c8) * 3) ^ bd ^ (b9 & ~b6) ^ m0;
            b0 = ~(RotateLeft(b0, 1) ^ a8);
            a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ c7) * 3) ^ be ^ (ba & ~b7) ^ m1;
            b1 = ~(RotateLeft(b1, 1) ^ a9);
            aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ c6) * 3) ^ bf ^ (bb & ~b8) ^ m2;
            b2 = ~(RotateLeft(b2, 1) ^ aa);
            ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ c5) * 3) ^ b0 ^ (bc & ~b9) ^ m3;
            b3 = ~(RotateLeft(b3, 1) ^ ab);
            a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ c4) * 3) ^ b1 ^ (bd & ~ba) ^ m4;
            b4 = ~(RotateLeft(b4, 1) ^ a0);
            a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ c3) * 3) ^ b2 ^ (be & ~bb) ^ m5;
            b5 = ~(RotateLeft(b5, 1) ^ a1);
            a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ c2) * 3) ^ b3 ^ (bf & ~bc) ^ m6;
            b6 = ~(RotateLeft(b6, 1) ^ a2);
            a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ c1) * 3) ^ b4 ^ (b0 & ~bd) ^ m7;
            b7 = ~(RotateLeft(b7, 1) ^ a3);
            a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ c0) * 3) ^ b5 ^ (b1 & ~be) ^ m8;
            b8 = ~(RotateLeft(b8, 1) ^ a4);
            a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ cf) * 3) ^ b6 ^ (b2 & ~bf) ^ m9;
            b9 = ~(RotateLeft(b9, 1) ^ a5);
            a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ ce) * 3) ^ b7 ^ (b3 & ~b0) ^ ma;
            ba = ~(RotateLeft(ba, 1) ^ a6);
            a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ cd) * 3) ^ b8 ^ (b4 & ~b1) ^ mb;
            bb = ~(RotateLeft(bb, 1) ^ a7);
            a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ cc) * 3) ^ b9 ^ (b5 & ~b2) ^ mc;
            bc = ~(RotateLeft(bc, 1) ^ a8);
            a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ cb) * 3) ^ ba ^ (b6 & ~b3) ^ md;
            bd = ~(RotateLeft(bd, 1) ^ a9);
            aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ ca) * 3) ^ bb ^ (b7 & ~b4) ^ me;
            be = ~(RotateLeft(be, 1) ^ aa);
            ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ c9) * 3) ^ bc ^ (b8 & ~b5) ^ mf;
            bf = ~(RotateLeft(bf, 1) ^ ab);
            #endregion

            ab = ab + c6;
            aa = aa + c5;
            a9 = a9 + c4;
            a8 = a8 + c3;
            a7 = a7 + c2;
            a6 = a6 + c1;
            a5 = a5 + c0;
            a4 = a4 + cf;
            a3 = a3 + ce;
            a2 = a2 + cd;
            a1 = a1 + cc;
            a0 = a0 + cb;
            ab = ab + ca;
            aa = aa + c9;
            a9 = a9 + c8;
            a8 = a8 + c7;
            a7 = a7 + c6;
            a6 = a6 + c5;
            a5 = a5 + c4;
            a4 = a4 + c3;
            a3 = a3 + c2;
            a2 = a2 + c1;
            a1 = a1 + c0;
            a0 = a0 + cf;
            ab = ab + ce;
            aa = aa + cd;
            a9 = a9 + cc;
            a8 = a8 + cb;
            a7 = a7 + ca;
            a6 = a6 + c9;
            a5 = a5 + c8;
            a4 = a4 + c7;
            a3 = a3 + c6;
            a2 = a2 + c5;
            a1 = a1 + c4;
            a0 = a0 + c3;

            #endregion

            for (int i = 0; i < 3; i++)
            {
                #region Swap
                uint tt;
                tt = b0;
                b0 = c0;
                c0 = tt;
                tt = b1;
                b1 = c1;
                c1 = tt;
                tt = b2;
                b2 = c2;
                c2 = tt;
                tt = b3;
                b3 = c3;
                c3 = tt;
                tt = b4;
                b4 = c4;
                c4 = tt;
                tt = b5;
                b5 = c5;
                c5 = tt;
                tt = b6;
                b6 = c6;
                c6 = tt;
                tt = b7;
                b7 = c7;
                c7 = tt;
                tt = b8;
                b8 = c8;
                c8 = tt;
                tt = b9;
                b9 = c9;
                c9 = tt;
                tt = ba;
                ba = ca;
                ca = tt;
                tt = bb;
                bb = cb;
                cb = tt;
                tt = bc;
                bc = cc;
                cc = tt;
                tt = bd;
                bd = cd;
                cd = tt;
                tt = be;
                be = ce;
                ce = tt;
                tt = bf;
                bf = cf;
                cf = tt;
                #endregion

                a0 ^= wLow;
                a1 ^= wHi;

                #region P

                b0 = RotateLeft(b0, 17);
                b1 = RotateLeft(b1, 17);
                b2 = RotateLeft(b2, 17);
                b3 = RotateLeft(b3, 17);
                b4 = RotateLeft(b4, 17);
                b5 = RotateLeft(b5, 17);
                b6 = RotateLeft(b6, 17);
                b7 = RotateLeft(b7, 17);
                b8 = RotateLeft(b8, 17);
                b9 = RotateLeft(b9, 17);
                ba = RotateLeft(ba, 17);
                bb = RotateLeft(bb, 17);
                bc = RotateLeft(bc, 17);
                bd = RotateLeft(bd, 17);
                be = RotateLeft(be, 17);
                bf = RotateLeft(bf, 17);

                #region Step 0
                a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ c8) * 3) ^ bd ^ (b9 & ~b6) ^ m0;
                b0 = ~(RotateLeft(b0, 1) ^ a0);
                a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ c7) * 3) ^ be ^ (ba & ~b7) ^ m1;
                b1 = ~(RotateLeft(b1, 1) ^ a1);
                a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ c6) * 3) ^ bf ^ (bb & ~b8) ^ m2;
                b2 = ~(RotateLeft(b2, 1) ^ a2);
                a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ c5) * 3) ^ b0 ^ (bc & ~b9) ^ m3;
                b3 = ~(RotateLeft(b3, 1) ^ a3);
                a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ c4) * 3) ^ b1 ^ (bd & ~ba) ^ m4;
                b4 = ~(RotateLeft(b4, 1) ^ a4);
                a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ c3) * 3) ^ b2 ^ (be & ~bb) ^ m5;
                b5 = ~(RotateLeft(b5, 1) ^ a5);
                a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ c2) * 3) ^ b3 ^ (bf & ~bc) ^ m6;
                b6 = ~(RotateLeft(b6, 1) ^ a6);
                a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ c1) * 3) ^ b4 ^ (b0 & ~bd) ^ m7;
                b7 = ~(RotateLeft(b7, 1) ^ a7);
                a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ c0) * 3) ^ b5 ^ (b1 & ~be) ^ m8;
                b8 = ~(RotateLeft(b8, 1) ^ a8);
                a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ cf) * 3) ^ b6 ^ (b2 & ~bf) ^ m9;
                b9 = ~(RotateLeft(b9, 1) ^ a9);
                aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ ce) * 3) ^ b7 ^ (b3 & ~b0) ^ ma;
                ba = ~(RotateLeft(ba, 1) ^ aa);
                ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ cd) * 3) ^ b8 ^ (b4 & ~b1) ^ mb;
                bb = ~(RotateLeft(bb, 1) ^ ab);
                a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ cc) * 3) ^ b9 ^ (b5 & ~b2) ^ mc;
                bc = ~(RotateLeft(bc, 1) ^ a0);
                a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ cb) * 3) ^ ba ^ (b6 & ~b3) ^ md;
                bd = ~(RotateLeft(bd, 1) ^ a1);
                a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ ca) * 3) ^ bb ^ (b7 & ~b4) ^ me;
                be = ~(RotateLeft(be, 1) ^ a2);
                a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ c9) * 3) ^ bc ^ (b8 & ~b5) ^ mf;
                bf = ~(RotateLeft(bf, 1) ^ a3);
                #endregion

                #region Step 1
                a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ c8) * 3) ^ bd ^ (b9 & ~b6) ^ m0;
                b0 = ~(RotateLeft(b0, 1) ^ a4);
                a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ c7) * 3) ^ be ^ (ba & ~b7) ^ m1;
                b1 = ~(RotateLeft(b1, 1) ^ a5);
                a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ c6) * 3) ^ bf ^ (bb & ~b8) ^ m2;
                b2 = ~(RotateLeft(b2, 1) ^ a6);
                a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ c5) * 3) ^ b0 ^ (bc & ~b9) ^ m3;
                b3 = ~(RotateLeft(b3, 1) ^ a7);
                a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ c4) * 3) ^ b1 ^ (bd & ~ba) ^ m4;
                b4 = ~(RotateLeft(b4, 1) ^ a8);
                a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ c3) * 3) ^ b2 ^ (be & ~bb) ^ m5;
                b5 = ~(RotateLeft(b5, 1) ^ a9);
                aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ c2) * 3) ^ b3 ^ (bf & ~bc) ^ m6;
                b6 = ~(RotateLeft(b6, 1) ^ aa);
                ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ c1) * 3) ^ b4 ^ (b0 & ~bd) ^ m7;
                b7 = ~(RotateLeft(b7, 1) ^ ab);
                a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ c0) * 3) ^ b5 ^ (b1 & ~be) ^ m8;
                b8 = ~(RotateLeft(b8, 1) ^ a0);
                a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ cf) * 3) ^ b6 ^ (b2 & ~bf) ^ m9;
                b9 = ~(RotateLeft(b9, 1) ^ a1);
                a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ ce) * 3) ^ b7 ^ (b3 & ~b0) ^ ma;
                ba = ~(RotateLeft(ba, 1) ^ a2);
                a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ cd) * 3) ^ b8 ^ (b4 & ~b1) ^ mb;
                bb = ~(RotateLeft(bb, 1) ^ a3);
                a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ cc) * 3) ^ b9 ^ (b5 & ~b2) ^ mc;
                bc = ~(RotateLeft(bc, 1) ^ a4);
                a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ cb) * 3) ^ ba ^ (b6 & ~b3) ^ md;
                bd = ~(RotateLeft(bd, 1) ^ a5);
                a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ ca) * 3) ^ bb ^ (b7 & ~b4) ^ me;
                be = ~(RotateLeft(be, 1) ^ a6);
                a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ c9) * 3) ^ bc ^ (b8 & ~b5) ^ mf;
                bf = ~(RotateLeft(bf, 1) ^ a7);
                #endregion

                #region Step 2
                a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ c8) * 3) ^ bd ^ (b9 & ~b6) ^ m0;
                b0 = ~(RotateLeft(b0, 1) ^ a8);
                a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ c7) * 3) ^ be ^ (ba & ~b7) ^ m1;
                b1 = ~(RotateLeft(b1, 1) ^ a9);
                aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ c6) * 3) ^ bf ^ (bb & ~b8) ^ m2;
                b2 = ~(RotateLeft(b2, 1) ^ aa);
                ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ c5) * 3) ^ b0 ^ (bc & ~b9) ^ m3;
                b3 = ~(RotateLeft(b3, 1) ^ ab);
                a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ c4) * 3) ^ b1 ^ (bd & ~ba) ^ m4;
                b4 = ~(RotateLeft(b4, 1) ^ a0);
                a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ c3) * 3) ^ b2 ^ (be & ~bb) ^ m5;
                b5 = ~(RotateLeft(b5, 1) ^ a1);
                a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ c2) * 3) ^ b3 ^ (bf & ~bc) ^ m6;
                b6 = ~(RotateLeft(b6, 1) ^ a2);
                a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ c1) * 3) ^ b4 ^ (b0 & ~bd) ^ m7;
                b7 = ~(RotateLeft(b7, 1) ^ a3);
                a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ c0) * 3) ^ b5 ^ (b1 & ~be) ^ m8;
                b8 = ~(RotateLeft(b8, 1) ^ a4);
                a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ cf) * 3) ^ b6 ^ (b2 & ~bf) ^ m9;
                b9 = ~(RotateLeft(b9, 1) ^ a5);
                a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ ce) * 3) ^ b7 ^ (b3 & ~b0) ^ ma;
                ba = ~(RotateLeft(ba, 1) ^ a6);
                a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ cd) * 3) ^ b8 ^ (b4 & ~b1) ^ mb;
                bb = ~(RotateLeft(bb, 1) ^ a7);
                a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ cc) * 3) ^ b9 ^ (b5 & ~b2) ^ mc;
                bc = ~(RotateLeft(bc, 1) ^ a8);
                a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ cb) * 3) ^ ba ^ (b6 & ~b3) ^ md;
                bd = ~(RotateLeft(bd, 1) ^ a9);
                aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ ca) * 3) ^ bb ^ (b7 & ~b4) ^ me;
                be = ~(RotateLeft(be, 1) ^ aa);
                ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ c9) * 3) ^ bc ^ (b8 & ~b5) ^ mf;
                bf = ~(RotateLeft(bf, 1) ^ ab);
                #endregion

                ab = ab + c6;
                aa = aa + c5;
                a9 = a9 + c4;
                a8 = a8 + c3;
                a7 = a7 + c2;
                a6 = a6 + c1;
                a5 = a5 + c0;
                a4 = a4 + cf;
                a3 = a3 + ce;
                a2 = a2 + cd;
                a1 = a1 + cc;
                a0 = a0 + cb;
                ab = ab + ca;
                aa = aa + c9;
                a9 = a9 + c8;
                a8 = a8 + c7;
                a7 = a7 + c6;
                a6 = a6 + c5;
                a5 = a5 + c4;
                a4 = a4 + c3;
                a3 = a3 + c2;
                a2 = a2 + c1;
                a1 = a1 + c0;
                a0 = a0 + cf;
                ab = ab + ce;
                aa = aa + cd;
                a9 = a9 + cc;
                a8 = a8 + cb;
                a7 = a7 + ca;
                a6 = a6 + c9;
                a5 = a5 + c8;
                a4 = a4 + c7;
                a3 = a3 + c6;
                a2 = a2 + c5;
                a1 = a1 + c4;
                a0 = a0 + c3;

                #endregion
            }

            return StateBToDigest(b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, ba, bb, bc, bd, be, bf);
        }

        protected virtual byte[] StateBToDigest(
            uint b0, uint b1, uint b2, uint b3, uint b4, uint b5, uint b6, uint b7,
            uint b8, uint b9, uint ba, uint bb, uint bc, uint bd, uint be, uint bf)
        {
            byte[] digest = new byte[64];
            GetBytes(b0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(b1, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(b2, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(b3, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(b4, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(b5, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(b6, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(b7, ByteOrder.LittleEndian, digest, 0x1C);
            GetBytes(b8, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(b9, ByteOrder.LittleEndian, digest, 0x24);
            GetBytes(ba, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(bb, ByteOrder.LittleEndian, digest, 0x2C);
            GetBytes(bc, ByteOrder.LittleEndian, digest, 0x30);
            GetBytes(bd, ByteOrder.LittleEndian, digest, 0x34);
            GetBytes(be, ByteOrder.LittleEndian, digest, 0x38);
            GetBytes(bf, ByteOrder.LittleEndian, digest, 0x3C);
            return digest;
        }

        private void ProcessBlock()
        {
            if (++wLow == 0)
                wHi++;

            uint m0 = ToUInt32(buffer, 0x00, ByteOrder.LittleEndian);
            uint m1 = ToUInt32(buffer, 0x04, ByteOrder.LittleEndian);
            uint m2 = ToUInt32(buffer, 0x08, ByteOrder.LittleEndian);
            uint m3 = ToUInt32(buffer, 0x0C, ByteOrder.LittleEndian);
            uint m4 = ToUInt32(buffer, 0x10, ByteOrder.LittleEndian);
            uint m5 = ToUInt32(buffer, 0x14, ByteOrder.LittleEndian);
            uint m6 = ToUInt32(buffer, 0x18, ByteOrder.LittleEndian);
            uint m7 = ToUInt32(buffer, 0x1C, ByteOrder.LittleEndian);
            uint m8 = ToUInt32(buffer, 0x20, ByteOrder.LittleEndian);
            uint m9 = ToUInt32(buffer, 0x24, ByteOrder.LittleEndian);
            uint ma = ToUInt32(buffer, 0x28, ByteOrder.LittleEndian);
            uint mb = ToUInt32(buffer, 0x2C, ByteOrder.LittleEndian);
            uint mc = ToUInt32(buffer, 0x30, ByteOrder.LittleEndian);
            uint md = ToUInt32(buffer, 0x34, ByteOrder.LittleEndian);
            uint me = ToUInt32(buffer, 0x38, ByteOrder.LittleEndian);
            uint mf = ToUInt32(buffer, 0x3C, ByteOrder.LittleEndian);

            uint a0 = this.a0 ^ wLow;
            uint a1 = this.a1 ^ wHi;
            uint a2 = this.a2;
            uint a3 = this.a3;
            uint a4 = this.a4;
            uint a5 = this.a5;
            uint a6 = this.a6;
            uint a7 = this.a7;
            uint a8 = this.a8;
            uint a9 = this.a9;
            uint aa = this.aa;
            uint ab = this.ab;

            uint b0 = this.b0 + m0;
            uint b1 = this.b1 + m1;
            uint b2 = this.b2 + m2;
            uint b3 = this.b3 + m3;
            uint b4 = this.b4 + m4;
            uint b5 = this.b5 + m5;
            uint b6 = this.b6 + m6;
            uint b7 = this.b7 + m7;
            uint b8 = this.b8 + m8;
            uint b9 = this.b9 + m9;
            uint ba = this.ba + ma;
            uint bb = this.bb + mb;
            uint bc = this.bc + mc;
            uint bd = this.bd + md;
            uint be = this.be + me;
            uint bf = this.bf + mf;

            uint c0 = this.c0;
            uint c1 = this.c1;
            uint c2 = this.c2;
            uint c3 = this.c3;
            uint c4 = this.c4;
            uint c5 = this.c5;
            uint c6 = this.c6;
            uint c7 = this.c7;
            uint c8 = this.c8;
            uint c9 = this.c9;
            uint ca = this.ca;
            uint cb = this.cb;
            uint cc = this.cc;
            uint cd = this.cd;
            uint ce = this.ce;
            uint cf = this.cf;

            #region P

            b0 = RotateLeft(b0, 17);
            b1 = RotateLeft(b1, 17);
            b2 = RotateLeft(b2, 17);
            b3 = RotateLeft(b3, 17);
            b4 = RotateLeft(b4, 17);
            b5 = RotateLeft(b5, 17);
            b6 = RotateLeft(b6, 17);
            b7 = RotateLeft(b7, 17);
            b8 = RotateLeft(b8, 17);
            b9 = RotateLeft(b9, 17);
            ba = RotateLeft(ba, 17);
            bb = RotateLeft(bb, 17);
            bc = RotateLeft(bc, 17);
            bd = RotateLeft(bd, 17);
            be = RotateLeft(be, 17);
            bf = RotateLeft(bf, 17);

            #region Step 0
            a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ c8) * 3) ^ bd ^ (b9 & ~b6) ^ m0;
            b0 = ~(RotateLeft(b0, 1) ^ a0);
            a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ c7) * 3) ^ be ^ (ba & ~b7) ^ m1;
            b1 = ~(RotateLeft(b1, 1) ^ a1);
            a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ c6) * 3) ^ bf ^ (bb & ~b8) ^ m2;
            b2 = ~(RotateLeft(b2, 1) ^ a2);
            a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ c5) * 3) ^ b0 ^ (bc & ~b9) ^ m3;
            b3 = ~(RotateLeft(b3, 1) ^ a3);
            a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ c4) * 3) ^ b1 ^ (bd & ~ba) ^ m4;
            b4 = ~(RotateLeft(b4, 1) ^ a4);
            a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ c3) * 3) ^ b2 ^ (be & ~bb) ^ m5;
            b5 = ~(RotateLeft(b5, 1) ^ a5);
            a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ c2) * 3) ^ b3 ^ (bf & ~bc) ^ m6;
            b6 = ~(RotateLeft(b6, 1) ^ a6);
            a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ c1) * 3) ^ b4 ^ (b0 & ~bd) ^ m7;
            b7 = ~(RotateLeft(b7, 1) ^ a7);
            a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ c0) * 3) ^ b5 ^ (b1 & ~be) ^ m8;
            b8 = ~(RotateLeft(b8, 1) ^ a8);
            a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ cf) * 3) ^ b6 ^ (b2 & ~bf) ^ m9;
            b9 = ~(RotateLeft(b9, 1) ^ a9);
            aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ ce) * 3) ^ b7 ^ (b3 & ~b0) ^ ma;
            ba = ~(RotateLeft(ba, 1) ^ aa);
            ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ cd) * 3) ^ b8 ^ (b4 & ~b1) ^ mb;
            bb = ~(RotateLeft(bb, 1) ^ ab);
            a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ cc) * 3) ^ b9 ^ (b5 & ~b2) ^ mc;
            bc = ~(RotateLeft(bc, 1) ^ a0);
            a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ cb) * 3) ^ ba ^ (b6 & ~b3) ^ md;
            bd = ~(RotateLeft(bd, 1) ^ a1);
            a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ ca) * 3) ^ bb ^ (b7 & ~b4) ^ me;
            be = ~(RotateLeft(be, 1) ^ a2);
            a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ c9) * 3) ^ bc ^ (b8 & ~b5) ^ mf;
            bf = ~(RotateLeft(bf, 1) ^ a3);
            #endregion

            #region Step 1
            a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ c8) * 3) ^ bd ^ (b9 & ~b6) ^ m0;
            b0 = ~(RotateLeft(b0, 1) ^ a4);
            a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ c7) * 3) ^ be ^ (ba & ~b7) ^ m1;
            b1 = ~(RotateLeft(b1, 1) ^ a5);
            a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ c6) * 3) ^ bf ^ (bb & ~b8) ^ m2;
            b2 = ~(RotateLeft(b2, 1) ^ a6);
            a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ c5) * 3) ^ b0 ^ (bc & ~b9) ^ m3;
            b3 = ~(RotateLeft(b3, 1) ^ a7);
            a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ c4) * 3) ^ b1 ^ (bd & ~ba) ^ m4;
            b4 = ~(RotateLeft(b4, 1) ^ a8);
            a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ c3) * 3) ^ b2 ^ (be & ~bb) ^ m5;
            b5 = ~(RotateLeft(b5, 1) ^ a9);
            aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ c2) * 3) ^ b3 ^ (bf & ~bc) ^ m6;
            b6 = ~(RotateLeft(b6, 1) ^ aa);
            ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ c1) * 3) ^ b4 ^ (b0 & ~bd) ^ m7;
            b7 = ~(RotateLeft(b7, 1) ^ ab);
            a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ c0) * 3) ^ b5 ^ (b1 & ~be) ^ m8;
            b8 = ~(RotateLeft(b8, 1) ^ a0);
            a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ cf) * 3) ^ b6 ^ (b2 & ~bf) ^ m9;
            b9 = ~(RotateLeft(b9, 1) ^ a1);
            a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ ce) * 3) ^ b7 ^ (b3 & ~b0) ^ ma;
            ba = ~(RotateLeft(ba, 1) ^ a2);
            a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ cd) * 3) ^ b8 ^ (b4 & ~b1) ^ mb;
            bb = ~(RotateLeft(bb, 1) ^ a3);
            a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ cc) * 3) ^ b9 ^ (b5 & ~b2) ^ mc;
            bc = ~(RotateLeft(bc, 1) ^ a4);
            a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ cb) * 3) ^ ba ^ (b6 & ~b3) ^ md;
            bd = ~(RotateLeft(bd, 1) ^ a5);
            a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ ca) * 3) ^ bb ^ (b7 & ~b4) ^ me;
            be = ~(RotateLeft(be, 1) ^ a6);
            a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ c9) * 3) ^ bc ^ (b8 & ~b5) ^ mf;
            bf = ~(RotateLeft(bf, 1) ^ a7);
            #endregion

            #region Step 2
            a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ c8) * 3) ^ bd ^ (b9 & ~b6) ^ m0;
            b0 = ~(RotateLeft(b0, 1) ^ a8);
            a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ c7) * 3) ^ be ^ (ba & ~b7) ^ m1;
            b1 = ~(RotateLeft(b1, 1) ^ a9);
            aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ c6) * 3) ^ bf ^ (bb & ~b8) ^ m2;
            b2 = ~(RotateLeft(b2, 1) ^ aa);
            ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ c5) * 3) ^ b0 ^ (bc & ~b9) ^ m3;
            b3 = ~(RotateLeft(b3, 1) ^ ab);
            a0 = ((a0 ^ (RotateLeft(ab, 15) * 5) ^ c4) * 3) ^ b1 ^ (bd & ~ba) ^ m4;
            b4 = ~(RotateLeft(b4, 1) ^ a0);
            a1 = ((a1 ^ (RotateLeft(a0, 15) * 5) ^ c3) * 3) ^ b2 ^ (be & ~bb) ^ m5;
            b5 = ~(RotateLeft(b5, 1) ^ a1);
            a2 = ((a2 ^ (RotateLeft(a1, 15) * 5) ^ c2) * 3) ^ b3 ^ (bf & ~bc) ^ m6;
            b6 = ~(RotateLeft(b6, 1) ^ a2);
            a3 = ((a3 ^ (RotateLeft(a2, 15) * 5) ^ c1) * 3) ^ b4 ^ (b0 & ~bd) ^ m7;
            b7 = ~(RotateLeft(b7, 1) ^ a3);
            a4 = ((a4 ^ (RotateLeft(a3, 15) * 5) ^ c0) * 3) ^ b5 ^ (b1 & ~be) ^ m8;
            b8 = ~(RotateLeft(b8, 1) ^ a4);
            a5 = ((a5 ^ (RotateLeft(a4, 15) * 5) ^ cf) * 3) ^ b6 ^ (b2 & ~bf) ^ m9;
            b9 = ~(RotateLeft(b9, 1) ^ a5);
            a6 = ((a6 ^ (RotateLeft(a5, 15) * 5) ^ ce) * 3) ^ b7 ^ (b3 & ~b0) ^ ma;
            ba = ~(RotateLeft(ba, 1) ^ a6);
            a7 = ((a7 ^ (RotateLeft(a6, 15) * 5) ^ cd) * 3) ^ b8 ^ (b4 & ~b1) ^ mb;
            bb = ~(RotateLeft(bb, 1) ^ a7);
            a8 = ((a8 ^ (RotateLeft(a7, 15) * 5) ^ cc) * 3) ^ b9 ^ (b5 & ~b2) ^ mc;
            bc = ~(RotateLeft(bc, 1) ^ a8);
            a9 = ((a9 ^ (RotateLeft(a8, 15) * 5) ^ cb) * 3) ^ ba ^ (b6 & ~b3) ^ md;
            bd = ~(RotateLeft(bd, 1) ^ a9);
            aa = ((aa ^ (RotateLeft(a9, 15) * 5) ^ ca) * 3) ^ bb ^ (b7 & ~b4) ^ me;
            be = ~(RotateLeft(be, 1) ^ aa);
            ab = ((ab ^ (RotateLeft(aa, 15) * 5) ^ c9) * 3) ^ bc ^ (b8 & ~b5) ^ mf;
            bf = ~(RotateLeft(bf, 1) ^ ab);
            #endregion

            ab = ab + c6;
            aa = aa + c5;
            a9 = a9 + c4;
            a8 = a8 + c3;
            a7 = a7 + c2;
            a6 = a6 + c1;
            a5 = a5 + c0;
            a4 = a4 + cf;
            a3 = a3 + ce;
            a2 = a2 + cd;
            a1 = a1 + cc;
            a0 = a0 + cb;
            ab = ab + ca;
            aa = aa + c9;
            a9 = a9 + c8;
            a8 = a8 + c7;
            a7 = a7 + c6;
            a6 = a6 + c5;
            a5 = a5 + c4;
            a4 = a4 + c3;
            a3 = a3 + c2;
            a2 = a2 + c1;
            a1 = a1 + c0;
            a0 = a0 + cf;
            ab = ab + ce;
            aa = aa + cd;
            a9 = a9 + cc;
            a8 = a8 + cb;
            a7 = a7 + ca;
            a6 = a6 + c9;
            a5 = a5 + c8;
            a4 = a4 + c7;
            a3 = a3 + c6;
            a2 = a2 + c5;
            a1 = a1 + c4;
            a0 = a0 + c3;

            #endregion

            c0 -= m0;
            c1 -= m1;
            c2 -= m2;
            c3 -= m3;
            c4 -= m4;
            c5 -= m5;
            c6 -= m6;
            c7 -= m7;
            c8 -= m8;
            c9 -= m9;
            ca -= ma;
            cb -= mb;
            cc -= mc;
            cd -= md;
            ce -= me;
            cf -= mf;

            this.a0 = a0;
            this.a1 = a1;
            this.a2 = a2;
            this.a3 = a3;
            this.a4 = a4;
            this.a5 = a5;
            this.a6 = a6;
            this.a7 = a7;
            this.a8 = a8;
            this.a9 = a9;
            this.aa = aa;
            this.ab = ab;

            this.b0 = c0;
            this.b1 = c1;
            this.b2 = c2;
            this.b3 = c3;
            this.b4 = c4;
            this.b5 = c5;
            this.b6 = c6;
            this.b7 = c7;
            this.b8 = c8;
            this.b9 = c9;
            this.ba = ca;
            this.bb = cb;
            this.bc = cc;
            this.bd = cd;
            this.be = ce;
            this.bf = cf;

            this.c0 = b0;
            this.c1 = b1;
            this.c2 = b2;
            this.c3 = b3;
            this.c4 = b4;
            this.c5 = b5;
            this.c6 = b6;
            this.c7 = b7;
            this.c8 = b8;
            this.c9 = b9;
            this.ca = ba;
            this.cb = bb;
            this.cc = bc;
            this.cd = bd;
            this.ce = be;
            this.cf = bf;
        }
    }
}