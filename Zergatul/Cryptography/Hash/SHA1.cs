using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class SHA1 : AbstractHash
    {
        public override int BlockSize => 64;
        public override int HashSize => 20;
        public override OID OID => OID.ISO.IdentifiedOrganization.OIW.SECSIG.Algorithms.SHA1;

        uint h0;
        uint h1;
        uint h2;
        uint h3;
        uint h4;

        uint[] w = new uint[80];

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
            _buffer.AddRange(BitHelper.GetBytes(_totalBytes * 8, ByteOrder.BigEndian));
        }

        protected override void ProcessBlock()
        {
            for (int i = 0; i < 16; i++)
                w[i] = BitHelper.ToUInt32(_block, i * 4, ByteOrder.BigEndian);

            for (int i = 16; i < 80; i++)
                w[i] = BitHelper.RotateLeft(w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16], 1);

            uint a = h0;
            uint b = h1;
            uint c = h2;
            uint d = h3;
            uint e = h4;

            for (int i = 0; i < 80; i++)
            {
                uint f;
                uint k;
                if (i < 20)
                {
                    f = (b & c) | (~b & d);
                    k = 0x5A827999;
                }
                else if (i < 40)
                {
                    f = b ^ c ^ d;
                    k = 0x6ED9EBA1;
                }
                else if (i < 60)
                {
                    f = (b & c) | (b & d) | (c & d);
                    k = 0x8F1BBCDC;
                }
                else
                {
                    f = b ^ c ^ d;
                    k = 0xCA62C1D6;
                }

                uint temp = unchecked(BitHelper.RotateLeft(a, 5) + f + e + k + w[i]);
                e = d;
                d = c;
                c = BitHelper.RotateLeft(b, 30);
                b = a;
                a = temp;
            }

            h0 = unchecked(h0 + a);
            h1 = unchecked(h1 + b);
            h2 = unchecked(h2 + c);
            h3 = unchecked(h3 + d);
            h4 = unchecked(h4 + e);
        }

        protected override byte[] InternalStateToBytes()
        {
            byte[] hash = new byte[HashSize];
            BitHelper.GetBytes(h0, ByteOrder.BigEndian, hash,  0);
            BitHelper.GetBytes(h1, ByteOrder.BigEndian, hash,  4);
            BitHelper.GetBytes(h2, ByteOrder.BigEndian, hash,  8);
            BitHelper.GetBytes(h3, ByteOrder.BigEndian, hash, 12);
            BitHelper.GetBytes(h4, ByteOrder.BigEndian, hash, 16);
            return hash;
        }
    }
}