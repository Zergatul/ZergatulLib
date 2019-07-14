using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Hamsi224 : Hamsi256
    {
        public override int DigestLength => 28;

        public override void Reset()
        {
            h0 = 0xC3967A67;
            h1 = 0xC3BC6C20;
            h2 = 0x4BC3BCC3;
            h3 = 0xA7C3BC6B;
            h4 = 0x2C204B61;
            h5 = 0x74686F6C;
            h6 = 0x69656B65;
            h7 = 0x20556E69;

            bufOffset = 0;
            count = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[28];
            GetBytes(h0, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.BigEndian, digest, 0x04);
            GetBytes(h2, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h3, ByteOrder.BigEndian, digest, 0x0C);
            GetBytes(h4, ByteOrder.BigEndian, digest, 0x10);
            GetBytes(h5, ByteOrder.BigEndian, digest, 0x14);
            GetBytes(h6, ByteOrder.BigEndian, digest, 0x18);
            return digest;
        }
    }
}