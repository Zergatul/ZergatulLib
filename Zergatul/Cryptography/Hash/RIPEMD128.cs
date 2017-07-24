using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class RIPEMD128 : RIPEMD
    {
        public RIPEMD128()
            : base(16)
        {

        }

        protected override void Init()
        {
            h0 = 0x67452301;
            h1 = 0xEFCDAB89;
            h2 = 0x98BADCFE;
            h3 = 0x10325476;
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
            uint ap = h0;
            uint bp = h1;
            uint cp = h2;
            uint dp = h3;

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
                }

            unchecked
            {
                uint t = h1 + c + dp;
                h1 = h2 + d + ap;
                h2 = h3 + a + bp;
                h3 = h0 + b + cp;
                h0 = t;
            }
        }
    }
}
