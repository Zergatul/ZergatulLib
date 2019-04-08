using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class BLAKE384 : BLAKE512
    {
        public override int DigestLength => 48;

        public BLAKE384()
        {
            appendBit = false;
        }

        public override void Reset()
        {
            h0 = 0xCBBB9D5DC1059ED8;
            h1 = 0x629A292A367CD507;
            h2 = 0x9159015A3070DD17;
            h3 = 0x152FECD8F70E5939;
            h4 = 0x67332667FFC00B31;
            h5 = 0x8EB44A8768581511;
            h6 = 0xDB0C2E0D64F98FA7;
            h7 = 0x47B5481DBEFA4FA4;
            s0 = 0;
            s1 = 0;
            s2 = 0;
            s3 = 0;

            bufOffset = 0;
            lengthHi = 0;
            lengthLo = 0;
        }

        protected override byte[] InternalStateToBytes()
        {
            byte[] digest = new byte[48];
            GetBytes(h0, ByteOrder.BigEndian, digest, 0x00);
            GetBytes(h1, ByteOrder.BigEndian, digest, 0x08);
            GetBytes(h2, ByteOrder.BigEndian, digest, 0x10);
            GetBytes(h3, ByteOrder.BigEndian, digest, 0x18);
            GetBytes(h4, ByteOrder.BigEndian, digest, 0x20);
            GetBytes(h5, ByteOrder.BigEndian, digest, 0x28);
            return digest;
        }
    }
}