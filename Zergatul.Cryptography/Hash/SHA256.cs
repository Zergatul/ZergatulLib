using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class SHA256 : AbstractHash
    {
        public override int BlockSize => 64;

        public override int HashSize => 32;

        private static uint[] k = new uint[]
        {
            0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5, 0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
            0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3, 0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
            0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC, 0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
            0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7, 0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
            0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13, 0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
            0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3, 0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
            0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5, 0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
            0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208, 0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2
        };

        uint h0;
        uint h1;
        uint h2;
        uint h3;
        uint h4;
        uint h5;
        uint h6;
        uint h7;

        uint[] w = new uint[64];

        protected override void Init()
        {
            h0 = 0x6A09E667;
            h1 = 0xBB67AE85;
            h2 = 0x3C6EF372;
            h3 = 0xA54FF53A;
            h4 = 0x510E527F;
            h5 = 0x9B05688C;
            h6 = 0x1F83D9AB;
            h7 = 0x5BE0CD19;
        }

        protected override void AddPadding()
        {
            _buffer.Add(0x80);
            while ((_buffer.Count + 8) % 64 != 0)
                _buffer.Add(0);
            _buffer.AddRange(BitHelper.GetBytes(_totalBytes * 8, ByteOrder.BigEndian));
        }

        protected override void ProcessBlock()
        {
            for (int i = 0; i < 16; i++)
                w[i] = BitHelper.ToUInt32(_block, i * 4, ByteOrder.BigEndian);

            for (int i = 16; i < 64; i++)
            {
                uint s0 = BitHelper.RotateRight(w[i - 15], 7) ^ BitHelper.RotateRight(w[i - 15], 18) ^ (w[i - 15] >> 3);
                uint s1 = BitHelper.RotateRight(w[i - 2], 17) ^ BitHelper.RotateRight(w[i - 2], 19) ^ (w[i - 2] >> 10);
                w[i] = unchecked(w[i - 16] + s0 + w[i - 7] + s1);
            }

            uint a = h0;
            uint b = h1;
            uint c = h2;
            uint d = h3;
            uint e = h4;
            uint f = h5;
            uint g = h6;
            uint h = h7;

            for (int i = 0; i < 64; i++)
            {
                uint s1 = BitHelper.RotateRight(e, 6) ^ BitHelper.RotateRight(e, 11) ^ BitHelper.RotateRight(e, 25);
                uint ch = (e & f) ^ ((~e) & g);
                uint temp1 = unchecked(h + s1 + ch + k[i] + w[i]);

                uint s0 = BitHelper.RotateRight(a, 2) ^ BitHelper.RotateRight(a, 13) ^ BitHelper.RotateRight(a, 22);
                uint maj = (a & b) ^ (a & c) ^ (b & c);
                uint temp2 = unchecked(s0 + maj);

                h = g;
                g = f;
                f = e;
                e = unchecked(d + temp1);
                d = c;
                c = b;
                b = a;
                a = unchecked(temp1 + temp2);
            }

            h0 = unchecked(h0 + a);
            h1 = unchecked(h1 + b);
            h2 = unchecked(h2 + c);
            h3 = unchecked(h3 + d);
            h4 = unchecked(h4 + e);
            h5 = unchecked(h5 + f);
            h6 = unchecked(h6 + g);
            h7 = unchecked(h7 + h);
        }

        protected override byte[] InternalStateToBytes()
        {
            var list = new List<byte>(HashSize);
            list.AddRange(BitHelper.GetBytes(h0, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h1, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h2, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h3, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h4, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h5, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h6, ByteOrder.BigEndian));
            list.AddRange(BitHelper.GetBytes(h7, ByteOrder.BigEndian));
            return list.ToArray();
        }
    }
}
