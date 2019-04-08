using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class BMW224 : BMW256
    {
        public override int DigestLength => 28;

        public override void Reset()
        {
            h0 = 0x00010203;
            h1 = 0x04050607;
            h2 = 0x08090A0B;
            h3 = 0x0C0D0E0F;
            h4 = 0x10111213;
            h5 = 0x14151617;
            h6 = 0x18191A1B;
            h7 = 0x1C1D1E1F;
            h8 = 0x20212223;
            h9 = 0x24252627;
            ha = 0x28292A2B;
            hb = 0x2C2D2E2F;
            hc = 0x30313233;
            hd = 0x34353637;
            he = 0x38393A3B;
            hf = 0x3C3D3E3F;
            bufOffset = 0;
            length = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            FinalProcess();

            byte[] digest = new byte[28];
            GetBytes(h9, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(ha, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(hb, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(hc, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(hd, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(he, ByteOrder.LittleEndian, digest, 0x14);
            GetBytes(hf, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}