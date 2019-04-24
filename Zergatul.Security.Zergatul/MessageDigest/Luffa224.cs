using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class Luffa224 : Luffa256
    {
        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[28];
            GetBytes(h00 ^ h10 ^ h20, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h01 ^ h11 ^ h21, ByteOrder.BigEndian, digest, 0x04);
            GetBytes(h02 ^ h12 ^ h22, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h03 ^ h13 ^ h23, ByteOrder.BigEndian, digest, 0x0C);
            GetBytes(h04 ^ h14 ^ h24, ByteOrder.BigEndian, digest, 0x10);
            GetBytes(h05 ^ h15 ^ h25, ByteOrder.BigEndian, digest, 0x14);
            GetBytes(h06 ^ h16 ^ h26, ByteOrder.BigEndian, digest, 0x18);
            return digest;
        }
    }
}