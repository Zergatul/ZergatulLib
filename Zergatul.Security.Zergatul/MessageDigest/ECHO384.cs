using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class ECHO384 : ECHO512
    {
        public override int DigestLength => 48;

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[48];
            GetBytes(v0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(v1, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(v2, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes(v3, ByteOrder.LittleEndian, digest, 0x18);
            GetBytes(v4, ByteOrder.LittleEndian, digest, 0x20);
            GetBytes(v5, ByteOrder.LittleEndian, digest, 0x28);
            return digest;
        }
    }
}