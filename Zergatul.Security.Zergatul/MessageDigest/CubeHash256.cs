using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class CubeHash256 : CubeHash
    {
        public override int DigestLength => 32;

        public override void Reset()
        {
            s00 = 0xEA2BD4B4;
            s01 = 0xCCD6F29F;
            s02 = 0x63117E71;
            s03 = 0x35481EAE;
            s04 = 0x22512D5B;
            s05 = 0xE5D94E63;
            s06 = 0x7E624131;
            s07 = 0xF4CC12BE;
            s08 = 0xC2D0B696;
            s09 = 0x42AF2070;
            s0a = 0xD0720C35;
            s0b = 0x3361DA8C;
            s0c = 0x28CCECA4;
            s0d = 0x8EF8AD83;
            s0e = 0x4680AC00;
            s0f = 0x40E5FBAB;
            s10 = 0xD89041C3;
            s11 = 0x6107FBD5;
            s12 = 0x6C859D41;
            s13 = 0xF0B26679;
            s14 = 0x09392549;
            s15 = 0x5FA25603;
            s16 = 0x65C892FD;
            s17 = 0x93CB6285;
            s18 = 0x2AF2B5AE;
            s19 = 0x9E4B4E60;
            s1a = 0x774ABFDD;
            s1b = 0x85254725;
            s1c = 0x15815AEB;
            s1d = 0x4AB6AAD6;
            s1e = 0x9CDAF8AF;
            s1f = 0xD6032C0A;

            bufOffset = 0;
        }

        public override byte[] Digest()
        {
            FinalProcess();

            byte[] digest = new byte[32];
            GetBytes(s00, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s01, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(s02, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s03, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(s04, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(s05, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(s06, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(s07, ByteOrder.LittleEndian, digest, 0x1C);
            return digest;
        }
    }
}