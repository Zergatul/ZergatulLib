using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHA1 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 64;
        public override int DigestLength => 20;

        uint h0;
        uint h1;
        uint h2;
        uint h3;
        uint h4;
        long length;

        public SHA1()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            h0 = 0x67452301;
            h1 = 0xEFCDAB89;
            h2 = 0x98BADCFE;
            h3 = 0x10325476;
            h4 = 0xC3D2E1F0;
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
            uint w10 = RotateLeft(w0d ^ w08 ^ w02 ^ w00, 1);
            uint w11 = RotateLeft(w0e ^ w09 ^ w03 ^ w01, 1);
            uint w12 = RotateLeft(w0f ^ w0a ^ w04 ^ w02, 1);
            uint w13 = RotateLeft(w10 ^ w0b ^ w05 ^ w03, 1);
            uint w14 = RotateLeft(w11 ^ w0c ^ w06 ^ w04, 1);
            uint w15 = RotateLeft(w12 ^ w0d ^ w07 ^ w05, 1);
            uint w16 = RotateLeft(w13 ^ w0e ^ w08 ^ w06, 1);
            uint w17 = RotateLeft(w14 ^ w0f ^ w09 ^ w07, 1);
            uint w18 = RotateLeft(w15 ^ w10 ^ w0a ^ w08, 1);
            uint w19 = RotateLeft(w16 ^ w11 ^ w0b ^ w09, 1);
            uint w1a = RotateLeft(w17 ^ w12 ^ w0c ^ w0a, 1);
            uint w1b = RotateLeft(w18 ^ w13 ^ w0d ^ w0b, 1);
            uint w1c = RotateLeft(w19 ^ w14 ^ w0e ^ w0c, 1);
            uint w1d = RotateLeft(w1a ^ w15 ^ w0f ^ w0d, 1);
            uint w1e = RotateLeft(w1b ^ w16 ^ w10 ^ w0e, 1);
            uint w1f = RotateLeft(w1c ^ w17 ^ w11 ^ w0f, 1);
            uint w20 = RotateLeft(w1d ^ w18 ^ w12 ^ w10, 1);
            uint w21 = RotateLeft(w1e ^ w19 ^ w13 ^ w11, 1);
            uint w22 = RotateLeft(w1f ^ w1a ^ w14 ^ w12, 1);
            uint w23 = RotateLeft(w20 ^ w1b ^ w15 ^ w13, 1);
            uint w24 = RotateLeft(w21 ^ w1c ^ w16 ^ w14, 1);
            uint w25 = RotateLeft(w22 ^ w1d ^ w17 ^ w15, 1);
            uint w26 = RotateLeft(w23 ^ w1e ^ w18 ^ w16, 1);
            uint w27 = RotateLeft(w24 ^ w1f ^ w19 ^ w17, 1);
            uint w28 = RotateLeft(w25 ^ w20 ^ w1a ^ w18, 1);
            uint w29 = RotateLeft(w26 ^ w21 ^ w1b ^ w19, 1);
            uint w2a = RotateLeft(w27 ^ w22 ^ w1c ^ w1a, 1);
            uint w2b = RotateLeft(w28 ^ w23 ^ w1d ^ w1b, 1);
            uint w2c = RotateLeft(w29 ^ w24 ^ w1e ^ w1c, 1);
            uint w2d = RotateLeft(w2a ^ w25 ^ w1f ^ w1d, 1);
            uint w2e = RotateLeft(w2b ^ w26 ^ w20 ^ w1e, 1);
            uint w2f = RotateLeft(w2c ^ w27 ^ w21 ^ w1f, 1);
            uint w30 = RotateLeft(w2d ^ w28 ^ w22 ^ w20, 1);
            uint w31 = RotateLeft(w2e ^ w29 ^ w23 ^ w21, 1);
            uint w32 = RotateLeft(w2f ^ w2a ^ w24 ^ w22, 1);
            uint w33 = RotateLeft(w30 ^ w2b ^ w25 ^ w23, 1);
            uint w34 = RotateLeft(w31 ^ w2c ^ w26 ^ w24, 1);
            uint w35 = RotateLeft(w32 ^ w2d ^ w27 ^ w25, 1);
            uint w36 = RotateLeft(w33 ^ w2e ^ w28 ^ w26, 1);
            uint w37 = RotateLeft(w34 ^ w2f ^ w29 ^ w27, 1);
            uint w38 = RotateLeft(w35 ^ w30 ^ w2a ^ w28, 1);
            uint w39 = RotateLeft(w36 ^ w31 ^ w2b ^ w29, 1);
            uint w3a = RotateLeft(w37 ^ w32 ^ w2c ^ w2a, 1);
            uint w3b = RotateLeft(w38 ^ w33 ^ w2d ^ w2b, 1);
            uint w3c = RotateLeft(w39 ^ w34 ^ w2e ^ w2c, 1);
            uint w3d = RotateLeft(w3a ^ w35 ^ w2f ^ w2d, 1);
            uint w3e = RotateLeft(w3b ^ w36 ^ w30 ^ w2e, 1);
            uint w3f = RotateLeft(w3c ^ w37 ^ w31 ^ w2f, 1);
            uint w40 = RotateLeft(w3d ^ w38 ^ w32 ^ w30, 1);
            uint w41 = RotateLeft(w3e ^ w39 ^ w33 ^ w31, 1);
            uint w42 = RotateLeft(w3f ^ w3a ^ w34 ^ w32, 1);
            uint w43 = RotateLeft(w40 ^ w3b ^ w35 ^ w33, 1);
            uint w44 = RotateLeft(w41 ^ w3c ^ w36 ^ w34, 1);
            uint w45 = RotateLeft(w42 ^ w3d ^ w37 ^ w35, 1);
            uint w46 = RotateLeft(w43 ^ w3e ^ w38 ^ w36, 1);
            uint w47 = RotateLeft(w44 ^ w3f ^ w39 ^ w37, 1);
            uint w48 = RotateLeft(w45 ^ w40 ^ w3a ^ w38, 1);
            uint w49 = RotateLeft(w46 ^ w41 ^ w3b ^ w39, 1);
            uint w4a = RotateLeft(w47 ^ w42 ^ w3c ^ w3a, 1);
            uint w4b = RotateLeft(w48 ^ w43 ^ w3d ^ w3b, 1);
            uint w4c = RotateLeft(w49 ^ w44 ^ w3e ^ w3c, 1);
            uint w4d = RotateLeft(w4a ^ w45 ^ w3f ^ w3d, 1);
            uint w4e = RotateLeft(w4b ^ w46 ^ w40 ^ w3e, 1);
            uint w4f = RotateLeft(w4c ^ w47 ^ w41 ^ w3f, 1);

            #endregion

            uint a = h0;
            uint b = h1;
            uint c = h2;
            uint d = h3;
            uint e = h4;

            #region Loop

            // Iteration 0
            e = RotateLeft(a, 5) + ((b & c) | (~b & d)) + e + 0x5A827999 + w00;
            b = RotateLeft(b, 30);
            // Iteration 1
            d = RotateLeft(e, 5) + ((a & b) | (~a & c)) + d + 0x5A827999 + w01;
            a = RotateLeft(a, 30);
            // Iteration 2
            c = RotateLeft(d, 5) + ((e & a) | (~e & b)) + c + 0x5A827999 + w02;
            e = RotateLeft(e, 30);
            // Iteration 3
            b = RotateLeft(c, 5) + ((d & e) | (~d & a)) + b + 0x5A827999 + w03;
            d = RotateLeft(d, 30);
            // Iteration 4
            a = RotateLeft(b, 5) + ((c & d) | (~c & e)) + a + 0x5A827999 + w04;
            c = RotateLeft(c, 30);
            // Iteration 5
            e = RotateLeft(a, 5) + ((b & c) | (~b & d)) + e + 0x5A827999 + w05;
            b = RotateLeft(b, 30);
            // Iteration 6
            d = RotateLeft(e, 5) + ((a & b) | (~a & c)) + d + 0x5A827999 + w06;
            a = RotateLeft(a, 30);
            // Iteration 7
            c = RotateLeft(d, 5) + ((e & a) | (~e & b)) + c + 0x5A827999 + w07;
            e = RotateLeft(e, 30);
            // Iteration 8
            b = RotateLeft(c, 5) + ((d & e) | (~d & a)) + b + 0x5A827999 + w08;
            d = RotateLeft(d, 30);
            // Iteration 9
            a = RotateLeft(b, 5) + ((c & d) | (~c & e)) + a + 0x5A827999 + w09;
            c = RotateLeft(c, 30);
            // Iteration 10
            e = RotateLeft(a, 5) + ((b & c) | (~b & d)) + e + 0x5A827999 + w0a;
            b = RotateLeft(b, 30);
            // Iteration 11
            d = RotateLeft(e, 5) + ((a & b) | (~a & c)) + d + 0x5A827999 + w0b;
            a = RotateLeft(a, 30);
            // Iteration 12
            c = RotateLeft(d, 5) + ((e & a) | (~e & b)) + c + 0x5A827999 + w0c;
            e = RotateLeft(e, 30);
            // Iteration 13
            b = RotateLeft(c, 5) + ((d & e) | (~d & a)) + b + 0x5A827999 + w0d;
            d = RotateLeft(d, 30);
            // Iteration 14
            a = RotateLeft(b, 5) + ((c & d) | (~c & e)) + a + 0x5A827999 + w0e;
            c = RotateLeft(c, 30);
            // Iteration 15
            e = RotateLeft(a, 5) + ((b & c) | (~b & d)) + e + 0x5A827999 + w0f;
            b = RotateLeft(b, 30);
            // Iteration 16
            d = RotateLeft(e, 5) + ((a & b) | (~a & c)) + d + 0x5A827999 + w10;
            a = RotateLeft(a, 30);
            // Iteration 17
            c = RotateLeft(d, 5) + ((e & a) | (~e & b)) + c + 0x5A827999 + w11;
            e = RotateLeft(e, 30);
            // Iteration 18
            b = RotateLeft(c, 5) + ((d & e) | (~d & a)) + b + 0x5A827999 + w12;
            d = RotateLeft(d, 30);
            // Iteration 19
            a = RotateLeft(b, 5) + ((c & d) | (~c & e)) + a + 0x5A827999 + w13;
            c = RotateLeft(c, 30);
            // Iteration 20
            e = RotateLeft(a, 5) + (b ^ c ^ d) + e + 0x6ED9EBA1 + w14;
            b = RotateLeft(b, 30);
            // Iteration 21
            d = RotateLeft(e, 5) + (a ^ b ^ c) + d + 0x6ED9EBA1 + w15;
            a = RotateLeft(a, 30);
            // Iteration 22
            c = RotateLeft(d, 5) + (e ^ a ^ b) + c + 0x6ED9EBA1 + w16;
            e = RotateLeft(e, 30);
            // Iteration 23
            b = RotateLeft(c, 5) + (d ^ e ^ a) + b + 0x6ED9EBA1 + w17;
            d = RotateLeft(d, 30);
            // Iteration 24
            a = RotateLeft(b, 5) + (c ^ d ^ e) + a + 0x6ED9EBA1 + w18;
            c = RotateLeft(c, 30);
            // Iteration 25
            e = RotateLeft(a, 5) + (b ^ c ^ d) + e + 0x6ED9EBA1 + w19;
            b = RotateLeft(b, 30);
            // Iteration 26
            d = RotateLeft(e, 5) + (a ^ b ^ c) + d + 0x6ED9EBA1 + w1a;
            a = RotateLeft(a, 30);
            // Iteration 27
            c = RotateLeft(d, 5) + (e ^ a ^ b) + c + 0x6ED9EBA1 + w1b;
            e = RotateLeft(e, 30);
            // Iteration 28
            b = RotateLeft(c, 5) + (d ^ e ^ a) + b + 0x6ED9EBA1 + w1c;
            d = RotateLeft(d, 30);
            // Iteration 29
            a = RotateLeft(b, 5) + (c ^ d ^ e) + a + 0x6ED9EBA1 + w1d;
            c = RotateLeft(c, 30);
            // Iteration 30
            e = RotateLeft(a, 5) + (b ^ c ^ d) + e + 0x6ED9EBA1 + w1e;
            b = RotateLeft(b, 30);
            // Iteration 31
            d = RotateLeft(e, 5) + (a ^ b ^ c) + d + 0x6ED9EBA1 + w1f;
            a = RotateLeft(a, 30);
            // Iteration 32
            c = RotateLeft(d, 5) + (e ^ a ^ b) + c + 0x6ED9EBA1 + w20;
            e = RotateLeft(e, 30);
            // Iteration 33
            b = RotateLeft(c, 5) + (d ^ e ^ a) + b + 0x6ED9EBA1 + w21;
            d = RotateLeft(d, 30);
            // Iteration 34
            a = RotateLeft(b, 5) + (c ^ d ^ e) + a + 0x6ED9EBA1 + w22;
            c = RotateLeft(c, 30);
            // Iteration 35
            e = RotateLeft(a, 5) + (b ^ c ^ d) + e + 0x6ED9EBA1 + w23;
            b = RotateLeft(b, 30);
            // Iteration 36
            d = RotateLeft(e, 5) + (a ^ b ^ c) + d + 0x6ED9EBA1 + w24;
            a = RotateLeft(a, 30);
            // Iteration 37
            c = RotateLeft(d, 5) + (e ^ a ^ b) + c + 0x6ED9EBA1 + w25;
            e = RotateLeft(e, 30);
            // Iteration 38
            b = RotateLeft(c, 5) + (d ^ e ^ a) + b + 0x6ED9EBA1 + w26;
            d = RotateLeft(d, 30);
            // Iteration 39
            a = RotateLeft(b, 5) + (c ^ d ^ e) + a + 0x6ED9EBA1 + w27;
            c = RotateLeft(c, 30);
            // Iteration 40
            e = RotateLeft(a, 5) + ((b & c) | (b & d) | (c & d)) + e + 0x8F1BBCDC + w28;
            b = RotateLeft(b, 30);
            // Iteration 41
            d = RotateLeft(e, 5) + ((a & b) | (a & c) | (b & c)) + d + 0x8F1BBCDC + w29;
            a = RotateLeft(a, 30);
            // Iteration 42
            c = RotateLeft(d, 5) + ((e & a) | (e & b) | (a & b)) + c + 0x8F1BBCDC + w2a;
            e = RotateLeft(e, 30);
            // Iteration 43
            b = RotateLeft(c, 5) + ((d & e) | (d & a) | (e & a)) + b + 0x8F1BBCDC + w2b;
            d = RotateLeft(d, 30);
            // Iteration 44
            a = RotateLeft(b, 5) + ((c & d) | (c & e) | (d & e)) + a + 0x8F1BBCDC + w2c;
            c = RotateLeft(c, 30);
            // Iteration 45
            e = RotateLeft(a, 5) + ((b & c) | (b & d) | (c & d)) + e + 0x8F1BBCDC + w2d;
            b = RotateLeft(b, 30);
            // Iteration 46
            d = RotateLeft(e, 5) + ((a & b) | (a & c) | (b & c)) + d + 0x8F1BBCDC + w2e;
            a = RotateLeft(a, 30);
            // Iteration 47
            c = RotateLeft(d, 5) + ((e & a) | (e & b) | (a & b)) + c + 0x8F1BBCDC + w2f;
            e = RotateLeft(e, 30);
            // Iteration 48
            b = RotateLeft(c, 5) + ((d & e) | (d & a) | (e & a)) + b + 0x8F1BBCDC + w30;
            d = RotateLeft(d, 30);
            // Iteration 49
            a = RotateLeft(b, 5) + ((c & d) | (c & e) | (d & e)) + a + 0x8F1BBCDC + w31;
            c = RotateLeft(c, 30);
            // Iteration 50
            e = RotateLeft(a, 5) + ((b & c) | (b & d) | (c & d)) + e + 0x8F1BBCDC + w32;
            b = RotateLeft(b, 30);
            // Iteration 51
            d = RotateLeft(e, 5) + ((a & b) | (a & c) | (b & c)) + d + 0x8F1BBCDC + w33;
            a = RotateLeft(a, 30);
            // Iteration 52
            c = RotateLeft(d, 5) + ((e & a) | (e & b) | (a & b)) + c + 0x8F1BBCDC + w34;
            e = RotateLeft(e, 30);
            // Iteration 53
            b = RotateLeft(c, 5) + ((d & e) | (d & a) | (e & a)) + b + 0x8F1BBCDC + w35;
            d = RotateLeft(d, 30);
            // Iteration 54
            a = RotateLeft(b, 5) + ((c & d) | (c & e) | (d & e)) + a + 0x8F1BBCDC + w36;
            c = RotateLeft(c, 30);
            // Iteration 55
            e = RotateLeft(a, 5) + ((b & c) | (b & d) | (c & d)) + e + 0x8F1BBCDC + w37;
            b = RotateLeft(b, 30);
            // Iteration 56
            d = RotateLeft(e, 5) + ((a & b) | (a & c) | (b & c)) + d + 0x8F1BBCDC + w38;
            a = RotateLeft(a, 30);
            // Iteration 57
            c = RotateLeft(d, 5) + ((e & a) | (e & b) | (a & b)) + c + 0x8F1BBCDC + w39;
            e = RotateLeft(e, 30);
            // Iteration 58
            b = RotateLeft(c, 5) + ((d & e) | (d & a) | (e & a)) + b + 0x8F1BBCDC + w3a;
            d = RotateLeft(d, 30);
            // Iteration 59
            a = RotateLeft(b, 5) + ((c & d) | (c & e) | (d & e)) + a + 0x8F1BBCDC + w3b;
            c = RotateLeft(c, 30);
            // Iteration 60
            e = RotateLeft(a, 5) + (b ^ c ^ d) + e + 0xCA62C1D6 + w3c;
            b = RotateLeft(b, 30);
            // Iteration 61
            d = RotateLeft(e, 5) + (a ^ b ^ c) + d + 0xCA62C1D6 + w3d;
            a = RotateLeft(a, 30);
            // Iteration 62
            c = RotateLeft(d, 5) + (e ^ a ^ b) + c + 0xCA62C1D6 + w3e;
            e = RotateLeft(e, 30);
            // Iteration 63
            b = RotateLeft(c, 5) + (d ^ e ^ a) + b + 0xCA62C1D6 + w3f;
            d = RotateLeft(d, 30);
            // Iteration 64
            a = RotateLeft(b, 5) + (c ^ d ^ e) + a + 0xCA62C1D6 + w40;
            c = RotateLeft(c, 30);
            // Iteration 65
            e = RotateLeft(a, 5) + (b ^ c ^ d) + e + 0xCA62C1D6 + w41;
            b = RotateLeft(b, 30);
            // Iteration 66
            d = RotateLeft(e, 5) + (a ^ b ^ c) + d + 0xCA62C1D6 + w42;
            a = RotateLeft(a, 30);
            // Iteration 67
            c = RotateLeft(d, 5) + (e ^ a ^ b) + c + 0xCA62C1D6 + w43;
            e = RotateLeft(e, 30);
            // Iteration 68
            b = RotateLeft(c, 5) + (d ^ e ^ a) + b + 0xCA62C1D6 + w44;
            d = RotateLeft(d, 30);
            // Iteration 69
            a = RotateLeft(b, 5) + (c ^ d ^ e) + a + 0xCA62C1D6 + w45;
            c = RotateLeft(c, 30);
            // Iteration 70
            e = RotateLeft(a, 5) + (b ^ c ^ d) + e + 0xCA62C1D6 + w46;
            b = RotateLeft(b, 30);
            // Iteration 71
            d = RotateLeft(e, 5) + (a ^ b ^ c) + d + 0xCA62C1D6 + w47;
            a = RotateLeft(a, 30);
            // Iteration 72
            c = RotateLeft(d, 5) + (e ^ a ^ b) + c + 0xCA62C1D6 + w48;
            e = RotateLeft(e, 30);
            // Iteration 73
            b = RotateLeft(c, 5) + (d ^ e ^ a) + b + 0xCA62C1D6 + w49;
            d = RotateLeft(d, 30);
            // Iteration 74
            a = RotateLeft(b, 5) + (c ^ d ^ e) + a + 0xCA62C1D6 + w4a;
            c = RotateLeft(c, 30);
            // Iteration 75
            e = RotateLeft(a, 5) + (b ^ c ^ d) + e + 0xCA62C1D6 + w4b;
            b = RotateLeft(b, 30);
            // Iteration 76
            d = RotateLeft(e, 5) + (a ^ b ^ c) + d + 0xCA62C1D6 + w4c;
            a = RotateLeft(a, 30);
            // Iteration 77
            c = RotateLeft(d, 5) + (e ^ a ^ b) + c + 0xCA62C1D6 + w4d;
            e = RotateLeft(e, 30);
            // Iteration 78
            b = RotateLeft(c, 5) + (d ^ e ^ a) + b + 0xCA62C1D6 + w4e;
            d = RotateLeft(d, 30);
            // Iteration 79
            a = RotateLeft(b, 5) + (c ^ d ^ e) + a + 0xCA62C1D6 + w4f;
            c = RotateLeft(c, 30);

            #endregion

            h0 += a;
            h1 += b;
            h2 += c;
            h3 += d;
            h4 += e;
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
            byte[] digest = new byte[20];
            GetBytes(h0, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.BigEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.BigEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.BigEndian, digest, 0x10);
            return digest;
        }
    }
}