using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Shabal224 : Shabal512
    {
        public override int DigestLength => 28;

        public override void Reset()
        {
            a0 = 0xA5201467;
            a1 = 0xA9B8D94A;
            a2 = 0xD4CED997;
            a3 = 0x68379D7B;
            a4 = 0xA7FC73BA;
            a5 = 0xF1A2546B;
            a6 = 0x606782BF;
            a7 = 0xE0BCFD0F;
            a8 = 0x2F25374E;
            a9 = 0x069A149F;
            aa = 0x5E2DFF25;
            ab = 0xFAECF061;

            b0 = 0xEC9905D8;
            b1 = 0xF21850CF;
            b2 = 0xC0A746C8;
            b3 = 0x21DAD498;
            b4 = 0x35156EEB;
            b5 = 0x088C97F2;
            b6 = 0x26303E40;
            b7 = 0x8A2D4FB5;
            b8 = 0xFEEE44B6;
            b9 = 0x8A1E9573;
            ba = 0x7B81111A;
            bb = 0xCBC139F0;
            bc = 0xA3513861;
            bd = 0x1D2C362E;
            be = 0x918C580E;
            bf = 0xB58E1B9C;

            c0 = 0xE4B573A1;
            c1 = 0x4C1A0880;
            c2 = 0x1E907C51;
            c3 = 0x04807EFD;
            c4 = 0x3AD8CDE5;
            c5 = 0x16B21302;
            c6 = 0x02512C53;
            c7 = 0x2204CB18;
            c8 = 0x99405F2D;
            c9 = 0xE5B648A1;
            ca = 0x70AB1D43;
            cb = 0xA10C25C2;
            cc = 0x16F1AC05;
            cd = 0x38BBEB56;
            ce = 0x9B01DC60;
            cf = 0xB1096D83;

            bufOffset = 0;
            wLow = 0;
            wHi = 0;
        }

        protected override byte[] StateBToDigest(
            uint b0, uint b1, uint b2, uint b3, uint b4, uint b5, uint b6, uint b7,
            uint b8, uint b9, uint ba, uint bb, uint bc, uint bd, uint be, uint bf)
        {
            byte[] digest = new byte[28];
            GetBytes(b9, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(ba, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(bb, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(bc, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(bd, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(be, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(bf, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}