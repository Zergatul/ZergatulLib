using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class CubeHash512 : CubeHash
    {
        public override int DigestLength => 64;

        public override void Reset()
        {
            s00 = 0x2AEA2A61;
            s01 = 0x50F494D4;
            s02 = 0x2D538B8B;
            s03 = 0x4167D83E;
            s04 = 0x3FEE2313;
            s05 = 0xC701CF8C;
            s06 = 0xCC39968E;
            s07 = 0x50AC5695;
            s08 = 0x4D42C787;
            s09 = 0xA647A8B3;
            s0a = 0x97CF0BEF;
            s0b = 0x825B4537;
            s0c = 0xEEF864D2;
            s0d = 0xF22090C4;
            s0e = 0xD0E5CD33;
            s0f = 0xA23911AE;
            s10 = 0xFCD398D9;
            s11 = 0x148FE485;
            s12 = 0x1B017BEF;
            s13 = 0xB6444532;
            s14 = 0x6A536159;
            s15 = 0x2FF5781C;
            s16 = 0x91FA7934;
            s17 = 0x0DBADEA9;
            s18 = 0xD65C8A2B;
            s19 = 0xA5A70E75;
            s1a = 0xB1C62456;
            s1b = 0xBC796576;
            s1c = 0x1921C8F7;
            s1d = 0xE7989AF1;
            s1e = 0x7795D246;
            s1f = 0xD43E3B44;

            bufOffset = 0;
        }

        public override byte[] Digest()
        {
            FinalProcess();

            byte[] digest = new byte[64];
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
            GetBytes(s0c, ByteOrder.LittleEndian, digest, 0x30);
            GetBytes(s0d, ByteOrder.LittleEndian, digest, 0x34);
            GetBytes(s0e, ByteOrder.LittleEndian, digest, 0x38);
            GetBytes(s0f, ByteOrder.LittleEndian, digest, 0x3C);
            return digest;
        }
    }
}