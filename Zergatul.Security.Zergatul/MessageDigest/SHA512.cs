using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHA512 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 128;
        public override int DigestLength => 64;

        protected ulong h0;
        protected ulong h1;
        protected ulong h2;
        protected ulong h3;
        protected ulong h4;
        protected ulong h5;
        protected ulong h6;
        protected ulong h7;
        protected ulong lengthLo;
        protected ulong lengthHi;

        public SHA512()
        {
            buffer = new byte[128];
            Reset();
        }

        public override void Reset()
        {
            h0 = 0x6A09E667F3BCC908;
            h1 = 0xBB67AE8584CAA73B;
            h2 = 0x3C6EF372FE94F82B;
            h3 = 0xA54FF53A5F1D36F1;
            h4 = 0x510E527FADE682D1;
            h5 = 0x9B05688C2B3E6C1F;
            h6 = 0x1F83D9ABFB41BD6B;
            h7 = 0x5BE0CD19137E2179;
            bufOffset = 0;
            lengthHi = 0;
            lengthLo = 0;
        }

        protected override void IncreaseLength(int value)
        {
            uint uvalue = (uint)value;
            lengthLo = lengthLo + uvalue;
            if (lengthLo < uvalue)
                lengthHi++;
        }

        protected override void ProcessBlock()
        {
            #region w

            ulong w00 = ToUInt64(buffer, 0x00, ByteOrder.BigEndian);
            ulong w01 = ToUInt64(buffer, 0x08, ByteOrder.BigEndian);
            ulong w02 = ToUInt64(buffer, 0x10, ByteOrder.BigEndian);
            ulong w03 = ToUInt64(buffer, 0x18, ByteOrder.BigEndian);
            ulong w04 = ToUInt64(buffer, 0x20, ByteOrder.BigEndian);
            ulong w05 = ToUInt64(buffer, 0x28, ByteOrder.BigEndian);
            ulong w06 = ToUInt64(buffer, 0x30, ByteOrder.BigEndian);
            ulong w07 = ToUInt64(buffer, 0x38, ByteOrder.BigEndian);
            ulong w08 = ToUInt64(buffer, 0x40, ByteOrder.BigEndian);
            ulong w09 = ToUInt64(buffer, 0x48, ByteOrder.BigEndian);
            ulong w0a = ToUInt64(buffer, 0x50, ByteOrder.BigEndian);
            ulong w0b = ToUInt64(buffer, 0x58, ByteOrder.BigEndian);
            ulong w0c = ToUInt64(buffer, 0x60, ByteOrder.BigEndian);
            ulong w0d = ToUInt64(buffer, 0x68, ByteOrder.BigEndian);
            ulong w0e = ToUInt64(buffer, 0x70, ByteOrder.BigEndian);
            ulong w0f = ToUInt64(buffer, 0x78, ByteOrder.BigEndian);
            ulong w10 = w00 + w09 + (RotateRight(w01, 1) ^ RotateRight(w01, 8) ^ (w01 >> 7)) + (RotateRight(w0e, 19) ^ RotateRight(w0e, 61) ^ (w0e >> 6));
            ulong w11 = w01 + w0a + (RotateRight(w02, 1) ^ RotateRight(w02, 8) ^ (w02 >> 7)) + (RotateRight(w0f, 19) ^ RotateRight(w0f, 61) ^ (w0f >> 6));
            ulong w12 = w02 + w0b + (RotateRight(w03, 1) ^ RotateRight(w03, 8) ^ (w03 >> 7)) + (RotateRight(w10, 19) ^ RotateRight(w10, 61) ^ (w10 >> 6));
            ulong w13 = w03 + w0c + (RotateRight(w04, 1) ^ RotateRight(w04, 8) ^ (w04 >> 7)) + (RotateRight(w11, 19) ^ RotateRight(w11, 61) ^ (w11 >> 6));
            ulong w14 = w04 + w0d + (RotateRight(w05, 1) ^ RotateRight(w05, 8) ^ (w05 >> 7)) + (RotateRight(w12, 19) ^ RotateRight(w12, 61) ^ (w12 >> 6));
            ulong w15 = w05 + w0e + (RotateRight(w06, 1) ^ RotateRight(w06, 8) ^ (w06 >> 7)) + (RotateRight(w13, 19) ^ RotateRight(w13, 61) ^ (w13 >> 6));
            ulong w16 = w06 + w0f + (RotateRight(w07, 1) ^ RotateRight(w07, 8) ^ (w07 >> 7)) + (RotateRight(w14, 19) ^ RotateRight(w14, 61) ^ (w14 >> 6));
            ulong w17 = w07 + w10 + (RotateRight(w08, 1) ^ RotateRight(w08, 8) ^ (w08 >> 7)) + (RotateRight(w15, 19) ^ RotateRight(w15, 61) ^ (w15 >> 6));
            ulong w18 = w08 + w11 + (RotateRight(w09, 1) ^ RotateRight(w09, 8) ^ (w09 >> 7)) + (RotateRight(w16, 19) ^ RotateRight(w16, 61) ^ (w16 >> 6));
            ulong w19 = w09 + w12 + (RotateRight(w0a, 1) ^ RotateRight(w0a, 8) ^ (w0a >> 7)) + (RotateRight(w17, 19) ^ RotateRight(w17, 61) ^ (w17 >> 6));
            ulong w1a = w0a + w13 + (RotateRight(w0b, 1) ^ RotateRight(w0b, 8) ^ (w0b >> 7)) + (RotateRight(w18, 19) ^ RotateRight(w18, 61) ^ (w18 >> 6));
            ulong w1b = w0b + w14 + (RotateRight(w0c, 1) ^ RotateRight(w0c, 8) ^ (w0c >> 7)) + (RotateRight(w19, 19) ^ RotateRight(w19, 61) ^ (w19 >> 6));
            ulong w1c = w0c + w15 + (RotateRight(w0d, 1) ^ RotateRight(w0d, 8) ^ (w0d >> 7)) + (RotateRight(w1a, 19) ^ RotateRight(w1a, 61) ^ (w1a >> 6));
            ulong w1d = w0d + w16 + (RotateRight(w0e, 1) ^ RotateRight(w0e, 8) ^ (w0e >> 7)) + (RotateRight(w1b, 19) ^ RotateRight(w1b, 61) ^ (w1b >> 6));
            ulong w1e = w0e + w17 + (RotateRight(w0f, 1) ^ RotateRight(w0f, 8) ^ (w0f >> 7)) + (RotateRight(w1c, 19) ^ RotateRight(w1c, 61) ^ (w1c >> 6));
            ulong w1f = w0f + w18 + (RotateRight(w10, 1) ^ RotateRight(w10, 8) ^ (w10 >> 7)) + (RotateRight(w1d, 19) ^ RotateRight(w1d, 61) ^ (w1d >> 6));
            ulong w20 = w10 + w19 + (RotateRight(w11, 1) ^ RotateRight(w11, 8) ^ (w11 >> 7)) + (RotateRight(w1e, 19) ^ RotateRight(w1e, 61) ^ (w1e >> 6));
            ulong w21 = w11 + w1a + (RotateRight(w12, 1) ^ RotateRight(w12, 8) ^ (w12 >> 7)) + (RotateRight(w1f, 19) ^ RotateRight(w1f, 61) ^ (w1f >> 6));
            ulong w22 = w12 + w1b + (RotateRight(w13, 1) ^ RotateRight(w13, 8) ^ (w13 >> 7)) + (RotateRight(w20, 19) ^ RotateRight(w20, 61) ^ (w20 >> 6));
            ulong w23 = w13 + w1c + (RotateRight(w14, 1) ^ RotateRight(w14, 8) ^ (w14 >> 7)) + (RotateRight(w21, 19) ^ RotateRight(w21, 61) ^ (w21 >> 6));
            ulong w24 = w14 + w1d + (RotateRight(w15, 1) ^ RotateRight(w15, 8) ^ (w15 >> 7)) + (RotateRight(w22, 19) ^ RotateRight(w22, 61) ^ (w22 >> 6));
            ulong w25 = w15 + w1e + (RotateRight(w16, 1) ^ RotateRight(w16, 8) ^ (w16 >> 7)) + (RotateRight(w23, 19) ^ RotateRight(w23, 61) ^ (w23 >> 6));
            ulong w26 = w16 + w1f + (RotateRight(w17, 1) ^ RotateRight(w17, 8) ^ (w17 >> 7)) + (RotateRight(w24, 19) ^ RotateRight(w24, 61) ^ (w24 >> 6));
            ulong w27 = w17 + w20 + (RotateRight(w18, 1) ^ RotateRight(w18, 8) ^ (w18 >> 7)) + (RotateRight(w25, 19) ^ RotateRight(w25, 61) ^ (w25 >> 6));
            ulong w28 = w18 + w21 + (RotateRight(w19, 1) ^ RotateRight(w19, 8) ^ (w19 >> 7)) + (RotateRight(w26, 19) ^ RotateRight(w26, 61) ^ (w26 >> 6));
            ulong w29 = w19 + w22 + (RotateRight(w1a, 1) ^ RotateRight(w1a, 8) ^ (w1a >> 7)) + (RotateRight(w27, 19) ^ RotateRight(w27, 61) ^ (w27 >> 6));
            ulong w2a = w1a + w23 + (RotateRight(w1b, 1) ^ RotateRight(w1b, 8) ^ (w1b >> 7)) + (RotateRight(w28, 19) ^ RotateRight(w28, 61) ^ (w28 >> 6));
            ulong w2b = w1b + w24 + (RotateRight(w1c, 1) ^ RotateRight(w1c, 8) ^ (w1c >> 7)) + (RotateRight(w29, 19) ^ RotateRight(w29, 61) ^ (w29 >> 6));
            ulong w2c = w1c + w25 + (RotateRight(w1d, 1) ^ RotateRight(w1d, 8) ^ (w1d >> 7)) + (RotateRight(w2a, 19) ^ RotateRight(w2a, 61) ^ (w2a >> 6));
            ulong w2d = w1d + w26 + (RotateRight(w1e, 1) ^ RotateRight(w1e, 8) ^ (w1e >> 7)) + (RotateRight(w2b, 19) ^ RotateRight(w2b, 61) ^ (w2b >> 6));
            ulong w2e = w1e + w27 + (RotateRight(w1f, 1) ^ RotateRight(w1f, 8) ^ (w1f >> 7)) + (RotateRight(w2c, 19) ^ RotateRight(w2c, 61) ^ (w2c >> 6));
            ulong w2f = w1f + w28 + (RotateRight(w20, 1) ^ RotateRight(w20, 8) ^ (w20 >> 7)) + (RotateRight(w2d, 19) ^ RotateRight(w2d, 61) ^ (w2d >> 6));
            ulong w30 = w20 + w29 + (RotateRight(w21, 1) ^ RotateRight(w21, 8) ^ (w21 >> 7)) + (RotateRight(w2e, 19) ^ RotateRight(w2e, 61) ^ (w2e >> 6));
            ulong w31 = w21 + w2a + (RotateRight(w22, 1) ^ RotateRight(w22, 8) ^ (w22 >> 7)) + (RotateRight(w2f, 19) ^ RotateRight(w2f, 61) ^ (w2f >> 6));
            ulong w32 = w22 + w2b + (RotateRight(w23, 1) ^ RotateRight(w23, 8) ^ (w23 >> 7)) + (RotateRight(w30, 19) ^ RotateRight(w30, 61) ^ (w30 >> 6));
            ulong w33 = w23 + w2c + (RotateRight(w24, 1) ^ RotateRight(w24, 8) ^ (w24 >> 7)) + (RotateRight(w31, 19) ^ RotateRight(w31, 61) ^ (w31 >> 6));
            ulong w34 = w24 + w2d + (RotateRight(w25, 1) ^ RotateRight(w25, 8) ^ (w25 >> 7)) + (RotateRight(w32, 19) ^ RotateRight(w32, 61) ^ (w32 >> 6));
            ulong w35 = w25 + w2e + (RotateRight(w26, 1) ^ RotateRight(w26, 8) ^ (w26 >> 7)) + (RotateRight(w33, 19) ^ RotateRight(w33, 61) ^ (w33 >> 6));
            ulong w36 = w26 + w2f + (RotateRight(w27, 1) ^ RotateRight(w27, 8) ^ (w27 >> 7)) + (RotateRight(w34, 19) ^ RotateRight(w34, 61) ^ (w34 >> 6));
            ulong w37 = w27 + w30 + (RotateRight(w28, 1) ^ RotateRight(w28, 8) ^ (w28 >> 7)) + (RotateRight(w35, 19) ^ RotateRight(w35, 61) ^ (w35 >> 6));
            ulong w38 = w28 + w31 + (RotateRight(w29, 1) ^ RotateRight(w29, 8) ^ (w29 >> 7)) + (RotateRight(w36, 19) ^ RotateRight(w36, 61) ^ (w36 >> 6));
            ulong w39 = w29 + w32 + (RotateRight(w2a, 1) ^ RotateRight(w2a, 8) ^ (w2a >> 7)) + (RotateRight(w37, 19) ^ RotateRight(w37, 61) ^ (w37 >> 6));
            ulong w3a = w2a + w33 + (RotateRight(w2b, 1) ^ RotateRight(w2b, 8) ^ (w2b >> 7)) + (RotateRight(w38, 19) ^ RotateRight(w38, 61) ^ (w38 >> 6));
            ulong w3b = w2b + w34 + (RotateRight(w2c, 1) ^ RotateRight(w2c, 8) ^ (w2c >> 7)) + (RotateRight(w39, 19) ^ RotateRight(w39, 61) ^ (w39 >> 6));
            ulong w3c = w2c + w35 + (RotateRight(w2d, 1) ^ RotateRight(w2d, 8) ^ (w2d >> 7)) + (RotateRight(w3a, 19) ^ RotateRight(w3a, 61) ^ (w3a >> 6));
            ulong w3d = w2d + w36 + (RotateRight(w2e, 1) ^ RotateRight(w2e, 8) ^ (w2e >> 7)) + (RotateRight(w3b, 19) ^ RotateRight(w3b, 61) ^ (w3b >> 6));
            ulong w3e = w2e + w37 + (RotateRight(w2f, 1) ^ RotateRight(w2f, 8) ^ (w2f >> 7)) + (RotateRight(w3c, 19) ^ RotateRight(w3c, 61) ^ (w3c >> 6));
            ulong w3f = w2f + w38 + (RotateRight(w30, 1) ^ RotateRight(w30, 8) ^ (w30 >> 7)) + (RotateRight(w3d, 19) ^ RotateRight(w3d, 61) ^ (w3d >> 6));
            ulong w40 = w30 + w39 + (RotateRight(w31, 1) ^ RotateRight(w31, 8) ^ (w31 >> 7)) + (RotateRight(w3e, 19) ^ RotateRight(w3e, 61) ^ (w3e >> 6));
            ulong w41 = w31 + w3a + (RotateRight(w32, 1) ^ RotateRight(w32, 8) ^ (w32 >> 7)) + (RotateRight(w3f, 19) ^ RotateRight(w3f, 61) ^ (w3f >> 6));
            ulong w42 = w32 + w3b + (RotateRight(w33, 1) ^ RotateRight(w33, 8) ^ (w33 >> 7)) + (RotateRight(w40, 19) ^ RotateRight(w40, 61) ^ (w40 >> 6));
            ulong w43 = w33 + w3c + (RotateRight(w34, 1) ^ RotateRight(w34, 8) ^ (w34 >> 7)) + (RotateRight(w41, 19) ^ RotateRight(w41, 61) ^ (w41 >> 6));
            ulong w44 = w34 + w3d + (RotateRight(w35, 1) ^ RotateRight(w35, 8) ^ (w35 >> 7)) + (RotateRight(w42, 19) ^ RotateRight(w42, 61) ^ (w42 >> 6));
            ulong w45 = w35 + w3e + (RotateRight(w36, 1) ^ RotateRight(w36, 8) ^ (w36 >> 7)) + (RotateRight(w43, 19) ^ RotateRight(w43, 61) ^ (w43 >> 6));
            ulong w46 = w36 + w3f + (RotateRight(w37, 1) ^ RotateRight(w37, 8) ^ (w37 >> 7)) + (RotateRight(w44, 19) ^ RotateRight(w44, 61) ^ (w44 >> 6));
            ulong w47 = w37 + w40 + (RotateRight(w38, 1) ^ RotateRight(w38, 8) ^ (w38 >> 7)) + (RotateRight(w45, 19) ^ RotateRight(w45, 61) ^ (w45 >> 6));
            ulong w48 = w38 + w41 + (RotateRight(w39, 1) ^ RotateRight(w39, 8) ^ (w39 >> 7)) + (RotateRight(w46, 19) ^ RotateRight(w46, 61) ^ (w46 >> 6));
            ulong w49 = w39 + w42 + (RotateRight(w3a, 1) ^ RotateRight(w3a, 8) ^ (w3a >> 7)) + (RotateRight(w47, 19) ^ RotateRight(w47, 61) ^ (w47 >> 6));
            ulong w4a = w3a + w43 + (RotateRight(w3b, 1) ^ RotateRight(w3b, 8) ^ (w3b >> 7)) + (RotateRight(w48, 19) ^ RotateRight(w48, 61) ^ (w48 >> 6));
            ulong w4b = w3b + w44 + (RotateRight(w3c, 1) ^ RotateRight(w3c, 8) ^ (w3c >> 7)) + (RotateRight(w49, 19) ^ RotateRight(w49, 61) ^ (w49 >> 6));
            ulong w4c = w3c + w45 + (RotateRight(w3d, 1) ^ RotateRight(w3d, 8) ^ (w3d >> 7)) + (RotateRight(w4a, 19) ^ RotateRight(w4a, 61) ^ (w4a >> 6));
            ulong w4d = w3d + w46 + (RotateRight(w3e, 1) ^ RotateRight(w3e, 8) ^ (w3e >> 7)) + (RotateRight(w4b, 19) ^ RotateRight(w4b, 61) ^ (w4b >> 6));
            ulong w4e = w3e + w47 + (RotateRight(w3f, 1) ^ RotateRight(w3f, 8) ^ (w3f >> 7)) + (RotateRight(w4c, 19) ^ RotateRight(w4c, 61) ^ (w4c >> 6));
            ulong w4f = w3f + w48 + (RotateRight(w40, 1) ^ RotateRight(w40, 8) ^ (w40 >> 7)) + (RotateRight(w4d, 19) ^ RotateRight(w4d, 61) ^ (w4d >> 6));

            #endregion

            ulong a = h0;
            ulong b = h1;
            ulong c = h2;
            ulong d = h3;
            ulong e = h4;
            ulong f = h5;
            ulong g = h6;
            ulong h = h7;
            ulong t;

            #region Loop

            // Iteration 0
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0x428A2F98D728AE22 + w00;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 1
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0x7137449123EF65CD + w01;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 2
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0xB5C0FBCFEC4D3B2F + w02;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 3
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0xE9B5DBA58189DBBC + w03;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 4
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0x3956C25BF348B538 + w04;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 5
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0x59F111F1B605D019 + w05;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 6
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0x923F82A4AF194F9B + w06;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 7
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0xAB1C5ED5DA6D8118 + w07;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 8
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0xD807AA98A3030242 + w08;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 9
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0x12835B0145706FBE + w09;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 10
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0x243185BE4EE4B28C + w0a;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 11
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0x550C7DC3D5FFB4E2 + w0b;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 12
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0x72BE5D74F27B896F + w0c;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 13
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0x80DEB1FE3B1696B1 + w0d;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 14
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0x9BDC06A725C71235 + w0e;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 15
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0xC19BF174CF692694 + w0f;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 16
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0xE49B69C19EF14AD2 + w10;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 17
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0xEFBE4786384F25E3 + w11;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 18
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0x0FC19DC68B8CD5B5 + w12;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 19
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0x240CA1CC77AC9C65 + w13;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 20
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0x2DE92C6F592B0275 + w14;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 21
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0x4A7484AA6EA6E483 + w15;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 22
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0x5CB0A9DCBD41FBD4 + w16;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 23
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0x76F988DA831153B5 + w17;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 24
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0x983E5152EE66DFAB + w18;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 25
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0xA831C66D2DB43210 + w19;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 26
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0xB00327C898FB213F + w1a;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 27
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0xBF597FC7BEEF0EE4 + w1b;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 28
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0xC6E00BF33DA88FC2 + w1c;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 29
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0xD5A79147930AA725 + w1d;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 30
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0x06CA6351E003826F + w1e;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 31
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0x142929670A0E6E70 + w1f;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 32
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0x27B70A8546D22FFC + w20;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 33
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0x2E1B21385C26C926 + w21;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 34
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0x4D2C6DFC5AC42AED + w22;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 35
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0x53380D139D95B3DF + w23;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 36
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0x650A73548BAF63DE + w24;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 37
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0x766A0ABB3C77B2A8 + w25;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 38
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0x81C2C92E47EDAEE6 + w26;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 39
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0x92722C851482353B + w27;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 40
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0xA2BFE8A14CF10364 + w28;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 41
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0xA81A664BBC423001 + w29;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 42
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0xC24B8B70D0F89791 + w2a;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 43
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0xC76C51A30654BE30 + w2b;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 44
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0xD192E819D6EF5218 + w2c;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 45
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0xD69906245565A910 + w2d;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 46
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0xF40E35855771202A + w2e;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 47
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0x106AA07032BBD1B8 + w2f;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 48
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0x19A4C116B8D2D0C8 + w30;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 49
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0x1E376C085141AB53 + w31;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 50
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0x2748774CDF8EEB99 + w32;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 51
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0x34B0BCB5E19B48A8 + w33;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 52
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0x391C0CB3C5C95A63 + w34;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 53
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0x4ED8AA4AE3418ACB + w35;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 54
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0x5B9CCA4F7763E373 + w36;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 55
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0x682E6FF3D6B2B8A3 + w37;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 56
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0x748F82EE5DEFB2FC + w38;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 57
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0x78A5636F43172F60 + w39;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 58
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0x84C87814A1F0AB72 + w3a;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 59
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0x8CC702081A6439EC + w3b;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 60
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0x90BEFFFA23631E28 + w3c;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 61
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0xA4506CEBDE82BDE9 + w3d;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 62
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0xBEF9A3F7B2C67915 + w3e;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 63
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0xC67178F2E372532B + w3f;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 64
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0xCA273ECEEA26619C + w40;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 65
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0xD186B8C721C0C207 + w41;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 66
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0xEADA7DD6CDE0EB1E + w42;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 67
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0xF57D4F7FEE6ED178 + w43;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 68
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0x06F067AA72176FBA + w44;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 69
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0x0A637DC5A2C898A6 + w45;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 70
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0x113F9804BEF90DAE + w46;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 71
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0x1B710B35131C471B + w47;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 72
            t = h + (RotateRight(e, 14) ^ RotateRight(e, 18) ^ RotateRight(e, 41)) + ((e & f) ^ (~e & g)) + 0x28DB77F523047D84 + w48;
            d = d + t;
            h = t + (RotateRight(a, 28) ^ RotateRight(a, 34) ^ RotateRight(a, 39)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 73
            t = g + (RotateRight(d, 14) ^ RotateRight(d, 18) ^ RotateRight(d, 41)) + ((d & e) ^ (~d & f)) + 0x32CAAB7B40C72493 + w49;
            c = c + t;
            g = t + (RotateRight(h, 28) ^ RotateRight(h, 34) ^ RotateRight(h, 39)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 74
            t = f + (RotateRight(c, 14) ^ RotateRight(c, 18) ^ RotateRight(c, 41)) + ((c & d) ^ (~c & e)) + 0x3C9EBE0A15C9BEBC + w4a;
            b = b + t;
            f = t + (RotateRight(g, 28) ^ RotateRight(g, 34) ^ RotateRight(g, 39)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 75
            t = e + (RotateRight(b, 14) ^ RotateRight(b, 18) ^ RotateRight(b, 41)) + ((b & c) ^ (~b & d)) + 0x431D67C49C100D4C + w4b;
            a = a + t;
            e = t + (RotateRight(f, 28) ^ RotateRight(f, 34) ^ RotateRight(f, 39)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 76
            t = d + (RotateRight(a, 14) ^ RotateRight(a, 18) ^ RotateRight(a, 41)) + ((a & b) ^ (~a & c)) + 0x4CC5D4BECB3E42B6 + w4c;
            h = h + t;
            d = t + (RotateRight(e, 28) ^ RotateRight(e, 34) ^ RotateRight(e, 39)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 77
            t = c + (RotateRight(h, 14) ^ RotateRight(h, 18) ^ RotateRight(h, 41)) + ((h & a) ^ (~h & b)) + 0x597F299CFC657E2A + w4d;
            g = g + t;
            c = t + (RotateRight(d, 28) ^ RotateRight(d, 34) ^ RotateRight(d, 39)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 78
            t = b + (RotateRight(g, 14) ^ RotateRight(g, 18) ^ RotateRight(g, 41)) + ((g & h) ^ (~g & a)) + 0x5FCB6FAB3AD6FAEC + w4e;
            f = f + t;
            b = t + (RotateRight(c, 28) ^ RotateRight(c, 34) ^ RotateRight(c, 39)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 79
            t = a + (RotateRight(f, 14) ^ RotateRight(f, 18) ^ RotateRight(f, 41)) + ((f & g) ^ (~f & h)) + 0x6C44198C4A475817 + w4f;
            e = e + t;
            a = t + (RotateRight(b, 28) ^ RotateRight(b, 34) ^ RotateRight(b, 39)) + ((b & c) ^ (b & d) ^ (c & d));

            #endregion

            h0 += a;
            h1 += b;
            h2 += c;
            h3 += d;
            h4 += e;
            h5 += f;
            h6 += g;
            h7 += h;
        }

        protected override void AddPadding()
        {
            buffer[bufOffset++] = 0x80;

            if (128 - bufOffset < 16)
            {
                while (bufOffset < 128)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
            }

            while (bufOffset < 112)
                buffer[bufOffset++] = 0;

            GetBytes((lengthHi << 3) | (lengthLo >> 61), ByteOrder.BigEndian, buffer, 112);
            GetBytes(lengthLo << 3, ByteOrder.BigEndian, buffer, 120);
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[64];
            GetBytes(h0, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h2, ByteOrder.BigEndian, digest, 0x10);
            GetBytes(h3, ByteOrder.BigEndian, digest, 0x18);
            GetBytes(h4, ByteOrder.BigEndian, digest, 0x20);
            GetBytes(h5, ByteOrder.BigEndian, digest, 0x28);
            GetBytes(h6, ByteOrder.BigEndian, digest, 0x30);
            GetBytes(h7, ByteOrder.BigEndian, digest, 0x38);
            return digest;
        }
    }
}