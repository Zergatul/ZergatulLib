using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class BMW512 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 128;
        public override int DigestLength => 64;

        protected ulong h0, h1, h2, h3, h4, h5, h6, h7, h8, h9, ha, hb, hc, hd, he, hf;
        protected ulong length;

        public BMW512()
        {
            buffer = new byte[128];
            Reset();
        }

        public override void Reset()
        {
            h0 = 0x8081828384858687;
            h1 = 0x88898A8B8C8D8E8F;
            h2 = 0x9091929394959697;
            h3 = 0x98999A9B9C9D9E9F;
            h4 = 0xA0A1A2A3A4A5A6A7;
            h5 = 0xA8A9AAABACADAEAF;
            h6 = 0xB0B1B2B3B4B5B6B7;
            h7 = 0xB8B9BABBBCBDBEBF;
            h8 = 0xC0C1C2C3C4C5C6C7;
            h9 = 0xC8C9CACBCCCDCECF;
            ha = 0xD0D1D2D3D4D5D6D7;
            hb = 0xD8D9DADBDCDDDEDF;
            hc = 0xE0E1E2E3E4E5E6E7;
            hd = 0xE8E9EAEBECEDEEEF;
            he = 0xF0F1F2F3F4F5F6F7;
            hf = 0xF8F9FAFBFCFDFEFF;
            bufOffset = 0;
            length = 0;
        }

        protected override void IncreaseLength(int value) => length += (ulong)value;

        protected override void ProcessBlock()
        {
            ulong m0 = ToUInt64(buffer, 0x00, ByteOrder.LittleEndian);
            ulong m1 = ToUInt64(buffer, 0x08, ByteOrder.LittleEndian);
            ulong m2 = ToUInt64(buffer, 0x10, ByteOrder.LittleEndian);
            ulong m3 = ToUInt64(buffer, 0x18, ByteOrder.LittleEndian);
            ulong m4 = ToUInt64(buffer, 0x20, ByteOrder.LittleEndian);
            ulong m5 = ToUInt64(buffer, 0x28, ByteOrder.LittleEndian);
            ulong m6 = ToUInt64(buffer, 0x30, ByteOrder.LittleEndian);
            ulong m7 = ToUInt64(buffer, 0x38, ByteOrder.LittleEndian);
            ulong m8 = ToUInt64(buffer, 0x40, ByteOrder.LittleEndian);
            ulong m9 = ToUInt64(buffer, 0x48, ByteOrder.LittleEndian);
            ulong ma = ToUInt64(buffer, 0x50, ByteOrder.LittleEndian);
            ulong mb = ToUInt64(buffer, 0x58, ByteOrder.LittleEndian);
            ulong mc = ToUInt64(buffer, 0x60, ByteOrder.LittleEndian);
            ulong md = ToUInt64(buffer, 0x68, ByteOrder.LittleEndian);
            ulong me = ToUInt64(buffer, 0x70, ByteOrder.LittleEndian);
            ulong mf = ToUInt64(buffer, 0x78, ByteOrder.LittleEndian);

            #region F0

            ulong q00 = (m5 ^ h5) - (m7 ^ h7) + (ma ^ ha) + (md ^ hd) + (me ^ he);
            q00 = ((q00 >> 1) ^ (q00 << 3) ^ RotateLeft(q00, 4) ^ RotateLeft(q00, 37)) + h1;
            ulong q01 = (m6 ^ h6) - (m8 ^ h8) + (mb ^ hb) + (me ^ he) - (mf ^ hf);
            q01 = ((q01 >> 1) ^ (q01 << 2) ^ RotateLeft(q01, 13) ^ RotateLeft(q01, 43)) + h2;
            ulong q02 = (m0 ^ h0) + (m7 ^ h7) + (m9 ^ h9) - (mc ^ hc) + (mf ^ hf);
            q02 = ((q02 >> 2) ^ (q02 << 1) ^ RotateLeft(q02, 19) ^ RotateLeft(q02, 53)) + h3;
            ulong q03 = (m0 ^ h0) - (m1 ^ h1) + (m8 ^ h8) - (ma ^ ha) + (md ^ hd);
            q03 = ((q03 >> 2) ^ (q03 << 2) ^ RotateLeft(q03, 28) ^ RotateLeft(q03, 59)) + h4;
            ulong q04 = (m1 ^ h1) + (m2 ^ h2) + (m9 ^ h9) - (mb ^ hb) - (me ^ he);
            q04 = ((q04 >> 1) ^ q04) + h5;
            ulong q05 = (m3 ^ h3) - (m2 ^ h2) + (ma ^ ha) - (mc ^ hc) + (mf ^ hf);
            q05 = ((q05 >> 1) ^ (q05 << 3) ^ RotateLeft(q05, 4) ^ RotateLeft(q05, 37)) + h6;
            ulong q06 = (m4 ^ h4) - (m0 ^ h0) - (m3 ^ h3) - (mb ^ hb) + (md ^ hd);
            q06 = ((q06 >> 1) ^ (q06 << 2) ^ RotateLeft(q06, 13) ^ RotateLeft(q06, 43)) + h7;
            ulong q07 = (m1 ^ h1) - (m4 ^ h4) - (m5 ^ h5) - (mc ^ hc) - (me ^ he);
            q07 = ((q07 >> 2) ^ (q07 << 1) ^ RotateLeft(q07, 19) ^ RotateLeft(q07, 53)) + h8;
            ulong q08 = (m2 ^ h2) - (m5 ^ h5) - (m6 ^ h6) + (md ^ hd) - (mf ^ hf);
            q08 = ((q08 >> 2) ^ (q08 << 2) ^ RotateLeft(q08, 28) ^ RotateLeft(q08, 59)) + h9;
            ulong q09 = (m0 ^ h0) - (m3 ^ h3) + (m6 ^ h6) - (m7 ^ h7) + (me ^ he);
            q09 = ((q09 >> 1) ^ q09) + ha;
            ulong q0a = (m8 ^ h8) - (m1 ^ h1) - (m4 ^ h4) - (m7 ^ h7) + (mf ^ hf);
            q0a = ((q0a >> 1) ^ (q0a << 3) ^ RotateLeft(q0a, 4) ^ RotateLeft(q0a, 37)) + hb;
            ulong q0b = (m8 ^ h8) - (m0 ^ h0) - (m2 ^ h2) - (m5 ^ h5) + (m9 ^ h9);
            q0b = ((q0b >> 1) ^ (q0b << 2) ^ RotateLeft(q0b, 13) ^ RotateLeft(q0b, 43)) + hc;
            ulong q0c = (m1 ^ h1) + (m3 ^ h3) - (m6 ^ h6) - (m9 ^ h9) + (ma ^ ha);
            q0c = ((q0c >> 2) ^ (q0c << 1) ^ RotateLeft(q0c, 19) ^ RotateLeft(q0c, 53)) + hd;
            ulong q0d = (m2 ^ h2) + (m4 ^ h4) + (m7 ^ h7) + (ma ^ ha) + (mb ^ hb);
            q0d = ((q0d >> 2) ^ (q0d << 2) ^ RotateLeft(q0d, 28) ^ RotateLeft(q0d, 59)) + he;
            ulong q0e = (m3 ^ h3) - (m5 ^ h5) + (m8 ^ h8) - (mb ^ hb) - (mc ^ hc);
            q0e = ((q0e >> 1) ^ q0e) + hf;
            ulong q0f = (mc ^ hc) - (m4 ^ h4) - (m6 ^ h6) - (m9 ^ h9) + (md ^ hd);
            q0f = ((q0f >> 1) ^ (q0f << 3) ^ RotateLeft(q0f, 4) ^ RotateLeft(q0f, 37)) + h0;

            #endregion

            #region F1

            ulong q10 =
                ((q00 >> 1) ^ (q00 << 2) ^ RotateLeft(q00, 13) ^ RotateLeft(q00, 43)) +
                ((q01 >> 2) ^ (q01 << 1) ^ RotateLeft(q01, 19) ^ RotateLeft(q01, 53)) +
                ((q02 >> 2) ^ (q02 << 2) ^ RotateLeft(q02, 28) ^ RotateLeft(q02, 59)) +
                ((q03 >> 1) ^ (q03 << 3) ^ RotateLeft(q03, 4) ^ RotateLeft(q03, 37)) +
                ((q04 >> 1) ^ (q04 << 2) ^ RotateLeft(q04, 13) ^ RotateLeft(q04, 43)) +
                ((q05 >> 2) ^ (q05 << 1) ^ RotateLeft(q05, 19) ^ RotateLeft(q05, 53)) +
                ((q06 >> 2) ^ (q06 << 2) ^ RotateLeft(q06, 28) ^ RotateLeft(q06, 59)) +
                ((q07 >> 1) ^ (q07 << 3) ^ RotateLeft(q07, 4) ^ RotateLeft(q07, 37)) +
                ((q08 >> 1) ^ (q08 << 2) ^ RotateLeft(q08, 13) ^ RotateLeft(q08, 43)) +
                ((q09 >> 2) ^ (q09 << 1) ^ RotateLeft(q09, 19) ^ RotateLeft(q09, 53)) +
                ((q0a >> 2) ^ (q0a << 2) ^ RotateLeft(q0a, 28) ^ RotateLeft(q0a, 59)) +
                ((q0b >> 1) ^ (q0b << 3) ^ RotateLeft(q0b, 4) ^ RotateLeft(q0b, 37)) +
                ((q0c >> 1) ^ (q0c << 2) ^ RotateLeft(q0c, 13) ^ RotateLeft(q0c, 43)) +
                ((q0d >> 2) ^ (q0d << 1) ^ RotateLeft(q0d, 19) ^ RotateLeft(q0d, 53)) +
                ((q0e >> 2) ^ (q0e << 2) ^ RotateLeft(q0e, 28) ^ RotateLeft(q0e, 59)) +
                ((q0f >> 1) ^ (q0f << 3) ^ RotateLeft(q0f, 4) ^ RotateLeft(q0f, 37)) +
                (h7 ^ (RotateLeft(m0, 1) + RotateLeft(m3, 4) - RotateLeft(ma, 11) + 0x5555555555555550));
            ulong q11 =
                ((q01 >> 1) ^ (q01 << 2) ^ RotateLeft(q01, 13) ^ RotateLeft(q01, 43)) +
                ((q02 >> 2) ^ (q02 << 1) ^ RotateLeft(q02, 19) ^ RotateLeft(q02, 53)) +
                ((q03 >> 2) ^ (q03 << 2) ^ RotateLeft(q03, 28) ^ RotateLeft(q03, 59)) +
                ((q04 >> 1) ^ (q04 << 3) ^ RotateLeft(q04, 4) ^ RotateLeft(q04, 37)) +
                ((q05 >> 1) ^ (q05 << 2) ^ RotateLeft(q05, 13) ^ RotateLeft(q05, 43)) +
                ((q06 >> 2) ^ (q06 << 1) ^ RotateLeft(q06, 19) ^ RotateLeft(q06, 53)) +
                ((q07 >> 2) ^ (q07 << 2) ^ RotateLeft(q07, 28) ^ RotateLeft(q07, 59)) +
                ((q08 >> 1) ^ (q08 << 3) ^ RotateLeft(q08, 4) ^ RotateLeft(q08, 37)) +
                ((q09 >> 1) ^ (q09 << 2) ^ RotateLeft(q09, 13) ^ RotateLeft(q09, 43)) +
                ((q0a >> 2) ^ (q0a << 1) ^ RotateLeft(q0a, 19) ^ RotateLeft(q0a, 53)) +
                ((q0b >> 2) ^ (q0b << 2) ^ RotateLeft(q0b, 28) ^ RotateLeft(q0b, 59)) +
                ((q0c >> 1) ^ (q0c << 3) ^ RotateLeft(q0c, 4) ^ RotateLeft(q0c, 37)) +
                ((q0d >> 1) ^ (q0d << 2) ^ RotateLeft(q0d, 13) ^ RotateLeft(q0d, 43)) +
                ((q0e >> 2) ^ (q0e << 1) ^ RotateLeft(q0e, 19) ^ RotateLeft(q0e, 53)) +
                ((q0f >> 2) ^ (q0f << 2) ^ RotateLeft(q0f, 28) ^ RotateLeft(q0f, 59)) +
                ((q10 >> 1) ^ (q10 << 3) ^ RotateLeft(q10, 4) ^ RotateLeft(q10, 37)) +
                (h8 ^ (RotateLeft(m1, 2) + RotateLeft(m4, 5) - RotateLeft(mb, 12) + 0x5AAAAAAAAAAAAAA5));
            ulong q12 =
                q02 + RotateLeft(q03, 5) + q04 + RotateLeft(q05, 11) +
                q06 + RotateLeft(q07, 27) + q08 + RotateLeft(q09, 32) +
                q0a + RotateLeft(q0b, 37) + q0c + RotateLeft(q0d, 43) +
                q0e + RotateLeft(q0f, 53) + ((q10 >> 1) ^ q10) + ((q11 >> 2) ^ q11) +
                (h9 ^ (RotateLeft(m2, 3) + RotateLeft(m5, 6) - RotateLeft(mc, 13) + 0x5FFFFFFFFFFFFFFA));
            ulong q13 =
                q03 + RotateLeft(q04, 5) + q05 + RotateLeft(q06, 11) +
                q07 + RotateLeft(q08, 27) + q09 + RotateLeft(q0a, 32) +
                q0b + RotateLeft(q0c, 37) + q0d + RotateLeft(q0e, 43) +
                q0f + RotateLeft(q10, 53) + ((q11 >> 1) ^ q11) + ((q12 >> 2) ^ q12) +
                (ha ^ (RotateLeft(m3, 4) + RotateLeft(m6, 7) - RotateLeft(md, 14) + 0x655555555555554F));
            ulong q14 =
                q04 + RotateLeft(q05, 5) + q06 + RotateLeft(q07, 11) +
                q08 + RotateLeft(q09, 27) + q0a + RotateLeft(q0b, 32) +
                q0c + RotateLeft(q0d, 37) + q0e + RotateLeft(q0f, 43) +
                q10 + RotateLeft(q11, 53) + ((q12 >> 1) ^ q12) + ((q13 >> 2) ^ q13) +
                (hb ^ (RotateLeft(m4, 5) + RotateLeft(m7, 8) - RotateLeft(me, 15) + 0x6AAAAAAAAAAAAAA4));
            ulong q15 =
                q05 + RotateLeft(q06, 5) + q07 + RotateLeft(q08, 11) +
                q09 + RotateLeft(q0a, 27) + q0b + RotateLeft(q0c, 32) +
                q0d + RotateLeft(q0e, 37) + q0f + RotateLeft(q10, 43) +
                q11 + RotateLeft(q12, 53) + ((q13 >> 1) ^ q13) + ((q14 >> 2) ^ q14) +
                (hc ^ (RotateLeft(m5, 6) + RotateLeft(m8, 9) - RotateLeft(mf, 16) + 0x6FFFFFFFFFFFFFF9));
            ulong q16 =
                q06 + RotateLeft(q07, 5) + q08 + RotateLeft(q09, 11) +
                q0a + RotateLeft(q0b, 27) + q0c + RotateLeft(q0d, 32) +
                q0e + RotateLeft(q0f, 37) + q10 + RotateLeft(q11, 43) +
                q12 + RotateLeft(q13, 53) + ((q14 >> 1) ^ q14) + ((q15 >> 2) ^ q15) +
                (hd ^ (RotateLeft(m6, 7) + RotateLeft(m9, 10) - RotateLeft(m0, 1) + 0x755555555555554E));
            ulong q17 =
                q07 + RotateLeft(q08, 5) + q09 + RotateLeft(q0a, 11) +
                q0b + RotateLeft(q0c, 27) + q0d + RotateLeft(q0e, 32) +
                q0f + RotateLeft(q10, 37) + q11 + RotateLeft(q12, 43) +
                q13 + RotateLeft(q14, 53) + ((q15 >> 1) ^ q15) + ((q16 >> 2) ^ q16) +
                (he ^ (RotateLeft(m7, 8) + RotateLeft(ma, 11) - RotateLeft(m1, 2) + 0x7AAAAAAAAAAAAAA3));
            ulong q18 =
                q08 + RotateLeft(q09, 5) + q0a + RotateLeft(q0b, 11) +
                q0c + RotateLeft(q0d, 27) + q0e + RotateLeft(q0f, 32) +
                q10 + RotateLeft(q11, 37) + q12 + RotateLeft(q13, 43) +
                q14 + RotateLeft(q15, 53) + ((q16 >> 1) ^ q16) + ((q17 >> 2) ^ q17) +
                (hf ^ (RotateLeft(m8, 9) + RotateLeft(mb, 12) - RotateLeft(m2, 3) + 0x7FFFFFFFFFFFFFF8));
            ulong q19 =
                q09 + RotateLeft(q0a, 5) + q0b + RotateLeft(q0c, 11) +
                q0d + RotateLeft(q0e, 27) + q0f + RotateLeft(q10, 32) +
                q11 + RotateLeft(q12, 37) + q13 + RotateLeft(q14, 43) +
                q15 + RotateLeft(q16, 53) + ((q17 >> 1) ^ q17) + ((q18 >> 2) ^ q18) +
                (h0 ^ (RotateLeft(m9, 10) + RotateLeft(mc, 13) - RotateLeft(m3, 4) + 0x855555555555554D));
            ulong q1a =
                q0a + RotateLeft(q0b, 5) + q0c + RotateLeft(q0d, 11) +
                q0e + RotateLeft(q0f, 27) + q10 + RotateLeft(q11, 32) +
                q12 + RotateLeft(q13, 37) + q14 + RotateLeft(q15, 43) +
                q16 + RotateLeft(q17, 53) + ((q18 >> 1) ^ q18) + ((q19 >> 2) ^ q19) +
                (h1 ^ (RotateLeft(ma, 11) + RotateLeft(md, 14) - RotateLeft(m4, 5) + 0x8AAAAAAAAAAAAAA2));
            ulong q1b =
                q0b + RotateLeft(q0c, 5) + q0d + RotateLeft(q0e, 11) +
                q0f + RotateLeft(q10, 27) + q11 + RotateLeft(q12, 32) +
                q13 + RotateLeft(q14, 37) + q15 + RotateLeft(q16, 43) +
                q17 + RotateLeft(q18, 53) + ((q19 >> 1) ^ q19) + ((q1a >> 2) ^ q1a) +
                (h2 ^ (RotateLeft(mb, 12) + RotateLeft(me, 15) - RotateLeft(m5, 6) + 0x8FFFFFFFFFFFFFF7));
            ulong q1c =
                q0c + RotateLeft(q0d, 5) + q0e + RotateLeft(q0f, 11) +
                q10 + RotateLeft(q11, 27) + q12 + RotateLeft(q13, 32) +
                q14 + RotateLeft(q15, 37) + q16 + RotateLeft(q17, 43) +
                q18 + RotateLeft(q19, 53) + ((q1a >> 1) ^ q1a) + ((q1b >> 2) ^ q1b) +
                (h3 ^ (RotateLeft(mc, 13) + RotateLeft(mf, 16) - RotateLeft(m6, 7) + 0x955555555555554C));
            ulong q1d =
                q0d + RotateLeft(q0e, 5) + q0f + RotateLeft(q10, 11) +
                q11 + RotateLeft(q12, 27) + q13 + RotateLeft(q14, 32) +
                q15 + RotateLeft(q16, 37) + q17 + RotateLeft(q18, 43) +
                q19 + RotateLeft(q1a, 53) + ((q1b >> 1) ^ q1b) + ((q1c >> 2) ^ q1c) +
                (h4 ^ (RotateLeft(md, 14) + RotateLeft(m0, 1) - RotateLeft(m7, 8) + 0x9AAAAAAAAAAAAAA1));
            ulong q1e =
                q0e + RotateLeft(q0f, 5) + q10 + RotateLeft(q11, 11) +
                q12 + RotateLeft(q13, 27) + q14 + RotateLeft(q15, 32) +
                q16 + RotateLeft(q17, 37) + q18 + RotateLeft(q19, 43) +
                q1a + RotateLeft(q1b, 53) + ((q1c >> 1) ^ q1c) + ((q1d >> 2) ^ q1d) +
                (h5 ^ (RotateLeft(me, 15) + RotateLeft(m1, 2) - RotateLeft(m8, 9) + 0x9FFFFFFFFFFFFFF6));
            ulong q1f =
                q0f + RotateLeft(q10, 5) + q11 + RotateLeft(q12, 11) +
                q13 + RotateLeft(q14, 27) + q15 + RotateLeft(q16, 32) +
                q17 + RotateLeft(q18, 37) + q19 + RotateLeft(q1a, 43) +
                q1b + RotateLeft(q1c, 53) + ((q1d >> 1) ^ q1d) + ((q1e >> 2) ^ q1e) +
                (h6 ^ (RotateLeft(mf, 16) + RotateLeft(m2, 3) - RotateLeft(m9, 10) + 0xA55555555555554B));

            #endregion

            #region F2

            ulong xl = q10 ^ q11 ^ q12 ^ q13 ^ q14 ^ q15 ^ q16 ^ q17;
            ulong xh = q18 ^ q19 ^ q1a ^ q1b ^ q1c ^ q1d ^ q1e ^ q1f ^ xl;
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

            if (128 - bufOffset < 8)
            {
                while (bufOffset < 128)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
            }

            while (bufOffset < 120)
                buffer[bufOffset++] = 0;

            GetBytes(length << 3, ByteOrder.LittleEndian, buffer, 120);
        }

        protected void FinalProcess()
        {
            GetBytes(h0, ByteOrder.LittleEndian, buffer, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, buffer, 0x08);
            GetBytes(h2, ByteOrder.LittleEndian, buffer, 0x10);
            GetBytes(h3, ByteOrder.LittleEndian, buffer, 0x18);
            GetBytes(h4, ByteOrder.LittleEndian, buffer, 0x20);
            GetBytes(h5, ByteOrder.LittleEndian, buffer, 0x28);
            GetBytes(h6, ByteOrder.LittleEndian, buffer, 0x30);
            GetBytes(h7, ByteOrder.LittleEndian, buffer, 0x38);
            GetBytes(h8, ByteOrder.LittleEndian, buffer, 0x40);
            GetBytes(h9, ByteOrder.LittleEndian, buffer, 0x48);
            GetBytes(ha, ByteOrder.LittleEndian, buffer, 0x50);
            GetBytes(hb, ByteOrder.LittleEndian, buffer, 0x58);
            GetBytes(hc, ByteOrder.LittleEndian, buffer, 0x60);
            GetBytes(hd, ByteOrder.LittleEndian, buffer, 0x68);
            GetBytes(he, ByteOrder.LittleEndian, buffer, 0x70);
            GetBytes(hf, ByteOrder.LittleEndian, buffer, 0x78);

            h0 = 0xAAAAAAAAAAAAAAA0;
            h1 = 0xAAAAAAAAAAAAAAA1;
            h2 = 0xAAAAAAAAAAAAAAA2;
            h3 = 0xAAAAAAAAAAAAAAA3;
            h4 = 0xAAAAAAAAAAAAAAA4;
            h5 = 0xAAAAAAAAAAAAAAA5;
            h6 = 0xAAAAAAAAAAAAAAA6;
            h7 = 0xAAAAAAAAAAAAAAA7;
            h8 = 0xAAAAAAAAAAAAAAA8;
            h9 = 0xAAAAAAAAAAAAAAA9;
            ha = 0xAAAAAAAAAAAAAAAA;
            hb = 0xAAAAAAAAAAAAAAAB;
            hc = 0xAAAAAAAAAAAAAAAC;
            hd = 0xAAAAAAAAAAAAAAAD;
            he = 0xAAAAAAAAAAAAAAAE;
            hf = 0xAAAAAAAAAAAAAAAF;

            ProcessBlock();
        }

        protected override byte[] InternalStateToDigest()
        {
            FinalProcess();

            byte[] digest = new byte[64];
            GetBytes(h8, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h9, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(ha, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(hb, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(hc, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(hd, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(he, ByteOrder.LittleEndian, digest, 0x30);
            GetBytes(hf, ByteOrder.LittleEndian, digest, 0x38);
            return digest;
        }
    }
}