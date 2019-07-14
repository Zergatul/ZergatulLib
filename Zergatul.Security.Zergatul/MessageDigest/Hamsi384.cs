using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Hamsi384 : Hamsi512
    {
        public override int DigestLength => 48;

        public override void Reset()
        {
            h0 = 0x656B7472;
            h1 = 0x6F746563;
            h2 = 0x686E6965;
            h3 = 0x6B2C2043;
            h4 = 0x6F6D7075;
            h5 = 0x74657220;
            h6 = 0x53656375;
            h7 = 0x72697479;
            h8 = 0x20616E64;
            h9 = 0x20496E64;
            ha = 0x75737472;
            hb = 0x69616C20;
            hc = 0x43727970;
            hd = 0x746F6772;
            he = 0x61706879;
            hf = 0x2C204B61;

            bufOffset = 0;
            count = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[48];
            GetBytes(h0, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.BigEndian, digest, 0x04);
            GetBytes(h3, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h4, ByteOrder.BigEndian, digest, 0x0C);
            GetBytes(h5, ByteOrder.BigEndian, digest, 0x10);
            GetBytes(h6, ByteOrder.BigEndian, digest, 0x14);
            GetBytes(h8, ByteOrder.BigEndian, digest, 0x18);
            GetBytes(h9, ByteOrder.BigEndian, digest, 0x1C);
            GetBytes(ha, ByteOrder.BigEndian, digest, 0x20);
            GetBytes(hc, ByteOrder.BigEndian, digest, 0x24);
            GetBytes(hd, ByteOrder.BigEndian, digest, 0x28);
            GetBytes(hf, ByteOrder.BigEndian, digest, 0x2C);
            return digest;
        }
    }
}