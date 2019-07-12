using System;
using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.SIMDCommon;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SIMD384 : SIMD512
    {
        public override int DigestLength => 48;

        public override void Reset()
        {
            Array.Copy(IV384, state, 32);

            bufOffset = 0;
            length = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[48];
            for (int i = 0; i < 12; i++)
                GetBytes(state[i], ByteOrder.LittleEndian, digest, i << 2);
            return digest;
        }
    }
}