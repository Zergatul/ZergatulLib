using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class JH384 : JH
    {
        public override int DigestLength => 48;

        public override void Reset()
        {
            s0 = 0x8A3913D8C63B1E48;
            s1 = 0x9B87DE4A895E3B6D;
            s2 = 0x2EAD80D468EAFA63;
            s3 = 0x67820F4821CB2C33;
            s4 = 0x28B982904DC8AE98;
            s5 = 0x4942114130EA55D4;
            s6 = 0xEC474892B255F536;
            s7 = 0xE13CF4BA930A25C7;
            s8 = 0x4C45DB278A7F9B56;
            s9 = 0x0EAF976349BDFC9E;
            sa = 0xCD80AA267DC29F58;
            sb = 0xDA2EEB9D8C8BC080;
            sc = 0x3A37D5F8E881798A;
            sd = 0x717AD1DDAD6739F4;
            se = 0x94D375A4BDD3B4A9;
            sf = 0x7F734298BA3F6C97;

            bufOffset = 0;
            blockCount = 0;
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