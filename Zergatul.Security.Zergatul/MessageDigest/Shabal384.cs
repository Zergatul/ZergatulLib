using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Shabal384 : Shabal512
    {
        public override int DigestLength => 48;

        public override void Reset()
        {
            a0 = 0xC8FCA331;
            a1 = 0xE55C504E;
            a2 = 0x003EBF26;
            a3 = 0xBB6B8D83;
            a4 = 0x7B0448C1;
            a5 = 0x41B82789;
            a6 = 0x0A7C9601;
            a7 = 0x8D659CFF;
            a8 = 0xB6E2673E;
            a9 = 0xCA54C77B;
            aa = 0x1460FD7E;
            ab = 0x3FCB8F2D;

            b0 = 0x527291FC;
            b1 = 0x2A16455F;
            b2 = 0x78E627E5;
            b3 = 0x944F169F;
            b4 = 0x1CA6F016;
            b5 = 0xA854EA25;
            b6 = 0x8DB98ABE;
            b7 = 0xF2C62641;
            b8 = 0x30117DCB;
            b9 = 0xCF5C4309;
            ba = 0x93711A25;
            bb = 0xF9F671B8;
            bc = 0xB01D2116;
            bd = 0x333F4B89;
            be = 0xB285D165;
            bf = 0x86829B36;

            c0 = 0xF764B11A;
            c1 = 0x76172146;
            c2 = 0xCEF6934D;
            c3 = 0xC6D28399;
            c4 = 0xFE095F61;
            c5 = 0x5E6018B4;
            c6 = 0x5048ECF5;
            c7 = 0x51353261;
            c8 = 0x6E6E36DC;
            c9 = 0x63130DAD;
            ca = 0xA9C69BD6;
            cb = 0x1E90EA0C;
            cc = 0x7C35073B;
            cd = 0x28D95E6D;
            ce = 0xAA340E0D;
            cf = 0xCB3DEE70;

            bufOffset = 0;
            wLow = 0;
            wHi = 0;
        }

        protected override byte[] StateBToDigest(
            uint b0, uint b1, uint b2, uint b3, uint b4, uint b5, uint b6, uint b7,
            uint b8, uint b9, uint ba, uint bb, uint bc, uint bd, uint be, uint bf)
        {
            byte[] digest = new byte[48];
            GetBytes(b4, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(b5, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(b6, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(b7, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(b8, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(b9, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(ba, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(bb, ByteOrder.LittleEndian, digest, 0x1C);
            GetBytes(bc, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(bd, ByteOrder.LittleEndian, digest, 0x24);
            GetBytes(be, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(bf, ByteOrder.LittleEndian, digest, 0x2C);
            return digest;
        }
    }
}