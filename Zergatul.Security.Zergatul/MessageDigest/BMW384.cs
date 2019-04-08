using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class BMW384 : BMW512
    {
        public override int DigestLength => 48;

        public override void Reset()
        {
            h0 = 0x0001020304050607;
            h1 = 0x08090A0B0C0D0E0F;
            h2 = 0x1011121314151617;
            h3 = 0x18191A1B1C1D1E1F;
            h4 = 0x2021222324252627;
            h5 = 0x28292A2B2C2D2E2F;
            h6 = 0x3031323334353637;
            h7 = 0x38393A3B3C3D3E3F;
            h8 = 0x4041424344454647;
            h9 = 0x48494A4B4C4D4E4F;
            ha = 0x5051525354555657;
            hb = 0x58595A5B5C5D5E5F;
            hc = 0x6061626364656667;
            hd = 0x68696A6B6C6D6E6F;
            he = 0x7071727374757677;
            hf = 0x78797A7B7C7D7E7F;
            bufOffset = 0;
            length = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            FinalProcess();

            byte[] digest = new byte[48];
            GetBytes(ha, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(hb, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(hc, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(hd, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(he, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(hf, ByteOrder.LittleEndian, digest, 0x28);
            return digest;
        }
    }
}