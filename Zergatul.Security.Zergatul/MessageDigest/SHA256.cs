using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHA256 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 64;
        public override int DigestLength => 32;

        protected uint h0;
        protected uint h1;
        protected uint h2;
        protected uint h3;
        protected uint h4;
        protected uint h5;
        protected uint h6;
        protected uint h7;
        protected long length;

        public SHA256()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            h0 = 0x6A09E667;
            h1 = 0xBB67AE85;
            h2 = 0x3C6EF372;
            h3 = 0xA54FF53A;
            h4 = 0x510E527F;
            h5 = 0x9B05688C;
            h6 = 0x1F83D9AB;
            h7 = 0x5BE0CD19;
            bufOffset = 0;
            length = 0;
        }

        protected override void IncreaseLength(int value) => length += value;

        protected override void ProcessBlock()
        {
            #region w

            uint w00 = ToUInt32(buffer, 0x00, ByteOrder.BigEndian);
            uint w01 = ToUInt32(buffer, 0x04, ByteOrder.BigEndian);
            uint w02 = ToUInt32(buffer, 0x08, ByteOrder.BigEndian);
            uint w03 = ToUInt32(buffer, 0x0C, ByteOrder.BigEndian);
            uint w04 = ToUInt32(buffer, 0x10, ByteOrder.BigEndian);
            uint w05 = ToUInt32(buffer, 0x14, ByteOrder.BigEndian);
            uint w06 = ToUInt32(buffer, 0x18, ByteOrder.BigEndian);
            uint w07 = ToUInt32(buffer, 0x1C, ByteOrder.BigEndian);
            uint w08 = ToUInt32(buffer, 0x20, ByteOrder.BigEndian);
            uint w09 = ToUInt32(buffer, 0x24, ByteOrder.BigEndian);
            uint w0a = ToUInt32(buffer, 0x28, ByteOrder.BigEndian);
            uint w0b = ToUInt32(buffer, 0x2C, ByteOrder.BigEndian);
            uint w0c = ToUInt32(buffer, 0x30, ByteOrder.BigEndian);
            uint w0d = ToUInt32(buffer, 0x34, ByteOrder.BigEndian);
            uint w0e = ToUInt32(buffer, 0x38, ByteOrder.BigEndian);
            uint w0f = ToUInt32(buffer, 0x3C, ByteOrder.BigEndian);
            uint w10 = w00 + w09 + (RotateRight(w01, 7) ^ RotateRight(w01, 18) ^ (w01 >> 3)) + (RotateRight(w0e, 17) ^ RotateRight(w0e, 19) ^ (w0e >> 10));
            uint w11 = w01 + w0a + (RotateRight(w02, 7) ^ RotateRight(w02, 18) ^ (w02 >> 3)) + (RotateRight(w0f, 17) ^ RotateRight(w0f, 19) ^ (w0f >> 10));
            uint w12 = w02 + w0b + (RotateRight(w03, 7) ^ RotateRight(w03, 18) ^ (w03 >> 3)) + (RotateRight(w10, 17) ^ RotateRight(w10, 19) ^ (w10 >> 10));
            uint w13 = w03 + w0c + (RotateRight(w04, 7) ^ RotateRight(w04, 18) ^ (w04 >> 3)) + (RotateRight(w11, 17) ^ RotateRight(w11, 19) ^ (w11 >> 10));
            uint w14 = w04 + w0d + (RotateRight(w05, 7) ^ RotateRight(w05, 18) ^ (w05 >> 3)) + (RotateRight(w12, 17) ^ RotateRight(w12, 19) ^ (w12 >> 10));
            uint w15 = w05 + w0e + (RotateRight(w06, 7) ^ RotateRight(w06, 18) ^ (w06 >> 3)) + (RotateRight(w13, 17) ^ RotateRight(w13, 19) ^ (w13 >> 10));
            uint w16 = w06 + w0f + (RotateRight(w07, 7) ^ RotateRight(w07, 18) ^ (w07 >> 3)) + (RotateRight(w14, 17) ^ RotateRight(w14, 19) ^ (w14 >> 10));
            uint w17 = w07 + w10 + (RotateRight(w08, 7) ^ RotateRight(w08, 18) ^ (w08 >> 3)) + (RotateRight(w15, 17) ^ RotateRight(w15, 19) ^ (w15 >> 10));
            uint w18 = w08 + w11 + (RotateRight(w09, 7) ^ RotateRight(w09, 18) ^ (w09 >> 3)) + (RotateRight(w16, 17) ^ RotateRight(w16, 19) ^ (w16 >> 10));
            uint w19 = w09 + w12 + (RotateRight(w0a, 7) ^ RotateRight(w0a, 18) ^ (w0a >> 3)) + (RotateRight(w17, 17) ^ RotateRight(w17, 19) ^ (w17 >> 10));
            uint w1a = w0a + w13 + (RotateRight(w0b, 7) ^ RotateRight(w0b, 18) ^ (w0b >> 3)) + (RotateRight(w18, 17) ^ RotateRight(w18, 19) ^ (w18 >> 10));
            uint w1b = w0b + w14 + (RotateRight(w0c, 7) ^ RotateRight(w0c, 18) ^ (w0c >> 3)) + (RotateRight(w19, 17) ^ RotateRight(w19, 19) ^ (w19 >> 10));
            uint w1c = w0c + w15 + (RotateRight(w0d, 7) ^ RotateRight(w0d, 18) ^ (w0d >> 3)) + (RotateRight(w1a, 17) ^ RotateRight(w1a, 19) ^ (w1a >> 10));
            uint w1d = w0d + w16 + (RotateRight(w0e, 7) ^ RotateRight(w0e, 18) ^ (w0e >> 3)) + (RotateRight(w1b, 17) ^ RotateRight(w1b, 19) ^ (w1b >> 10));
            uint w1e = w0e + w17 + (RotateRight(w0f, 7) ^ RotateRight(w0f, 18) ^ (w0f >> 3)) + (RotateRight(w1c, 17) ^ RotateRight(w1c, 19) ^ (w1c >> 10));
            uint w1f = w0f + w18 + (RotateRight(w10, 7) ^ RotateRight(w10, 18) ^ (w10 >> 3)) + (RotateRight(w1d, 17) ^ RotateRight(w1d, 19) ^ (w1d >> 10));
            uint w20 = w10 + w19 + (RotateRight(w11, 7) ^ RotateRight(w11, 18) ^ (w11 >> 3)) + (RotateRight(w1e, 17) ^ RotateRight(w1e, 19) ^ (w1e >> 10));
            uint w21 = w11 + w1a + (RotateRight(w12, 7) ^ RotateRight(w12, 18) ^ (w12 >> 3)) + (RotateRight(w1f, 17) ^ RotateRight(w1f, 19) ^ (w1f >> 10));
            uint w22 = w12 + w1b + (RotateRight(w13, 7) ^ RotateRight(w13, 18) ^ (w13 >> 3)) + (RotateRight(w20, 17) ^ RotateRight(w20, 19) ^ (w20 >> 10));
            uint w23 = w13 + w1c + (RotateRight(w14, 7) ^ RotateRight(w14, 18) ^ (w14 >> 3)) + (RotateRight(w21, 17) ^ RotateRight(w21, 19) ^ (w21 >> 10));
            uint w24 = w14 + w1d + (RotateRight(w15, 7) ^ RotateRight(w15, 18) ^ (w15 >> 3)) + (RotateRight(w22, 17) ^ RotateRight(w22, 19) ^ (w22 >> 10));
            uint w25 = w15 + w1e + (RotateRight(w16, 7) ^ RotateRight(w16, 18) ^ (w16 >> 3)) + (RotateRight(w23, 17) ^ RotateRight(w23, 19) ^ (w23 >> 10));
            uint w26 = w16 + w1f + (RotateRight(w17, 7) ^ RotateRight(w17, 18) ^ (w17 >> 3)) + (RotateRight(w24, 17) ^ RotateRight(w24, 19) ^ (w24 >> 10));
            uint w27 = w17 + w20 + (RotateRight(w18, 7) ^ RotateRight(w18, 18) ^ (w18 >> 3)) + (RotateRight(w25, 17) ^ RotateRight(w25, 19) ^ (w25 >> 10));
            uint w28 = w18 + w21 + (RotateRight(w19, 7) ^ RotateRight(w19, 18) ^ (w19 >> 3)) + (RotateRight(w26, 17) ^ RotateRight(w26, 19) ^ (w26 >> 10));
            uint w29 = w19 + w22 + (RotateRight(w1a, 7) ^ RotateRight(w1a, 18) ^ (w1a >> 3)) + (RotateRight(w27, 17) ^ RotateRight(w27, 19) ^ (w27 >> 10));
            uint w2a = w1a + w23 + (RotateRight(w1b, 7) ^ RotateRight(w1b, 18) ^ (w1b >> 3)) + (RotateRight(w28, 17) ^ RotateRight(w28, 19) ^ (w28 >> 10));
            uint w2b = w1b + w24 + (RotateRight(w1c, 7) ^ RotateRight(w1c, 18) ^ (w1c >> 3)) + (RotateRight(w29, 17) ^ RotateRight(w29, 19) ^ (w29 >> 10));
            uint w2c = w1c + w25 + (RotateRight(w1d, 7) ^ RotateRight(w1d, 18) ^ (w1d >> 3)) + (RotateRight(w2a, 17) ^ RotateRight(w2a, 19) ^ (w2a >> 10));
            uint w2d = w1d + w26 + (RotateRight(w1e, 7) ^ RotateRight(w1e, 18) ^ (w1e >> 3)) + (RotateRight(w2b, 17) ^ RotateRight(w2b, 19) ^ (w2b >> 10));
            uint w2e = w1e + w27 + (RotateRight(w1f, 7) ^ RotateRight(w1f, 18) ^ (w1f >> 3)) + (RotateRight(w2c, 17) ^ RotateRight(w2c, 19) ^ (w2c >> 10));
            uint w2f = w1f + w28 + (RotateRight(w20, 7) ^ RotateRight(w20, 18) ^ (w20 >> 3)) + (RotateRight(w2d, 17) ^ RotateRight(w2d, 19) ^ (w2d >> 10));
            uint w30 = w20 + w29 + (RotateRight(w21, 7) ^ RotateRight(w21, 18) ^ (w21 >> 3)) + (RotateRight(w2e, 17) ^ RotateRight(w2e, 19) ^ (w2e >> 10));
            uint w31 = w21 + w2a + (RotateRight(w22, 7) ^ RotateRight(w22, 18) ^ (w22 >> 3)) + (RotateRight(w2f, 17) ^ RotateRight(w2f, 19) ^ (w2f >> 10));
            uint w32 = w22 + w2b + (RotateRight(w23, 7) ^ RotateRight(w23, 18) ^ (w23 >> 3)) + (RotateRight(w30, 17) ^ RotateRight(w30, 19) ^ (w30 >> 10));
            uint w33 = w23 + w2c + (RotateRight(w24, 7) ^ RotateRight(w24, 18) ^ (w24 >> 3)) + (RotateRight(w31, 17) ^ RotateRight(w31, 19) ^ (w31 >> 10));
            uint w34 = w24 + w2d + (RotateRight(w25, 7) ^ RotateRight(w25, 18) ^ (w25 >> 3)) + (RotateRight(w32, 17) ^ RotateRight(w32, 19) ^ (w32 >> 10));
            uint w35 = w25 + w2e + (RotateRight(w26, 7) ^ RotateRight(w26, 18) ^ (w26 >> 3)) + (RotateRight(w33, 17) ^ RotateRight(w33, 19) ^ (w33 >> 10));
            uint w36 = w26 + w2f + (RotateRight(w27, 7) ^ RotateRight(w27, 18) ^ (w27 >> 3)) + (RotateRight(w34, 17) ^ RotateRight(w34, 19) ^ (w34 >> 10));
            uint w37 = w27 + w30 + (RotateRight(w28, 7) ^ RotateRight(w28, 18) ^ (w28 >> 3)) + (RotateRight(w35, 17) ^ RotateRight(w35, 19) ^ (w35 >> 10));
            uint w38 = w28 + w31 + (RotateRight(w29, 7) ^ RotateRight(w29, 18) ^ (w29 >> 3)) + (RotateRight(w36, 17) ^ RotateRight(w36, 19) ^ (w36 >> 10));
            uint w39 = w29 + w32 + (RotateRight(w2a, 7) ^ RotateRight(w2a, 18) ^ (w2a >> 3)) + (RotateRight(w37, 17) ^ RotateRight(w37, 19) ^ (w37 >> 10));
            uint w3a = w2a + w33 + (RotateRight(w2b, 7) ^ RotateRight(w2b, 18) ^ (w2b >> 3)) + (RotateRight(w38, 17) ^ RotateRight(w38, 19) ^ (w38 >> 10));
            uint w3b = w2b + w34 + (RotateRight(w2c, 7) ^ RotateRight(w2c, 18) ^ (w2c >> 3)) + (RotateRight(w39, 17) ^ RotateRight(w39, 19) ^ (w39 >> 10));
            uint w3c = w2c + w35 + (RotateRight(w2d, 7) ^ RotateRight(w2d, 18) ^ (w2d >> 3)) + (RotateRight(w3a, 17) ^ RotateRight(w3a, 19) ^ (w3a >> 10));
            uint w3d = w2d + w36 + (RotateRight(w2e, 7) ^ RotateRight(w2e, 18) ^ (w2e >> 3)) + (RotateRight(w3b, 17) ^ RotateRight(w3b, 19) ^ (w3b >> 10));
            uint w3e = w2e + w37 + (RotateRight(w2f, 7) ^ RotateRight(w2f, 18) ^ (w2f >> 3)) + (RotateRight(w3c, 17) ^ RotateRight(w3c, 19) ^ (w3c >> 10));
            uint w3f = w2f + w38 + (RotateRight(w30, 7) ^ RotateRight(w30, 18) ^ (w30 >> 3)) + (RotateRight(w3d, 17) ^ RotateRight(w3d, 19) ^ (w3d >> 10));

            #endregion

            uint a = h0;
            uint b = h1;
            uint c = h2;
            uint d = h3;
            uint e = h4;
            uint f = h5;
            uint g = h6;
            uint h = h7;
            uint t;

            #region Loop

            // Iteration 0
            t = h + (RotateRight(e, 6) ^ RotateRight(e, 11) ^ RotateRight(e, 25)) + ((e & f) ^ (~e & g)) + 0x428A2F98 + w00;
            d = d + t;
            h = t + (RotateRight(a, 2) ^ RotateRight(a, 13) ^ RotateRight(a, 22)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 1
            t = g + (RotateRight(d, 6) ^ RotateRight(d, 11) ^ RotateRight(d, 25)) + ((d & e) ^ (~d & f)) + 0x71374491 + w01;
            c = c + t;
            g = t + (RotateRight(h, 2) ^ RotateRight(h, 13) ^ RotateRight(h, 22)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 2
            t = f + (RotateRight(c, 6) ^ RotateRight(c, 11) ^ RotateRight(c, 25)) + ((c & d) ^ (~c & e)) + 0xB5C0FBCF + w02;
            b = b + t;
            f = t + (RotateRight(g, 2) ^ RotateRight(g, 13) ^ RotateRight(g, 22)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 3
            t = e + (RotateRight(b, 6) ^ RotateRight(b, 11) ^ RotateRight(b, 25)) + ((b & c) ^ (~b & d)) + 0xE9B5DBA5 + w03;
            a = a + t;
            e = t + (RotateRight(f, 2) ^ RotateRight(f, 13) ^ RotateRight(f, 22)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 4
            t = d + (RotateRight(a, 6) ^ RotateRight(a, 11) ^ RotateRight(a, 25)) + ((a & b) ^ (~a & c)) + 0x3956C25B + w04;
            h = h + t;
            d = t + (RotateRight(e, 2) ^ RotateRight(e, 13) ^ RotateRight(e, 22)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 5
            t = c + (RotateRight(h, 6) ^ RotateRight(h, 11) ^ RotateRight(h, 25)) + ((h & a) ^ (~h & b)) + 0x59F111F1 + w05;
            g = g + t;
            c = t + (RotateRight(d, 2) ^ RotateRight(d, 13) ^ RotateRight(d, 22)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 6
            t = b + (RotateRight(g, 6) ^ RotateRight(g, 11) ^ RotateRight(g, 25)) + ((g & h) ^ (~g & a)) + 0x923F82A4 + w06;
            f = f + t;
            b = t + (RotateRight(c, 2) ^ RotateRight(c, 13) ^ RotateRight(c, 22)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 7
            t = a + (RotateRight(f, 6) ^ RotateRight(f, 11) ^ RotateRight(f, 25)) + ((f & g) ^ (~f & h)) + 0xAB1C5ED5 + w07;
            e = e + t;
            a = t + (RotateRight(b, 2) ^ RotateRight(b, 13) ^ RotateRight(b, 22)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 8
            t = h + (RotateRight(e, 6) ^ RotateRight(e, 11) ^ RotateRight(e, 25)) + ((e & f) ^ (~e & g)) + 0xD807AA98 + w08;
            d = d + t;
            h = t + (RotateRight(a, 2) ^ RotateRight(a, 13) ^ RotateRight(a, 22)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 9
            t = g + (RotateRight(d, 6) ^ RotateRight(d, 11) ^ RotateRight(d, 25)) + ((d & e) ^ (~d & f)) + 0x12835B01 + w09;
            c = c + t;
            g = t + (RotateRight(h, 2) ^ RotateRight(h, 13) ^ RotateRight(h, 22)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 10
            t = f + (RotateRight(c, 6) ^ RotateRight(c, 11) ^ RotateRight(c, 25)) + ((c & d) ^ (~c & e)) + 0x243185BE + w0a;
            b = b + t;
            f = t + (RotateRight(g, 2) ^ RotateRight(g, 13) ^ RotateRight(g, 22)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 11
            t = e + (RotateRight(b, 6) ^ RotateRight(b, 11) ^ RotateRight(b, 25)) + ((b & c) ^ (~b & d)) + 0x550C7DC3 + w0b;
            a = a + t;
            e = t + (RotateRight(f, 2) ^ RotateRight(f, 13) ^ RotateRight(f, 22)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 12
            t = d + (RotateRight(a, 6) ^ RotateRight(a, 11) ^ RotateRight(a, 25)) + ((a & b) ^ (~a & c)) + 0x72BE5D74 + w0c;
            h = h + t;
            d = t + (RotateRight(e, 2) ^ RotateRight(e, 13) ^ RotateRight(e, 22)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 13
            t = c + (RotateRight(h, 6) ^ RotateRight(h, 11) ^ RotateRight(h, 25)) + ((h & a) ^ (~h & b)) + 0x80DEB1FE + w0d;
            g = g + t;
            c = t + (RotateRight(d, 2) ^ RotateRight(d, 13) ^ RotateRight(d, 22)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 14
            t = b + (RotateRight(g, 6) ^ RotateRight(g, 11) ^ RotateRight(g, 25)) + ((g & h) ^ (~g & a)) + 0x9BDC06A7 + w0e;
            f = f + t;
            b = t + (RotateRight(c, 2) ^ RotateRight(c, 13) ^ RotateRight(c, 22)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 15
            t = a + (RotateRight(f, 6) ^ RotateRight(f, 11) ^ RotateRight(f, 25)) + ((f & g) ^ (~f & h)) + 0xC19BF174 + w0f;
            e = e + t;
            a = t + (RotateRight(b, 2) ^ RotateRight(b, 13) ^ RotateRight(b, 22)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 16
            t = h + (RotateRight(e, 6) ^ RotateRight(e, 11) ^ RotateRight(e, 25)) + ((e & f) ^ (~e & g)) + 0xE49B69C1 + w10;
            d = d + t;
            h = t + (RotateRight(a, 2) ^ RotateRight(a, 13) ^ RotateRight(a, 22)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 17
            t = g + (RotateRight(d, 6) ^ RotateRight(d, 11) ^ RotateRight(d, 25)) + ((d & e) ^ (~d & f)) + 0xEFBE4786 + w11;
            c = c + t;
            g = t + (RotateRight(h, 2) ^ RotateRight(h, 13) ^ RotateRight(h, 22)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 18
            t = f + (RotateRight(c, 6) ^ RotateRight(c, 11) ^ RotateRight(c, 25)) + ((c & d) ^ (~c & e)) + 0x0FC19DC6 + w12;
            b = b + t;
            f = t + (RotateRight(g, 2) ^ RotateRight(g, 13) ^ RotateRight(g, 22)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 19
            t = e + (RotateRight(b, 6) ^ RotateRight(b, 11) ^ RotateRight(b, 25)) + ((b & c) ^ (~b & d)) + 0x240CA1CC + w13;
            a = a + t;
            e = t + (RotateRight(f, 2) ^ RotateRight(f, 13) ^ RotateRight(f, 22)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 20
            t = d + (RotateRight(a, 6) ^ RotateRight(a, 11) ^ RotateRight(a, 25)) + ((a & b) ^ (~a & c)) + 0x2DE92C6F + w14;
            h = h + t;
            d = t + (RotateRight(e, 2) ^ RotateRight(e, 13) ^ RotateRight(e, 22)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 21
            t = c + (RotateRight(h, 6) ^ RotateRight(h, 11) ^ RotateRight(h, 25)) + ((h & a) ^ (~h & b)) + 0x4A7484AA + w15;
            g = g + t;
            c = t + (RotateRight(d, 2) ^ RotateRight(d, 13) ^ RotateRight(d, 22)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 22
            t = b + (RotateRight(g, 6) ^ RotateRight(g, 11) ^ RotateRight(g, 25)) + ((g & h) ^ (~g & a)) + 0x5CB0A9DC + w16;
            f = f + t;
            b = t + (RotateRight(c, 2) ^ RotateRight(c, 13) ^ RotateRight(c, 22)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 23
            t = a + (RotateRight(f, 6) ^ RotateRight(f, 11) ^ RotateRight(f, 25)) + ((f & g) ^ (~f & h)) + 0x76F988DA + w17;
            e = e + t;
            a = t + (RotateRight(b, 2) ^ RotateRight(b, 13) ^ RotateRight(b, 22)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 24
            t = h + (RotateRight(e, 6) ^ RotateRight(e, 11) ^ RotateRight(e, 25)) + ((e & f) ^ (~e & g)) + 0x983E5152 + w18;
            d = d + t;
            h = t + (RotateRight(a, 2) ^ RotateRight(a, 13) ^ RotateRight(a, 22)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 25
            t = g + (RotateRight(d, 6) ^ RotateRight(d, 11) ^ RotateRight(d, 25)) + ((d & e) ^ (~d & f)) + 0xA831C66D + w19;
            c = c + t;
            g = t + (RotateRight(h, 2) ^ RotateRight(h, 13) ^ RotateRight(h, 22)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 26
            t = f + (RotateRight(c, 6) ^ RotateRight(c, 11) ^ RotateRight(c, 25)) + ((c & d) ^ (~c & e)) + 0xB00327C8 + w1a;
            b = b + t;
            f = t + (RotateRight(g, 2) ^ RotateRight(g, 13) ^ RotateRight(g, 22)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 27
            t = e + (RotateRight(b, 6) ^ RotateRight(b, 11) ^ RotateRight(b, 25)) + ((b & c) ^ (~b & d)) + 0xBF597FC7 + w1b;
            a = a + t;
            e = t + (RotateRight(f, 2) ^ RotateRight(f, 13) ^ RotateRight(f, 22)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 28
            t = d + (RotateRight(a, 6) ^ RotateRight(a, 11) ^ RotateRight(a, 25)) + ((a & b) ^ (~a & c)) + 0xC6E00BF3 + w1c;
            h = h + t;
            d = t + (RotateRight(e, 2) ^ RotateRight(e, 13) ^ RotateRight(e, 22)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 29
            t = c + (RotateRight(h, 6) ^ RotateRight(h, 11) ^ RotateRight(h, 25)) + ((h & a) ^ (~h & b)) + 0xD5A79147 + w1d;
            g = g + t;
            c = t + (RotateRight(d, 2) ^ RotateRight(d, 13) ^ RotateRight(d, 22)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 30
            t = b + (RotateRight(g, 6) ^ RotateRight(g, 11) ^ RotateRight(g, 25)) + ((g & h) ^ (~g & a)) + 0x06CA6351 + w1e;
            f = f + t;
            b = t + (RotateRight(c, 2) ^ RotateRight(c, 13) ^ RotateRight(c, 22)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 31
            t = a + (RotateRight(f, 6) ^ RotateRight(f, 11) ^ RotateRight(f, 25)) + ((f & g) ^ (~f & h)) + 0x14292967 + w1f;
            e = e + t;
            a = t + (RotateRight(b, 2) ^ RotateRight(b, 13) ^ RotateRight(b, 22)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 32
            t = h + (RotateRight(e, 6) ^ RotateRight(e, 11) ^ RotateRight(e, 25)) + ((e & f) ^ (~e & g)) + 0x27B70A85 + w20;
            d = d + t;
            h = t + (RotateRight(a, 2) ^ RotateRight(a, 13) ^ RotateRight(a, 22)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 33
            t = g + (RotateRight(d, 6) ^ RotateRight(d, 11) ^ RotateRight(d, 25)) + ((d & e) ^ (~d & f)) + 0x2E1B2138 + w21;
            c = c + t;
            g = t + (RotateRight(h, 2) ^ RotateRight(h, 13) ^ RotateRight(h, 22)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 34
            t = f + (RotateRight(c, 6) ^ RotateRight(c, 11) ^ RotateRight(c, 25)) + ((c & d) ^ (~c & e)) + 0x4D2C6DFC + w22;
            b = b + t;
            f = t + (RotateRight(g, 2) ^ RotateRight(g, 13) ^ RotateRight(g, 22)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 35
            t = e + (RotateRight(b, 6) ^ RotateRight(b, 11) ^ RotateRight(b, 25)) + ((b & c) ^ (~b & d)) + 0x53380D13 + w23;
            a = a + t;
            e = t + (RotateRight(f, 2) ^ RotateRight(f, 13) ^ RotateRight(f, 22)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 36
            t = d + (RotateRight(a, 6) ^ RotateRight(a, 11) ^ RotateRight(a, 25)) + ((a & b) ^ (~a & c)) + 0x650A7354 + w24;
            h = h + t;
            d = t + (RotateRight(e, 2) ^ RotateRight(e, 13) ^ RotateRight(e, 22)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 37
            t = c + (RotateRight(h, 6) ^ RotateRight(h, 11) ^ RotateRight(h, 25)) + ((h & a) ^ (~h & b)) + 0x766A0ABB + w25;
            g = g + t;
            c = t + (RotateRight(d, 2) ^ RotateRight(d, 13) ^ RotateRight(d, 22)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 38
            t = b + (RotateRight(g, 6) ^ RotateRight(g, 11) ^ RotateRight(g, 25)) + ((g & h) ^ (~g & a)) + 0x81C2C92E + w26;
            f = f + t;
            b = t + (RotateRight(c, 2) ^ RotateRight(c, 13) ^ RotateRight(c, 22)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 39
            t = a + (RotateRight(f, 6) ^ RotateRight(f, 11) ^ RotateRight(f, 25)) + ((f & g) ^ (~f & h)) + 0x92722C85 + w27;
            e = e + t;
            a = t + (RotateRight(b, 2) ^ RotateRight(b, 13) ^ RotateRight(b, 22)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 40
            t = h + (RotateRight(e, 6) ^ RotateRight(e, 11) ^ RotateRight(e, 25)) + ((e & f) ^ (~e & g)) + 0xA2BFE8A1 + w28;
            d = d + t;
            h = t + (RotateRight(a, 2) ^ RotateRight(a, 13) ^ RotateRight(a, 22)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 41
            t = g + (RotateRight(d, 6) ^ RotateRight(d, 11) ^ RotateRight(d, 25)) + ((d & e) ^ (~d & f)) + 0xA81A664B + w29;
            c = c + t;
            g = t + (RotateRight(h, 2) ^ RotateRight(h, 13) ^ RotateRight(h, 22)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 42
            t = f + (RotateRight(c, 6) ^ RotateRight(c, 11) ^ RotateRight(c, 25)) + ((c & d) ^ (~c & e)) + 0xC24B8B70 + w2a;
            b = b + t;
            f = t + (RotateRight(g, 2) ^ RotateRight(g, 13) ^ RotateRight(g, 22)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 43
            t = e + (RotateRight(b, 6) ^ RotateRight(b, 11) ^ RotateRight(b, 25)) + ((b & c) ^ (~b & d)) + 0xC76C51A3 + w2b;
            a = a + t;
            e = t + (RotateRight(f, 2) ^ RotateRight(f, 13) ^ RotateRight(f, 22)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 44
            t = d + (RotateRight(a, 6) ^ RotateRight(a, 11) ^ RotateRight(a, 25)) + ((a & b) ^ (~a & c)) + 0xD192E819 + w2c;
            h = h + t;
            d = t + (RotateRight(e, 2) ^ RotateRight(e, 13) ^ RotateRight(e, 22)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 45
            t = c + (RotateRight(h, 6) ^ RotateRight(h, 11) ^ RotateRight(h, 25)) + ((h & a) ^ (~h & b)) + 0xD6990624 + w2d;
            g = g + t;
            c = t + (RotateRight(d, 2) ^ RotateRight(d, 13) ^ RotateRight(d, 22)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 46
            t = b + (RotateRight(g, 6) ^ RotateRight(g, 11) ^ RotateRight(g, 25)) + ((g & h) ^ (~g & a)) + 0xF40E3585 + w2e;
            f = f + t;
            b = t + (RotateRight(c, 2) ^ RotateRight(c, 13) ^ RotateRight(c, 22)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 47
            t = a + (RotateRight(f, 6) ^ RotateRight(f, 11) ^ RotateRight(f, 25)) + ((f & g) ^ (~f & h)) + 0x106AA070 + w2f;
            e = e + t;
            a = t + (RotateRight(b, 2) ^ RotateRight(b, 13) ^ RotateRight(b, 22)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 48
            t = h + (RotateRight(e, 6) ^ RotateRight(e, 11) ^ RotateRight(e, 25)) + ((e & f) ^ (~e & g)) + 0x19A4C116 + w30;
            d = d + t;
            h = t + (RotateRight(a, 2) ^ RotateRight(a, 13) ^ RotateRight(a, 22)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 49
            t = g + (RotateRight(d, 6) ^ RotateRight(d, 11) ^ RotateRight(d, 25)) + ((d & e) ^ (~d & f)) + 0x1E376C08 + w31;
            c = c + t;
            g = t + (RotateRight(h, 2) ^ RotateRight(h, 13) ^ RotateRight(h, 22)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 50
            t = f + (RotateRight(c, 6) ^ RotateRight(c, 11) ^ RotateRight(c, 25)) + ((c & d) ^ (~c & e)) + 0x2748774C + w32;
            b = b + t;
            f = t + (RotateRight(g, 2) ^ RotateRight(g, 13) ^ RotateRight(g, 22)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 51
            t = e + (RotateRight(b, 6) ^ RotateRight(b, 11) ^ RotateRight(b, 25)) + ((b & c) ^ (~b & d)) + 0x34B0BCB5 + w33;
            a = a + t;
            e = t + (RotateRight(f, 2) ^ RotateRight(f, 13) ^ RotateRight(f, 22)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 52
            t = d + (RotateRight(a, 6) ^ RotateRight(a, 11) ^ RotateRight(a, 25)) + ((a & b) ^ (~a & c)) + 0x391C0CB3 + w34;
            h = h + t;
            d = t + (RotateRight(e, 2) ^ RotateRight(e, 13) ^ RotateRight(e, 22)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 53
            t = c + (RotateRight(h, 6) ^ RotateRight(h, 11) ^ RotateRight(h, 25)) + ((h & a) ^ (~h & b)) + 0x4ED8AA4A + w35;
            g = g + t;
            c = t + (RotateRight(d, 2) ^ RotateRight(d, 13) ^ RotateRight(d, 22)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 54
            t = b + (RotateRight(g, 6) ^ RotateRight(g, 11) ^ RotateRight(g, 25)) + ((g & h) ^ (~g & a)) + 0x5B9CCA4F + w36;
            f = f + t;
            b = t + (RotateRight(c, 2) ^ RotateRight(c, 13) ^ RotateRight(c, 22)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 55
            t = a + (RotateRight(f, 6) ^ RotateRight(f, 11) ^ RotateRight(f, 25)) + ((f & g) ^ (~f & h)) + 0x682E6FF3 + w37;
            e = e + t;
            a = t + (RotateRight(b, 2) ^ RotateRight(b, 13) ^ RotateRight(b, 22)) + ((b & c) ^ (b & d) ^ (c & d));
            // Iteration 56
            t = h + (RotateRight(e, 6) ^ RotateRight(e, 11) ^ RotateRight(e, 25)) + ((e & f) ^ (~e & g)) + 0x748F82EE + w38;
            d = d + t;
            h = t + (RotateRight(a, 2) ^ RotateRight(a, 13) ^ RotateRight(a, 22)) + ((a & b) ^ (a & c) ^ (b & c));
            // Iteration 57
            t = g + (RotateRight(d, 6) ^ RotateRight(d, 11) ^ RotateRight(d, 25)) + ((d & e) ^ (~d & f)) + 0x78A5636F + w39;
            c = c + t;
            g = t + (RotateRight(h, 2) ^ RotateRight(h, 13) ^ RotateRight(h, 22)) + ((h & a) ^ (h & b) ^ (a & b));
            // Iteration 58
            t = f + (RotateRight(c, 6) ^ RotateRight(c, 11) ^ RotateRight(c, 25)) + ((c & d) ^ (~c & e)) + 0x84C87814 + w3a;
            b = b + t;
            f = t + (RotateRight(g, 2) ^ RotateRight(g, 13) ^ RotateRight(g, 22)) + ((g & h) ^ (g & a) ^ (h & a));
            // Iteration 59
            t = e + (RotateRight(b, 6) ^ RotateRight(b, 11) ^ RotateRight(b, 25)) + ((b & c) ^ (~b & d)) + 0x8CC70208 + w3b;
            a = a + t;
            e = t + (RotateRight(f, 2) ^ RotateRight(f, 13) ^ RotateRight(f, 22)) + ((f & g) ^ (f & h) ^ (g & h));
            // Iteration 60
            t = d + (RotateRight(a, 6) ^ RotateRight(a, 11) ^ RotateRight(a, 25)) + ((a & b) ^ (~a & c)) + 0x90BEFFFA + w3c;
            h = h + t;
            d = t + (RotateRight(e, 2) ^ RotateRight(e, 13) ^ RotateRight(e, 22)) + ((e & f) ^ (e & g) ^ (f & g));
            // Iteration 61
            t = c + (RotateRight(h, 6) ^ RotateRight(h, 11) ^ RotateRight(h, 25)) + ((h & a) ^ (~h & b)) + 0xA4506CEB + w3d;
            g = g + t;
            c = t + (RotateRight(d, 2) ^ RotateRight(d, 13) ^ RotateRight(d, 22)) + ((d & e) ^ (d & f) ^ (e & f));
            // Iteration 62
            t = b + (RotateRight(g, 6) ^ RotateRight(g, 11) ^ RotateRight(g, 25)) + ((g & h) ^ (~g & a)) + 0xBEF9A3F7 + w3e;
            f = f + t;
            b = t + (RotateRight(c, 2) ^ RotateRight(c, 13) ^ RotateRight(c, 22)) + ((c & d) ^ (c & e) ^ (d & e));
            // Iteration 63
            t = a + (RotateRight(f, 6) ^ RotateRight(f, 11) ^ RotateRight(f, 25)) + ((f & g) ^ (~f & h)) + 0xC67178F2 + w3f;
            e = e + t;
            a = t + (RotateRight(b, 2) ^ RotateRight(b, 13) ^ RotateRight(b, 22)) + ((b & c) ^ (b & d) ^ (c & d));

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

            if (64 - bufOffset < 8)
            {
                while (bufOffset < 64)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
            }

            while (bufOffset < 56)
                buffer[bufOffset++] = 0;

            GetBytes(length << 3, ByteOrder.BigEndian, buffer, 56);
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[32];
            GetBytes(h0, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.BigEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.BigEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.BigEndian, digest, 0x10);
            GetBytes(h5, ByteOrder.BigEndian, digest, 0x14);
            GetBytes(h6, ByteOrder.BigEndian, digest, 0x18);
            GetBytes(h7, ByteOrder.BigEndian, digest, 0x1C);
            return digest;
        }
    }
}