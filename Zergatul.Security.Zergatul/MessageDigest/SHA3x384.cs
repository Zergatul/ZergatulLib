using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHA3x384 : Keccak
    {
        public override int DigestLength => 48;

        public SHA3x384()
        {
            padding = 0x06;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[48];
            GetBytes(s00, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s10, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s20, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(s30, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(s40, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(s01, ByteOrder.LittleEndian, digest, 0x28);
            return digest;
        }
    }
}