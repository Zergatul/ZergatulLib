
using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class ECHO224 : ECHO256
    {
        public override int DigestLength => 28;

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[28];
            GetBytes(v0, ByteOrder.LittleEndian, digest, 0x00);
            GetBytes(v1, ByteOrder.LittleEndian, digest, 0x08);
            GetBytes(v2, ByteOrder.LittleEndian, digest, 0x10);
            GetBytes((uint)v3, ByteOrder.LittleEndian, digest, 0x18);
            return digest;
        }
    }
}