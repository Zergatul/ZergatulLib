using System;
using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.WhirlpoolConstants;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Whirlpool : Security.MessageDigest
    {
        public override int BlockLength => 64;
        public override int DigestLength => 64;

        private byte[] buffer;
        private int bufOffset;
        private ulong s0, s1, s2, s3, s4, s5, s6, s7;
        private ulong count;

        public Whirlpool()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            s0 = s1 = s2 = s3 = s4 = s5 = s6 = s7 = 0;

            bufOffset = 0;
            count = 0;
        }

        public override void Update(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            count += (ulong)length;

            while (offset < length)
            {
                int copy = System.Math.Min(64 - bufOffset, length);
                Buffer.BlockCopy(data, offset, buffer, bufOffset, copy);

                offset += copy;
                length -= copy;
                bufOffset += copy;

                if (bufOffset == 64)
                {
                    bufOffset = 0;
                    ProcessBlock();
                }
            }
        }

        public override byte[] Digest()
        {
            buffer[bufOffset++] = 0x80;
            if (bufOffset >= 32)
            {
                while (bufOffset < 64)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
            }
            while (bufOffset < 32)
                buffer[bufOffset++] = 0;

            // don't support long messages, fill with zero
            while (bufOffset < 56)
                buffer[bufOffset++] = 0;
            GetBytes(count << 3, ByteOrder.BigEndian, buffer, 56);

            ProcessBlock();

            byte[] digest = new byte[64];
            GetBytes(s0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s1, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s2, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(s3, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(s4, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(s5, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(s6, ByteOrder.LittleEndian, digest, 0x30);
            GetBytes(s7, ByteOrder.LittleEndian, digest, 0x38);
            return digest;
        }

        private void ProcessBlock()
        {
            ulong n0 = ToUInt64(buffer, 0x00, ByteOrder.LittleEndian);
            ulong n1 = ToUInt64(buffer, 0x08, ByteOrder.LittleEndian);
            ulong n2 = ToUInt64(buffer, 0x10, ByteOrder.LittleEndian);
            ulong n3 = ToUInt64(buffer, 0x18, ByteOrder.LittleEndian);
            ulong n4 = ToUInt64(buffer, 0x20, ByteOrder.LittleEndian);
            ulong n5 = ToUInt64(buffer, 0x28, ByteOrder.LittleEndian);
            ulong n6 = ToUInt64(buffer, 0x30, ByteOrder.LittleEndian);
            ulong n7 = ToUInt64(buffer, 0x38, ByteOrder.LittleEndian);

            ulong h0 = s0;
            ulong h1 = s1;
            ulong h2 = s2;
            ulong h3 = s3;
            ulong h4 = s4;
            ulong h5 = s5;
            ulong h6 = s6;
            ulong h7 = s7;

            n0 ^= h0;
            n1 ^= h1;
            n2 ^= h2;
            n3 ^= h3;
            n4 ^= h4;
            n5 ^= h5;
            n6 ^= h6;
            n7 ^= h7;

            for (int r = 0; r < 10; r++)
            {
                ulong t0, t1, t2, t3, t4, t5, t6, t7;

                t0 = T0[(h0 >> 0) & 0xFF] ^ T1[(h7 >> 8) & 0xFF] ^ T2[(h6 >> 16) & 0xFF] ^ T3[(h5 >> 24) & 0xFF] ^ T4[(h4 >> 32) & 0xFF] ^ T5[(h3 >> 40) & 0xFF] ^ T6[(h2 >> 48) & 0xFF] ^ T7[(h1 >> 56) & 0xFF] ^ RC[r];
                t1 = T0[(h1 >> 0) & 0xFF] ^ T1[(h0 >> 8) & 0xFF] ^ T2[(h7 >> 16) & 0xFF] ^ T3[(h6 >> 24) & 0xFF] ^ T4[(h5 >> 32) & 0xFF] ^ T5[(h4 >> 40) & 0xFF] ^ T6[(h3 >> 48) & 0xFF] ^ T7[(h2 >> 56) & 0xFF] ^ 0;
                t2 = T0[(h2 >> 0) & 0xFF] ^ T1[(h1 >> 8) & 0xFF] ^ T2[(h0 >> 16) & 0xFF] ^ T3[(h7 >> 24) & 0xFF] ^ T4[(h6 >> 32) & 0xFF] ^ T5[(h5 >> 40) & 0xFF] ^ T6[(h4 >> 48) & 0xFF] ^ T7[(h3 >> 56) & 0xFF] ^ 0;
                t3 = T0[(h3 >> 0) & 0xFF] ^ T1[(h2 >> 8) & 0xFF] ^ T2[(h1 >> 16) & 0xFF] ^ T3[(h0 >> 24) & 0xFF] ^ T4[(h7 >> 32) & 0xFF] ^ T5[(h6 >> 40) & 0xFF] ^ T6[(h5 >> 48) & 0xFF] ^ T7[(h4 >> 56) & 0xFF] ^ 0;
                t4 = T0[(h4 >> 0) & 0xFF] ^ T1[(h3 >> 8) & 0xFF] ^ T2[(h2 >> 16) & 0xFF] ^ T3[(h1 >> 24) & 0xFF] ^ T4[(h0 >> 32) & 0xFF] ^ T5[(h7 >> 40) & 0xFF] ^ T6[(h6 >> 48) & 0xFF] ^ T7[(h5 >> 56) & 0xFF] ^ 0;
                t5 = T0[(h5 >> 0) & 0xFF] ^ T1[(h4 >> 8) & 0xFF] ^ T2[(h3 >> 16) & 0xFF] ^ T3[(h2 >> 24) & 0xFF] ^ T4[(h1 >> 32) & 0xFF] ^ T5[(h0 >> 40) & 0xFF] ^ T6[(h7 >> 48) & 0xFF] ^ T7[(h6 >> 56) & 0xFF] ^ 0;
                t6 = T0[(h6 >> 0) & 0xFF] ^ T1[(h5 >> 8) & 0xFF] ^ T2[(h4 >> 16) & 0xFF] ^ T3[(h3 >> 24) & 0xFF] ^ T4[(h2 >> 32) & 0xFF] ^ T5[(h1 >> 40) & 0xFF] ^ T6[(h0 >> 48) & 0xFF] ^ T7[(h7 >> 56) & 0xFF] ^ 0;
                t7 = T0[(h7 >> 0) & 0xFF] ^ T1[(h6 >> 8) & 0xFF] ^ T2[(h5 >> 16) & 0xFF] ^ T3[(h4 >> 24) & 0xFF] ^ T4[(h3 >> 32) & 0xFF] ^ T5[(h2 >> 40) & 0xFF] ^ T6[(h1 >> 48) & 0xFF] ^ T7[(h0 >> 56) & 0xFF] ^ 0;

                h0 = t0;
                h1 = t1;
                h2 = t2;
                h3 = t3;
                h4 = t4;
                h5 = t5;
                h6 = t6;
                h7 = t7;

                t0 = T0[(n0 >> 0) & 0xFF] ^ T1[(n7 >> 8) & 0xFF] ^ T2[(n6 >> 16) & 0xFF] ^ T3[(n5 >> 24) & 0xFF] ^ T4[(n4 >> 32) & 0xFF] ^ T5[(n3 >> 40) & 0xFF] ^ T6[(n2 >> 48) & 0xFF] ^ T7[(n1 >> 56) & 0xFF] ^ h0;
                t1 = T0[(n1 >> 0) & 0xFF] ^ T1[(n0 >> 8) & 0xFF] ^ T2[(n7 >> 16) & 0xFF] ^ T3[(n6 >> 24) & 0xFF] ^ T4[(n5 >> 32) & 0xFF] ^ T5[(n4 >> 40) & 0xFF] ^ T6[(n3 >> 48) & 0xFF] ^ T7[(n2 >> 56) & 0xFF] ^ h1;
                t2 = T0[(n2 >> 0) & 0xFF] ^ T1[(n1 >> 8) & 0xFF] ^ T2[(n0 >> 16) & 0xFF] ^ T3[(n7 >> 24) & 0xFF] ^ T4[(n6 >> 32) & 0xFF] ^ T5[(n5 >> 40) & 0xFF] ^ T6[(n4 >> 48) & 0xFF] ^ T7[(n3 >> 56) & 0xFF] ^ h2;
                t3 = T0[(n3 >> 0) & 0xFF] ^ T1[(n2 >> 8) & 0xFF] ^ T2[(n1 >> 16) & 0xFF] ^ T3[(n0 >> 24) & 0xFF] ^ T4[(n7 >> 32) & 0xFF] ^ T5[(n6 >> 40) & 0xFF] ^ T6[(n5 >> 48) & 0xFF] ^ T7[(n4 >> 56) & 0xFF] ^ h3;
                t4 = T0[(n4 >> 0) & 0xFF] ^ T1[(n3 >> 8) & 0xFF] ^ T2[(n2 >> 16) & 0xFF] ^ T3[(n1 >> 24) & 0xFF] ^ T4[(n0 >> 32) & 0xFF] ^ T5[(n7 >> 40) & 0xFF] ^ T6[(n6 >> 48) & 0xFF] ^ T7[(n5 >> 56) & 0xFF] ^ h4;
                t5 = T0[(n5 >> 0) & 0xFF] ^ T1[(n4 >> 8) & 0xFF] ^ T2[(n3 >> 16) & 0xFF] ^ T3[(n2 >> 24) & 0xFF] ^ T4[(n1 >> 32) & 0xFF] ^ T5[(n0 >> 40) & 0xFF] ^ T6[(n7 >> 48) & 0xFF] ^ T7[(n6 >> 56) & 0xFF] ^ h5;
                t6 = T0[(n6 >> 0) & 0xFF] ^ T1[(n5 >> 8) & 0xFF] ^ T2[(n4 >> 16) & 0xFF] ^ T3[(n3 >> 24) & 0xFF] ^ T4[(n2 >> 32) & 0xFF] ^ T5[(n1 >> 40) & 0xFF] ^ T6[(n0 >> 48) & 0xFF] ^ T7[(n7 >> 56) & 0xFF] ^ h6;
                t7 = T0[(n7 >> 0) & 0xFF] ^ T1[(n6 >> 8) & 0xFF] ^ T2[(n5 >> 16) & 0xFF] ^ T3[(n4 >> 24) & 0xFF] ^ T4[(n3 >> 32) & 0xFF] ^ T5[(n2 >> 40) & 0xFF] ^ T6[(n1 >> 48) & 0xFF] ^ T7[(n0 >> 56) & 0xFF] ^ h7;

                n0 = t0;
                n1 = t1;
                n2 = t2;
                n3 = t3;
                n4 = t4;
                n5 = t5;
                n6 = t6;
                n7 = t7;
            }

            s0 ^= n0 ^ ToUInt64(buffer, 0x00, ByteOrder.LittleEndian);
            s1 ^= n1 ^ ToUInt64(buffer, 0x08, ByteOrder.LittleEndian);
            s2 ^= n2 ^ ToUInt64(buffer, 0x10, ByteOrder.LittleEndian);
            s3 ^= n3 ^ ToUInt64(buffer, 0x18, ByteOrder.LittleEndian);
            s4 ^= n4 ^ ToUInt64(buffer, 0x20, ByteOrder.LittleEndian);
            s5 ^= n5 ^ ToUInt64(buffer, 0x28, ByteOrder.LittleEndian);
            s6 ^= n6 ^ ToUInt64(buffer, 0x30, ByteOrder.LittleEndian);
            s7 ^= n7 ^ ToUInt64(buffer, 0x38, ByteOrder.LittleEndian);
        }
    }
}