using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class RIPEMD160 : AbstractHash
    {
        public override int BlockSize => 64;
        public override int HashSize => 20;

        private static readonly byte[] r = new byte[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            7, 4, 13, 1, 10, 6, 15, 3, 12, 0, 9, 5, 2, 14, 11, 8,
            3, 10, 14, 4, 9, 15, 8, 1, 2, 7, 0, 6, 13, 11, 5, 12,
            1, 9, 11, 10, 0, 8, 12, 4, 13, 3, 7, 15, 14, 5, 6, 2,
            4, 0, 5, 9, 7, 12, 2, 10, 14, 1, 3, 8, 11, 6, 15, 13
        };

        private static readonly byte[] rp = new byte[]
        {
            5, 14, 7, 0, 9, 2, 11, 4, 13, 6, 15, 8, 1, 10, 3, 12,
            6, 11, 3, 7, 0, 13, 5, 10, 14, 15, 8, 12, 4, 9, 1, 2,
            15, 5, 1, 3, 7, 14, 6, 9, 11, 8, 12, 2, 10, 0, 4, 13,
            8, 6, 4, 1, 3, 11, 15, 0, 5, 12, 2, 13, 9, 7, 10, 14,
            12, 15, 10, 4, 1, 5, 8, 7, 6, 2, 13, 14, 0, 3, 9, 11
        };

        private static readonly byte[] s = new byte[]
        {
            11, 14, 15, 12, 5, 8, 7, 9, 11, 13, 14, 15, 6, 7, 9, 8,
            7, 6, 8, 13, 11, 9, 7, 15, 7, 12, 15, 9, 11, 7, 13, 12,
            11, 13, 6, 7, 14, 9, 13, 15, 14, 8, 13, 6, 5, 12, 7, 5,
            11, 12, 14, 15, 14, 15, 9, 8, 9, 14, 5, 6, 8, 6, 5, 12,
            9, 15, 5, 11, 6, 8, 13, 12, 5, 12, 13, 14, 11, 8, 5, 6
        };

        private static readonly byte[] sp = new byte[]
        {
            8, 9, 9, 11, 13, 15, 15, 5, 7, 7, 8, 11, 14, 14, 12, 6,
            9, 13, 15, 7, 12, 8, 9, 11, 7, 7, 12, 7, 6, 15, 13, 11,
            9, 7, 15, 11, 8, 6, 6, 14, 12, 13, 5, 14, 13, 13, 7, 5,
            15, 5, 8, 11, 14, 14, 6, 14, 6, 9, 12, 9, 12, 5, 15, 8,
            8, 5, 12, 9, 12, 5, 14, 6, 8, 13, 6, 5, 15, 13, 11, 11
        };

        uint h0, h1, h2, h3, h4;
        uint[] m = new uint[16];

        protected override void Init()
        {
            h0 = 0x67452301;
            h1 = 0xEFCDAB89;
            h2 = 0x98BADCFE;
            h3 = 0x10325476;
            h4 = 0xC3D2E1F0;
        }

        protected override void AddPadding()
        {
            _buffer.Add(0x80);
            while ((_buffer.Count + 8) % 64 != 0)
                _buffer.Add(0);
            ulong totalBits = _totalBytes * 8;
            _buffer.AddRange(BitHelper.GetBytes((uint)totalBits, ByteOrder.LittleEndian));
            _buffer.AddRange(BitHelper.GetBytes((uint)(totalBits >> 32), ByteOrder.LittleEndian));
        }

        private static uint f(uint j, uint x, uint y, uint z)
        {
            if (j < 16) return x ^ y ^ z;
            if (j < 32) return (x & y) | (~x & z);
            if (j < 48) return (x | ~y) ^ z;
            if (j < 64) return (x & z) | (y & ~z);
            if (j < 80) return x ^ (y | ~z);
            throw new InvalidOperationException();
        }

        private static uint k(uint j)
        {
            if (j < 16) return 0;
            if (j < 32) return 0x5A827999;
            if (j < 48) return 0x6ED9EBA1;
            if (j < 64) return 0x8F1BBCDC;
            if (j < 80) return 0xA953FD4E;
            throw new InvalidOperationException();
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

        protected override void ProcessBlock()
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

            for (int i = 0; i < 16; i++)
                m[i] = BitHelper.ToUInt32(_block, i * 4, ByteOrder.LittleEndian);

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

        protected override byte[] InternalStateToBytes()
        {
            var list = new List<byte>(HashSize);
            list.AddRange(BitHelper.GetBytes(h0, ByteOrder.LittleEndian));
            list.AddRange(BitHelper.GetBytes(h1, ByteOrder.LittleEndian));
            list.AddRange(BitHelper.GetBytes(h2, ByteOrder.LittleEndian));
            list.AddRange(BitHelper.GetBytes(h3, ByteOrder.LittleEndian));
            list.AddRange(BitHelper.GetBytes(h4, ByteOrder.LittleEndian));
            return list.ToArray();
        }
    }
}