using System;
using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.SIMDCommon;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SIMD224 : SIMD256
    {
        public override int DigestLength => 28;

        public override void Reset()
        {
            Array.Copy(IV224, state, 16);

            bufOffset = 0;
            length = 0;
        }

        protected override byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[28];
            for (int i = 0; i < 7; i++)
                GetBytes(state[i], ByteOrder.LittleEndian, digest, i << 2);
            return digest;
        }
    }
}