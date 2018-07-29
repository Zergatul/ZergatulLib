using Zergatul.Cryptography.Hash.Base;

namespace Zergatul.Cryptography.Hash
{
    public class BMW256 : BMW_32Bit
    {
        public override int HashSize => 32;

        protected override void Init()
        {
            h[0x0] = 0x40414243;
            h[0x1] = 0x44454647;
            h[0x2] = 0x48494A4B;
            h[0x3] = 0x4C4D4E4F;
            h[0x4] = 0x50515253;
            h[0x5] = 0x54555657;
            h[0x6] = 0x58595A5B;
            h[0x7] = 0x5C5D5E5F;
            h[0x8] = 0x60616263;
            h[0x9] = 0x64656667;
            h[0xA] = 0x68696A6B;
            h[0xB] = 0x6C6D6E6F;
            h[0xC] = 0x70717273;
            h[0xD] = 0x74757677;
            h[0xE] = 0x78797A7B;
            h[0xF] = 0x7C7D7E7F;
        }

        protected override byte[] InternalStateToBytes()
        {
            DoFinal();
            byte[] hash = BitHelper.ToByteArray(h, ByteOrder.LittleEndian);
            return ByteArray.SubArray(hash, 32, 32);
        }
    }
}