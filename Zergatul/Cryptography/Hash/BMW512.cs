using Zergatul.Cryptography.Hash.Base;

namespace Zergatul.Cryptography.Hash
{
    public class BMW512 : BMW_64Bit
    {
        public override int HashSize => 64;

        protected override void Init()
        {
            h[0x0] = 0x8081828384858687;
            h[0x1] = 0x88898A8B8C8D8E8F;
            h[0x2] = 0x9091929394959697;
            h[0x3] = 0x98999A9B9C9D9E9F;
            h[0x4] = 0xA0A1A2A3A4A5A6A7;
            h[0x5] = 0xA8A9AAABACADAEAF;
            h[0x6] = 0xB0B1B2B3B4B5B6B7;
            h[0x7] = 0xB8B9BABBBCBDBEBF;
            h[0x8] = 0xC0C1C2C3C4C5C6C7;
            h[0x9] = 0xC8C9CACBCCCDCECF;
            h[0xA] = 0xD0D1D2D3D4D5D6D7;
            h[0xB] = 0xD8D9DADBDCDDDEDF;
            h[0xC] = 0xE0E1E2E3E4E5E6E7;
            h[0xD] = 0xE8E9EAEBECEDEEEF;
            h[0xE] = 0xF0F1F2F3F4F5F6F7;
            h[0xF] = 0xF8F9FAFBFCFDFEFF;
        }

        protected override byte[] InternalStateToBytes()
        {
            DoFinal();
            byte[] hash = BitHelper.ToByteArray(h, ByteOrder.LittleEndian);
            return ByteArray.SubArray(hash, 64, HashSize);
        }
    }
}