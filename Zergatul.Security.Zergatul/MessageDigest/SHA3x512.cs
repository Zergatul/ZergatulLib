using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SHA3x512 : Keccak
    {
        public override int DigestLength => 64;

        public SHA3x512()
        {
            padding = 0x06;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[64];
            GetBytes(s00, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(s10, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(s20, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(s30, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(s40, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(s01, ByteOrder.LittleEndian, digest, 0x28);
            GetBytes(s11, ByteOrder.LittleEndian, digest, 0x30);
            GetBytes(s21, ByteOrder.LittleEndian, digest, 0x38);
            return digest;
        }
    }
}