using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public class DES : AbstractBlockCipher
    {
        public override int BlockSize => 8;
        public override int KeySize => 8;

        private static readonly int[] PC1 = new int[]
        {
            49, 42, 35, 28, 21, 14,  7,  0,
            50, 43, 36, 29, 22, 15,  8,  1,
            51, 44, 37, 30, 23, 16,  9,  2,
            52, 45, 38, 31, 55, 48, 41, 34,
            27, 20, 13,  6, 54, 47, 40, 33,
            26, 19, 12,  5, 53, 46, 39, 32,
            25, 18, 11,  4, 24, 17, 10,  3
        };

        private static readonly int[] PC2 = new int[]
        {
            13, 16, 10, 23,  0,  4,  2, 27,
            14,  5, 20,  9, 22, 18, 11,  3,
            25,  7, 15,  6, 26, 19, 12,  1,
            40, 51, 30, 36, 46, 54, 29, 39,
            50, 44, 32, 47, 43, 48, 38, 55,
            33, 52, 45, 41, 49, 35, 28, 31
        };

        private static readonly int[] Shift = new int[]
        {
            1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
        };

        private static readonly int[] IP = new int[]
        {
            57, 49, 41, 33, 25, 17,  9,  1,
            59, 51, 43, 35, 27, 19, 11,  3,
            61, 53, 45, 37, 29, 21, 13,  5,
            63, 55, 47, 39, 31, 23, 15,  7,
            56, 48, 40, 32, 24, 16,  8,  0,
            58, 50, 42, 34, 26, 18, 10,  2,
            60, 52, 44, 36, 28, 20, 12,  4,
            62, 54, 46, 38, 30, 22, 14,  6
        };

        private static readonly int[] IPInv = new int[]
        {
            39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29, 36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25, 32, 0, 40, 8, 48, 16, 56, 24
        };

        private static readonly int[] EBit = new int[]
        {
            31,  0,  1,  2,  3,  4,  3,  4,
             5,  6,  7,  8,  7,  8,  9, 10,
            11, 12, 11, 12, 13, 14, 15, 16,
            15, 16, 17, 18, 19, 20, 19, 20,
            21, 22, 23, 24, 23, 24, 25, 26,
            27, 28, 27, 28, 29, 30, 31,  0
        };

        private static readonly uint[] S1 = new uint[]
        {
            14,  4,  13,  1,   2, 15,  11,  8,   3, 10,   6, 12,   5,  9,   0,  7,
            0, 15,   7,  4,  14,  2,  13,  1,  10,  6,  12, 11,   9,  5,   3,  8,
            4,  1,  14,  8,  13,  6,   2, 11,  15, 12,   9,  7,   3, 10,   5,  0,
            15, 12,   8,  2,   4,  9,   1,  7,   5, 11,   3, 14,  10,  0,   6, 13,
        };

        private static readonly uint[] S2 = new uint[]
        {
            15,  1,   8, 14,   6, 11,   3,  4,   9,  7,   2, 13,  12,  0,   5, 10,
      3, 13,   4,  7,  15,  2,   8, 14,  12,  0,   1, 10,   6,  9,  11,  5,
      0, 14,   7, 11,  10,  4,  13,  1,   5,  8,  12,  6,   9,  3,   2, 15,
     13,  8,  10,  1,   3, 15,   4,  2,  11,  6,   7, 12,   0,  5,  14,  9,
        };

        private static readonly uint[] S3 = new uint[]
        {
            10,  0,   9, 14,   6,  3,  15,  5,   1, 13,  12,  7,  11,  4,   2,  8,
     13,  7,   0,  9,   3,  4,   6, 10,   2,  8,   5, 14,  12, 11,  15,  1,
     13,  6,   4,  9,   8, 15,   3,  0,  11,  1,   2, 12,   5, 10,  14,  7,
      1, 10,  13,  0,   6,  9,   8,  7,   4, 15,  14,  3,  11,  5,   2, 12,
        };

        private static readonly uint[] S4 = new uint[]
        {
              7, 13,  14,  3,   0,  6,   9, 10,   1,  2,   8,  5,  11, 12,   4, 15,
     13,  8,  11,  5,   6, 15,   0,  3,   4,  7,   2, 12,   1, 10,  14,  9,
     10,  6,   9,  0,  12, 11,   7, 13,  15,  1,   3, 14,   5,  2,   8,  4,
      3, 15,   0,  6,  10,  1,  13,  8,   9,  4,   5, 11,  12,  7,   2, 14,
        };

        private static readonly uint[] S5 = new uint[]
        {
            2, 12,   4,  1,   7, 10,  11,  6,   8,  5,   3, 15,  13,  0,  14,  9,
     14, 11,   2, 12,   4,  7,  13,  1,   5,  0,  15, 10,   3,  9,   8,  6,
      4,  2,   1, 11,  10, 13,   7,  8,  15,  9,  12,  5,   6,  3,   0, 14,
     11,  8,  12,  7,   1, 14,   2, 13,   6, 15,   0,  9,  10,  4,   5,  3,
        };

        private static readonly uint[] S6 = new uint[]
        {
            12,  1,  10, 15,   9,  2,   6,  8,   0, 13,   3,  4,  14,  7,   5, 11,
     10, 15,   4,  2,   7, 12,   9,  5,   6,  1,  13, 14,   0, 11,   3,  8,
      9, 14,  15,  5,   2,  8,  12,  3,   7,  0,   4, 10,   1, 13,  11,  6,
      4,  3,   2, 12,   9,  5,  15, 10,  11, 14,   1,  7,   6,  0,   8, 13,
        };

        private static readonly uint[] S7 = new uint[]
        {
            4, 11,   2, 14,  15,  0,   8, 13,   3, 12,   9,  7,   5, 10,   6,  1,
     13,  0,  11,  7,   4,  9,   1, 10,  14,  3,   5, 12,   2, 15,   8,  6,
      1,  4,  11, 13,  12,  3,   7, 14,  10, 15,   6,  8,   0,  5,   9,  2,
      6, 11,  13,  8,   1,  4,  10,  7,   9,  5,   0, 15,  14,  2,   3, 12,
        };

        private static readonly uint[] S8 = new uint[]
        {
            13,  2,   8,  4,   6, 15,  11,  1,  10,  9,   3, 14,   5,  0,  12,  7,
      1, 15,  13,  8,  10,  3,   7,  4,  12,  5,   6, 11,   0, 14,   9,  2,
      7, 11,   4,  1,   9, 12,  14,  2,   0,  6,  10, 13,  15,  3,   5,  8,
      2,  1,  14,  7,   4, 10,   8, 13,  15, 12,   9,  0,   3,  5,   6, 11,
        };

        private static readonly int[] P = new int[]
        {
            15, 6, 19, 20, 28, 11, 27, 16, 0, 14, 22, 25, 4, 17, 30, 9, 1, 7, 23, 13, 31, 26, 2, 8, 18, 12, 29, 5, 21, 10, 3, 24
        };

        private static ulong[] KeyExpansion(byte[] key)
        {
            bool[] kb = new bool[56];
            for (int bit = 0; bit < 56; bit++)
            {
                int bitExp = (bit / 7) * 8 + bit % 7;
                kb[bit] = (key[bitExp / 8] & (1 << (7 - (bitExp % 8)))) != 0;
            }

            bool[] kbp = new bool[56];
            for (int i = 0; i < 56; i++)
                kbp[i] = kb[PC1[i]];

            uint[] c = new uint[17];
            uint[] d = new uint[17];

            for (int i = 0; i < 28; i++)
            {
                if (kbp[i])
                    c[0] |= (1U << (27 - i));
                if (kbp[28 + i])
                    d[0] |= (1U << (27 - i));
            }

            for (int i = 1; i <= 16; i++)
            {
                int bits = Shift[i - 1];
                c[i] = ((c[i - 1] << bits) | (c[i - 1] >> (28 - bits))) & 0xFFFFFFF;
                d[i] = ((d[i - 1] << bits) | (d[i - 1] >> (28 - bits))) & 0xFFFFFFF;
            }

            ulong[] k = new ulong[16];
            for (int i = 0; i < 16; i++)
            {
                for (int bit = 0; bit < 48; bit++)
                {
                    int refbit = PC2[bit];
                    bool set = refbit < 28 ? (c[i + 1] & (1 << (27 - refbit))) != 0 : (d[i + 1] & (1 << (55 - refbit))) != 0;
                    if (set)
                        k[i] |= 1UL << (47 - bit);
                }
            }

            return k;
        }

        private static int SI(ulong index)
        {
            int row = (int)(((index >> 4) & 0x02) | (index & 0x01));
            int col = (int)((index >> 1) & 0x0F);
            return row * 16 + col;
        }

        private static uint F(uint r, ulong k)
        {
            ulong e = 0;
            for (int bit = 0; bit < 48; bit++)
                if ((r & (1U << (31 - EBit[bit]))) != 0)
                    e |= 1UL << (47 - bit);

            ulong x = e ^ k;

            r =
                (S1[SI((x >> 42) & 0x3F)] << 28) |
                (S2[SI((x >> 36) & 0x3F)] << 24) |
                (S3[SI((x >> 30) & 0x3F)] << 20) |
                (S4[SI((x >> 24) & 0x3F)] << 16) |
                (S5[SI((x >> 18) & 0x3F)] << 12) |
                (S6[SI((x >> 12) & 0x3F)] << 08) |
                (S7[SI((x >> 06) & 0x3F)] << 04) |
                (S8[SI((x >> 00) & 0x3F)] << 00);

            uint rp = 0;
            for (int bit = 0; bit < 32; bit++)
                if ((r & (1U << (31 - P[bit]))) != 0)
                    rp |= 1U << (31 - bit);

            return rp;
        }

        public override Func<byte[], byte[]> CreateEncryptor(byte[] key)
        {
            ulong[] K = KeyExpansion(key);

            return (block) =>
            {
                ulong m = BitHelper.ToUInt64(block, 0, ByteOrder.BigEndian);

                ulong mp = 0;
                for (int bit = 0; bit < 64; bit++)
                    if ((m & (1UL << (63 - IP[bit]))) != 0)
                        mp |= 1UL << (63 - bit);

                uint l = (uint)(mp >> 32);
                uint r = (uint)mp;

                for (int n = 0; n < 16; n++)
                {
                    uint lprev = l;
                    l = r;
                    r = lprev ^ F(r, K[n]);
                }

                m = ((ulong)r << 32) | l;
                mp = 0;
                for (int bit = 0; bit < 64; bit++)
                    if ((m & (1UL << (63 - IPInv[bit]))) != 0)
                        mp |= 1UL << (63 - bit);

                return BitHelper.GetBytes(mp, ByteOrder.BigEndian);
            };
        }

        public override Func<byte[], byte[]> CreateDecryptor(byte[] key)
        {
            ulong[] K = KeyExpansion(key);

            return (block) =>
            {
                ulong m = BitHelper.ToUInt64(block, 0, ByteOrder.BigEndian);

                ulong mp = 0;
                for (int bit = 0; bit < 64; bit++)
                    if ((m & (1UL << (63 - IP[bit]))) != 0)
                        mp |= 1UL << (63 - bit);

                uint l = (uint)(mp >> 32);
                uint r = (uint)mp;

                for (int n = 15; n >= 0; n--)
                {
                    uint lprev = l;
                    l = r;
                    r = lprev ^ F(r, K[n]);
                }

                m = ((ulong)r << 32) | l;
                mp = 0;
                for (int bit = 0; bit < 64; bit++)
                    if ((m & (1UL << (63 - IPInv[bit]))) != 0)
                        mp |= 1UL << (63 - bit);

                return BitHelper.GetBytes(mp, ByteOrder.BigEndian);
            };
        }
    }
}