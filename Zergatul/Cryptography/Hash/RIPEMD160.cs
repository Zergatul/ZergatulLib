using System;
using Zergatul.Cryptography.Hash.Base;

namespace Zergatul.Cryptography.Hash
{
    public class RIPEMD160 : RIPEMD
    {
        public override int HashSize => 20;

        protected override void Init()
        {
            h0 = 0x67452301;
            h1 = 0xEFCDAB89;
            h2 = 0x98BADCFE;
            h3 = 0x10325476;
            h4 = 0xC3D2E1F0;
        }

        private static uint kp(uint j)
        {
            if (j < 16) return 0x50A28BE6;
            if (j < 32) return 0x5C4DD124;
            if (j < 48) return 0x6D703EF3;
            if (j < 64) return 0x7A6D76E9;
            if (j < 80) return 0;
            throw new InvalidOperationException();
        }

        protected override void ProcessBlockInternal()
        {
            uint a = h0;
            uint b = h1;
            uint c = h2;
            uint d = h3;
            uint e = h4;
            uint ap = h0;
            uint bp = h1;
            uint cp = h2;
            uint dp = h3;
            uint ep = h4;

            for (uint i = 0; i < 80; i++)
                unchecked
                {
                    uint t = BitHelper.RotateLeft(a + f(i, b, c, d) + m[r[i]] + k(i), s[i]) + e;
                    a = e;
                    e = d;
                    d = BitHelper.RotateLeft(c, 10);
                    c = b;
                    b = t;

                    t = BitHelper.RotateLeft(ap + f(79 - i, bp, cp, dp) + m[rp[i]] + kp(i), sp[i]) + ep;
                    ap = ep;
                    ep = dp;
                    dp = BitHelper.RotateLeft(cp, 10);
                    cp = bp;
                    bp = t;
                }

            unchecked
            {
                uint t = h1 + c + dp;
                h1 = h2 + d + ep;
                h2 = h3 + e + ap;
                h3 = h4 + a + bp;
                h4 = h0 + b + cp;
                h0 = t;
            }
        }
    }
}