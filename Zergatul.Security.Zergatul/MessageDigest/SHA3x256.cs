using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHA3x256 : Keccak
    {
        public override int DigestLength => 32;

        public SHA3x256()
        {
            padding = 0x06;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[32];
            GetBytes(s00, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s10, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s20, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(s30, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}