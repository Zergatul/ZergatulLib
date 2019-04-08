using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class CubeHash384 : CubeHash
    {
        public override int DigestLength => 48;

        public override void Reset()
        {
            s00 = 0xE623087E;
            s01 = 0x04C00C87;
            s02 = 0x5EF46453;
            s03 = 0x69524B13;
            s04 = 0x1A05C7A9;
            s05 = 0x3528DF88;
            s06 = 0x6BDD01B5;
            s07 = 0x5057B792;
            s08 = 0x6AA7A922;
            s09 = 0x649C7EEE;
            s0a = 0xF426309F;
            s0b = 0xCB629052;
            s0c = 0xFC8E20ED;
            s0d = 0xB3482BAB;
            s0e = 0xF89E5E7E;
            s0f = 0xD83D4DE4;
            s10 = 0x44BFC10D;
            s11 = 0x5FC1E63D;
            s12 = 0x2104E6CB;
            s13 = 0x17958F7F;
            s14 = 0xDBEAEF70;
            s15 = 0xB4B97E1E;
            s16 = 0x32C195F6;
            s17 = 0x6184A8E4;
            s18 = 0x796C2543;
            s19 = 0x23DE176D;
            s1a = 0xD33BBAEC;
            s1b = 0x0C12E5D2;
            s1c = 0x4EB95A7B;
            s1d = 0x2D18BA01;
            s1e = 0x04EE475F;
            s1f = 0x1FC5F22E;

            bufOffset = 0;
        }

        public override byte[] Digest()
        {
            FinalProcess();

            byte[] digest = new byte[48];
            GetBytes(s00, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s01, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(s02, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s03, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(s04, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(s05, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(s06, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(s07, ByteOrder.LittleEndian, digest, 0x1C);
            GetBytes(s08, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(s09, ByteOrder.LittleEndian, digest, 0x24);
            GetBytes(s0a, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(s0b, ByteOrder.LittleEndian, digest, 0x2C);
            return digest;
        }
    }
}