using System;
using Zergatul.Cryptography.Hash.Base;

namespace Zergatul.Cryptography.Hash
{
    public class RIPEMD320 : RIPEMD
    {
        public override int HashSize => 40;

        protected override void Init()
        {
            h0 = 0x67452301;
            h1 = 0xEFCDAB89;
            h2 = 0x98BADCFE;
            h3 = 0x10325476;
            h4 = 0xC3D2E1F0;
            h5 = 0x76543210;
            h6 = 0xFEDCBA98;
            h7 = 0x89ABCDEF;
            h8 = 0x01234567;
            h9 = 0x3C2D1E0F;
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
            uint ap = h5;
            uint bp = h6;
            uint cp = h7;
            uint dp = h8;
            uint ep = h9;

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

                    if (i == 15)
                    {
                        uint buf = b;
                        b = bp;
                        bp = buf;
                    }
                    if (i == 31)
                    {
                        uint buf = d;
                        d = dp;
                        dp = buf;
                    }
                    if (i == 47)
                    {
                        uint buf = a;
                        a = ap;
                        ap = buf;
                    }
                    if (i == 63)
                    {
                        uint buf = c;
                        c = cp;
                        cp = buf;
                    }
                    if (i == 79)
                    {
                        uint buf = e;
                        e = ep;
                        ep = buf;
                    }
                }

            unchecked
            {
                h0 += a;
                h1 += b;
                h2 += c;
                h3 += d;
                h4 += e;
                h5 += ap;
                h6 += bp;
                h7 += cp;
                h8 += dp;
                h9 += ep;
            }
        }
    }
}