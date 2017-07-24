using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class RIPEMD256 : RIPEMD
    {
        public override int HashSize => 32;

        protected override void Init()
        {
            h0 = 0x67452301;
            h1 = 0xEFCDAB89;
            h2 = 0x98BADCFE;
            h3 = 0x10325476;
            h4 = 0x76543210;
            h5 = 0xFEDCBA98;
            h6 = 0x89ABCDEF;
            h7 = 0x01234567;
        }

        private static uint kp(uint j)
        {
            if (j < 16) return 0x50A28BE6;
            if (j < 32) return 0x5C4DD124;
            if (j < 48) return 0x6D703EF3;
            if (j < 64) return 0;
            throw new InvalidOperationException();
        }

        protected override void ProcessBlockInternal()
        {
            uint a = h0;
            uint b = h1;
            uint c = h2;
            uint d = h3;
            uint ap = h4;
            uint bp = h5;
            uint cp = h6;
            uint dp = h7;

            for (uint i = 0; i < 64; i++)
                unchecked
                {
                    uint t = BitHelper.RotateLeft(a + f(i, b, c, d) + m[r[i]] + k(i), s[i]);
                    a = d;
                    d = c;
                    c = b;
                    b = t;

                    t = BitHelper.RotateLeft(ap + f(63 - i, bp, cp, dp) + m[rp[i]] + kp(i), sp[i]);
                    ap = dp;
                    dp = cp;
                    cp = bp;
                    bp = t;

                    if (i == 15)
                    {
                        uint buf = a;
                        a = ap;
                        ap = buf;
                    }
                    if (i == 31)
                    {
                        uint buf = b;
                        b = bp;
                        bp = buf;
                    }
                    if (i == 47)
                    {
                        uint buf = c;
                        c = cp;
                        cp = buf;
                    }
                    if (i == 63)
                    {
                        uint buf = d;
                        d = dp;
                        dp = buf;
                    }
                }

            unchecked
            {
                h0 += a;
                h1 += b;
                h2 += c;
                h3 += d;
                h4 += ap;
                h5 += bp;
                h6 += cp;
                h7 += dp;
            }
        }
    }
}
