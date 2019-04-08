using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class JH512 : JH
    {
        public override int DigestLength => 64;

        public override void Reset()
        {
            s0 = 0x17AA003E964BD16F;
            s1 = 0x43D5157A052E6A63;
            s2 = 0x0BEF970C8D5E228A;
            s3 = 0x61C3B3F2591234E9;
            s4 = 0x1E806F53C1A01D89;
            s5 = 0x806D2BEA6B05A92A;
            s6 = 0xA6BA7520DBCC8E58;
            s7 = 0xF73BF8BA763A0FA9;
            s8 = 0x694AE34105E66901;
            s9 = 0x5AE66F2E8E8AB546;
            sa = 0x243C84C1D0A74710;
            sb = 0x99C15A2DB1716E3B;
            sc = 0x56F8B19DECF657CF;
            sd = 0x56B116577C8806A7;
            se = 0xFB1785E6DFFCC2E3;
            sf = 0x4BDD8CCC78465A54;

            bufOffset = 0;
            blockCount = 0;
        }

        protected override byte[] InternalStateToDigest()
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