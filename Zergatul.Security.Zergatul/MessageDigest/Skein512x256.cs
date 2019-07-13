using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Skein512x256 : Skein512x512
    {
        public override int DigestLength => 32;

        public override void Reset()
        {
            h0 = 0xCCD044A12FDB3E13;
            h1 = 0xE83590301A79A9EB;
            h2 = 0x55AEA0614F816E6F;
            h3 = 0x2A2767A4AE9B94DB;
            h4 = 0xEC06025E74DD7683;
            h5 = 0xE7A436CDC4746251;
            h6 = 0xC36FBAF9393AD185;
            h7 = 0x3EEDBA1833EDFC13;

            bufOffset = 0;
            blockCount = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[32];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}