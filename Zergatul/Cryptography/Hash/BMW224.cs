using Zergatul.Cryptography.Hash.Base;

namespace Zergatul.Cryptography.Hash
{
    public class BMW224 : BMW_32Bit
    {
        public override int HashSize => 28;

        protected override void Init()
        {
            h[0x0] = 0x00010203;
            h[0x1] = 0x04050607;
            h[0x2] = 0x08090A0B;
            h[0x3] = 0x0C0D0E0F;
            h[0x4] = 0x10111213;
            h[0x5] = 0x14151617;
            h[0x6] = 0x18191A1B;
            h[0x7] = 0x1C1D1E1F;
            h[0x8] = 0x20212223;
            h[0x9] = 0x24252627;
            h[0xA] = 0x28292A2B;
            h[0xB] = 0x2C2D2E2F;
            h[0xC] = 0x30313233;
            h[0xD] = 0x34353637;
            h[0xE] = 0x38393A3B;
            h[0xF] = 0x3C3D3E3F;
        }

        protected override byte[] InternalStateToBytes()
        {
            DoFinal();
            byte[] hash = BitHelper.ToByteArray(h, ByteOrder.LittleEndian);
            return ByteArray.SubArray(hash, 36, 28);
        }
    }
}