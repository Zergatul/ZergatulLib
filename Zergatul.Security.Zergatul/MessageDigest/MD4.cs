using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class MD4 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 64;
        public override int DigestLength => 16;

        uint sa;
        uint sb;
        uint sc;
        uint sd;
        long length;

        public MD4()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            sa = 0x67452301;
            sb = 0xEFCDAB89;
            sc = 0x98BADCFE;
            sd = 0x10325476;
            bufOffset = 0;
            length = 0;
        }

        protected override void IncreaseLength(int value)  => length += value;

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

            uint a = sa;
            uint b = sb;
            uint c = sc;
            uint d = sd;

            #region Loop

            // Round 1
            a = RotateLeft(a + ((b & c) | (~b & d)) + m0, 0x03);
            d = RotateLeft(d + ((a & b) | (~a & c)) + m1, 0x07);
            c = RotateLeft(c + ((d & a) | (~d & b)) + m2, 0x0b);
            b = RotateLeft(b + ((c & d) | (~c & a)) + m3, 0x13);
            a = RotateLeft(a + ((b & c) | (~b & d)) + m4, 0x03);
            d = RotateLeft(d + ((a & b) | (~a & c)) + m5, 0x07);
            c = RotateLeft(c + ((d & a) | (~d & b)) + m6, 0x0b);
            b = RotateLeft(b + ((c & d) | (~c & a)) + m7, 0x13);
            a = RotateLeft(a + ((b & c) | (~b & d)) + m8, 0x03);
            d = RotateLeft(d + ((a & b) | (~a & c)) + m9, 0x07);
            c = RotateLeft(c + ((d & a) | (~d & b)) + ma, 0x0b);
            b = RotateLeft(b + ((c & d) | (~c & a)) + mb, 0x13);
            a = RotateLeft(a + ((b & c) | (~b & d)) + mc, 0x03);
            d = RotateLeft(d + ((a & b) | (~a & c)) + md, 0x07);
            c = RotateLeft(c + ((d & a) | (~d & b)) + me, 0x0b);
            b = RotateLeft(b + ((c & d) | (~c & a)) + mf, 0x13);

            // Round 2
            a = RotateLeft(a + ((b & c) | (b & d) | (c & d)) + m0 + 0x5A827999, 0x03);
            d = RotateLeft(d + ((a & b) | (a & c) | (b & c)) + m4 + 0x5A827999, 0x05);
            c = RotateLeft(c + ((d & a) | (d & b) | (a & b)) + m8 + 0x5A827999, 0x09);
            b = RotateLeft(b + ((c & d) | (c & a) | (d & a)) + mc + 0x5A827999, 0x0d);
            a = RotateLeft(a + ((b & c) | (b & d) | (c & d)) + m1 + 0x5A827999, 0x03);
            d = RotateLeft(d + ((a & b) | (a & c) | (b & c)) + m5 + 0x5A827999, 0x05);
            c = RotateLeft(c + ((d & a) | (d & b) | (a & b)) + m9 + 0x5A827999, 0x09);
            b = RotateLeft(b + ((c & d) | (c & a) | (d & a)) + md + 0x5A827999, 0x0d);
            a = RotateLeft(a + ((b & c) | (b & d) | (c & d)) + m2 + 0x5A827999, 0x03);
            d = RotateLeft(d + ((a & b) | (a & c) | (b & c)) + m6 + 0x5A827999, 0x05);
            c = RotateLeft(c + ((d & a) | (d & b) | (a & b)) + ma + 0x5A827999, 0x09);
            b = RotateLeft(b + ((c & d) | (c & a) | (d & a)) + me + 0x5A827999, 0x0d);
            a = RotateLeft(a + ((b & c) | (b & d) | (c & d)) + m3 + 0x5A827999, 0x03);
            d = RotateLeft(d + ((a & b) | (a & c) | (b & c)) + m7 + 0x5A827999, 0x05);
            c = RotateLeft(c + ((d & a) | (d & b) | (a & b)) + mb + 0x5A827999, 0x09);
            b = RotateLeft(b + ((c & d) | (c & a) | (d & a)) + mf + 0x5A827999, 0x0d);

            // Round 3
            a = RotateLeft(a + (b ^ c ^ d) + m0 + 0x6ED9EBA1, 0x03);
            d = RotateLeft(d + (a ^ b ^ c) + m8 + 0x6ED9EBA1, 0x09);
            c = RotateLeft(c + (d ^ a ^ b) + m4 + 0x6ED9EBA1, 0x0b);
            b = RotateLeft(b + (c ^ d ^ a) + mc + 0x6ED9EBA1, 0x0f);
            a = RotateLeft(a + (b ^ c ^ d) + m2 + 0x6ED9EBA1, 0x03);
            d = RotateLeft(d + (a ^ b ^ c) + ma + 0x6ED9EBA1, 0x09);
            c = RotateLeft(c + (d ^ a ^ b) + m6 + 0x6ED9EBA1, 0x0b);
            b = RotateLeft(b + (c ^ d ^ a) + me + 0x6ED9EBA1, 0x0f);
            a = RotateLeft(a + (b ^ c ^ d) + m1 + 0x6ED9EBA1, 0x03);
            d = RotateLeft(d + (a ^ b ^ c) + m9 + 0x6ED9EBA1, 0x09);
            c = RotateLeft(c + (d ^ a ^ b) + m5 + 0x6ED9EBA1, 0x0b);
            b = RotateLeft(b + (c ^ d ^ a) + md + 0x6ED9EBA1, 0x0f);
            a = RotateLeft(a + (b ^ c ^ d) + m3 + 0x6ED9EBA1, 0x03);
            d = RotateLeft(d + (a ^ b ^ c) + mb + 0x6ED9EBA1, 0x09);
            c = RotateLeft(c + (d ^ a ^ b) + m7 + 0x6ED9EBA1, 0x0b);
            b = RotateLeft(b + (c ^ d ^ a) + mf + 0x6ED9EBA1, 0x0f);

            #endregion

            sa += a;
            sb += b;
            sc += c;
            sd += d;
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
            GetBytes(sa, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(sb, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(sc, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(sd, ByteOrder.LittleEndian, digest, 0x0C);
            return digest;
        }
    }
}