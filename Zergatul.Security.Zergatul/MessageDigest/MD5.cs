using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class MD5 : AbstractMessageDigestWithLength
    {
        public override int BlockLength => 64;
        public override int DigestLength => 16;

        uint a0;
        uint b0;
        uint c0;
        uint d0;
        long length;

        public MD5()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            a0 = 0x67452301;
            b0 = 0xEFCDAB89;
            c0 = 0x98BADCFE;
            d0 = 0x10325476;
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

            uint a = a0;
            uint b = b0;
            uint c = c0;
            uint d = d0;
            uint f;

            #region Loop

            // Iteration 0
            f = ((b & c) | (~b & d)) + a + 0xD76AA478 + m0;
            a = b + RotateLeft(f, 7);
            // Iteration 1
            f = ((a & b) | (~a & c)) + d + 0xE8C7B756 + m1;
            d = a + RotateLeft(f, 12);
            // Iteration 2
            f = ((d & a) | (~d & b)) + c + 0x242070DB + m2;
            c = d + RotateLeft(f, 17);
            // Iteration 3
            f = ((c & d) | (~c & a)) + b + 0xC1BDCEEE + m3;
            b = c + RotateLeft(f, 22);
            // Iteration 4
            f = ((b & c) | (~b & d)) + a + 0xF57C0FAF + m4;
            a = b + RotateLeft(f, 7);
            // Iteration 5
            f = ((a & b) | (~a & c)) + d + 0x4787C62A + m5;
            d = a + RotateLeft(f, 12);
            // Iteration 6
            f = ((d & a) | (~d & b)) + c + 0xA8304613 + m6;
            c = d + RotateLeft(f, 17);
            // Iteration 7
            f = ((c & d) | (~c & a)) + b + 0xFD469501 + m7;
            b = c + RotateLeft(f, 22);
            // Iteration 8
            f = ((b & c) | (~b & d)) + a + 0x698098D8 + m8;
            a = b + RotateLeft(f, 7);
            // Iteration 9
            f = ((a & b) | (~a & c)) + d + 0x8B44F7AF + m9;
            d = a + RotateLeft(f, 12);
            // Iteration 10
            f = ((d & a) | (~d & b)) + c + 0xFFFF5BB1 + ma;
            c = d + RotateLeft(f, 17);
            // Iteration 11
            f = ((c & d) | (~c & a)) + b + 0x895CD7BE + mb;
            b = c + RotateLeft(f, 22);
            // Iteration 12
            f = ((b & c) | (~b & d)) + a + 0x6B901122 + mc;
            a = b + RotateLeft(f, 7);
            // Iteration 13
            f = ((a & b) | (~a & c)) + d + 0xFD987193 + md;
            d = a + RotateLeft(f, 12);
            // Iteration 14
            f = ((d & a) | (~d & b)) + c + 0xA679438E + me;
            c = d + RotateLeft(f, 17);
            // Iteration 15
            f = ((c & d) | (~c & a)) + b + 0x49B40821 + mf;
            b = c + RotateLeft(f, 22);
            // Iteration 16
            f = ((d & b) | (~d & c)) + a + 0xF61E2562 + m1;
            a = b + RotateLeft(f, 5);
            // Iteration 17
            f = ((c & a) | (~c & b)) + d + 0xC040B340 + m6;
            d = a + RotateLeft(f, 9);
            // Iteration 18
            f = ((b & d) | (~b & a)) + c + 0x265E5A51 + mb;
            c = d + RotateLeft(f, 14);
            // Iteration 19
            f = ((a & c) | (~a & d)) + b + 0xE9B6C7AA + m0;
            b = c + RotateLeft(f, 20);
            // Iteration 20
            f = ((d & b) | (~d & c)) + a + 0xD62F105D + m5;
            a = b + RotateLeft(f, 5);
            // Iteration 21
            f = ((c & a) | (~c & b)) + d + 0x02441453 + ma;
            d = a + RotateLeft(f, 9);
            // Iteration 22
            f = ((b & d) | (~b & a)) + c + 0xD8A1E681 + mf;
            c = d + RotateLeft(f, 14);
            // Iteration 23
            f = ((a & c) | (~a & d)) + b + 0xE7D3FBC8 + m4;
            b = c + RotateLeft(f, 20);
            // Iteration 24
            f = ((d & b) | (~d & c)) + a + 0x21E1CDE6 + m9;
            a = b + RotateLeft(f, 5);
            // Iteration 25
            f = ((c & a) | (~c & b)) + d + 0xC33707D6 + me;
            d = a + RotateLeft(f, 9);
            // Iteration 26
            f = ((b & d) | (~b & a)) + c + 0xF4D50D87 + m3;
            c = d + RotateLeft(f, 14);
            // Iteration 27
            f = ((a & c) | (~a & d)) + b + 0x455A14ED + m8;
            b = c + RotateLeft(f, 20);
            // Iteration 28
            f = ((d & b) | (~d & c)) + a + 0xA9E3E905 + md;
            a = b + RotateLeft(f, 5);
            // Iteration 29
            f = ((c & a) | (~c & b)) + d + 0xFCEFA3F8 + m2;
            d = a + RotateLeft(f, 9);
            // Iteration 30
            f = ((b & d) | (~b & a)) + c + 0x676F02D9 + m7;
            c = d + RotateLeft(f, 14);
            // Iteration 31
            f = ((a & c) | (~a & d)) + b + 0x8D2A4C8A + mc;
            b = c + RotateLeft(f, 20);
            // Iteration 32
            f = (b ^ c ^ d) + a + 0xFFFA3942 + m5;
            a = b + RotateLeft(f, 4);
            // Iteration 33
            f = (a ^ b ^ c) + d + 0x8771F681 + m8;
            d = a + RotateLeft(f, 11);
            // Iteration 34
            f = (d ^ a ^ b) + c + 0x6D9D6122 + mb;
            c = d + RotateLeft(f, 16);
            // Iteration 35
            f = (c ^ d ^ a) + b + 0xFDE5380C + me;
            b = c + RotateLeft(f, 23);
            // Iteration 36
            f = (b ^ c ^ d) + a + 0xA4BEEA44 + m1;
            a = b + RotateLeft(f, 4);
            // Iteration 37
            f = (a ^ b ^ c) + d + 0x4BDECFA9 + m4;
            d = a + RotateLeft(f, 11);
            // Iteration 38
            f = (d ^ a ^ b) + c + 0xF6BB4B60 + m7;
            c = d + RotateLeft(f, 16);
            // Iteration 39
            f = (c ^ d ^ a) + b + 0xBEBFBC70 + ma;
            b = c + RotateLeft(f, 23);
            // Iteration 40
            f = (b ^ c ^ d) + a + 0x289B7EC6 + md;
            a = b + RotateLeft(f, 4);
            // Iteration 41
            f = (a ^ b ^ c) + d + 0xEAA127FA + m0;
            d = a + RotateLeft(f, 11);
            // Iteration 42
            f = (d ^ a ^ b) + c + 0xD4EF3085 + m3;
            c = d + RotateLeft(f, 16);
            // Iteration 43
            f = (c ^ d ^ a) + b + 0x04881D05 + m6;
            b = c + RotateLeft(f, 23);
            // Iteration 44
            f = (b ^ c ^ d) + a + 0xD9D4D039 + m9;
            a = b + RotateLeft(f, 4);
            // Iteration 45
            f = (a ^ b ^ c) + d + 0xE6DB99E5 + mc;
            d = a + RotateLeft(f, 11);
            // Iteration 46
            f = (d ^ a ^ b) + c + 0x1FA27CF8 + mf;
            c = d + RotateLeft(f, 16);
            // Iteration 47
            f = (c ^ d ^ a) + b + 0xC4AC5665 + m2;
            b = c + RotateLeft(f, 23);
            // Iteration 48
            f = (c ^ (b | ~d)) + a + 0xF4292244 + m0;
            a = b + RotateLeft(f, 6);
            // Iteration 49
            f = (b ^ (a | ~c)) + d + 0x432AFF97 + m7;
            d = a + RotateLeft(f, 10);
            // Iteration 50
            f = (a ^ (d | ~b)) + c + 0xAB9423A7 + me;
            c = d + RotateLeft(f, 15);
            // Iteration 51
            f = (d ^ (c | ~a)) + b + 0xFC93A039 + m5;
            b = c + RotateLeft(f, 21);
            // Iteration 52
            f = (c ^ (b | ~d)) + a + 0x655B59C3 + mc;
            a = b + RotateLeft(f, 6);
            // Iteration 53
            f = (b ^ (a | ~c)) + d + 0x8F0CCC92 + m3;
            d = a + RotateLeft(f, 10);
            // Iteration 54
            f = (a ^ (d | ~b)) + c + 0xFFEFF47D + ma;
            c = d + RotateLeft(f, 15);
            // Iteration 55
            f = (d ^ (c | ~a)) + b + 0x85845DD1 + m1;
            b = c + RotateLeft(f, 21);
            // Iteration 56
            f = (c ^ (b | ~d)) + a + 0x6FA87E4F + m8;
            a = b + RotateLeft(f, 6);
            // Iteration 57
            f = (b ^ (a | ~c)) + d + 0xFE2CE6E0 + mf;
            d = a + RotateLeft(f, 10);
            // Iteration 58
            f = (a ^ (d | ~b)) + c + 0xA3014314 + m6;
            c = d + RotateLeft(f, 15);
            // Iteration 59
            f = (d ^ (c | ~a)) + b + 0x4E0811A1 + md;
            b = c + RotateLeft(f, 21);
            // Iteration 60
            f = (c ^ (b | ~d)) + a + 0xF7537E82 + m4;
            a = b + RotateLeft(f, 6);
            // Iteration 61
            f = (b ^ (a | ~c)) + d + 0xBD3AF235 + mb;
            d = a + RotateLeft(f, 10);
            // Iteration 62
            f = (a ^ (d | ~b)) + c + 0x2AD7D2BB + m2;
            c = d + RotateLeft(f, 15);
            // Iteration 63
            f = (d ^ (c | ~a)) + b + 0xEB86D391 + m9;
            b = c + RotateLeft(f, 21);

            #endregion

            a0 = a0 + a;
            b0 = b0 + b;
            c0 = c0 + c;
            d0 = d0 + d;
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
            GetBytes(a0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(b0, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(c0, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(d0, ByteOrder.LittleEndian, digest, 0x0C);
            return digest;
        }
    }
}