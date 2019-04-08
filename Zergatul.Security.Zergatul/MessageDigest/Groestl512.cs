using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.GroestlConstants;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Groestl512 : AbstractMessageDigest
    {
        public override int BlockLength => 128;
        public override int DigestLength => 64;

        protected ulong s0, s1, s2, s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf;
        protected ulong blocks;

        public Groestl512()
        {
            buffer = new byte[128];
            Reset();
        }

        public override void Reset()
        {
            s0 = s1 = s2 = s3 = s4 = s5 = s6 = s7 = s8 = s9 = sa = sb = sc = sd = se = 0;
            sf = 0x0002000000000000;

            bufOffset = 0;
            blocks = 0;
        }

        public override byte[] Digest()
        {
            buffer[bufOffset++] = 0x80;

            if (bufOffset > 120)
            {
                while (bufOffset < 128)
                    buffer[bufOffset++] = 0;
                bufOffset = 0;
                ProcessBlock();
            }

            blocks++;
            while (bufOffset <= 120)
                buffer[bufOffset++] = 0;
            GetBytes(blocks, ByteOrder.BigEndian, buffer, 120);
            ProcessBlock();

            ulong x0 = s0;
            ulong x1 = s1;
            ulong x2 = s2;
            ulong x3 = s3;
            ulong x4 = s4;
            ulong x5 = s5;
            ulong x6 = s6;
            ulong x7 = s7;
            ulong x8 = s8;
            ulong x9 = s9;
            ulong xa = sa;
            ulong xb = sb;
            ulong xc = sc;
            ulong xd = sd;
            ulong xe = se;
            ulong xf = sf;

            #region P(x)

            for (ulong r = 0; r < 14; r++)
            {
                x0 ^= 0x00 + r;
                x1 ^= 0x10 + r;
                x2 ^= 0x20 + r;
                x3 ^= 0x30 + r;
                x4 ^= 0x40 + r;
                x5 ^= 0x50 + r;
                x6 ^= 0x60 + r;
                x7 ^= 0x70 + r;
                x8 ^= 0x80 + r;
                x9 ^= 0x90 + r;
                xa ^= 0xA0 + r;
                xb ^= 0xB0 + r;
                xc ^= 0xC0 + r;
                xd ^= 0xD0 + r;
                xe ^= 0xE0 + r;
                xf ^= 0xF0 + r;
                ulong t0 =
                    T0[(x0 >> 0x00) & 0xFF] ^ T1[(x1 >> 0x08) & 0xFF] ^ T2[(x2 >> 0x10) & 0xFF] ^ T3[(x3 >> 0x18) & 0xFF] ^
                    T4[(x4 >> 0x20) & 0xFF] ^ T5[(x5 >> 0x28) & 0xFF] ^ T6[(x6 >> 0x30) & 0xFF] ^ T7[(xb >> 0x38) & 0xFF];
                ulong t1 =
                    T0[(x1 >> 0x00) & 0xFF] ^ T1[(x2 >> 0x08) & 0xFF] ^ T2[(x3 >> 0x10) & 0xFF] ^ T3[(x4 >> 0x18) & 0xFF] ^
                    T4[(x5 >> 0x20) & 0xFF] ^ T5[(x6 >> 0x28) & 0xFF] ^ T6[(x7 >> 0x30) & 0xFF] ^ T7[(xc >> 0x38) & 0xFF];
                ulong t2 =
                    T0[(x2 >> 0x00) & 0xFF] ^ T1[(x3 >> 0x08) & 0xFF] ^ T2[(x4 >> 0x10) & 0xFF] ^ T3[(x5 >> 0x18) & 0xFF] ^
                    T4[(x6 >> 0x20) & 0xFF] ^ T5[(x7 >> 0x28) & 0xFF] ^ T6[(x8 >> 0x30) & 0xFF] ^ T7[(xd >> 0x38) & 0xFF];
                ulong t3 =
                    T0[(x3 >> 0x00) & 0xFF] ^ T1[(x4 >> 0x08) & 0xFF] ^ T2[(x5 >> 0x10) & 0xFF] ^ T3[(x6 >> 0x18) & 0xFF] ^
                    T4[(x7 >> 0x20) & 0xFF] ^ T5[(x8 >> 0x28) & 0xFF] ^ T6[(x9 >> 0x30) & 0xFF] ^ T7[(xe >> 0x38) & 0xFF];
                ulong t4 =
                    T0[(x4 >> 0x00) & 0xFF] ^ T1[(x5 >> 0x08) & 0xFF] ^ T2[(x6 >> 0x10) & 0xFF] ^ T3[(x7 >> 0x18) & 0xFF] ^
                    T4[(x8 >> 0x20) & 0xFF] ^ T5[(x9 >> 0x28) & 0xFF] ^ T6[(xa >> 0x30) & 0xFF] ^ T7[(xf >> 0x38) & 0xFF];
                ulong t5 =
                    T0[(x5 >> 0x00) & 0xFF] ^ T1[(x6 >> 0x08) & 0xFF] ^ T2[(x7 >> 0x10) & 0xFF] ^ T3[(x8 >> 0x18) & 0xFF] ^
                    T4[(x9 >> 0x20) & 0xFF] ^ T5[(xa >> 0x28) & 0xFF] ^ T6[(xb >> 0x30) & 0xFF] ^ T7[(x0 >> 0x38) & 0xFF];
                ulong t6 =
                    T0[(x6 >> 0x00) & 0xFF] ^ T1[(x7 >> 0x08) & 0xFF] ^ T2[(x8 >> 0x10) & 0xFF] ^ T3[(x9 >> 0x18) & 0xFF] ^
                    T4[(xa >> 0x20) & 0xFF] ^ T5[(xb >> 0x28) & 0xFF] ^ T6[(xc >> 0x30) & 0xFF] ^ T7[(x1 >> 0x38) & 0xFF];
                ulong t7 =
                    T0[(x7 >> 0x00) & 0xFF] ^ T1[(x8 >> 0x08) & 0xFF] ^ T2[(x9 >> 0x10) & 0xFF] ^ T3[(xa >> 0x18) & 0xFF] ^
                    T4[(xb >> 0x20) & 0xFF] ^ T5[(xc >> 0x28) & 0xFF] ^ T6[(xd >> 0x30) & 0xFF] ^ T7[(x2 >> 0x38) & 0xFF];
                ulong t8 =
                    T0[(x8 >> 0x00) & 0xFF] ^ T1[(x9 >> 0x08) & 0xFF] ^ T2[(xa >> 0x10) & 0xFF] ^ T3[(xb >> 0x18) & 0xFF] ^
                    T4[(xc >> 0x20) & 0xFF] ^ T5[(xd >> 0x28) & 0xFF] ^ T6[(xe >> 0x30) & 0xFF] ^ T7[(x3 >> 0x38) & 0xFF];
                ulong t9 =
                    T0[(x9 >> 0x00) & 0xFF] ^ T1[(xa >> 0x08) & 0xFF] ^ T2[(xb >> 0x10) & 0xFF] ^ T3[(xc >> 0x18) & 0xFF] ^
                    T4[(xd >> 0x20) & 0xFF] ^ T5[(xe >> 0x28) & 0xFF] ^ T6[(xf >> 0x30) & 0xFF] ^ T7[(x4 >> 0x38) & 0xFF];
                ulong ta =
                    T0[(xa >> 0x00) & 0xFF] ^ T1[(xb >> 0x08) & 0xFF] ^ T2[(xc >> 0x10) & 0xFF] ^ T3[(xd >> 0x18) & 0xFF] ^
                    T4[(xe >> 0x20) & 0xFF] ^ T5[(xf >> 0x28) & 0xFF] ^ T6[(x0 >> 0x30) & 0xFF] ^ T7[(x5 >> 0x38) & 0xFF];
                ulong tb =
                    T0[(xb >> 0x00) & 0xFF] ^ T1[(xc >> 0x08) & 0xFF] ^ T2[(xd >> 0x10) & 0xFF] ^ T3[(xe >> 0x18) & 0xFF] ^
                    T4[(xf >> 0x20) & 0xFF] ^ T5[(x0 >> 0x28) & 0xFF] ^ T6[(x1 >> 0x30) & 0xFF] ^ T7[(x6 >> 0x38) & 0xFF];
                ulong tc =
                    T0[(xc >> 0x00) & 0xFF] ^ T1[(xd >> 0x08) & 0xFF] ^ T2[(xe >> 0x10) & 0xFF] ^ T3[(xf >> 0x18) & 0xFF] ^
                    T4[(x0 >> 0x20) & 0xFF] ^ T5[(x1 >> 0x28) & 0xFF] ^ T6[(x2 >> 0x30) & 0xFF] ^ T7[(x7 >> 0x38) & 0xFF];
                ulong td =
                    T0[(xd >> 0x00) & 0xFF] ^ T1[(xe >> 0x08) & 0xFF] ^ T2[(xf >> 0x10) & 0xFF] ^ T3[(x0 >> 0x18) & 0xFF] ^
                    T4[(x1 >> 0x20) & 0xFF] ^ T5[(x2 >> 0x28) & 0xFF] ^ T6[(x3 >> 0x30) & 0xFF] ^ T7[(x8 >> 0x38) & 0xFF];
                ulong te =
                    T0[(xe >> 0x00) & 0xFF] ^ T1[(xf >> 0x08) & 0xFF] ^ T2[(x0 >> 0x10) & 0xFF] ^ T3[(x1 >> 0x18) & 0xFF] ^
                    T4[(x2 >> 0x20) & 0xFF] ^ T5[(x3 >> 0x28) & 0xFF] ^ T6[(x4 >> 0x30) & 0xFF] ^ T7[(x9 >> 0x38) & 0xFF];
                ulong tf =
                    T0[(xf >> 0x00) & 0xFF] ^ T1[(x0 >> 0x08) & 0xFF] ^ T2[(x1 >> 0x10) & 0xFF] ^ T3[(x2 >> 0x18) & 0xFF] ^
                    T4[(x3 >> 0x20) & 0xFF] ^ T5[(x4 >> 0x28) & 0xFF] ^ T6[(x5 >> 0x30) & 0xFF] ^ T7[(xa >> 0x38) & 0xFF];
                x0 = t0;
                x1 = t1;
                x2 = t2;
                x3 = t3;
                x4 = t4;
                x5 = t5;
                x6 = t6;
                x7 = t7;
                x8 = t8;
                x9 = t9;
                xa = ta;
                xb = tb;
                xc = tc;
                xd = td;
                xe = te;
                xf = tf;
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
            s8 ^= x8;
            s9 ^= x9;
            sa ^= xa;
            sb ^= xb;
            sc ^= xc;
            sd ^= xd;
            se ^= xe;
            sf ^= xf;

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
            ulong m8 = ToUInt64(buffer, 0x40, ByteOrder.LittleEndian);
            ulong m9 = ToUInt64(buffer, 0x48, ByteOrder.LittleEndian);
            ulong ma = ToUInt64(buffer, 0x50, ByteOrder.LittleEndian);
            ulong mb = ToUInt64(buffer, 0x58, ByteOrder.LittleEndian);
            ulong mc = ToUInt64(buffer, 0x60, ByteOrder.LittleEndian);
            ulong md = ToUInt64(buffer, 0x68, ByteOrder.LittleEndian);
            ulong me = ToUInt64(buffer, 0x70, ByteOrder.LittleEndian);
            ulong mf = ToUInt64(buffer, 0x78, ByteOrder.LittleEndian);

            ulong g0 = m0 ^ s0;
            ulong g1 = m1 ^ s1;
            ulong g2 = m2 ^ s2;
            ulong g3 = m3 ^ s3;
            ulong g4 = m4 ^ s4;
            ulong g5 = m5 ^ s5;
            ulong g6 = m6 ^ s6;
            ulong g7 = m7 ^ s7;
            ulong g8 = m8 ^ s8;
            ulong g9 = m9 ^ s9;
            ulong ga = ma ^ sa;
            ulong gb = mb ^ sb;
            ulong gc = mc ^ sc;
            ulong gd = md ^ sd;
            ulong ge = me ^ se;
            ulong gf = mf ^ sf;

            #region P(g)

            for (ulong r = 0; r < 14; r++)
            {
                g0 ^= 0x00 + r;
                g1 ^= 0x10 + r;
                g2 ^= 0x20 + r;
                g3 ^= 0x30 + r;
                g4 ^= 0x40 + r;
                g5 ^= 0x50 + r;
                g6 ^= 0x60 + r;
                g7 ^= 0x70 + r;
                g8 ^= 0x80 + r;
                g9 ^= 0x90 + r;
                ga ^= 0xA0 + r;
                gb ^= 0xB0 + r;
                gc ^= 0xC0 + r;
                gd ^= 0xD0 + r;
                ge ^= 0xE0 + r;
                gf ^= 0xF0 + r;
                ulong t0 =
                    T0[(g0 >> 0x00) & 0xFF] ^ T1[(g1 >> 0x08) & 0xFF] ^ T2[(g2 >> 0x10) & 0xFF] ^ T3[(g3 >> 0x18) & 0xFF] ^
                    T4[(g4 >> 0x20) & 0xFF] ^ T5[(g5 >> 0x28) & 0xFF] ^ T6[(g6 >> 0x30) & 0xFF] ^ T7[(gb >> 0x38) & 0xFF];
                ulong t1 =
                    T0[(g1 >> 0x00) & 0xFF] ^ T1[(g2 >> 0x08) & 0xFF] ^ T2[(g3 >> 0x10) & 0xFF] ^ T3[(g4 >> 0x18) & 0xFF] ^
                    T4[(g5 >> 0x20) & 0xFF] ^ T5[(g6 >> 0x28) & 0xFF] ^ T6[(g7 >> 0x30) & 0xFF] ^ T7[(gc >> 0x38) & 0xFF];
                ulong t2 =
                    T0[(g2 >> 0x00) & 0xFF] ^ T1[(g3 >> 0x08) & 0xFF] ^ T2[(g4 >> 0x10) & 0xFF] ^ T3[(g5 >> 0x18) & 0xFF] ^
                    T4[(g6 >> 0x20) & 0xFF] ^ T5[(g7 >> 0x28) & 0xFF] ^ T6[(g8 >> 0x30) & 0xFF] ^ T7[(gd >> 0x38) & 0xFF];
                ulong t3 =
                    T0[(g3 >> 0x00) & 0xFF] ^ T1[(g4 >> 0x08) & 0xFF] ^ T2[(g5 >> 0x10) & 0xFF] ^ T3[(g6 >> 0x18) & 0xFF] ^
                    T4[(g7 >> 0x20) & 0xFF] ^ T5[(g8 >> 0x28) & 0xFF] ^ T6[(g9 >> 0x30) & 0xFF] ^ T7[(ge >> 0x38) & 0xFF];
                ulong t4 =
                    T0[(g4 >> 0x00) & 0xFF] ^ T1[(g5 >> 0x08) & 0xFF] ^ T2[(g6 >> 0x10) & 0xFF] ^ T3[(g7 >> 0x18) & 0xFF] ^
                    T4[(g8 >> 0x20) & 0xFF] ^ T5[(g9 >> 0x28) & 0xFF] ^ T6[(ga >> 0x30) & 0xFF] ^ T7[(gf >> 0x38) & 0xFF];
                ulong t5 =
                    T0[(g5 >> 0x00) & 0xFF] ^ T1[(g6 >> 0x08) & 0xFF] ^ T2[(g7 >> 0x10) & 0xFF] ^ T3[(g8 >> 0x18) & 0xFF] ^
                    T4[(g9 >> 0x20) & 0xFF] ^ T5[(ga >> 0x28) & 0xFF] ^ T6[(gb >> 0x30) & 0xFF] ^ T7[(g0 >> 0x38) & 0xFF];
                ulong t6 =
                    T0[(g6 >> 0x00) & 0xFF] ^ T1[(g7 >> 0x08) & 0xFF] ^ T2[(g8 >> 0x10) & 0xFF] ^ T3[(g9 >> 0x18) & 0xFF] ^
                    T4[(ga >> 0x20) & 0xFF] ^ T5[(gb >> 0x28) & 0xFF] ^ T6[(gc >> 0x30) & 0xFF] ^ T7[(g1 >> 0x38) & 0xFF];
                ulong t7 =
                    T0[(g7 >> 0x00) & 0xFF] ^ T1[(g8 >> 0x08) & 0xFF] ^ T2[(g9 >> 0x10) & 0xFF] ^ T3[(ga >> 0x18) & 0xFF] ^
                    T4[(gb >> 0x20) & 0xFF] ^ T5[(gc >> 0x28) & 0xFF] ^ T6[(gd >> 0x30) & 0xFF] ^ T7[(g2 >> 0x38) & 0xFF];
                ulong t8 =
                    T0[(g8 >> 0x00) & 0xFF] ^ T1[(g9 >> 0x08) & 0xFF] ^ T2[(ga >> 0x10) & 0xFF] ^ T3[(gb >> 0x18) & 0xFF] ^
                    T4[(gc >> 0x20) & 0xFF] ^ T5[(gd >> 0x28) & 0xFF] ^ T6[(ge >> 0x30) & 0xFF] ^ T7[(g3 >> 0x38) & 0xFF];
                ulong t9 =
                    T0[(g9 >> 0x00) & 0xFF] ^ T1[(ga >> 0x08) & 0xFF] ^ T2[(gb >> 0x10) & 0xFF] ^ T3[(gc >> 0x18) & 0xFF] ^
                    T4[(gd >> 0x20) & 0xFF] ^ T5[(ge >> 0x28) & 0xFF] ^ T6[(gf >> 0x30) & 0xFF] ^ T7[(g4 >> 0x38) & 0xFF];
                ulong ta =
                    T0[(ga >> 0x00) & 0xFF] ^ T1[(gb >> 0x08) & 0xFF] ^ T2[(gc >> 0x10) & 0xFF] ^ T3[(gd >> 0x18) & 0xFF] ^
                    T4[(ge >> 0x20) & 0xFF] ^ T5[(gf >> 0x28) & 0xFF] ^ T6[(g0 >> 0x30) & 0xFF] ^ T7[(g5 >> 0x38) & 0xFF];
                ulong tb =
                    T0[(gb >> 0x00) & 0xFF] ^ T1[(gc >> 0x08) & 0xFF] ^ T2[(gd >> 0x10) & 0xFF] ^ T3[(ge >> 0x18) & 0xFF] ^
                    T4[(gf >> 0x20) & 0xFF] ^ T5[(g0 >> 0x28) & 0xFF] ^ T6[(g1 >> 0x30) & 0xFF] ^ T7[(g6 >> 0x38) & 0xFF];
                ulong tc =
                    T0[(gc >> 0x00) & 0xFF] ^ T1[(gd >> 0x08) & 0xFF] ^ T2[(ge >> 0x10) & 0xFF] ^ T3[(gf >> 0x18) & 0xFF] ^
                    T4[(g0 >> 0x20) & 0xFF] ^ T5[(g1 >> 0x28) & 0xFF] ^ T6[(g2 >> 0x30) & 0xFF] ^ T7[(g7 >> 0x38) & 0xFF];
                ulong td =
                    T0[(gd >> 0x00) & 0xFF] ^ T1[(ge >> 0x08) & 0xFF] ^ T2[(gf >> 0x10) & 0xFF] ^ T3[(g0 >> 0x18) & 0xFF] ^
                    T4[(g1 >> 0x20) & 0xFF] ^ T5[(g2 >> 0x28) & 0xFF] ^ T6[(g3 >> 0x30) & 0xFF] ^ T7[(g8 >> 0x38) & 0xFF];
                ulong te =
                    T0[(ge >> 0x00) & 0xFF] ^ T1[(gf >> 0x08) & 0xFF] ^ T2[(g0 >> 0x10) & 0xFF] ^ T3[(g1 >> 0x18) & 0xFF] ^
                    T4[(g2 >> 0x20) & 0xFF] ^ T5[(g3 >> 0x28) & 0xFF] ^ T6[(g4 >> 0x30) & 0xFF] ^ T7[(g9 >> 0x38) & 0xFF];
                ulong tf =
                    T0[(gf >> 0x00) & 0xFF] ^ T1[(g0 >> 0x08) & 0xFF] ^ T2[(g1 >> 0x10) & 0xFF] ^ T3[(g2 >> 0x18) & 0xFF] ^
                    T4[(g3 >> 0x20) & 0xFF] ^ T5[(g4 >> 0x28) & 0xFF] ^ T6[(g5 >> 0x30) & 0xFF] ^ T7[(ga >> 0x38) & 0xFF];
                g0 = t0;
                g1 = t1;
                g2 = t2;
                g3 = t3;
                g4 = t4;
                g5 = t5;
                g6 = t6;
                g7 = t7;
                g8 = t8;
                g9 = t9;
                ga = ta;
                gb = tb;
                gc = tc;
                gd = td;
                ge = te;
                gf = tf;
            }

            #endregion

            #region Q(m)

            for (ulong r = 0; r < 14; r++)
            {
                m0 ^= (r << 56) ^ 0xFFFFFFFFFFFFFFFF;
                m1 ^= (r << 56) ^ 0xEFFFFFFFFFFFFFFF;
                m2 ^= (r << 56) ^ 0xDFFFFFFFFFFFFFFF;
                m3 ^= (r << 56) ^ 0xCFFFFFFFFFFFFFFF;
                m4 ^= (r << 56) ^ 0xBFFFFFFFFFFFFFFF;
                m5 ^= (r << 56) ^ 0xAFFFFFFFFFFFFFFF;
                m6 ^= (r << 56) ^ 0x9FFFFFFFFFFFFFFF;
                m7 ^= (r << 56) ^ 0x8FFFFFFFFFFFFFFF;
                m8 ^= (r << 56) ^ 0x7FFFFFFFFFFFFFFF;
                m9 ^= (r << 56) ^ 0x6FFFFFFFFFFFFFFF;
                ma ^= (r << 56) ^ 0x5FFFFFFFFFFFFFFF;
                mb ^= (r << 56) ^ 0x4FFFFFFFFFFFFFFF;
                mc ^= (r << 56) ^ 0x3FFFFFFFFFFFFFFF;
                md ^= (r << 56) ^ 0x2FFFFFFFFFFFFFFF;
                me ^= (r << 56) ^ 0x1FFFFFFFFFFFFFFF;
                mf ^= (r << 56) ^ 0x0FFFFFFFFFFFFFFF;
                ulong t0 =
                    T0[(m1 >> 0x00) & 0xFF] ^ T1[(m3 >> 0x08) & 0xFF] ^ T2[(m5 >> 0x10) & 0xFF] ^ T3[(mb >> 0x18) & 0xFF] ^
                    T4[(m0 >> 0x20) & 0xFF] ^ T5[(m2 >> 0x28) & 0xFF] ^ T6[(m4 >> 0x30) & 0xFF] ^ T7[(m6 >> 0x38) & 0xFF];
                ulong t1 =
                    T0[(m2 >> 0x00) & 0xFF] ^ T1[(m4 >> 0x08) & 0xFF] ^ T2[(m6 >> 0x10) & 0xFF] ^ T3[(mc >> 0x18) & 0xFF] ^
                    T4[(m1 >> 0x20) & 0xFF] ^ T5[(m3 >> 0x28) & 0xFF] ^ T6[(m5 >> 0x30) & 0xFF] ^ T7[(m7 >> 0x38) & 0xFF];
                ulong t2 =
                    T0[(m3 >> 0x00) & 0xFF] ^ T1[(m5 >> 0x08) & 0xFF] ^ T2[(m7 >> 0x10) & 0xFF] ^ T3[(md >> 0x18) & 0xFF] ^
                    T4[(m2 >> 0x20) & 0xFF] ^ T5[(m4 >> 0x28) & 0xFF] ^ T6[(m6 >> 0x30) & 0xFF] ^ T7[(m8 >> 0x38) & 0xFF];
                ulong t3 =
                    T0[(m4 >> 0x00) & 0xFF] ^ T1[(m6 >> 0x08) & 0xFF] ^ T2[(m8 >> 0x10) & 0xFF] ^ T3[(me >> 0x18) & 0xFF] ^
                    T4[(m3 >> 0x20) & 0xFF] ^ T5[(m5 >> 0x28) & 0xFF] ^ T6[(m7 >> 0x30) & 0xFF] ^ T7[(m9 >> 0x38) & 0xFF];
                ulong t4 =
                    T0[(m5 >> 0x00) & 0xFF] ^ T1[(m7 >> 0x08) & 0xFF] ^ T2[(m9 >> 0x10) & 0xFF] ^ T3[(mf >> 0x18) & 0xFF] ^
                    T4[(m4 >> 0x20) & 0xFF] ^ T5[(m6 >> 0x28) & 0xFF] ^ T6[(m8 >> 0x30) & 0xFF] ^ T7[(ma >> 0x38) & 0xFF];
                ulong t5 =
                    T0[(m6 >> 0x00) & 0xFF] ^ T1[(m8 >> 0x08) & 0xFF] ^ T2[(ma >> 0x10) & 0xFF] ^ T3[(m0 >> 0x18) & 0xFF] ^
                    T4[(m5 >> 0x20) & 0xFF] ^ T5[(m7 >> 0x28) & 0xFF] ^ T6[(m9 >> 0x30) & 0xFF] ^ T7[(mb >> 0x38) & 0xFF];
                ulong t6 =
                    T0[(m7 >> 0x00) & 0xFF] ^ T1[(m9 >> 0x08) & 0xFF] ^ T2[(mb >> 0x10) & 0xFF] ^ T3[(m1 >> 0x18) & 0xFF] ^
                    T4[(m6 >> 0x20) & 0xFF] ^ T5[(m8 >> 0x28) & 0xFF] ^ T6[(ma >> 0x30) & 0xFF] ^ T7[(mc >> 0x38) & 0xFF];
                ulong t7 =
                    T0[(m8 >> 0x00) & 0xFF] ^ T1[(ma >> 0x08) & 0xFF] ^ T2[(mc >> 0x10) & 0xFF] ^ T3[(m2 >> 0x18) & 0xFF] ^
                    T4[(m7 >> 0x20) & 0xFF] ^ T5[(m9 >> 0x28) & 0xFF] ^ T6[(mb >> 0x30) & 0xFF] ^ T7[(md >> 0x38) & 0xFF];
                ulong t8 =
                    T0[(m9 >> 0x00) & 0xFF] ^ T1[(mb >> 0x08) & 0xFF] ^ T2[(md >> 0x10) & 0xFF] ^ T3[(m3 >> 0x18) & 0xFF] ^
                    T4[(m8 >> 0x20) & 0xFF] ^ T5[(ma >> 0x28) & 0xFF] ^ T6[(mc >> 0x30) & 0xFF] ^ T7[(me >> 0x38) & 0xFF];
                ulong t9 =
                    T0[(ma >> 0x00) & 0xFF] ^ T1[(mc >> 0x08) & 0xFF] ^ T2[(me >> 0x10) & 0xFF] ^ T3[(m4 >> 0x18) & 0xFF] ^
                    T4[(m9 >> 0x20) & 0xFF] ^ T5[(mb >> 0x28) & 0xFF] ^ T6[(md >> 0x30) & 0xFF] ^ T7[(mf >> 0x38) & 0xFF];
                ulong ta =
                    T0[(mb >> 0x00) & 0xFF] ^ T1[(md >> 0x08) & 0xFF] ^ T2[(mf >> 0x10) & 0xFF] ^ T3[(m5 >> 0x18) & 0xFF] ^
                    T4[(ma >> 0x20) & 0xFF] ^ T5[(mc >> 0x28) & 0xFF] ^ T6[(me >> 0x30) & 0xFF] ^ T7[(m0 >> 0x38) & 0xFF];
                ulong tb =
                    T0[(mc >> 0x00) & 0xFF] ^ T1[(me >> 0x08) & 0xFF] ^ T2[(m0 >> 0x10) & 0xFF] ^ T3[(m6 >> 0x18) & 0xFF] ^
                    T4[(mb >> 0x20) & 0xFF] ^ T5[(md >> 0x28) & 0xFF] ^ T6[(mf >> 0x30) & 0xFF] ^ T7[(m1 >> 0x38) & 0xFF];
                ulong tc =
                    T0[(md >> 0x00) & 0xFF] ^ T1[(mf >> 0x08) & 0xFF] ^ T2[(m1 >> 0x10) & 0xFF] ^ T3[(m7 >> 0x18) & 0xFF] ^
                    T4[(mc >> 0x20) & 0xFF] ^ T5[(me >> 0x28) & 0xFF] ^ T6[(m0 >> 0x30) & 0xFF] ^ T7[(m2 >> 0x38) & 0xFF];
                ulong td =
                    T0[(me >> 0x00) & 0xFF] ^ T1[(m0 >> 0x08) & 0xFF] ^ T2[(m2 >> 0x10) & 0xFF] ^ T3[(m8 >> 0x18) & 0xFF] ^
                    T4[(md >> 0x20) & 0xFF] ^ T5[(mf >> 0x28) & 0xFF] ^ T6[(m1 >> 0x30) & 0xFF] ^ T7[(m3 >> 0x38) & 0xFF];
                ulong te =
                    T0[(mf >> 0x00) & 0xFF] ^ T1[(m1 >> 0x08) & 0xFF] ^ T2[(m3 >> 0x10) & 0xFF] ^ T3[(m9 >> 0x18) & 0xFF] ^
                    T4[(me >> 0x20) & 0xFF] ^ T5[(m0 >> 0x28) & 0xFF] ^ T6[(m2 >> 0x30) & 0xFF] ^ T7[(m4 >> 0x38) & 0xFF];
                ulong tf =
                    T0[(m0 >> 0x00) & 0xFF] ^ T1[(m2 >> 0x08) & 0xFF] ^ T2[(m4 >> 0x10) & 0xFF] ^ T3[(ma >> 0x18) & 0xFF] ^
                    T4[(mf >> 0x20) & 0xFF] ^ T5[(m1 >> 0x28) & 0xFF] ^ T6[(m3 >> 0x30) & 0xFF] ^ T7[(m5 >> 0x38) & 0xFF];
                m0 = t0;
                m1 = t1;
                m2 = t2;
                m3 = t3;
                m4 = t4;
                m5 = t5;
                m6 = t6;
                m7 = t7;
                m8 = t8;
                m9 = t9;
                ma = ta;
                mb = tb;
                mc = tc;
                md = td;
                me = te;
                mf = tf;
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
            s8 ^= g8 ^ m8;
            s9 ^= g9 ^ m9;
            sa ^= ga ^ ma;
            sb ^= gb ^ mb;
            sc ^= gc ^ mc;
            sd ^= gd ^ md;
            se ^= ge ^ me;
            sf ^= gf ^ mf;

            blocks++;
        }

        protected virtual byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[64];
            GetBytes(s8, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s9, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(sa, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(sb, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(sc, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(sd, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(se, ByteOrder.LittleEndian, digest, 0x30);
            GetBytes(sf, ByteOrder.LittleEndian, digest, 0x38);
            return digest;
        }
    }
}