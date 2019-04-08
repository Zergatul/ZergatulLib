using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class RIPEMD160 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 64;
        public override int DigestLength => 20;

        uint h0;
        uint h1;
        uint h2;
        uint h3;
        uint h4;
        long length;

        public RIPEMD160()
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
            uint e = h4;
            uint ap = h0;
            uint bp = h1;
            uint cp = h2;
            uint dp = h3;
            uint ep = h4;

            #region Loop

            // Iteration 0
            a = RotateLeft(a + (b ^ c ^ d) + m0, 11) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + (bp ^ (cp | ~dp)) + m5 + 0x50A28BE6, 8) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 1
            e = RotateLeft(e + (a ^ b ^ c) + m1, 14) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + (ap ^ (bp | ~cp)) + me + 0x50A28BE6, 9) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 2
            d = RotateLeft(d + (e ^ a ^ b) + m2, 15) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + (ep ^ (ap | ~bp)) + m7 + 0x50A28BE6, 9) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 3
            c = RotateLeft(c + (d ^ e ^ a) + m3, 12) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + (dp ^ (ep | ~ap)) + m0 + 0x50A28BE6, 11) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 4
            b = RotateLeft(b + (c ^ d ^ e) + m4, 5) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + (cp ^ (dp | ~ep)) + m9 + 0x50A28BE6, 13) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 5
            a = RotateLeft(a + (b ^ c ^ d) + m5, 8) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + (bp ^ (cp | ~dp)) + m2 + 0x50A28BE6, 15) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 6
            e = RotateLeft(e + (a ^ b ^ c) + m6, 7) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + (ap ^ (bp | ~cp)) + mb + 0x50A28BE6, 15) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 7
            d = RotateLeft(d + (e ^ a ^ b) + m7, 9) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + (ep ^ (ap | ~bp)) + m4 + 0x50A28BE6, 5) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 8
            c = RotateLeft(c + (d ^ e ^ a) + m8, 11) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + (dp ^ (ep | ~ap)) + md + 0x50A28BE6, 7) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 9
            b = RotateLeft(b + (c ^ d ^ e) + m9, 13) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + (cp ^ (dp | ~ep)) + m6 + 0x50A28BE6, 7) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 10
            a = RotateLeft(a + (b ^ c ^ d) + ma, 14) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + (bp ^ (cp | ~dp)) + mf + 0x50A28BE6, 8) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 11
            e = RotateLeft(e + (a ^ b ^ c) + mb, 15) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + (ap ^ (bp | ~cp)) + m8 + 0x50A28BE6, 11) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 12
            d = RotateLeft(d + (e ^ a ^ b) + mc, 6) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + (ep ^ (ap | ~bp)) + m1 + 0x50A28BE6, 14) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 13
            c = RotateLeft(c + (d ^ e ^ a) + md, 7) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + (dp ^ (ep | ~ap)) + ma + 0x50A28BE6, 14) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 14
            b = RotateLeft(b + (c ^ d ^ e) + me, 9) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + (cp ^ (dp | ~ep)) + m3 + 0x50A28BE6, 12) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 15
            a = RotateLeft(a + (b ^ c ^ d) + mf, 8) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + (bp ^ (cp | ~dp)) + mc + 0x50A28BE6, 6) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 16
            e = RotateLeft(e + ((a & b) | (~a & c)) + m7 + 0x5A827999, 7) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap & cp) | (bp & ~cp)) + m6 + 0x5C4DD124, 9) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 17
            d = RotateLeft(d + ((e & a) | (~e & b)) + m4 + 0x5A827999, 6) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep & bp) | (ap & ~bp)) + mb + 0x5C4DD124, 13) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 18
            c = RotateLeft(c + ((d & e) | (~d & a)) + md + 0x5A827999, 8) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp & ap) | (ep & ~ap)) + m3 + 0x5C4DD124, 15) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 19
            b = RotateLeft(b + ((c & d) | (~c & e)) + m1 + 0x5A827999, 13) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + ((cp & ep) | (dp & ~ep)) + m7 + 0x5C4DD124, 7) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 20
            a = RotateLeft(a + ((b & c) | (~b & d)) + ma + 0x5A827999, 11) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + ((bp & dp) | (cp & ~dp)) + m0 + 0x5C4DD124, 12) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 21
            e = RotateLeft(e + ((a & b) | (~a & c)) + m6 + 0x5A827999, 9) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap & cp) | (bp & ~cp)) + md + 0x5C4DD124, 8) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 22
            d = RotateLeft(d + ((e & a) | (~e & b)) + mf + 0x5A827999, 7) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep & bp) | (ap & ~bp)) + m5 + 0x5C4DD124, 9) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 23
            c = RotateLeft(c + ((d & e) | (~d & a)) + m3 + 0x5A827999, 15) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp & ap) | (ep & ~ap)) + ma + 0x5C4DD124, 11) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 24
            b = RotateLeft(b + ((c & d) | (~c & e)) + mc + 0x5A827999, 7) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + ((cp & ep) | (dp & ~ep)) + me + 0x5C4DD124, 7) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 25
            a = RotateLeft(a + ((b & c) | (~b & d)) + m0 + 0x5A827999, 12) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + ((bp & dp) | (cp & ~dp)) + mf + 0x5C4DD124, 7) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 26
            e = RotateLeft(e + ((a & b) | (~a & c)) + m9 + 0x5A827999, 15) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap & cp) | (bp & ~cp)) + m8 + 0x5C4DD124, 12) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 27
            d = RotateLeft(d + ((e & a) | (~e & b)) + m5 + 0x5A827999, 9) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep & bp) | (ap & ~bp)) + mc + 0x5C4DD124, 7) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 28
            c = RotateLeft(c + ((d & e) | (~d & a)) + m2 + 0x5A827999, 11) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp & ap) | (ep & ~ap)) + m4 + 0x5C4DD124, 6) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 29
            b = RotateLeft(b + ((c & d) | (~c & e)) + me + 0x5A827999, 7) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + ((cp & ep) | (dp & ~ep)) + m9 + 0x5C4DD124, 15) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 30
            a = RotateLeft(a + ((b & c) | (~b & d)) + mb + 0x5A827999, 13) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + ((bp & dp) | (cp & ~dp)) + m1 + 0x5C4DD124, 13) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 31
            e = RotateLeft(e + ((a & b) | (~a & c)) + m8 + 0x5A827999, 12) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap & cp) | (bp & ~cp)) + m2 + 0x5C4DD124, 11) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 32
            d = RotateLeft(d + ((e | ~a) ^ b) + m3 + 0x6ED9EBA1, 11) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep | ~ap) ^ bp) + mf + 0x6D703EF3, 9) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 33
            c = RotateLeft(c + ((d | ~e) ^ a) + ma + 0x6ED9EBA1, 13) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp | ~ep) ^ ap) + m5 + 0x6D703EF3, 7) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 34
            b = RotateLeft(b + ((c | ~d) ^ e) + me + 0x6ED9EBA1, 6) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + ((cp | ~dp) ^ ep) + m1 + 0x6D703EF3, 15) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 35
            a = RotateLeft(a + ((b | ~c) ^ d) + m4 + 0x6ED9EBA1, 7) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + ((bp | ~cp) ^ dp) + m3 + 0x6D703EF3, 11) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 36
            e = RotateLeft(e + ((a | ~b) ^ c) + m9 + 0x6ED9EBA1, 14) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap | ~bp) ^ cp) + m7 + 0x6D703EF3, 8) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 37
            d = RotateLeft(d + ((e | ~a) ^ b) + mf + 0x6ED9EBA1, 9) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep | ~ap) ^ bp) + me + 0x6D703EF3, 6) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 38
            c = RotateLeft(c + ((d | ~e) ^ a) + m8 + 0x6ED9EBA1, 13) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp | ~ep) ^ ap) + m6 + 0x6D703EF3, 6) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 39
            b = RotateLeft(b + ((c | ~d) ^ e) + m1 + 0x6ED9EBA1, 15) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + ((cp | ~dp) ^ ep) + m9 + 0x6D703EF3, 14) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 40
            a = RotateLeft(a + ((b | ~c) ^ d) + m2 + 0x6ED9EBA1, 14) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + ((bp | ~cp) ^ dp) + mb + 0x6D703EF3, 12) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 41
            e = RotateLeft(e + ((a | ~b) ^ c) + m7 + 0x6ED9EBA1, 8) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap | ~bp) ^ cp) + m8 + 0x6D703EF3, 13) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 42
            d = RotateLeft(d + ((e | ~a) ^ b) + m0 + 0x6ED9EBA1, 13) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep | ~ap) ^ bp) + mc + 0x6D703EF3, 5) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 43
            c = RotateLeft(c + ((d | ~e) ^ a) + m6 + 0x6ED9EBA1, 6) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp | ~ep) ^ ap) + m2 + 0x6D703EF3, 14) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 44
            b = RotateLeft(b + ((c | ~d) ^ e) + md + 0x6ED9EBA1, 5) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + ((cp | ~dp) ^ ep) + ma + 0x6D703EF3, 13) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 45
            a = RotateLeft(a + ((b | ~c) ^ d) + mb + 0x6ED9EBA1, 12) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + ((bp | ~cp) ^ dp) + m0 + 0x6D703EF3, 13) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 46
            e = RotateLeft(e + ((a | ~b) ^ c) + m5 + 0x6ED9EBA1, 7) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap | ~bp) ^ cp) + m4 + 0x6D703EF3, 7) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 47
            d = RotateLeft(d + ((e | ~a) ^ b) + mc + 0x6ED9EBA1, 5) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep | ~ap) ^ bp) + md + 0x6D703EF3, 5) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 48
            c = RotateLeft(c + ((d & a) | (e & ~a)) + m1 + 0x8F1BBCDC, 11) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp & ep) | (~dp & ap)) + m8 + 0x7A6D76E9, 15) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 49
            b = RotateLeft(b + ((c & e) | (d & ~e)) + m9 + 0x8F1BBCDC, 12) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + ((cp & dp) | (~cp & ep)) + m6 + 0x7A6D76E9, 5) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 50
            a = RotateLeft(a + ((b & d) | (c & ~d)) + mb + 0x8F1BBCDC, 14) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + ((bp & cp) | (~bp & dp)) + m4 + 0x7A6D76E9, 8) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 51
            e = RotateLeft(e + ((a & c) | (b & ~c)) + ma + 0x8F1BBCDC, 15) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap & bp) | (~ap & cp)) + m1 + 0x7A6D76E9, 11) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 52
            d = RotateLeft(d + ((e & b) | (a & ~b)) + m0 + 0x8F1BBCDC, 14) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep & ap) | (~ep & bp)) + m3 + 0x7A6D76E9, 14) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 53
            c = RotateLeft(c + ((d & a) | (e & ~a)) + m8 + 0x8F1BBCDC, 15) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp & ep) | (~dp & ap)) + mb + 0x7A6D76E9, 14) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 54
            b = RotateLeft(b + ((c & e) | (d & ~e)) + mc + 0x8F1BBCDC, 9) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + ((cp & dp) | (~cp & ep)) + mf + 0x7A6D76E9, 6) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 55
            a = RotateLeft(a + ((b & d) | (c & ~d)) + m4 + 0x8F1BBCDC, 8) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + ((bp & cp) | (~bp & dp)) + m0 + 0x7A6D76E9, 14) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 56
            e = RotateLeft(e + ((a & c) | (b & ~c)) + md + 0x8F1BBCDC, 9) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap & bp) | (~ap & cp)) + m5 + 0x7A6D76E9, 6) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 57
            d = RotateLeft(d + ((e & b) | (a & ~b)) + m3 + 0x8F1BBCDC, 14) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep & ap) | (~ep & bp)) + mc + 0x7A6D76E9, 9) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 58
            c = RotateLeft(c + ((d & a) | (e & ~a)) + m7 + 0x8F1BBCDC, 5) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp & ep) | (~dp & ap)) + m2 + 0x7A6D76E9, 12) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 59
            b = RotateLeft(b + ((c & e) | (d & ~e)) + mf + 0x8F1BBCDC, 6) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + ((cp & dp) | (~cp & ep)) + md + 0x7A6D76E9, 9) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 60
            a = RotateLeft(a + ((b & d) | (c & ~d)) + me + 0x8F1BBCDC, 8) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + ((bp & cp) | (~bp & dp)) + m9 + 0x7A6D76E9, 12) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 61
            e = RotateLeft(e + ((a & c) | (b & ~c)) + m5 + 0x8F1BBCDC, 6) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + ((ap & bp) | (~ap & cp)) + m7 + 0x7A6D76E9, 5) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 62
            d = RotateLeft(d + ((e & b) | (a & ~b)) + m6 + 0x8F1BBCDC, 5) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + ((ep & ap) | (~ep & bp)) + ma + 0x7A6D76E9, 15) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 63
            c = RotateLeft(c + ((d & a) | (e & ~a)) + m2 + 0x8F1BBCDC, 12) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + ((dp & ep) | (~dp & ap)) + me + 0x7A6D76E9, 8) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 64
            b = RotateLeft(b + (c ^ (d | ~e)) + m4 + 0xA953FD4E, 9) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + (cp ^ dp ^ ep) + mc, 8) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 65
            a = RotateLeft(a + (b ^ (c | ~d)) + m0 + 0xA953FD4E, 15) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + (bp ^ cp ^ dp) + mf, 5) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 66
            e = RotateLeft(e + (a ^ (b | ~c)) + m5 + 0xA953FD4E, 5) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + (ap ^ bp ^ cp) + ma, 12) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 67
            d = RotateLeft(d + (e ^ (a | ~b)) + m9 + 0xA953FD4E, 11) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + (ep ^ ap ^ bp) + m4, 9) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 68
            c = RotateLeft(c + (d ^ (e | ~a)) + m7 + 0xA953FD4E, 6) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + (dp ^ ep ^ ap) + m1, 12) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 69
            b = RotateLeft(b + (c ^ (d | ~e)) + mc + 0xA953FD4E, 8) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + (cp ^ dp ^ ep) + m5, 5) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 70
            a = RotateLeft(a + (b ^ (c | ~d)) + m2 + 0xA953FD4E, 13) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + (bp ^ cp ^ dp) + m8, 14) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 71
            e = RotateLeft(e + (a ^ (b | ~c)) + ma + 0xA953FD4E, 12) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + (ap ^ bp ^ cp) + m7, 6) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 72
            d = RotateLeft(d + (e ^ (a | ~b)) + me + 0xA953FD4E, 5) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + (ep ^ ap ^ bp) + m6, 8) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 73
            c = RotateLeft(c + (d ^ (e | ~a)) + m1 + 0xA953FD4E, 12) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + (dp ^ ep ^ ap) + m2, 13) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 74
            b = RotateLeft(b + (c ^ (d | ~e)) + m3 + 0xA953FD4E, 13) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + (cp ^ dp ^ ep) + md, 6) + ap;
            dp = RotateLeft(dp, 10);
            // Iteration 75
            a = RotateLeft(a + (b ^ (c | ~d)) + m8 + 0xA953FD4E, 14) + e;
            c = RotateLeft(c, 10);
            ap = RotateLeft(ap + (bp ^ cp ^ dp) + me, 5) + ep;
            cp = RotateLeft(cp, 10);
            // Iteration 76
            e = RotateLeft(e + (a ^ (b | ~c)) + mb + 0xA953FD4E, 11) + d;
            b = RotateLeft(b, 10);
            ep = RotateLeft(ep + (ap ^ bp ^ cp) + m0, 15) + dp;
            bp = RotateLeft(bp, 10);
            // Iteration 77
            d = RotateLeft(d + (e ^ (a | ~b)) + m6 + 0xA953FD4E, 8) + c;
            a = RotateLeft(a, 10);
            dp = RotateLeft(dp + (ep ^ ap ^ bp) + m3, 13) + cp;
            ap = RotateLeft(ap, 10);
            // Iteration 78
            c = RotateLeft(c + (d ^ (e | ~a)) + mf + 0xA953FD4E, 5) + b;
            e = RotateLeft(e, 10);
            cp = RotateLeft(cp + (dp ^ ep ^ ap) + m9, 11) + bp;
            ep = RotateLeft(ep, 10);
            // Iteration 79
            b = RotateLeft(b + (c ^ (d | ~e)) + md + 0xA953FD4E, 6) + a;
            d = RotateLeft(d, 10);
            bp = RotateLeft(bp + (cp ^ dp ^ ep) + mb, 11) + ap;
            dp = RotateLeft(dp, 10);

            #endregion

            uint t = h1 + c + dp;
            h1 = h2 + d + ep;
            h2 = h3 + e + ap;
            h3 = h4 + a + bp;
            h4 = h0 + b + cp;
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
            byte[] digest = new byte[20];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.LittleEndian, digest, 0x10);
            return digest;
        }
    }
}