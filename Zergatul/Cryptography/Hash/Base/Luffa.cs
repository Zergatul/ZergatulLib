using System;

namespace Zergatul.Cryptography.Hash.Base
{
    public abstract class Luffa : AbstractHash
    {
        public override int BlockSize => 32;

        private static readonly uint[][] V = new uint[][]
        {
            new uint[]
            {
                0x6D251E69, 0x44B051E0, 0x4EAA6FB4, 0xDBF78465,
                0x6E292011, 0x90152DF4, 0xEE058139, 0xDEF610BB,
            },
            new uint[]
            {
                0xC3B44B95, 0xD9D2F256, 0x70EEE9A0, 0xDE099FA3,
                0x5D9B0557, 0x8FC944B3, 0xCF1CCF0E, 0x746CD581,
            },
            new uint[]
            {
                0xF7EFC89D, 0x5DBA5781, 0x04016CE5, 0xAD659C05,
                0x0306194F, 0x666D1836, 0x24AA230A, 0x8B264AE7,
            },
            new uint[]
            {
                0x858075D5, 0x36D79CCE, 0xE571F7D7, 0x204B1F67,
                0x35870C6A, 0x57E9E923, 0x14BCB808, 0x7CDE72CE,
            },
            new uint[]
            {
                0x6C68E9BE, 0x5EC41E22, 0xC825B7C7, 0xAFFB4363,
                0xF5DF3999, 0x0FC688F1, 0xB07224CC, 0x03E86CEA,
            }
        };

        private static readonly ulong[] RCW010 = new ulong[]
        {
            0xb6de10ed303994a6, 0x70f47aaec0e65299,
            0x0707a3d46cc33a12, 0x1c1e8f51dc56983e,
            0x707a3d451e00108f, 0xaeb285627800423d,
            0xbaca15898f5b7882, 0x40a46f3e96e1db12
        };

        private static readonly ulong[] RCW014 = new ulong[]
        {
            0x01685f3de0337818, 0x05a17cf4441ba90d,
            0xbd09caca7f34d442, 0xf4272b289389217f,
            0x144ae5cce5a8bce6, 0xfaa7ae2b5274baf4,
            0x2e48f1c126889ba7, 0xb923c7049a226e9d
        };

        private uint[][] v;
        private uint[] m;

        protected Luffa()
        {
            v = new uint[5][];
            for (int i = 0; i < 5; i++)
                v[i] = new uint[8];

            m = new uint[8];
        }

        protected override void Init()
        {
            for (int i = 0; i < 5; i++)
                Array.Copy(V, 0, v, 0, 8);
        }

        protected override void ProcessBlock()
        {
            BitHelper.ToUInt32Array(_block, ByteOrder.LittleEndian, m);

            MI5();
            P5();
        }

        private void MI5()
        {
            uint[] a = new uint[8];
            uint[] b = new uint[8];

            Xor(a, v[0], v[1]);
            Xor(b, v[2], v[3]);
            Xor(a, a, b);
            Xor(a, a, v[4]);
            M2(a, a);
            Xor(v[0], a, v[0]);
            Xor(v[1], a, v[1]);
            Xor(v[2], a, v[2]);
            Xor(v[3], a, v[3]);
            Xor(v[4], a, v[4]);
            M2(b, v[0]);
            Xor(b, b, v[1]);
            M2(v[1], v[1]);
            Xor(v[1], v[1], v[2]);
            M2(v[2], v[2]);
            Xor(v[2], v[2], v[3]);
            M2(v[3], v[3]);
            Xor(v[3], v[3], v[4]);
            M2(v[4], v[4]);
            Xor(v[4], v[4], v[0]);
            M2(v[0], b);
            Xor(v[0], v[0], v[4]);
            M2(v[4], v[4]);
            Xor(v[4], v[4], v[3]);
            M2(v[3], v[3]);
            Xor(v[3], v[3], v[2]);
            M2(v[2], v[2]);
            Xor(v[2], v[2], v[1]);
            M2(v[1], v[1]);
            Xor(v[1], v[1], b);
            Xor(v[0], v[0], m);
            M2(m, m);
            Xor(v[1], v[1], m);
            M2(m, m);
            Xor(v[2], v[2], m);
            M2(m, m);
            Xor(v[3], v[3], m);
            M2(m, m);
            Xor(v[4], v[4], m);
        }

        private void P5()
        {
            ulong[] w = new ulong[8];
            Tweak5();
            for (int i = 0; i < 8; i++)
                w[i] = (ulong)v[0][i] | (v[1][i] << 32);
            for (int r = 0; r < 8; r++)
            {
                SUB_CRUMB_GEN_64(ref w[0], ref w[1], ref w[2], ref w[3]);
                SUB_CRUMB_GEN_64(ref w[5], ref w[6], ref w[7], ref w[4]);
                MixWord64(ref w[0], ref w[4]);
                MixWord64(ref w[1], ref w[5]);
                MixWord64(ref w[2], ref w[6]);
                MixWord64(ref w[3], ref w[7]);
                w[0] ^= RCW010[r];
                w[4] ^= RCW014[r];
            }
        }

        private void SUB_CRUMB_GEN_64(ref ulong a0, ref ulong a1, ref ulong a2, ref ulong a3)
        {
            ulong tmp;
            tmp = a0;
            a0 |= a1;
            a2 ^= a3;
            a1 = ~a1;
            a0 ^= a3;
            a3 &= tmp;
            a1 ^= a3;
            a3 ^= a2;
            a2 &= a0;
            a0 = ~a0;
            a2 ^= a1;
            a1 |= a3;
            tmp ^= a1;
            a3 ^= a2;
            a2 &= a1;
            a1 ^= a0;
            a0 = tmp;
        }

        private void MixWord64(ref ulong u, ref ulong v)
        {
            uint ul, uh, vl, vh;
            v ^= u;
            ul = (uint)u;
            uh = (uint)(u >> 32);
            vl = (uint)v;
            vh = (uint)(v >> 32);
            ul = BitHelper.RotateLeft(ul,  2) ^ vl;
            vl = BitHelper.RotateLeft(vl, 14) ^ ul;
            ul = BitHelper.RotateLeft(ul, 10) ^ vl;
            vl = BitHelper.RotateLeft(vl,  1);
            uh = BitHelper.RotateLeft(uh,  2) ^ vh;
            vh = BitHelper.RotateLeft(vh, 14) ^ uh;
            uh = BitHelper.RotateLeft(uh, 10) ^ vh;
            vh = BitHelper.RotateLeft(vh,  1);
            u = (ulong)ul | (uh << 32);
            v = (ulong)vl | (vh << 32);
        }

        private void Tweak5()
        {
            for (int i = 1; i < 5; i++)
            {
                v[i][4] = BitHelper.RotateLeft(v[i][4], i);
                v[i][5] = BitHelper.RotateLeft(v[i][5], i);
                v[i][6] = BitHelper.RotateLeft(v[i][6], i);
                v[i][7] = BitHelper.RotateLeft(v[i][7], i);
            }
        }

        private void Xor(uint[] result, uint[] a, uint[] b)
        {
            for (int i = 0; i < 8; i++)
                result[i] = a[i] ^ b[i];
        }

        private void M2(uint[] d, uint[] s)
        {
            uint tmp = s[7];
            d[7] = s[6];
            d[6] = s[5];
            d[5] = s[4];
            d[4] = s[3] ^ tmp;
            d[3] = s[2] ^ tmp;
            d[2] = s[1];
            d[1] = s[0] ^ tmp;
            d[0] = tmp;
        }
    }
}