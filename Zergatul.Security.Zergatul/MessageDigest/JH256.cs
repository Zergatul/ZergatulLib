using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class JH256 : JH
    {
        public override int DigestLength => 32;

        public override void Reset()
        {
            s0 = 0xEBD3202C41A398EB;
            s1 = 0xC145B29C7BBECD92;
            s2 = 0xFAC7D4609151931C;
            s3 = 0x038A507ED6820026;
            s4 = 0x45B92677269E23A4;
            s5 = 0x77941AD4481AFBE0;
            s6 = 0x7A176B0226ABB5CD;
            s7 = 0xA82FFF0F4224F056;
            s8 = 0x754D2E7F8996A371;
            s9 = 0x62E27DF70849141D;
            sa = 0x948F2476F7957627;
            sb = 0x6C29804757B6D587;
            sc = 0x6C0D8EAC2D275E5C;
            sd = 0x0F7A0557C6508451;
            se = 0xEA12247067D3E47B;
            sf = 0x69D71CD313ABE389;

            bufOffset = 0;
            blockCount = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[32];
            GetBytes(sc, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(sd, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(se, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(sf, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}