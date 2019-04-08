using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class BLAKE224 : BLAKE256
    {
        public override int DigestLength => 28;

        public BLAKE224()
        {
            appendBit = false;
        }

        public override void Reset()
        {
            h0 = 0xC1059ED8;
            h1 = 0x367CD507;
            h2 = 0x3070DD17;
            h3 = 0xF70E5939;
            h4 = 0xFFC00B31;
            h5 = 0x68581511;
            h6 = 0x64F98FA7;
            h7 = 0xBEFA4FA4;
            s0 = 0;
            s1 = 0;
            s2 = 0;
            s3 = 0;

            bufOffset = 0;
            length = 0;
        }

        protected override byte[] InternalStateToBytes()
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