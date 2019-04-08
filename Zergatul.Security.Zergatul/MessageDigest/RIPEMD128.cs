using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class RIPEMD128 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 64;
        public override int DigestLength => 16;

        uint h0;
        uint h1;
        uint h2;
        uint h3;
        long length;

        public RIPEMD128()
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

            uint a = h0;
            uint b = h1;
            uint c = h2;
            uint d = h3;
            uint ap = h0;
            uint bp = h1;
            uint cp = h2;
            uint dp = h3;

            #region Loop

            // Iteration 0
            a = RotateLeft(a + (b ^ c ^ d) + m0, 11);
            ap = RotateLeft(ap + ((bp & dp) | (cp & ~dp)) + m5 + 0x50A28BE6, 8);
            // Iteration 1
            d = RotateLeft(d + (a ^ b ^ c) + m1, 14);
            dp = RotateLeft(dp + ((ap & cp) | (bp & ~cp)) + me + 0x50A28BE6, 9);
            // Iteration 2
            c = RotateLeft(c + (d ^ a ^ b) + m2, 15);
            cp = RotateLeft(cp + ((dp & bp) | (ap & ~bp)) + m7 + 0x50A28BE6, 9);
            // Iteration 3
            b = RotateLeft(b + (c ^ d ^ a) + m3, 12);
            bp = RotateLeft(bp + ((cp & ap) | (dp & ~ap)) + m0 + 0x50A28BE6, 11);
            // Iteration 4
            a = RotateLeft(a + (b ^ c ^ d) + m4, 5);
            ap = RotateLeft(ap + ((bp & dp) | (cp & ~dp)) + m9 + 0x50A28BE6, 13);
            // Iteration 5
            d = RotateLeft(d + (a ^ b ^ c) + m5, 8);
            dp = RotateLeft(dp + ((ap & cp) | (bp & ~cp)) + m2 + 0x50A28BE6, 15);
            // Iteration 6
            c = RotateLeft(c + (d ^ a ^ b) + m6, 7);
            cp = RotateLeft(cp + ((dp & bp) | (ap & ~bp)) + mb + 0x50A28BE6, 15);
            // Iteration 7
            b = RotateLeft(b + (c ^ d ^ a) + m7, 9);
            bp = RotateLeft(bp + ((cp & ap) | (dp & ~ap)) + m4 + 0x50A28BE6, 5);
            // Iteration 8
            a = RotateLeft(a + (b ^ c ^ d) + m8, 11);
            ap = RotateLeft(ap + ((bp & dp) | (cp & ~dp)) + md + 0x50A28BE6, 7);
            // Iteration 9
            d = RotateLeft(d + (a ^ b ^ c) + m9, 13);
            dp = RotateLeft(dp + ((ap & cp) | (bp & ~cp)) + m6 + 0x50A28BE6, 7);
            // Iteration 10
            c = RotateLeft(c + (d ^ a ^ b) + ma, 14);
            cp = RotateLeft(cp + ((dp & bp) | (ap & ~bp)) + mf + 0x50A28BE6, 8);
            // Iteration 11
            b = RotateLeft(b + (c ^ d ^ a) + mb, 15);
            bp = RotateLeft(bp + ((cp & ap) | (dp & ~ap)) + m8 + 0x50A28BE6, 11);
            // Iteration 12
            a = RotateLeft(a + (b ^ c ^ d) + mc, 6);
            ap = RotateLeft(ap + ((bp & dp) | (cp & ~dp)) + m1 + 0x50A28BE6, 14);
            // Iteration 13
            d = RotateLeft(d + (a ^ b ^ c) + md, 7);
            dp = RotateLeft(dp + ((ap & cp) | (bp & ~cp)) + ma + 0x50A28BE6, 14);
            // Iteration 14
            c = RotateLeft(c + (d ^ a ^ b) + me, 9);
            cp = RotateLeft(cp + ((dp & bp) | (ap & ~bp)) + m3 + 0x50A28BE6, 12);
            // Iteration 15
            b = RotateLeft(b + (c ^ d ^ a) + mf, 8);
            bp = RotateLeft(bp + ((cp & ap) | (dp & ~ap)) + mc + 0x50A28BE6, 6);
            // Iteration 16
            a = RotateLeft(a + ((b & c) | (~b & d)) + m7 + 0x5A827999, 7);
            ap = RotateLeft(ap + ((bp | ~cp) ^ dp) + m6 + 0x5C4DD124, 9);
            // Iteration 17
            d = RotateLeft(d + ((a & b) | (~a & c)) + m4 + 0x5A827999, 6);
            dp = RotateLeft(dp + ((ap | ~bp) ^ cp) + mb + 0x5C4DD124, 13);
            // Iteration 18
            c = RotateLeft(c + ((d & a) | (~d & b)) + md + 0x5A827999, 8);
            cp = RotateLeft(cp + ((dp | ~ap) ^ bp) + m3 + 0x5C4DD124, 15);
            // Iteration 19
            b = RotateLeft(b + ((c & d) | (~c & a)) + m1 + 0x5A827999, 13);
            bp = RotateLeft(bp + ((cp | ~dp) ^ ap) + m7 + 0x5C4DD124, 7);
            // Iteration 20
            a = RotateLeft(a + ((b & c) | (~b & d)) + ma + 0x5A827999, 11);
            ap = RotateLeft(ap + ((bp | ~cp) ^ dp) + m0 + 0x5C4DD124, 12);
            // Iteration 21
            d = RotateLeft(d + ((a & b) | (~a & c)) + m6 + 0x5A827999, 9);
            dp = RotateLeft(dp + ((ap | ~bp) ^ cp) + md + 0x5C4DD124, 8);
            // Iteration 22
            c = RotateLeft(c + ((d & a) | (~d & b)) + mf + 0x5A827999, 7);
            cp = RotateLeft(cp + ((dp | ~ap) ^ bp) + m5 + 0x5C4DD124, 9);
            // Iteration 23
            b = RotateLeft(b + ((c & d) | (~c & a)) + m3 + 0x5A827999, 15);
            bp = RotateLeft(bp + ((cp | ~dp) ^ ap) + ma + 0x5C4DD124, 11);
            // Iteration 24
            a = RotateLeft(a + ((b & c) | (~b & d)) + mc + 0x5A827999, 7);
            ap = RotateLeft(ap + ((bp | ~cp) ^ dp) + me + 0x5C4DD124, 7);
            // Iteration 25
            d = RotateLeft(d + ((a & b) | (~a & c)) + m0 + 0x5A827999, 12);
            dp = RotateLeft(dp + ((ap | ~bp) ^ cp) + mf + 0x5C4DD124, 7);
            // Iteration 26
            c = RotateLeft(c + ((d & a) | (~d & b)) + m9 + 0x5A827999, 15);
            cp = RotateLeft(cp + ((dp | ~ap) ^ bp) + m8 + 0x5C4DD124, 12);
            // Iteration 27
            b = RotateLeft(b + ((c & d) | (~c & a)) + m5 + 0x5A827999, 9);
            bp = RotateLeft(bp + ((cp | ~dp) ^ ap) + mc + 0x5C4DD124, 7);
            // Iteration 28
            a = RotateLeft(a + ((b & c) | (~b & d)) + m2 + 0x5A827999, 11);
            ap = RotateLeft(ap + ((bp | ~cp) ^ dp) + m4 + 0x5C4DD124, 6);
            // Iteration 29
            d = RotateLeft(d + ((a & b) | (~a & c)) + me + 0x5A827999, 7);
            dp = RotateLeft(dp + ((ap | ~bp) ^ cp) + m9 + 0x5C4DD124, 15);
            // Iteration 30
            c = RotateLeft(c + ((d & a) | (~d & b)) + mb + 0x5A827999, 13);
            cp = RotateLeft(cp + ((dp | ~ap) ^ bp) + m1 + 0x5C4DD124, 13);
            // Iteration 31
            b = RotateLeft(b + ((c & d) | (~c & a)) + m8 + 0x5A827999, 12);
            bp = RotateLeft(bp + ((cp | ~dp) ^ ap) + m2 + 0x5C4DD124, 11);
            // Iteration 32
            a = RotateLeft(a + ((b | ~c) ^ d) + m3 + 0x6ED9EBA1, 11);
            ap = RotateLeft(ap + ((bp & cp) | (~bp & dp)) + mf + 0x6D703EF3, 9);
            // Iteration 33
            d = RotateLeft(d + ((a | ~b) ^ c) + ma + 0x6ED9EBA1, 13);
            dp = RotateLeft(dp + ((ap & bp) | (~ap & cp)) + m5 + 0x6D703EF3, 7);
            // Iteration 34
            c = RotateLeft(c + ((d | ~a) ^ b) + me + 0x6ED9EBA1, 6);
            cp = RotateLeft(cp + ((dp & ap) | (~dp & bp)) + m1 + 0x6D703EF3, 15);
            // Iteration 35
            b = RotateLeft(b + ((c | ~d) ^ a) + m4 + 0x6ED9EBA1, 7);
            bp = RotateLeft(bp + ((cp & dp) | (~cp & ap)) + m3 + 0x6D703EF3, 11);
            // Iteration 36
            a = RotateLeft(a + ((b | ~c) ^ d) + m9 + 0x6ED9EBA1, 14);
            ap = RotateLeft(ap + ((bp & cp) | (~bp & dp)) + m7 + 0x6D703EF3, 8);
            // Iteration 37
            d = RotateLeft(d + ((a | ~b) ^ c) + mf + 0x6ED9EBA1, 9);
            dp = RotateLeft(dp + ((ap & bp) | (~ap & cp)) + me + 0x6D703EF3, 6);
            // Iteration 38
            c = RotateLeft(c + ((d | ~a) ^ b) + m8 + 0x6ED9EBA1, 13);
            cp = RotateLeft(cp + ((dp & ap) | (~dp & bp)) + m6 + 0x6D703EF3, 6);
            // Iteration 39
            b = RotateLeft(b + ((c | ~d) ^ a) + m1 + 0x6ED9EBA1, 15);
            bp = RotateLeft(bp + ((cp & dp) | (~cp & ap)) + m9 + 0x6D703EF3, 14);
            // Iteration 40
            a = RotateLeft(a + ((b | ~c) ^ d) + m2 + 0x6ED9EBA1, 14);
            ap = RotateLeft(ap + ((bp & cp) | (~bp & dp)) + mb + 0x6D703EF3, 12);
            // Iteration 41
            d = RotateLeft(d + ((a | ~b) ^ c) + m7 + 0x6ED9EBA1, 8);
            dp = RotateLeft(dp + ((ap & bp) | (~ap & cp)) + m8 + 0x6D703EF3, 13);
            // Iteration 42
            c = RotateLeft(c + ((d | ~a) ^ b) + m0 + 0x6ED9EBA1, 13);
            cp = RotateLeft(cp + ((dp & ap) | (~dp & bp)) + mc + 0x6D703EF3, 5);
            // Iteration 43
            b = RotateLeft(b + ((c | ~d) ^ a) + m6 + 0x6ED9EBA1, 6);
            bp = RotateLeft(bp + ((cp & dp) | (~cp & ap)) + m2 + 0x6D703EF3, 14);
            // Iteration 44
            a = RotateLeft(a + ((b | ~c) ^ d) + md + 0x6ED9EBA1, 5);
            ap = RotateLeft(ap + ((bp & cp) | (~bp & dp)) + ma + 0x6D703EF3, 13);
            // Iteration 45
            d = RotateLeft(d + ((a | ~b) ^ c) + mb + 0x6ED9EBA1, 12);
            dp = RotateLeft(dp + ((ap & bp) | (~ap & cp)) + m0 + 0x6D703EF3, 13);
            // Iteration 46
            c = RotateLeft(c + ((d | ~a) ^ b) + m5 + 0x6ED9EBA1, 7);
            cp = RotateLeft(cp + ((dp & ap) | (~dp & bp)) + m4 + 0x6D703EF3, 7);
            // Iteration 47
            b = RotateLeft(b + ((c | ~d) ^ a) + mc + 0x6ED9EBA1, 5);
            bp = RotateLeft(bp + ((cp & dp) | (~cp & ap)) + md + 0x6D703EF3, 5);
            // Iteration 48
            a = RotateLeft(a + ((b & d) | (c & ~d)) + m1 + 0x8F1BBCDC, 11);
            ap = RotateLeft(ap + (bp ^ cp ^ dp) + m8, 15);
            // Iteration 49
            d = RotateLeft(d + ((a & c) | (b & ~c)) + m9 + 0x8F1BBCDC, 12);
            dp = RotateLeft(dp + (ap ^ bp ^ cp) + m6, 5);
            // Iteration 50
            c = RotateLeft(c + ((d & b) | (a & ~b)) + mb + 0x8F1BBCDC, 14);
            cp = RotateLeft(cp + (dp ^ ap ^ bp) + m4, 8);
            // Iteration 51
            b = RotateLeft(b + ((c & a) | (d & ~a)) + ma + 0x8F1BBCDC, 15);
            bp = RotateLeft(bp + (cp ^ dp ^ ap) + m1, 11);
            // Iteration 52
            a = RotateLeft(a + ((b & d) | (c & ~d)) + m0 + 0x8F1BBCDC, 14);
            ap = RotateLeft(ap + (bp ^ cp ^ dp) + m3, 14);
            // Iteration 53
            d = RotateLeft(d + ((a & c) | (b & ~c)) + m8 + 0x8F1BBCDC, 15);
            dp = RotateLeft(dp + (ap ^ bp ^ cp) + mb, 14);
            // Iteration 54
            c = RotateLeft(c + ((d & b) | (a & ~b)) + mc + 0x8F1BBCDC, 9);
            cp = RotateLeft(cp + (dp ^ ap ^ bp) + mf, 6);
            // Iteration 55
            b = RotateLeft(b + ((c & a) | (d & ~a)) + m4 + 0x8F1BBCDC, 8);
            bp = RotateLeft(bp + (cp ^ dp ^ ap) + m0, 14);
            // Iteration 56
            a = RotateLeft(a + ((b & d) | (c & ~d)) + md + 0x8F1BBCDC, 9);
            ap = RotateLeft(ap + (bp ^ cp ^ dp) + m5, 6);
            // Iteration 57
            d = RotateLeft(d + ((a & c) | (b & ~c)) + m3 + 0x8F1BBCDC, 14);
            dp = RotateLeft(dp + (ap ^ bp ^ cp) + mc, 9);
            // Iteration 58
            c = RotateLeft(c + ((d & b) | (a & ~b)) + m7 + 0x8F1BBCDC, 5);
            cp = RotateLeft(cp + (dp ^ ap ^ bp) + m2, 12);
            // Iteration 59
            b = RotateLeft(b + ((c & a) | (d & ~a)) + mf + 0x8F1BBCDC, 6);
            bp = RotateLeft(bp + (cp ^ dp ^ ap) + md, 9);
            // Iteration 60
            a = RotateLeft(a + ((b & d) | (c & ~d)) + me + 0x8F1BBCDC, 8);
            ap = RotateLeft(ap + (bp ^ cp ^ dp) + m9, 12);
            // Iteration 61
            d = RotateLeft(d + ((a & c) | (b & ~c)) + m5 + 0x8F1BBCDC, 6);
            dp = RotateLeft(dp + (ap ^ bp ^ cp) + m7, 5);
            // Iteration 62
            c = RotateLeft(c + ((d & b) | (a & ~b)) + m6 + 0x8F1BBCDC, 5);
            cp = RotateLeft(cp + (dp ^ ap ^ bp) + ma, 15);
            // Iteration 63
            b = RotateLeft(b + ((c & a) | (d & ~a)) + m2 + 0x8F1BBCDC, 12);
            bp = RotateLeft(bp + (cp ^ dp ^ ap) + me, 8);

            #endregion

            uint t = h1 + c + dp;
            h1 = h2 + d + ap;
            h2 = h3 + a + bp;
            h3 = h0 + b + cp;
            h0 = t;
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

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[16];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x0C);
            return digest;
        }
    }
}