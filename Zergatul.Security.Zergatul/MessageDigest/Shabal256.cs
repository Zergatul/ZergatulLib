using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Shabal256 : Shabal512
    {
        public override int DigestLength => 32;

        public override void Reset()
        {
            a0 = 0x52F84552;
            a1 = 0xE54B7999;
            a2 = 0x2D8EE3EC;
            a3 = 0xB9645191;
            a4 = 0xE0078B86;
            a5 = 0xBB7C44C9;
            a6 = 0xD2B5C1CA;
            a7 = 0xB0D2EB8C;
            a8 = 0x14CE5A45;
            a9 = 0x22AF50DC;
            aa = 0xEFFDBC6B;
            ab = 0xEB21B74A;

            b0 = 0xB555C6EE;
            b1 = 0x3E710596;
            b2 = 0xA72A652F;
            b3 = 0x9301515F;
            b4 = 0xDA28C1FA;
            b5 = 0x696FD868;
            b6 = 0x9CB6BF72;
            b7 = 0x0AFE4002;
            b8 = 0xA6E03615;
            b9 = 0x5138C1D4;
            ba = 0xBE216306;
            bb = 0xB38B8890;
            bc = 0x3EA8B96B;
            bd = 0x3299ACE4;
            be = 0x30924DD4;
            bf = 0x55CB34A5;

            c0 = 0xB405F031;
            c1 = 0xC4233EBA;
            c2 = 0xB3733979;
            c3 = 0xC0DD9D55;
            c4 = 0xC51C28AE;
            c5 = 0xA327B8E1;
            c6 = 0x56C56167;
            c7 = 0xED614433;
            c8 = 0x88B59D60;
            c9 = 0x60E2CEBA;
            ca = 0x758B4B8B;
            cb = 0x83E82A7F;
            cc = 0xBC968828;
            cd = 0xE6E00BF7;
            ce = 0xBA839E55;
            cf = 0x9B491C60;

            bufOffset = 0;
            wLow = 0;
            wHi = 0;
        }

        protected override byte[] StateBToDigest(
            uint b0, uint b1, uint b2, uint b3, uint b4, uint b5, uint b6, uint b7,
            uint b8, uint b9, uint ba, uint bb, uint bc, uint bd, uint be, uint bf)
        {
            byte[] digest = new byte[32];
            GetBytes(b8, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(b9, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(ba, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(bb, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(bc, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(bd, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(be, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(bf, ByteOrder.LittleEndian, digest, 0x1C);
            return digest;
        }
    }
}