using Zergatul.Cryptography.Hash.Base;

namespace Zergatul.Cryptography.Hash
{
    public class BMW384 : BMW_64Bit
    {
        public override int HashSize => 48;

        protected override void Init()
        {
            h[0x0] = 0x0001020304050607;
            h[0x1] = 0x08090A0B0C0D0E0F;
            h[0x2] = 0x1011121314151617;
            h[0x3] = 0x18191A1B1C1D1E1F;
            h[0x4] = 0x2021222324252627;
            h[0x5] = 0x28292A2B2C2D2E2F;
            h[0x6] = 0x3031323334353637;
            h[0x7] = 0x38393A3B3C3D3E3F;
            h[0x8] = 0x4041424344454647;
            h[0x9] = 0x48494A4B4C4D4E4F;
            h[0xA] = 0x5051525354555657;
            h[0xB] = 0x58595A5B5C5D5E5F;
            h[0xC] = 0x6061626364656667;
            h[0xD] = 0x68696A6B6C6D6E6F;
            h[0xE] = 0x7071727374757677;
            h[0xF] = 0x78797A7B7C7D7E7F;
        }

        protected override byte[] InternalStateToBytes()
        {
            DoFinal();
            byte[] hash = BitHelper.ToByteArray(h, ByteOrder.LittleEndian);
            return ByteArray.SubArray(hash, 80, HashSize);
        }
    }
}