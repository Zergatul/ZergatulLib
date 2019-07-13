using System;
using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Skein512x512 : Security.MessageDigest
    {
        public override int BlockLength => 64;
        public override int DigestLength => 64;

        private byte[] buffer;
        protected int bufOffset;
        protected ulong h0, h1, h2, h3, h4, h5, h6, h7;
        protected ulong blockCount;

        public Skein512x512()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            h0 = 0x4903ADFF749C51CE;
            h1 = 0x0D95DE399746DF03;
            h2 = 0x8FD1934127C79BCE;
            h3 = 0x9A255629FF352CB1;
            h4 = 0x5DB62599DF6CA7B0;
            h5 = 0xEABE394CA9D5C3F4;
            h6 = 0x991112C71A75B523;
            h7 = 0xAE18A40B660FCC33;

            bufOffset = 0;
            blockCount = 0;
        }

        public override void Update(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            while (length > 0)
            {
                if (bufOffset == 64)
                {
                    blockCount++;
                    ProcessBlock(0x60UL + (blockCount == 1 ? 0x80UL : 0UL), 0);
                    bufOffset = 0;
                }

                int copy = System.Math.Min(64 - bufOffset, length);
                Buffer.BlockCopy(data, offset, buffer, bufOffset, copy);

                offset += copy;
                length -= copy;
                bufOffset += copy;
            }
        }

        public override byte[] Digest()
        {
            for (int i = bufOffset; i < 64; i++)
                buffer[i] = 0;

            ProcessBlock(0x160UL + (blockCount == 0 ? 0x80UL : 0UL), (ulong)bufOffset);

            blockCount = 0;
            for (int i = 0; i < 64; i++)
                buffer[i] = 0;

            ProcessBlock(0x1FE, 8);

            return InternalStateToDigest();
        }

        protected virtual byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[64];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(h4, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(h5, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(h6, ByteOrder.LittleEndian, digest, 0x30);
            GetBytes(h7, ByteOrder.LittleEndian, digest, 0x38);
            return digest;
        }

        private void ProcessBlock(ulong etype, ulong extra)
        {
            ulong h0 = this.h0;
            ulong h1 = this.h1;
            ulong h2 = this.h2;
            ulong h3 = this.h3;
            ulong h4 = this.h4;
            ulong h5 = this.h5;
            ulong h6 = this.h6;
            ulong h7 = this.h7;
            ulong h8, t0, t1, t2;

            ulong m0 = ToUInt64(buffer, 0x00, ByteOrder.LittleEndian);
            ulong m1 = ToUInt64(buffer, 0x08, ByteOrder.LittleEndian);
            ulong m2 = ToUInt64(buffer, 0x10, ByteOrder.LittleEndian);
            ulong m3 = ToUInt64(buffer, 0x18, ByteOrder.LittleEndian);
            ulong m4 = ToUInt64(buffer, 0x20, ByteOrder.LittleEndian);
            ulong m5 = ToUInt64(buffer, 0x28, ByteOrder.LittleEndian);
            ulong m6 = ToUInt64(buffer, 0x30, ByteOrder.LittleEndian);
            ulong m7 = ToUInt64(buffer, 0x38, ByteOrder.LittleEndian);

            ulong p0 = m0;
            ulong p1 = m1;
            ulong p2 = m2;
            ulong p3 = m3;
            ulong p4 = m4;
            ulong p5 = m5;
            ulong p6 = m6;
            ulong p7 = m7;

            t0 = (blockCount << 6) + extra;
            t1 = (blockCount >> 58) + (etype << 55);

            #region Main

            h8 = h0 ^ h1 ^ h2 ^ h3 ^ h4 ^ h5 ^ h6 ^ h7 ^ 0x1BD11BDAA9FC1A22;
            t2 = t0 ^ t1;
            #region TFBIG_4e(0)
            p0 += h0;
            p1 += h1;
            p2 += h2;
            p3 += h3;
            p4 += h4;
            p5 += h5 + t0;
            p6 += h6 + t1;
            p7 += h7 + 0;
            p0 += p1;
            p1 = RotateLeft(p1, 46) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 36) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 19) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 37) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 33) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 27) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 14) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 42) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 17) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 49) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 36) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 39) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 44) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 9) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 54) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 56) ^ p4;
            #endregion
            #region TFBIG_4o(1)
            p0 += h1;
            p1 += h2;
            p2 += h3;
            p3 += h4;
            p4 += h5;
            p5 += h6 + t1;
            p6 += h7 + t2;
            p7 += h8 + 1;
            p0 += p1;
            p1 = RotateLeft(p1, 39) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 30) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 34) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 24) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 13) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 50) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 10) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 17) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 25) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 29) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 39) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 43) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 8) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 35) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 56) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 22) ^ p4;
            #endregion
            #region TFBIG_4e(2)
            p0 += h2;
            p1 += h3;
            p2 += h4;
            p3 += h5;
            p4 += h6;
            p5 += h7 + t2;
            p6 += h8 + t0;
            p7 += h0 + 2;
            p0 += p1;
            p1 = RotateLeft(p1, 46) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 36) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 19) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 37) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 33) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 27) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 14) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 42) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 17) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 49) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 36) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 39) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 44) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 9) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 54) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 56) ^ p4;
            #endregion
            #region TFBIG_4o(3)
            p0 += h3;
            p1 += h4;
            p2 += h5;
            p3 += h6;
            p4 += h7;
            p5 += h8 + t0;
            p6 += h0 + t1;
            p7 += h1 + 3;
            p0 += p1;
            p1 = RotateLeft(p1, 39) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 30) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 34) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 24) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 13) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 50) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 10) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 17) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 25) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 29) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 39) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 43) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 8) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 35) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 56) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 22) ^ p4;
            #endregion
            #region TFBIG_4e(4)
            p0 += h4;
            p1 += h5;
            p2 += h6;
            p3 += h7;
            p4 += h8;
            p5 += h0 + t1;
            p6 += h1 + t2;
            p7 += h2 + 4;
            p0 += p1;
            p1 = RotateLeft(p1, 46) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 36) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 19) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 37) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 33) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 27) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 14) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 42) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 17) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 49) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 36) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 39) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 44) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 9) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 54) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 56) ^ p4;
            #endregion
            #region TFBIG_4o(5)
            p0 += h5;
            p1 += h6;
            p2 += h7;
            p3 += h8;
            p4 += h0;
            p5 += h1 + t2;
            p6 += h2 + t0;
            p7 += h3 + 5;
            p0 += p1;
            p1 = RotateLeft(p1, 39) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 30) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 34) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 24) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 13) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 50) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 10) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 17) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 25) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 29) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 39) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 43) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 8) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 35) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 56) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 22) ^ p4;
            #endregion
            #region TFBIG_4e(6)
            p0 += h6;
            p1 += h7;
            p2 += h8;
            p3 += h0;
            p4 += h1;
            p5 += h2 + t0;
            p6 += h3 + t1;
            p7 += h4 + 6;
            p0 += p1;
            p1 = RotateLeft(p1, 46) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 36) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 19) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 37) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 33) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 27) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 14) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 42) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 17) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 49) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 36) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 39) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 44) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 9) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 54) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 56) ^ p4;
            #endregion
            #region TFBIG_4o(7)
            p0 += h7;
            p1 += h8;
            p2 += h0;
            p3 += h1;
            p4 += h2;
            p5 += h3 + t1;
            p6 += h4 + t2;
            p7 += h5 + 7;
            p0 += p1;
            p1 = RotateLeft(p1, 39) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 30) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 34) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 24) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 13) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 50) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 10) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 17) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 25) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 29) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 39) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 43) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 8) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 35) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 56) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 22) ^ p4;
            #endregion
            #region TFBIG_4e(8)
            p0 += h8;
            p1 += h0;
            p2 += h1;
            p3 += h2;
            p4 += h3;
            p5 += h4 + t2;
            p6 += h5 + t0;
            p7 += h6 + 8;
            p0 += p1;
            p1 = RotateLeft(p1, 46) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 36) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 19) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 37) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 33) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 27) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 14) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 42) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 17) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 49) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 36) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 39) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 44) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 9) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 54) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 56) ^ p4;
            #endregion
            #region TFBIG_4o(9)
            p0 += h0;
            p1 += h1;
            p2 += h2;
            p3 += h3;
            p4 += h4;
            p5 += h5 + t0;
            p6 += h6 + t1;
            p7 += h7 + 9;
            p0 += p1;
            p1 = RotateLeft(p1, 39) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 30) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 34) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 24) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 13) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 50) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 10) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 17) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 25) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 29) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 39) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 43) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 8) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 35) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 56) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 22) ^ p4;
            #endregion
            #region TFBIG_4e(10)
            p0 += h1;
            p1 += h2;
            p2 += h3;
            p3 += h4;
            p4 += h5;
            p5 += h6 + t1;
            p6 += h7 + t2;
            p7 += h8 + 10;
            p0 += p1;
            p1 = RotateLeft(p1, 46) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 36) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 19) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 37) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 33) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 27) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 14) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 42) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 17) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 49) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 36) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 39) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 44) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 9) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 54) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 56) ^ p4;
            #endregion
            #region TFBIG_4o(11)
            p0 += h2;
            p1 += h3;
            p2 += h4;
            p3 += h5;
            p4 += h6;
            p5 += h7 + t2;
            p6 += h8 + t0;
            p7 += h0 + 11;
            p0 += p1;
            p1 = RotateLeft(p1, 39) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 30) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 34) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 24) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 13) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 50) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 10) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 17) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 25) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 29) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 39) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 43) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 8) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 35) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 56) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 22) ^ p4;
            #endregion
            #region TFBIG_4e(12)
            p0 += h3;
            p1 += h4;
            p2 += h5;
            p3 += h6;
            p4 += h7;
            p5 += h8 + t0;
            p6 += h0 + t1;
            p7 += h1 + 12;
            p0 += p1;
            p1 = RotateLeft(p1, 46) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 36) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 19) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 37) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 33) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 27) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 14) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 42) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 17) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 49) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 36) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 39) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 44) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 9) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 54) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 56) ^ p4;
            #endregion
            #region TFBIG_4o(13)
            p0 += h4;
            p1 += h5;
            p2 += h6;
            p3 += h7;
            p4 += h8;
            p5 += h0 + t1;
            p6 += h1 + t2;
            p7 += h2 + 13;
            p0 += p1;
            p1 = RotateLeft(p1, 39) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 30) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 34) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 24) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 13) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 50) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 10) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 17) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 25) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 29) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 39) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 43) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 8) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 35) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 56) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 22) ^ p4;
            #endregion
            #region TFBIG_4e(14)
            p0 += h5;
            p1 += h6;
            p2 += h7;
            p3 += h8;
            p4 += h0;
            p5 += h1 + t2;
            p6 += h2 + t0;
            p7 += h3 + 14;
            p0 += p1;
            p1 = RotateLeft(p1, 46) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 36) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 19) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 37) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 33) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 27) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 14) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 42) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 17) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 49) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 36) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 39) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 44) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 9) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 54) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 56) ^ p4;
            #endregion
            #region TFBIG_4o(15)
            p0 += h6;
            p1 += h7;
            p2 += h8;
            p3 += h0;
            p4 += h1;
            p5 += h2 + t0;
            p6 += h3 + t1;
            p7 += h4 + 15;
            p0 += p1;
            p1 = RotateLeft(p1, 39) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 30) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 34) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 24) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 13) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 50) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 10) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 17) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 25) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 29) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 39) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 43) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 8) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 35) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 56) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 22) ^ p4;
            #endregion
            #region TFBIG_4e(16)
            p0 += h7;
            p1 += h8;
            p2 += h0;
            p3 += h1;
            p4 += h2;
            p5 += h3 + t1;
            p6 += h4 + t2;
            p7 += h5 + 16;
            p0 += p1;
            p1 = RotateLeft(p1, 46) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 36) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 19) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 37) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 33) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 27) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 14) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 42) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 17) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 49) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 36) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 39) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 44) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 9) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 54) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 56) ^ p4;
            #endregion
            #region TFBIG_4o(17)
            p0 += h8;
            p1 += h0;
            p2 += h1;
            p3 += h2;
            p4 += h3;
            p5 += h4 + t2;
            p6 += h5 + t0;
            p7 += h6 + 17;
            p0 += p1;
            p1 = RotateLeft(p1, 39) ^ p0;
            p2 += p3;
            p3 = RotateLeft(p3, 30) ^ p2;
            p4 += p5;
            p5 = RotateLeft(p5, 34) ^ p4;
            p6 += p7;
            p7 = RotateLeft(p7, 24) ^ p6;
            p2 += p1;
            p1 = RotateLeft(p1, 13) ^ p2;
            p4 += p7;
            p7 = RotateLeft(p7, 50) ^ p4;
            p6 += p5;
            p5 = RotateLeft(p5, 10) ^ p6;
            p0 += p3;
            p3 = RotateLeft(p3, 17) ^ p0;
            p4 += p1;
            p1 = RotateLeft(p1, 25) ^ p4;
            p6 += p3;
            p3 = RotateLeft(p3, 29) ^ p6;
            p0 += p5;
            p5 = RotateLeft(p5, 39) ^ p0;
            p2 += p7;
            p7 = RotateLeft(p7, 43) ^ p2;
            p6 += p1;
            p1 = RotateLeft(p1, 8) ^ p6;
            p0 += p7;
            p7 = RotateLeft(p7, 35) ^ p0;
            p2 += p5;
            p5 = RotateLeft(p5, 56) ^ p2;
            p4 += p3;
            p3 = RotateLeft(p3, 22) ^ p4;
            #endregion
            p0 += h0;
            p1 += h1;
            p2 += h2;
            p3 += h3;
            p4 += h4;
            p5 += h5 + t0;
            p6 += h6 + t1;
            p7 += h7 + 18;

            #endregion

            this.h0 = m0 ^ p0;
            this.h1 = m1 ^ p1;
            this.h2 = m2 ^ p2;
            this.h3 = m3 ^ p3;
            this.h4 = m4 ^ p4;
            this.h5 = m5 ^ p5;
            this.h6 = m6 ^ p6;
            this.h7 = m7 ^ p7;
        }
    }
}