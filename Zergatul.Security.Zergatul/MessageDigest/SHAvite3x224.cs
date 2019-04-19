using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHAvite3x224 : SHAvite3x256
    {
        public override int DigestLength => 28;

        public override void Reset()
        {
            h0 = 0x6774F31C;
            h1 = 0x990AE210;
            h2 = 0xC87D4274;
            h3 = 0xC9546371;
            h4 = 0x62B2AEA8;
            h5 = 0x4B5801D8;
            h6 = 0x1B702860;
            h7 = 0x842F3017;

            c0 = c1 = 0;

            bufOffset = 0;
        }

        public override byte[] Digest()
        {
            Close(7);

            byte[] digest = new byte[28];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(h5, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(h6, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}