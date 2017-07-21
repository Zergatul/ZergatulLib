using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public class MD5 : AbstractHash
    {
        public override int BlockSize => 64;
        public override int HashSize => 20;

        private static int[] s = new int[]
        {
            7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
            5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
            4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
            6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
        };

        private static uint[] k = new uint[]
        {
            0xD76AA478, 0xE8C7B756, 0x242070DB, 0xC1BDCEEE,
            0xF57C0FAF, 0x4787C62A, 0xA8304613, 0xFD469501,
            0x698098D8, 0x8B44F7AF, 0xFFFF5BB1, 0x895CD7BE,
            0x6B901122, 0xFD987193, 0xA679438E, 0x49B40821,
            0xF61E2562, 0xC040B340, 0x265E5A51, 0xE9B6C7AA,
            0xD62F105D, 0x02441453, 0xD8A1E681, 0xE7D3FBC8,
            0x21E1CDE6, 0xC33707D6, 0xF4D50D87, 0x455A14ED,
            0xA9E3E905, 0xFCEFA3F8, 0x676F02D9, 0x8D2A4C8A,
            0xFFFA3942, 0x8771F681, 0x6D9D6122, 0xFDE5380C,
            0xA4BEEA44, 0x4BDECFA9, 0xF6BB4B60, 0xBEBFBC70,
            0x289B7EC6, 0xEAA127FA, 0xD4EF3085, 0x04881D05,
            0xD9D4D039, 0xE6DB99E5, 0x1FA27CF8, 0xC4AC5665,
            0xF4292244, 0x432AFF97, 0xAB9423A7, 0xFC93A039,
            0x655B59C3, 0x8F0CCC92, 0xFFEFF47D, 0x85845DD1,
            0x6FA87E4F, 0xFE2CE6E0, 0xA3014314, 0x4E0811A1,
            0xF7537E82, 0xBD3AF235, 0x2AD7D2BB, 0xEB86D391
        };

        uint a0;
        uint b0;
        uint c0;
        uint d0;

        uint[] m = new uint[16];

        protected override void Init()
        {
            a0 = 0x67452301;
            b0 = 0xEFCDAB89;
            c0 = 0x98BADCFE;
            d0 = 0x10325476;
        }

        protected override void AddPadding()
        {
            _buffer.Add(0x80);
            while ((_buffer.Count + 8) % 64 != 0)
                _buffer.Add(0);
            _buffer.AddRange(BitHelper.GetBytes(_totalBytes * 8, ByteOrder.LittleEndian));
        }

        protected override void ProcessBlock()
        {
            for (int i = 0; i < 16; i++)
                m[i] = BitHelper.ToUInt32(_block, i * 4, ByteOrder.LittleEndian);

            uint a = a0;
            uint b = b0;
            uint c = c0;
            uint d = d0;

            for (uint i = 0; i < 64; i++)
            {
                uint f, g;
                if (i < 16)
                {
                    f = (b & c) | (~b & d);
                    g = i;
                }
                else if (i < 32)
                {
                    f = (d & b) | (~d & c);
                    g = (5 * i + 1) % 16;
                }
                else if (i < 48)
                {
                    f = b ^ c ^ d;
                    g = (3 * i + 5) % 16;
                }
                else
                {
                    f = c ^ (b | ~d);
                    g = (7 * i) % 16;
                }
                f = unchecked(f + a + k[i] + m[g]);
                a = d;
                d = c;
                c = b;
                b = unchecked(b + BitHelper.RotateLeft(f, s[i]));
            }

            a0 = unchecked(a0 + a);
            b0 = unchecked(b0 + b);
            c0 = unchecked(c0 + c);
            d0 = unchecked(d0 + d);
        }

        protected override byte[] InternalStateToBytes()
        {
            var list = new List<byte>(HashSize);
            list.AddRange(BitHelper.GetBytes(a0, ByteOrder.LittleEndian));
            list.AddRange(BitHelper.GetBytes(b0, ByteOrder.LittleEndian));
            list.AddRange(BitHelper.GetBytes(c0, ByteOrder.LittleEndian));
            list.AddRange(BitHelper.GetBytes(d0, ByteOrder.LittleEndian));
            return list.ToArray();
        }
    }
}