using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Groestl224 : Groestl256
    {
        public override int DigestLength => 28;

        public override void Reset()
        {
            s0 = s1 = s2 = s3 = s4 = s5 = s6 = 0;
            s7 = 0xE000000000000000;

            bufOffset = 0;
            blocks = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[28];
            digest[0] = (byte)(s4 >> 32);
            digest[1] = (byte)(s4 >> 40);
            digest[2] = (byte)(s4 >> 48);
            digest[3] = (byte)(s4 >> 56);
            GetBytes(s5, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(s6, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(s7, ByteOrder.LittleEndian, digest, 0x14);
            return digest;
        }
    }
}