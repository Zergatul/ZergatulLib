using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.GroestlConstants;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Groestl256 : AbstractMessageDigest
    {
        public override int BlockLength => 64;
        public override int DigestLength => 32;

        protected ulong s0, s1, s2, s3, s4, s5, s6, s7;
        protected ulong blocks;

        public Groestl256()
        {
            buffer = new byte[64];
            Reset();
        }

        public override void Reset()
        {
            s0 = s1 = s2 = s3 = s4 = s5 = s6 = 0;
            s7 = 0x0001000000000000;

            bufOffset = 0;
            blocks = 0;
        }

        public override byte[] Digest()
        {
            buffer[bufOffset++] = 0x80;

            if (bufOffset > 56)
            {
                while (bufOffset < 64)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
            }

            blocks++;
            while (bufOffset <= 56)
                buffer[bufOffset++] = 0;
            GetBytes(blocks, ByteOrder.BigEndian, buffer, 56);
            ProcessBlock();

            ulong x0 = s0;
            ulong x1 = s1;
            ulong x2 = s2;
            ulong x3 = s3;
            ulong x4 = s4;
            ulong x5 = s5;
            ulong x6 = s6;
            ulong x7 = s7;

            #region P(x)

            for (ulong r = 0; r < 10; r++)
            {
                x0 ^= 0x00 + r;
                x1 ^= 0x10 + r;
                x2 ^= 0x20 + r;
                x3 ^= 0x30 + r;
                x4 ^= 0x40 + r;
                x5 ^= 0x50 + r;
                x6 ^= 0x60 + r;
                x7 ^= 0x70 + r;
                ulong t0 =
                    T0[(x0 >> 0x00) & 0xFF] ^ T1[(x1 >> 0x08) & 0xFF] ^ T2[(x2 >> 0x10) & 0xFF] ^ T3[(x3 >> 0x18) & 0xFF] ^
                    T4[(x4 >> 0x20) & 0xFF] ^ T5[(x5 >> 0x28) & 0xFF] ^ T6[(x6 >> 0x30) & 0xFF] ^ T7[(x7 >> 0x38) & 0xFF];
                ulong t1 =
                    T0[(x1 >> 0x00) & 0xFF] ^ T1[(x2 >> 0x08) & 0xFF] ^ T2[(x3 >> 0x10) & 0xFF] ^ T3[(x4 >> 0x18) & 0xFF] ^
                    T4[(x5 >> 0x20) & 0xFF] ^ T5[(x6 >> 0x28) & 0xFF] ^ T6[(x7 >> 0x30) & 0xFF] ^ T7[(x0 >> 0x38) & 0xFF];
                ulong t2 =
                    T0[(x2 >> 0x00) & 0xFF] ^ T1[(x3 >> 0x08) & 0xFF] ^ T2[(x4 >> 0x10) & 0xFF] ^ T3[(x5 >> 0x18) & 0xFF] ^
                    T4[(x6 >> 0x20) & 0xFF] ^ T5[(x7 >> 0x28) & 0xFF] ^ T6[(x0 >> 0x30) & 0xFF] ^ T7[(x1 >> 0x38) & 0xFF];
                ulong t3 =
                    T0[(x3 >> 0x00) & 0xFF] ^ T1[(x4 >> 0x08) & 0xFF] ^ T2[(x5 >> 0x10) & 0xFF] ^ T3[(x6 >> 0x18) & 0xFF] ^
                    T4[(x7 >> 0x20) & 0xFF] ^ T5[(x0 >> 0x28) & 0xFF] ^ T6[(x1 >> 0x30) & 0xFF] ^ T7[(x2 >> 0x38) & 0xFF];
                ulong t4 =
                    T0[(x4 >> 0x00) & 0xFF] ^ T1[(x5 >> 0x08) & 0xFF] ^ T2[(x6 >> 0x10) & 0xFF] ^ T3[(x7 >> 0x18) & 0xFF] ^
                    T4[(x0 >> 0x20) & 0xFF] ^ T5[(x1 >> 0x28) & 0xFF] ^ T6[(x2 >> 0x30) & 0xFF] ^ T7[(x3 >> 0x38) & 0xFF];
                ulong t5 =
                    T0[(x5 >> 0x00) & 0xFF] ^ T1[(x6 >> 0x08) & 0xFF] ^ T2[(x7 >> 0x10) & 0xFF] ^ T3[(x0 >> 0x18) & 0xFF] ^
                    T4[(x1 >> 0x20) & 0xFF] ^ T5[(x2 >> 0x28) & 0xFF] ^ T6[(x3 >> 0x30) & 0xFF] ^ T7[(x4 >> 0x38) & 0xFF];
                ulong t6 =
                    T0[(x6 >> 0x00) & 0xFF] ^ T1[(x7 >> 0x08) & 0xFF] ^ T2[(x0 >> 0x10) & 0xFF] ^ T3[(x1 >> 0x18) & 0xFF] ^
                    T4[(x2 >> 0x20) & 0xFF] ^ T5[(x3 >> 0x28) & 0xFF] ^ T6[(x4 >> 0x30) & 0xFF] ^ T7[(x5 >> 0x38) & 0xFF];
                ulong t7 =
                    T0[(x7 >> 0x00) & 0xFF] ^ T1[(x0 >> 0x08) & 0xFF] ^ T2[(x1 >> 0x10) & 0xFF] ^ T3[(x2 >> 0x18) & 0xFF] ^
                    T4[(x3 >> 0x20) & 0xFF] ^ T5[(x4 >> 0x28) & 0xFF] ^ T6[(x5 >> 0x30) & 0xFF] ^ T7[(x6 >> 0x38) & 0xFF];
                x0 = t0;
                x1 = t1;
                x2 = t2;
                x3 = t3;
                x4 = t4;
                x5 = t5;
                x6 = t6;
                x7 = t7;
            }

            #endregion

            s0 ^= x0;
            s1 ^= x1;
            s2 ^= x2;
            s3 ^= x3;
            s4 ^= x4;
            s5 ^= x5;
            s6 ^= x6;
            s7 ^= x7;

            return InternalStateToDigest();
        }

        protected override void ProcessBlock()
        {
            ulong m0 = ToUInt64(buffer, 0x00, ByteOrder.LittleEndian);
            ulong m1 = ToUInt64(buffer, 0x08, ByteOrder.LittleEndian);
            ulong m2 = ToUInt64(buffer, 0x10, ByteOrder.LittleEndian);
            ulong m3 = ToUInt64(buffer, 0x18, ByteOrder.LittleEndian);
            ulong m4 = ToUInt64(buffer, 0x20, ByteOrder.LittleEndian);
            ulong m5 = ToUInt64(buffer, 0x28, ByteOrder.LittleEndian);
            ulong m6 = ToUInt64(buffer, 0x30, ByteOrder.LittleEndian);
            ulong m7 = ToUInt64(buffer, 0x38, ByteOrder.LittleEndian);

            ulong g0 = m0 ^ s0;
            ulong g1 = m1 ^ s1;
            ulong g2 = m2 ^ s2;
            ulong g3 = m3 ^ s3;
            ulong g4 = m4 ^ s4;
            ulong g5 = m5 ^ s5;
            ulong g6 = m6 ^ s6;
            ulong g7 = m7 ^ s7;

            #region P(g)

            for (ulong r = 0; r < 10; r++)
            {
                g0 ^= 0x00 + r;
                g1 ^= 0x10 + r;
                g2 ^= 0x20 + r;
                g3 ^= 0x30 + r;
                g4 ^= 0x40 + r;
                g5 ^= 0x50 + r;
                g6 ^= 0x60 + r;
                g7 ^= 0x70 + r;
                ulong t0 =
                    T0[(g0 >> 0x00) & 0xFF] ^ T1[(g1 >> 0x08) & 0xFF] ^ T2[(g2 >> 0x10) & 0xFF] ^ T3[(g3 >> 0x18) & 0xFF] ^
                    T4[(g4 >> 0x20) & 0xFF] ^ T5[(g5 >> 0x28) & 0xFF] ^ T6[(g6 >> 0x30) & 0xFF] ^ T7[(g7 >> 0x38) & 0xFF];
                ulong t1 =
                    T0[(g1 >> 0x00) & 0xFF] ^ T1[(g2 >> 0x08) & 0xFF] ^ T2[(g3 >> 0x10) & 0xFF] ^ T3[(g4 >> 0x18) & 0xFF] ^
                    T4[(g5 >> 0x20) & 0xFF] ^ T5[(g6 >> 0x28) & 0xFF] ^ T6[(g7 >> 0x30) & 0xFF] ^ T7[(g0 >> 0x38) & 0xFF];
                ulong t2 =
                    T0[(g2 >> 0x00) & 0xFF] ^ T1[(g3 >> 0x08) & 0xFF] ^ T2[(g4 >> 0x10) & 0xFF] ^ T3[(g5 >> 0x18) & 0xFF] ^
                    T4[(g6 >> 0x20) & 0xFF] ^ T5[(g7 >> 0x28) & 0xFF] ^ T6[(g0 >> 0x30) & 0xFF] ^ T7[(g1 >> 0x38) & 0xFF];
                ulong t3 =
                    T0[(g3 >> 0x00) & 0xFF] ^ T1[(g4 >> 0x08) & 0xFF] ^ T2[(g5 >> 0x10) & 0xFF] ^ T3[(g6 >> 0x18) & 0xFF] ^
                    T4[(g7 >> 0x20) & 0xFF] ^ T5[(g0 >> 0x28) & 0xFF] ^ T6[(g1 >> 0x30) & 0xFF] ^ T7[(g2 >> 0x38) & 0xFF];
                ulong t4 =
                    T0[(g4 >> 0x00) & 0xFF] ^ T1[(g5 >> 0x08) & 0xFF] ^ T2[(g6 >> 0x10) & 0xFF] ^ T3[(g7 >> 0x18) & 0xFF] ^
                    T4[(g0 >> 0x20) & 0xFF] ^ T5[(g1 >> 0x28) & 0xFF] ^ T6[(g2 >> 0x30) & 0xFF] ^ T7[(g3 >> 0x38) & 0xFF];
                ulong t5 =
                    T0[(g5 >> 0x00) & 0xFF] ^ T1[(g6 >> 0x08) & 0xFF] ^ T2[(g7 >> 0x10) & 0xFF] ^ T3[(g0 >> 0x18) & 0xFF] ^
                    T4[(g1 >> 0x20) & 0xFF] ^ T5[(g2 >> 0x28) & 0xFF] ^ T6[(g3 >> 0x30) & 0xFF] ^ T7[(g4 >> 0x38) & 0xFF];
                ulong t6 =
                    T0[(g6 >> 0x00) & 0xFF] ^ T1[(g7 >> 0x08) & 0xFF] ^ T2[(g0 >> 0x10) & 0xFF] ^ T3[(g1 >> 0x18) & 0xFF] ^
                    T4[(g2 >> 0x20) & 0xFF] ^ T5[(g3 >> 0x28) & 0xFF] ^ T6[(g4 >> 0x30) & 0xFF] ^ T7[(g5 >> 0x38) & 0xFF];
                ulong t7 =
                    T0[(g7 >> 0x00) & 0xFF] ^ T1[(g0 >> 0x08) & 0xFF] ^ T2[(g1 >> 0x10) & 0xFF] ^ T3[(g2 >> 0x18) & 0xFF] ^
                    T4[(g3 >> 0x20) & 0xFF] ^ T5[(g4 >> 0x28) & 0xFF] ^ T6[(g5 >> 0x30) & 0xFF] ^ T7[(g6 >> 0x38) & 0xFF];
                g0 = t0;
                g1 = t1;
                g2 = t2;
                g3 = t3;
                g4 = t4;
                g5 = t5;
                g6 = t6;
                g7 = t7;
            }

            #endregion

            #region Q(m)

            for (ulong r = 0; r < 10; r++)
            {
                m0 ^= (r << 56) ^ 0xFFFFFFFFFFFFFFFF;
                m1 ^= (r << 56) ^ 0xEFFFFFFFFFFFFFFF;
                m2 ^= (r << 56) ^ 0xDFFFFFFFFFFFFFFF;
                m3 ^= (r << 56) ^ 0xCFFFFFFFFFFFFFFF;
                m4 ^= (r << 56) ^ 0xBFFFFFFFFFFFFFFF;
                m5 ^= (r << 56) ^ 0xAFFFFFFFFFFFFFFF;
                m6 ^= (r << 56) ^ 0x9FFFFFFFFFFFFFFF;
                m7 ^= (r << 56) ^ 0x8FFFFFFFFFFFFFFF;
                ulong t0 =
                    T0[(m1 >> 0x00) & 0xFF] ^ T1[(m3 >> 0x08) & 0xFF] ^ T2[(m5 >> 0x10) & 0xFF] ^ T3[(m7 >> 0x18) & 0xFF] ^
                    T4[(m0 >> 0x20) & 0xFF] ^ T5[(m2 >> 0x28) & 0xFF] ^ T6[(m4 >> 0x30) & 0xFF] ^ T7[(m6 >> 0x38) & 0xFF];
                ulong t1 =
                    T0[(m2 >> 0x00) & 0xFF] ^ T1[(m4 >> 0x08) & 0xFF] ^ T2[(m6 >> 0x10) & 0xFF] ^ T3[(m0 >> 0x18) & 0xFF] ^
                    T4[(m1 >> 0x20) & 0xFF] ^ T5[(m3 >> 0x28) & 0xFF] ^ T6[(m5 >> 0x30) & 0xFF] ^ T7[(m7 >> 0x38) & 0xFF];
                ulong t2 =
                    T0[(m3 >> 0x00) & 0xFF] ^ T1[(m5 >> 0x08) & 0xFF] ^ T2[(m7 >> 0x10) & 0xFF] ^ T3[(m1 >> 0x18) & 0xFF] ^
                    T4[(m2 >> 0x20) & 0xFF] ^ T5[(m4 >> 0x28) & 0xFF] ^ T6[(m6 >> 0x30) & 0xFF] ^ T7[(m0 >> 0x38) & 0xFF];
                ulong t3 =
                    T0[(m4 >> 0x00) & 0xFF] ^ T1[(m6 >> 0x08) & 0xFF] ^ T2[(m0 >> 0x10) & 0xFF] ^ T3[(m2 >> 0x18) & 0xFF] ^
                    T4[(m3 >> 0x20) & 0xFF] ^ T5[(m5 >> 0x28) & 0xFF] ^ T6[(m7 >> 0x30) & 0xFF] ^ T7[(m1 >> 0x38) & 0xFF];
                ulong t4 =
                    T0[(m5 >> 0x00) & 0xFF] ^ T1[(m7 >> 0x08) & 0xFF] ^ T2[(m1 >> 0x10) & 0xFF] ^ T3[(m3 >> 0x18) & 0xFF] ^
                    T4[(m4 >> 0x20) & 0xFF] ^ T5[(m6 >> 0x28) & 0xFF] ^ T6[(m0 >> 0x30) & 0xFF] ^ T7[(m2 >> 0x38) & 0xFF];
                ulong t5 =
                    T0[(m6 >> 0x00) & 0xFF] ^ T1[(m0 >> 0x08) & 0xFF] ^ T2[(m2 >> 0x10) & 0xFF] ^ T3[(m4 >> 0x18) & 0xFF] ^
                    T4[(m5 >> 0x20) & 0xFF] ^ T5[(m7 >> 0x28) & 0xFF] ^ T6[(m1 >> 0x30) & 0xFF] ^ T7[(m3 >> 0x38) & 0xFF];
                ulong t6 =
                    T0[(m7 >> 0x00) & 0xFF] ^ T1[(m1 >> 0x08) & 0xFF] ^ T2[(m3 >> 0x10) & 0xFF] ^ T3[(m5 >> 0x18) & 0xFF] ^
                    T4[(m6 >> 0x20) & 0xFF] ^ T5[(m0 >> 0x28) & 0xFF] ^ T6[(m2 >> 0x30) & 0xFF] ^ T7[(m4 >> 0x38) & 0xFF];
                ulong t7 =
                    T0[(m0 >> 0x00) & 0xFF] ^ T1[(m2 >> 0x08) & 0xFF] ^ T2[(m4 >> 0x10) & 0xFF] ^ T3[(m6 >> 0x18) & 0xFF] ^
                    T4[(m7 >> 0x20) & 0xFF] ^ T5[(m1 >> 0x28) & 0xFF] ^ T6[(m3 >> 0x30) & 0xFF] ^ T7[(m5 >> 0x38) & 0xFF];
                m0 = t0;
                m1 = t1;
                m2 = t2;
                m3 = t3;
                m4 = t4;
                m5 = t5;
                m6 = t6;
                m7 = t7;
            }

            #endregion

            s0 ^= g0 ^ m0;
            s1 ^= g1 ^ m1;
            s2 ^= g2 ^ m2;
            s3 ^= g3 ^ m3;
            s4 ^= g4 ^ m4;
            s5 ^= g5 ^ m5;
            s6 ^= g6 ^ m6;
            s7 ^= g7 ^ m7;

            blocks++;
        }

        protected virtual byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[32];
            GetBytes(s4, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s5, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s6, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(s7, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}