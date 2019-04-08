using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Groestl384 : Groestl512
    {
        public override int DigestLength => 48;

        public override void Reset()
        {
            s0 = s1 = s2 = s3 = s4 = s5 = s6 = s7 = s8 = s9 = sa = sb = sc = sd = se = 0;
            sf = 0x8001000000000000;

            bufOffset = 0;
            blocks = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[48];
            GetBytes(sa, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(sb, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(sc, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(sd, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(se, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(sf, ByteOrder.LittleEndian, digest, 0x28);
            return digest;
        }
    }
}