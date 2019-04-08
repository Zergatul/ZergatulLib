using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class BMW256 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 64;
        public override int DigestLength => 32;

        protected uint h0, h1, h2, h3, h4, h5, h6, h7, h8, h9, ha, hb, hc, hd, he, hf;
        protected long length;

        public BMW256()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            h0 = 0x40414243;
            h1 = 0x44454647;
            h2 = 0x48494A4B;
            h3 = 0x4C4D4E4F;
            h4 = 0x50515253;
            h5 = 0x54555657;
            h6 = 0x58595A5B;
            h7 = 0x5C5D5E5F;
            h8 = 0x60616263;
            h9 = 0x64656667;
            ha = 0x68696A6B;
            hb = 0x6C6D6E6F;
            hc = 0x70717273;
            hd = 0x74757677;
            he = 0x78797A7B;
            hf = 0x7C7D7E7F;
            bufOffset = 0;
            length = 0;
        }

        protected override void IncreaseLength(int value) => length += value;

        protected override void ProcessBlock()
        {
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

            #region F0

            uint q00 = (m5 ^ h5) - (m7 ^ h7) + (ma ^ ha) + (md ^ hd) + (me ^ he);
            q00 = ((q00 >> 1) ^ (q00 << 3) ^ RotateLeft(q00, 4) ^ RotateLeft(q00, 19)) + h1;
            uint q01 = (m6 ^ h6) - (m8 ^ h8) + (mb ^ hb) + (me ^ he) - (mf ^ hf);
            q01 = ((q01 >> 1) ^ (q01 << 2) ^ RotateLeft(q01, 8) ^ RotateLeft(q01, 23)) + h2;
            uint q02 = (m0 ^ h0) + (m7 ^ h7) + (m9 ^ h9) - (mc ^ hc) + (mf ^ hf);
            q02 = ((q02 >> 2) ^ (q02 << 1) ^ RotateLeft(q02, 12) ^ RotateLeft(q02, 25)) + h3;
            uint q03 = (m0 ^ h0) - (m1 ^ h1) + (m8 ^ h8) - (ma ^ ha) + (md ^ hd);
            q03 = ((q03 >> 2) ^ (q03 << 2) ^ RotateLeft(q03, 15) ^ RotateLeft(q03, 29)) + h4;
            uint q04 = (m1 ^ h1) + (m2 ^ h2) + (m9 ^ h9) - (mb ^ hb) - (me ^ he);
            q04 = ((q04 >> 1) ^ q04) + h5;
            uint q05 = (m3 ^ h3) - (m2 ^ h2) + (ma ^ ha) - (mc ^ hc) + (mf ^ hf);
            q05 = ((q05 >> 1) ^ (q05 << 3) ^ RotateLeft(q05, 4) ^ RotateLeft(q05, 19)) + h6;
            uint q06 = (m4 ^ h4) - (m0 ^ h0) - (m3 ^ h3) - (mb ^ hb) + (md ^ hd);
            q06 = ((q06 >> 1) ^ (q06 << 2) ^ RotateLeft(q06, 8) ^ RotateLeft(q06, 23)) + h7;
            uint q07 = (m1 ^ h1) - (m4 ^ h4) - (m5 ^ h5) - (mc ^ hc) - (me ^ he);
            q07 = ((q07 >> 2) ^ (q07 << 1) ^ RotateLeft(q07, 12) ^ RotateLeft(q07, 25)) + h8;
            uint q08 = (m2 ^ h2) - (m5 ^ h5) - (m6 ^ h6) + (md ^ hd) - (mf ^ hf);
            q08 = ((q08 >> 2) ^ (q08 << 2) ^ RotateLeft(q08, 15) ^ RotateLeft(q08, 29)) + h9;
            uint q09 = (m0 ^ h0) - (m3 ^ h3) + (m6 ^ h6) - (m7 ^ h7) + (me ^ he);
            q09 = ((q09 >> 1) ^ q09) + ha;
            uint q0a = (m8 ^ h8) - (m1 ^ h1) - (m4 ^ h4) - (m7 ^ h7) + (mf ^ hf);
            q0a = ((q0a >> 1) ^ (q0a << 3) ^ RotateLeft(q0a, 4) ^ RotateLeft(q0a, 19)) + hb;
            uint q0b = (m8 ^ h8) - (m0 ^ h0) - (m2 ^ h2) - (m5 ^ h5) + (m9 ^ h9);
            q0b = ((q0b >> 1) ^ (q0b << 2) ^ RotateLeft(q0b, 8) ^ RotateLeft(q0b, 23)) + hc;
            uint q0c = (m1 ^ h1) + (m3 ^ h3) - (m6 ^ h6) - (m9 ^ h9) + (ma ^ ha);
            q0c = ((q0c >> 2) ^ (q0c << 1) ^ RotateLeft(q0c, 12) ^ RotateLeft(q0c, 25)) + hd;
            uint q0d = (m2 ^ h2) + (m4 ^ h4) + (m7 ^ h7) + (ma ^ ha) + (mb ^ hb);
            q0d = ((q0d >> 2) ^ (q0d << 2) ^ RotateLeft(q0d, 15) ^ RotateLeft(q0d, 29)) + he;
            uint q0e = (m3 ^ h3) - (m5 ^ h5) + (m8 ^ h8) - (mb ^ hb) - (mc ^ hc);
            q0e = ((q0e >> 1) ^ q0e) + hf;
            uint q0f = (mc ^ hc) - (m4 ^ h4) - (m6 ^ h6) - (m9 ^ h9) + (md ^ hd);
            q0f = ((q0f >> 1) ^ (q0f << 3) ^ RotateLeft(q0f, 4) ^ RotateLeft(q0f, 19)) + h0;

            #endregion

            #region F1

            uint q10 =
                ((q00 >> 1) ^ (q00 << 2) ^ RotateLeft(q00, 8) ^ RotateLeft(q00, 23)) +
                ((q01 >> 2) ^ (q01 << 1) ^ RotateLeft(q01, 12) ^ RotateLeft(q01, 25)) +
                ((q02 >> 2) ^ (q02 << 2) ^ RotateLeft(q02, 15) ^ RotateLeft(q02, 29)) +
                ((q03 >> 1) ^ (q03 << 3) ^ RotateLeft(q03, 4) ^ RotateLeft(q03, 19)) +
                ((q04 >> 1) ^ (q04 << 2) ^ RotateLeft(q04, 8) ^ RotateLeft(q04, 23)) +
                ((q05 >> 2) ^ (q05 << 1) ^ RotateLeft(q05, 12) ^ RotateLeft(q05, 25)) +
                ((q06 >> 2) ^ (q06 << 2) ^ RotateLeft(q06, 15) ^ RotateLeft(q06, 29)) +
                ((q07 >> 1) ^ (q07 << 3) ^ RotateLeft(q07, 4) ^ RotateLeft(q07, 19)) +
                ((q08 >> 1) ^ (q08 << 2) ^ RotateLeft(q08, 8) ^ RotateLeft(q08, 23)) +
                ((q09 >> 2) ^ (q09 << 1) ^ RotateLeft(q09, 12) ^ RotateLeft(q09, 25)) +
                ((q0a >> 2) ^ (q0a << 2) ^ RotateLeft(q0a, 15) ^ RotateLeft(q0a, 29)) +
                ((q0b >> 1) ^ (q0b << 3) ^ RotateLeft(q0b, 4) ^ RotateLeft(q0b, 19)) +
                ((q0c >> 1) ^ (q0c << 2) ^ RotateLeft(q0c, 8) ^ RotateLeft(q0c, 23)) +
                ((q0d >> 2) ^ (q0d << 1) ^ RotateLeft(q0d, 12) ^ RotateLeft(q0d, 25)) +
                ((q0e >> 2) ^ (q0e << 2) ^ RotateLeft(q0e, 15) ^ RotateLeft(q0e, 29)) +
                ((q0f >> 1) ^ (q0f << 3) ^ RotateLeft(q0f, 4) ^ RotateLeft(q0f, 19)) +
                (h7 ^ (RotateLeft(m0, 1) + RotateLeft(m3, 4) - RotateLeft(ma, 11) + 0x55555550));
            uint q11 =
                ((q01 >> 1) ^ (q01 << 2) ^ RotateLeft(q01, 8) ^ RotateLeft(q01, 23)) +
                ((q02 >> 2) ^ (q02 << 1) ^ RotateLeft(q02, 12) ^ RotateLeft(q02, 25)) +
                ((q03 >> 2) ^ (q03 << 2) ^ RotateLeft(q03, 15) ^ RotateLeft(q03, 29)) +
                ((q04 >> 1) ^ (q04 << 3) ^ RotateLeft(q04, 4) ^ RotateLeft(q04, 19)) +
                ((q05 >> 1) ^ (q05 << 2) ^ RotateLeft(q05, 8) ^ RotateLeft(q05, 23)) +
                ((q06 >> 2) ^ (q06 << 1) ^ RotateLeft(q06, 12) ^ RotateLeft(q06, 25)) +
                ((q07 >> 2) ^ (q07 << 2) ^ RotateLeft(q07, 15) ^ RotateLeft(q07, 29)) +
                ((q08 >> 1) ^ (q08 << 3) ^ RotateLeft(q08, 4) ^ RotateLeft(q08, 19)) +
                ((q09 >> 1) ^ (q09 << 2) ^ RotateLeft(q09, 8) ^ RotateLeft(q09, 23)) +
                ((q0a >> 2) ^ (q0a << 1) ^ RotateLeft(q0a, 12) ^ RotateLeft(q0a, 25)) +
                ((q0b >> 2) ^ (q0b << 2) ^ RotateLeft(q0b, 15) ^ RotateLeft(q0b, 29)) +
                ((q0c >> 1) ^ (q0c << 3) ^ RotateLeft(q0c, 4) ^ RotateLeft(q0c, 19)) +
                ((q0d >> 1) ^ (q0d << 2) ^ RotateLeft(q0d, 8) ^ RotateLeft(q0d, 23)) +
                ((q0e >> 2) ^ (q0e << 1) ^ RotateLeft(q0e, 12) ^ RotateLeft(q0e, 25)) +
                ((q0f >> 2) ^ (q0f << 2) ^ RotateLeft(q0f, 15) ^ RotateLeft(q0f, 29)) +
                ((q10 >> 1) ^ (q10 << 3) ^ RotateLeft(q10, 4) ^ RotateLeft(q10, 19)) +
                (h8 ^ (RotateLeft(m1, 2) + RotateLeft(m4, 5) - RotateLeft(mb, 12) + 0x5AAAAAA5));
            uint q12 =
                q02 + RotateLeft(q03, 3) + q04 + RotateLeft(q05, 7) +
                q06 + RotateLeft(q07, 13) + q08 + RotateLeft(q09, 16) +
                q0a + RotateLeft(q0b, 19) + q0c + RotateLeft(q0d, 23) +
                q0e + RotateLeft(q0f, 27) + ((q10 >> 1) ^ q10) + ((q11 >> 2) ^ q11) +
                (h9 ^ (RotateLeft(m2, 3) + RotateLeft(m5, 6) - RotateLeft(mc, 13) + 0x5FFFFFFA));
            uint q13 =
                q03 + RotateLeft(q04, 3) + q05 + RotateLeft(q06, 7) +
                q07 + RotateLeft(q08, 13) + q09 + RotateLeft(q0a, 16) +
                q0b + RotateLeft(q0c, 19) + q0d + RotateLeft(q0e, 23) +
                q0f + RotateLeft(q10, 27) + ((q11 >> 1) ^ q11) + ((q12 >> 2) ^ q12) +
                (ha ^ (RotateLeft(m3, 4) + RotateLeft(m6, 7) - RotateLeft(md, 14) + 0x6555554F));
            uint q14 =
                q04 + RotateLeft(q05, 3) + q06 + RotateLeft(q07, 7) +
                q08 + RotateLeft(q09, 13) + q0a + RotateLeft(q0b, 16) +
                q0c + RotateLeft(q0d, 19) + q0e + RotateLeft(q0f, 23) +
                q10 + RotateLeft(q11, 27) + ((q12 >> 1) ^ q12) + ((q13 >> 2) ^ q13) +
                (hb ^ (RotateLeft(m4, 5) + RotateLeft(m7, 8) - RotateLeft(me, 15) + 0x6AAAAAA4));
            uint q15 =
                q05 + RotateLeft(q06, 3) + q07 + RotateLeft(q08, 7) +
                q09 + RotateLeft(q0a, 13) + q0b + RotateLeft(q0c, 16) +
                q0d + RotateLeft(q0e, 19) + q0f + RotateLeft(q10, 23) +
                q11 + RotateLeft(q12, 27) + ((q13 >> 1) ^ q13) + ((q14 >> 2) ^ q14) +
                (hc ^ (RotateLeft(m5, 6) + RotateLeft(m8, 9) - RotateLeft(mf, 16) + 0x6FFFFFF9));
            uint q16 =
                q06 + RotateLeft(q07, 3) + q08 + RotateLeft(q09, 7) +
                q0a + RotateLeft(q0b, 13) + q0c + RotateLeft(q0d, 16) +
                q0e + RotateLeft(q0f, 19) + q10 + RotateLeft(q11, 23) +
                q12 + RotateLeft(q13, 27) + ((q14 >> 1) ^ q14) + ((q15 >> 2) ^ q15) +
                (hd ^ (RotateLeft(m6, 7) + RotateLeft(m9, 10) - RotateLeft(m0, 1) + 0x7555554E));
            uint q17 =
                q07 + RotateLeft(q08, 3) + q09 + RotateLeft(q0a, 7) +
                q0b + RotateLeft(q0c, 13) + q0d + RotateLeft(q0e, 16) +
                q0f + RotateLeft(q10, 19) + q11 + RotateLeft(q12, 23) +
                q13 + RotateLeft(q14, 27) + ((q15 >> 1) ^ q15) + ((q16 >> 2) ^ q16) +
                (he ^ (RotateLeft(m7, 8) + RotateLeft(ma, 11) - RotateLeft(m1, 2) + 0x7AAAAAA3));
            uint q18 =
                q08 + RotateLeft(q09, 3) + q0a + RotateLeft(q0b, 7) +
                q0c + RotateLeft(q0d, 13) + q0e + RotateLeft(q0f, 16) +
                q10 + RotateLeft(q11, 19) + q12 + RotateLeft(q13, 23) +
                q14 + RotateLeft(q15, 27) + ((q16 >> 1) ^ q16) + ((q17 >> 2) ^ q17) +
                (hf ^ (RotateLeft(m8, 9) + RotateLeft(mb, 12) - RotateLeft(m2, 3) + 0x7FFFFFF8));
            uint q19 =
                q09 + RotateLeft(q0a, 3) + q0b + RotateLeft(q0c, 7) +
                q0d + RotateLeft(q0e, 13) + q0f + RotateLeft(q10, 16) +
                q11 + RotateLeft(q12, 19) + q13 + RotateLeft(q14, 23) +
                q15 + RotateLeft(q16, 27) + ((q17 >> 1) ^ q17) + ((q18 >> 2) ^ q18) +
                (h0 ^ (RotateLeft(m9, 10) + RotateLeft(mc, 13) - RotateLeft(m3, 4) + 0x8555554D));
            uint q1a =
                q0a + RotateLeft(q0b, 3) + q0c + RotateLeft(q0d, 7) +
                q0e + RotateLeft(q0f, 13) + q10 + RotateLeft(q11, 16) +
                q12 + RotateLeft(q13, 19) + q14 + RotateLeft(q15, 23) +
                q16 + RotateLeft(q17, 27) + ((q18 >> 1) ^ q18) + ((q19 >> 2) ^ q19) +
                (h1 ^ (RotateLeft(ma, 11) + RotateLeft(md, 14) - RotateLeft(m4, 5) + 0x8AAAAAA2));
            uint q1b =
                q0b + RotateLeft(q0c, 3) + q0d + RotateLeft(q0e, 7) +
                q0f + RotateLeft(q10, 13) + q11 + RotateLeft(q12, 16) +
                q13 + RotateLeft(q14, 19) + q15 + RotateLeft(q16, 23) +
                q17 + RotateLeft(q18, 27) + ((q19 >> 1) ^ q19) + ((q1a >> 2) ^ q1a) +
                (h2 ^ (RotateLeft(mb, 12) + RotateLeft(me, 15) - RotateLeft(m5, 6) + 0x8FFFFFF7));
            uint q1c =
                q0c + RotateLeft(q0d, 3) + q0e + RotateLeft(q0f, 7) +
                q10 + RotateLeft(q11, 13) + q12 + RotateLeft(q13, 16) +
                q14 + RotateLeft(q15, 19) + q16 + RotateLeft(q17, 23) +
                q18 + RotateLeft(q19, 27) + ((q1a >> 1) ^ q1a) + ((q1b >> 2) ^ q1b) +
                (h3 ^ (RotateLeft(mc, 13) + RotateLeft(mf, 16) - RotateLeft(m6, 7) + 0x9555554C));
            uint q1d =
                q0d + RotateLeft(q0e, 3) + q0f + RotateLeft(q10, 7) +
                q11 + RotateLeft(q12, 13) + q13 + RotateLeft(q14, 16) +
                q15 + RotateLeft(q16, 19) + q17 + RotateLeft(q18, 23) +
                q19 + RotateLeft(q1a, 27) + ((q1b >> 1) ^ q1b) + ((q1c >> 2) ^ q1c) +
                (h4 ^ (RotateLeft(md, 14) + RotateLeft(m0, 1) - RotateLeft(m7, 8) + 0x9AAAAAA1));
            uint q1e =
                q0e + RotateLeft(q0f, 3) + q10 + RotateLeft(q11, 7) +
                q12 + RotateLeft(q13, 13) + q14 + RotateLeft(q15, 16) +
                q16 + RotateLeft(q17, 19) + q18 + RotateLeft(q19, 23) +
                q1a + RotateLeft(q1b, 27) + ((q1c >> 1) ^ q1c) + ((q1d >> 2) ^ q1d) +
                (h5 ^ (RotateLeft(me, 15) + RotateLeft(m1, 2) - RotateLeft(m8, 9) + 0x9FFFFFF6));
            uint q1f =
                q0f + RotateLeft(q10, 3) + q11 + RotateLeft(q12, 7) +
                q13 + RotateLeft(q14, 13) + q15 + RotateLeft(q16, 16) +
                q17 + RotateLeft(q18, 19) + q19 + RotateLeft(q1a, 23) +
                q1b + RotateLeft(q1c, 27) + ((q1d >> 1) ^ q1d) + ((q1e >> 2) ^ q1e) +
                (h6 ^ (RotateLeft(mf, 16) + RotateLeft(m2, 3) - RotateLeft(m9, 10) + 0xA555554B));

            #endregion

            #region F2

            uint xl = q10 ^ q11 ^ q12 ^ q13 ^ q14 ^ q15 ^ q16 ^ q17;
            uint xh = q18 ^ q19 ^ q1a ^ q1b ^ q1c ^ q1d ^ q1e ^ q1f ^ xl;
            h0 = ((xh << 5) ^ (q10 >> 5) ^ m0) + (xl ^ q18 ^ q00);
            h1 = ((xh >> 7) ^ (q11 << 8) ^ m1) + (xl ^ q19 ^ q01);
            h2 = ((xh >> 5) ^ (q12 << 5) ^ m2) + (xl ^ q1a ^ q02);
            h3 = ((xh >> 1) ^ (q13 << 5) ^ m3) + (xl ^ q1b ^ q03);
            h4 = ((xh >> 3) ^ (q14 << 0) ^ m4) + (xl ^ q1c ^ q04);
            h5 = ((xh << 6) ^ (q15 >> 6) ^ m5) + (xl ^ q1d ^ q05);
            h6 = ((xh >> 4) ^ (q16 << 6) ^ m6) + (xl ^ q1e ^ q06);
            h7 = ((xh >> 11) ^ (q17 << 2) ^ m7) + (xl ^ q1f ^ q07);
            h8 = RotateLeft(h4, 9) + (xh ^ q18 ^ m8) + ((xl << 8) ^ q17 ^ q08);
            h9 = RotateLeft(h5, 10) + (xh ^ q19 ^ m9) + ((xl >> 6) ^ q10 ^ q09);
            ha = RotateLeft(h6, 11) + (xh ^ q1a ^ ma) + ((xl << 6) ^ q11 ^ q0a);
            hb = RotateLeft(h7, 12) + (xh ^ q1b ^ mb) + ((xl << 4) ^ q12 ^ q0b);
            hc = RotateLeft(h0, 13) + (xh ^ q1c ^ mc) + ((xl >> 3) ^ q13 ^ q0c);
            hd = RotateLeft(h1, 14) + (xh ^ q1d ^ md) + ((xl >> 4) ^ q14 ^ q0d);
            he = RotateLeft(h2, 15) + (xh ^ q1e ^ me) + ((xl >> 7) ^ q15 ^ q0e);
            hf = RotateLeft(h3, 16) + (xh ^ q1f ^ mf) + ((xl >> 2) ^ q16 ^ q0f);

            #endregion
        }

        protected override void AddPadding()
        {
            buffer[bufOffset++] = 0x80;

            if (64 - bufOffset < 8)
            {
                while (bufOffset < 64)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
            }

            while (bufOffset < 56)
                buffer[bufOffset++] = 0;

            GetBytes(length << 3, ByteOrder.LittleEndian, buffer, 56);
        }

        protected void FinalProcess()
        {
            GetBytes(h0, ByteOrder.LittleEndian, buffer, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, buffer, 0x04);
            GetBytes(h2, ByteOrder.LittleEndian, buffer, 0x08);
            GetBytes(h3, ByteOrder.LittleEndian, buffer, 0x0C);
            GetBytes(h4, ByteOrder.LittleEndian, buffer, 0x10);
            GetBytes(h5, ByteOrder.LittleEndian, buffer, 0x14);
            GetBytes(h6, ByteOrder.LittleEndian, buffer, 0x18);
            GetBytes(h7, ByteOrder.LittleEndian, buffer, 0x1C);
            GetBytes(h8, ByteOrder.LittleEndian, buffer, 0x20);
            GetBytes(h9, ByteOrder.LittleEndian, buffer, 0x24);
            GetBytes(ha, ByteOrder.LittleEndian, buffer, 0x28);
            GetBytes(hb, ByteOrder.LittleEndian, buffer, 0x2C);
            GetBytes(hc, ByteOrder.LittleEndian, buffer, 0x30);
            GetBytes(hd, ByteOrder.LittleEndian, buffer, 0x34);
            GetBytes(he, ByteOrder.LittleEndian, buffer, 0x38);
            GetBytes(hf, ByteOrder.LittleEndian, buffer, 0x3C);

            h0 = 0xAAAAAAA0;
            h1 = 0xAAAAAAA1;
            h2 = 0xAAAAAAA2;
            h3 = 0xAAAAAAA3;
            h4 = 0xAAAAAAA4;
            h5 = 0xAAAAAAA5;
            h6 = 0xAAAAAAA6;
            h7 = 0xAAAAAAA7;
            h8 = 0xAAAAAAA8;
            h9 = 0xAAAAAAA9;
            ha = 0xAAAAAAAA;
            hb = 0xAAAAAAAB;
            hc = 0xAAAAAAAC;
            hd = 0xAAAAAAAD;
            he = 0xAAAAAAAE;
            hf = 0xAAAAAAAF;

            ProcessBlock();
        }

        protected override byte[] InternalStateToDigest()
        {
            FinalProcess();

            byte[] digest = new byte[32];
            GetBytes(h8, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h9, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(ha, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(hb, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(hc, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(hd, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(he, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(hf, ByteOrder.LittleEndian, digest, 0x1C);
            return digest;
        }
    }
}