using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class RIPEMD256 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 64;
        public override int DigestLength => 32;

        uint h0;
        uint h1;
        uint h2;
        uint h3;
        uint h4;
        uint h5;
        uint h6;
        uint h7;
        long length;

        public RIPEMD256()
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
            h4 = 0x76543210;
            h5 = 0xFEDCBA98;
            h6 = 0x89ABCDEF;
            h7 = 0x01234567;
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
            uint ap = h4;
            uint bp = h5;
            uint cp = h6;
            uint dp = h7;

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
            ap = RotateLeft(ap + ((b & c) | (~b & d)) + m7 + 0x5A827999, 7);
            a = RotateLeft(a + ((bp | ~cp) ^ dp) + m6 + 0x5C4DD124, 9);
            // Iteration 17
            d = RotateLeft(d + ((ap & b) | (~ap & c)) + m4 + 0x5A827999, 6);
            dp = RotateLeft(dp + ((a | ~bp) ^ cp) + mb + 0x5C4DD124, 13);
            // Iteration 18
            c = RotateLeft(c + ((d & ap) | (~d & b)) + md + 0x5A827999, 8);
            cp = RotateLeft(cp + ((dp | ~a) ^ bp) + m3 + 0x5C4DD124, 15);
            // Iteration 19
            b = RotateLeft(b + ((c & d) | (~c & ap)) + m1 + 0x5A827999, 13);
            bp = RotateLeft(bp + ((cp | ~dp) ^ a) + m7 + 0x5C4DD124, 7);
            // Iteration 20
            ap = RotateLeft(ap + ((b & c) | (~b & d)) + ma + 0x5A827999, 11);
            a = RotateLeft(a + ((bp | ~cp) ^ dp) + m0 + 0x5C4DD124, 12);
            // Iteration 21
            d = RotateLeft(d + ((ap & b) | (~ap & c)) + m6 + 0x5A827999, 9);
            dp = RotateLeft(dp + ((a | ~bp) ^ cp) + md + 0x5C4DD124, 8);
            // Iteration 22
            c = RotateLeft(c + ((d & ap) | (~d & b)) + mf + 0x5A827999, 7);
            cp = RotateLeft(cp + ((dp | ~a) ^ bp) + m5 + 0x5C4DD124, 9);
            // Iteration 23
            b = RotateLeft(b + ((c & d) | (~c & ap)) + m3 + 0x5A827999, 15);
            bp = RotateLeft(bp + ((cp | ~dp) ^ a) + ma + 0x5C4DD124, 11);
            // Iteration 24
            ap = RotateLeft(ap + ((b & c) | (~b & d)) + mc + 0x5A827999, 7);
            a = RotateLeft(a + ((bp | ~cp) ^ dp) + me + 0x5C4DD124, 7);
            // Iteration 25
            d = RotateLeft(d + ((ap & b) | (~ap & c)) + m0 + 0x5A827999, 12);
            dp = RotateLeft(dp + ((a | ~bp) ^ cp) + mf + 0x5C4DD124, 7);
            // Iteration 26
            c = RotateLeft(c + ((d & ap) | (~d & b)) + m9 + 0x5A827999, 15);
            cp = RotateLeft(cp + ((dp | ~a) ^ bp) + m8 + 0x5C4DD124, 12);
            // Iteration 27
            b = RotateLeft(b + ((c & d) | (~c & ap)) + m5 + 0x5A827999, 9);
            bp = RotateLeft(bp + ((cp | ~dp) ^ a) + mc + 0x5C4DD124, 7);
            // Iteration 28
            ap = RotateLeft(ap + ((b & c) | (~b & d)) + m2 + 0x5A827999, 11);
            a = RotateLeft(a + ((bp | ~cp) ^ dp) + m4 + 0x5C4DD124, 6);
            // Iteration 29
            d = RotateLeft(d + ((ap & b) | (~ap & c)) + me + 0x5A827999, 7);
            dp = RotateLeft(dp + ((a | ~bp) ^ cp) + m9 + 0x5C4DD124, 15);
            // Iteration 30
            c = RotateLeft(c + ((d & ap) | (~d & b)) + mb + 0x5A827999, 13);
            cp = RotateLeft(cp + ((dp | ~a) ^ bp) + m1 + 0x5C4DD124, 13);
            // Iteration 31
            b = RotateLeft(b + ((c & d) | (~c & ap)) + m8 + 0x5A827999, 12);
            bp = RotateLeft(bp + ((cp | ~dp) ^ a) + m2 + 0x5C4DD124, 11);
            // Iteration 32
            ap = RotateLeft(ap + ((bp | ~c) ^ d) + m3 + 0x6ED9EBA1, 11);
            a = RotateLeft(a + ((b & cp) | (~b & dp)) + mf + 0x6D703EF3, 9);
            // Iteration 33
            d = RotateLeft(d + ((ap | ~bp) ^ c) + ma + 0x6ED9EBA1, 13);
            dp = RotateLeft(dp + ((a & b) | (~a & cp)) + m5 + 0x6D703EF3, 7);
            // Iteration 34
            c = RotateLeft(c + ((d | ~ap) ^ bp) + me + 0x6ED9EBA1, 6);
            cp = RotateLeft(cp + ((dp & a) | (~dp & b)) + m1 + 0x6D703EF3, 15);
            // Iteration 35
            bp = RotateLeft(bp + ((c | ~d) ^ ap) + m4 + 0x6ED9EBA1, 7);
            b = RotateLeft(b + ((cp & dp) | (~cp & a)) + m3 + 0x6D703EF3, 11);
            // Iteration 36
            ap = RotateLeft(ap + ((bp | ~c) ^ d) + m9 + 0x6ED9EBA1, 14);
            a = RotateLeft(a + ((b & cp) | (~b & dp)) + m7 + 0x6D703EF3, 8);
            // Iteration 37
            d = RotateLeft(d + ((ap | ~bp) ^ c) + mf + 0x6ED9EBA1, 9);
            dp = RotateLeft(dp + ((a & b) | (~a & cp)) + me + 0x6D703EF3, 6);
            // Iteration 38
            c = RotateLeft(c + ((d | ~ap) ^ bp) + m8 + 0x6ED9EBA1, 13);
            cp = RotateLeft(cp + ((dp & a) | (~dp & b)) + m6 + 0x6D703EF3, 6);
            // Iteration 39
            bp = RotateLeft(bp + ((c | ~d) ^ ap) + m1 + 0x6ED9EBA1, 15);
            b = RotateLeft(b + ((cp & dp) | (~cp & a)) + m9 + 0x6D703EF3, 14);
            // Iteration 40
            ap = RotateLeft(ap + ((bp | ~c) ^ d) + m2 + 0x6ED9EBA1, 14);
            a = RotateLeft(a + ((b & cp) | (~b & dp)) + mb + 0x6D703EF3, 12);
            // Iteration 41
            d = RotateLeft(d + ((ap | ~bp) ^ c) + m7 + 0x6ED9EBA1, 8);
            dp = RotateLeft(dp + ((a & b) | (~a & cp)) + m8 + 0x6D703EF3, 13);
            // Iteration 42
            c = RotateLeft(c + ((d | ~ap) ^ bp) + m0 + 0x6ED9EBA1, 13);
            cp = RotateLeft(cp + ((dp & a) | (~dp & b)) + mc + 0x6D703EF3, 5);
            // Iteration 43
            bp = RotateLeft(bp + ((c | ~d) ^ ap) + m6 + 0x6ED9EBA1, 6);
            b = RotateLeft(b + ((cp & dp) | (~cp & a)) + m2 + 0x6D703EF3, 14);
            // Iteration 44
            ap = RotateLeft(ap + ((bp | ~c) ^ d) + md + 0x6ED9EBA1, 5);
            a = RotateLeft(a + ((b & cp) | (~b & dp)) + ma + 0x6D703EF3, 13);
            // Iteration 45
            d = RotateLeft(d + ((ap | ~bp) ^ c) + mb + 0x6ED9EBA1, 12);
            dp = RotateLeft(dp + ((a & b) | (~a & cp)) + m0 + 0x6D703EF3, 13);
            // Iteration 46
            c = RotateLeft(c + ((d | ~ap) ^ bp) + m5 + 0x6ED9EBA1, 7);
            cp = RotateLeft(cp + ((dp & a) | (~dp & b)) + m4 + 0x6D703EF3, 7);
            // Iteration 47
            bp = RotateLeft(bp + ((c | ~d) ^ ap) + mc + 0x6ED9EBA1, 5);
            b = RotateLeft(b + ((cp & dp) | (~cp & a)) + md + 0x6D703EF3, 5);
            // Iteration 48
            ap = RotateLeft(ap + ((bp & d) | (cp & ~d)) + m1 + 0x8F1BBCDC, 11);
            a = RotateLeft(a + (b ^ c ^ dp) + m8, 15);
            // Iteration 49
            d = RotateLeft(d + ((ap & cp) | (bp & ~cp)) + m9 + 0x8F1BBCDC, 12);
            dp = RotateLeft(dp + (a ^ b ^ c) + m6, 5);
            // Iteration 50
            cp = RotateLeft(cp + ((d & bp) | (ap & ~bp)) + mb + 0x8F1BBCDC, 14);
            c = RotateLeft(c + (dp ^ a ^ b) + m4, 8);
            // Iteration 51
            bp = RotateLeft(bp + ((cp & ap) | (d & ~ap)) + ma + 0x8F1BBCDC, 15);
            b = RotateLeft(b + (c ^ dp ^ a) + m1, 11);
            // Iteration 52
            ap = RotateLeft(ap + ((bp & d) | (cp & ~d)) + m0 + 0x8F1BBCDC, 14);
            a = RotateLeft(a + (b ^ c ^ dp) + m3, 14);
            // Iteration 53
            d = RotateLeft(d + ((ap & cp) | (bp & ~cp)) + m8 + 0x8F1BBCDC, 15);
            dp = RotateLeft(dp + (a ^ b ^ c) + mb, 14);
            // Iteration 54
            cp = RotateLeft(cp + ((d & bp) | (ap & ~bp)) + mc + 0x8F1BBCDC, 9);
            c = RotateLeft(c + (dp ^ a ^ b) + mf, 6);
            // Iteration 55
            bp = RotateLeft(bp + ((cp & ap) | (d & ~ap)) + m4 + 0x8F1BBCDC, 8);
            b = RotateLeft(b + (c ^ dp ^ a) + m0, 14);
            // Iteration 56
            ap = RotateLeft(ap + ((bp & d) | (cp & ~d)) + md + 0x8F1BBCDC, 9);
            a = RotateLeft(a + (b ^ c ^ dp) + m5, 6);
            // Iteration 57
            d = RotateLeft(d + ((ap & cp) | (bp & ~cp)) + m3 + 0x8F1BBCDC, 14);
            dp = RotateLeft(dp + (a ^ b ^ c) + mc, 9);
            // Iteration 58
            cp = RotateLeft(cp + ((d & bp) | (ap & ~bp)) + m7 + 0x8F1BBCDC, 5);
            c = RotateLeft(c + (dp ^ a ^ b) + m2, 12);
            // Iteration 59
            bp = RotateLeft(bp + ((cp & ap) | (d & ~ap)) + mf + 0x8F1BBCDC, 6);
            b = RotateLeft(b + (c ^ dp ^ a) + md, 9);
            // Iteration 60
            ap = RotateLeft(ap + ((bp & d) | (cp & ~d)) + me + 0x8F1BBCDC, 8);
            a = RotateLeft(a + (b ^ c ^ dp) + m9, 12);
            // Iteration 61
            d = RotateLeft(d + ((ap & cp) | (bp & ~cp)) + m5 + 0x8F1BBCDC, 6);
            dp = RotateLeft(dp + (a ^ b ^ c) + m7, 5);
            // Iteration 62
            cp = RotateLeft(cp + ((d & bp) | (ap & ~bp)) + m6 + 0x8F1BBCDC, 5);
            c = RotateLeft(c + (dp ^ a ^ b) + ma, 15);
            // Iteration 63
            bp = RotateLeft(bp + ((cp & ap) | (d & ~ap)) + m2 + 0x8F1BBCDC, 12);
            b = RotateLeft(b + (c ^ dp ^ a) + me, 8);

            #endregion

            h0 += ap;
            h1 += bp;
            h2 += cp;
            h3 += dp;
            h4 += a;
            h5 += b;
            h6 += c;
            h7 += d;
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
            byte[] digest = new byte[32];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(h5, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(h6, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(h7, ByteOrder.LittleEndian, digest, 0x1C);
            return digest;
        }
    }
}