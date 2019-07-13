using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Skein512x384 : Skein512x512
    {
        public override int DigestLength => 48;

        public override void Reset()
        {
            h0 = 0xA3F6C6BF3A75EF5F;
            h1 = 0xB0FEF9CCFD84FAA4;
            h2 = 0x9D77DD663D770CFE;
            h3 = 0xD798CBF3B468FDDA;
            h4 = 0x1BC4A6668A0E4465;
            h5 = 0x7ED7D434E5807407;
            h6 = 0x548FC1ACD4EC44D6;
            h7 = 0x266E17546AA18FF8;

            bufOffset = 0;
            blockCount = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[48];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(h4, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(h5, ByteOrder.LittleEndian, digest, 0x28);
            return digest;
        }
    }
}