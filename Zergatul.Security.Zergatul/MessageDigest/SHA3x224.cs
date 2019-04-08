using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHA3x224 : Keccak
    {
        public override int DigestLength => 28;

        public SHA3x224()
        {
            padding = 0x06;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[28];
            GetBytes(s00, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s10, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s20, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes((uint)s30, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}