using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Skein512x224 : Skein512x512
    {
        public override int DigestLength => 28;

        public override void Reset()
        {
            h0 = 0xCCD0616248677224;
            h1 = 0xCBA65CF3A92339EF;
            h2 = 0x8CCD69D652FF4B64;
            h3 = 0x398AED7B3AB890B4;
            h4 = 0x0F59D1B1457D2BD0;
            h5 = 0x6776FE6575D4EB3D;
            h6 = 0x99FBC70E997413E9;
            h7 = 0x9E2CFCCFE1C41EF7;

            bufOffset = 0;
            blockCount = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[28];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x10);
            digest[24] = (byte)h3;
            digest[25] = (byte)(h3 >> 8);
            digest[26] = (byte)(h3 >> 16);
            digest[27] = (byte)(h3 >> 24);
            return digest;
        }
    }
}