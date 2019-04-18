using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHAvite3x384 : SHAvite3x512
    {
        public override int DigestLength => 48;

        public override void Reset()
        {
            h0 = 0x83DF1545;
            h1 = 0xF9AAEC13;
            h2 = 0xF4803CB0;
            h3 = 0x11FE1F47;
            h4 = 0xDA6CD269;
            h5 = 0x4F53FCD7;
            h6 = 0x950529A2;
            h7 = 0x97908147;
            h8 = 0xB0A4D7AF;
            h9 = 0x2B9132BF;
            ha = 0x226E607D;
            hb = 0x3C0F8D7C;
            hc = 0x487B3F0F;
            hd = 0x04363E22;
            he = 0x0155C99C;
            hf = 0xEC2E20D3;

            c0 = c1 = c2 = c3 = 0;

            bufOffset = 0;
        }

        public override byte[] Digest()
        {
            Close(12);

            byte[] digest = new byte[48];
            GetBytes(h0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(h5, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(h6, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(h7, ByteOrder.LittleEndian, digest, 0x1C);
            GetBytes(h8, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(h9, ByteOrder.LittleEndian, digest, 0x24);
            GetBytes(ha, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(hb, ByteOrder.LittleEndian, digest, 0x2C);
            return digest;
        }
    }
}