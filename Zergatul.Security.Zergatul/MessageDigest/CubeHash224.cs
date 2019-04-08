using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class CubeHash224 : CubeHash
    {
        public override int DigestLength => 28;

        public override void Reset()
        {
            s00 = 0xB0FC8217;
            s01 = 0x1BEE1A90;
            s02 = 0x829E1A22;
            s03 = 0x6362C342;
            s04 = 0x24D91C30;
            s05 = 0x03A7AA24;
            s06 = 0xA63721C8;
            s07 = 0x85B0E2EF;
            s08 = 0xF35D13F3;
            s09 = 0x41DA807D;
            s0a = 0x21A70CA6;
            s0b = 0x1F4E9774;
            s0c = 0xB3E1C932;
            s0d = 0xEB0A79A8;
            s0e = 0xCDDAAA66;
            s0f = 0xE2F6ECAA;
            s10 = 0x0A713362;
            s11 = 0xAA3080E0;
            s12 = 0xD8F23A32;
            s13 = 0xCEF15E28;
            s14 = 0xDB086314;
            s15 = 0x7F709DF7;
            s16 = 0xACD228A4;
            s17 = 0x704D6ECE;
            s18 = 0xAA3EC95F;
            s19 = 0xE387C214;
            s1a = 0x3A6445FF;
            s1b = 0x9CAB81C3;
            s1c = 0xC73D4B98;
            s1d = 0xD277AEBE;
            s1e = 0xFD20151C;
            s1f = 0x00CB573E;

            bufOffset = 0;
        }

        public override byte[] Digest()
        {
            FinalProcess();

            byte[] digest = new byte[28];
            GetBytes(s00, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s01, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(s02, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s03, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(s04, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(s05, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(s06, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}