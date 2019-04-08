using static Zergatul.BitHelper;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class JH224 : JH
    {
        public override int DigestLength => 28;

        public override void Reset()
        {
            s0 = 0xAC989AF962DDFE2D;
            s1 = 0xE734D619D6AC7CAE;
            s2 = 0x161230BC051083A4;
            s3 = 0x941466C9C63860B8;
            s4 = 0x6F7080259F89D966;
            s5 = 0xDC1A9B1D1BA39ECE;
            s6 = 0x106E367B5F32E811;
            s7 = 0xC106FA027F8594F9;
            s8 = 0xB340C8D85C1B4F1B;
            s9 = 0x9980736E7FA1F697;
            sa = 0xD3A3EAADA593DFDC;
            sb = 0x689A53C9DEE831A4;
            sc = 0xE4A186EC8AA9B422;
            sd = 0xF06CE59C95AC74D5;
            se = 0xBF2BABB5EA0D9615;
            sf = 0x6EEA64DDF0DC1196;

            bufOffset = 0;
            blockCount = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[28];
            digest[0] = (byte)(sc >> 32);
            digest[1] = (byte)(sc >> 40);
            digest[2] = (byte)(sc >> 48);
            digest[3] = (byte)(sc >> 56);
            GetBytes(sd, ByteOrder.LittleEndian, digest, 0x04);
            GetBytes(se, ByteOrder.LittleEndian, digest, 0x0C);
            GetBytes(sf, ByteOrder.LittleEndian, digest, 0x14);
            return digest;
        }
    }
}