using System;
using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.SIMDCommon;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SIMD256 : Security.MessageDigest
    {
        public override int BlockLength => 64;
        public override int DigestLength => 32;

        protected byte[] buffer;
        protected int bufOffset;
        protected uint[] state;
        protected ulong length;
        private readonly int[] y;
        private readonly uint[][] IV;
        private readonly uint[] R;
        private readonly uint[][] w;

        public SIMD256()
        {
            buffer = new byte[64];
            state = new uint[16];
            y = new int[128];
            IV = new uint[4][]
            {
                new uint[4],
                new uint[4],
                new uint[4],
                new uint[4],
            };
            R = new uint[4];
            w = new uint[8][]
            {
                new uint[4],
                new uint[4],
                new uint[4],
                new uint[4],
                new uint[4],
                new uint[4],
                new uint[4],
                new uint[4],
            };

            Reset();
        }

        public override void Reset()
        {
            Array.Copy(IV256, state, 16);

            bufOffset = 0;
            length = 0;
        }

        public override byte[] Digest()
        {
            if (bufOffset != 0)
            {
                while (bufOffset < 64)
                    buffer[bufOffset++] = 0;
                ProcessBlock(false);
            }

            GetBytes(length, ByteOrder.LittleEndian, buffer, 0);
            for (int i = 8; i < 64; i++)
                buffer[i] = 0;
            ProcessBlock(true);

            return InternalStateToDigest();
        }

        public override void Update(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            this.length += (ulong)length << 3;

            while (length > 0)
            {
                int copy = System.Math.Min(buffer.Length - bufOffset, length);
                Buffer.BlockCopy(data, offset, buffer, bufOffset, copy);

                offset += copy;
                length -= copy;
                bufOffset += copy;

                if (bufOffset == buffer.Length)
                {
                    bufOffset = 0;
                    ProcessBlock(false);
                }
            }
        }

        protected void ProcessBlock(bool final)
        {
            for (int i = 0; i < 0x40; i++)
                y[i] = buffer[i];

            FFT_128_halfzero(y, final);

            for (int i = 0; i < 4; i++)
            {
                IV[0][i] = state[i];
                IV[1][i] = state[i + 4];
                IV[2][i] = state[i + 8];
                IV[3][i] = state[i + 12];
            }

            for (int i = 0; i < 4; i++)
            {
                state[i] ^= ToUInt32(buffer, 4 * i, ByteOrder.LittleEndian);
                state[i + 4] ^= ToUInt32(buffer, 4 * (4 + i), ByteOrder.LittleEndian);
                state[i + 8] ^= ToUInt32(buffer, 4 * (8 + i), ByteOrder.LittleEndian);
                state[i + 12] ^= ToUInt32(buffer, 4 * (12 + i), ByteOrder.LittleEndian);
            }

            Round4(state, y, 0, 3, 23, 17, 27, R, w);
            Round4(state, y, 1, 28, 19, 22, 7, R, w);
            Round4(state, y, 2, 29, 9, 15, 5, R, w);
            Round4(state, y, 3, 4, 13, 10, 25, R, w);

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[0 + j], 4);
            for (int j = 0; j < 4; j++)
            {
                state[12 + j] = state[12 + j] + IV[0][j] + IF(state[0 + j], state[4 + j], state[8 + j]);
                state[12 + j] = RotateLeft(state[12 + j], 13) + R[j ^ (32 % 3 + 1)];
                state[0 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[12 + j], 13);
            for (int j = 0; j < 4; j++)
            {
                state[8 + j] = state[8 + j] + IV[1][j] + IF(state[12 + j], state[0 + j], state[4 + j]);
                state[8 + j] = RotateLeft(state[8 + j], 10) + R[j ^ (33 % 3 + 1)];
                state[12 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[8 + j], 10);
            for (int j = 0; j < 4; j++)
            {
                state[4 + j] = state[4 + j] + IV[2][j] + IF(state[8 + j], state[12 + j], state[0 + j]);
                state[4 + j] = RotateLeft(state[4 + j], 25) + R[j ^ (34 % 3 + 1)];
                state[8 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[4 + j], 25);
            for (int j = 0; j < 4; j++)
            {
                state[0 + j] = state[0 + j] + IV[3][j] + IF(state[4 + j], state[8 + j], state[12 + j]);
                state[0 + j] = RotateLeft(state[0 + j], 4) + R[j ^ (35 % 3 + 1)];
                state[4 + j] = R[j];
            }
        }

        protected virtual byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[32];
            for (int i = 0; i < 8; i++)
                GetBytes(state[i], ByteOrder.LittleEndian, digest, i << 2);
            return digest;
        }

        private static void FFT_64(int[] y, int index)
        {
            for (int i = 0; i < 8; i++)
                FFT_8(y, index + i, 8);

            for (int i = 8; i < 0x40; i++)
                if ((i & 7) != 0)
                    y[index + i] = Reduce(y[index + i] * FFT64_8_8_Twiddle[i]);

            for (int i = 0; i < 8; i++)
                FFT_8(y, index + 8 * i, 1);
        }

        private static void FFT_128_halfzero(int[] y, bool final)
        {
            int t0 = y[0x3F];

            for (int i = 0; i < 0x3F; i++)
                y[0x40 | i] = Reduce(y[i] * FFT128_2_64_Twiddle[i]);

            if (final)
            {
                int t1 = y[0x3D];
                y[0x3D] = Reduce(t1 + 1);
                y[0x7D] = Reduce((t1 - 1) * FFT128_2_64_Twiddle[0x3D]);
            }

            y[0x3F] = Reduce(t0 + 1);
            y[0x7F] = Reduce((t0 - 1) * FFT128_2_64_Twiddle[0x3F]);

            FFT_64(y, 0x00);
            FFT_64(y, 0x40);
        }

        private static void Round4(uint[] state, int[] y, int i, int r, int s, int t, int u, uint[] R, uint[][] w)
        {
            int code = i < 2 ? 185 : 233;

            for (int a = 0; a < 8; a++)
                for (int b = 0; b < 4; b++)
                    w[a][b] = (uint)(((y[Q4[8 * i + a][b]] * code) << 16) | ((y[P4[8 * i + a][b]] * code) & 0xFFFF));

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[0 + j], r);
            for (int j = 0; j < 4; j++)
            {
                state[12 + j] = state[12 + j] + w[0][j] + IF(state[0 + j], state[4 + j], state[8 + j]);
                state[12 + j] = RotateLeft(state[12 + j], s) + R[j ^ ((8 * i + 0) % 3 + 1)];
                state[0 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[12 + j], s);
            for (int j = 0; j < 4; j++)
            {
                state[8 + j] = state[8 + j] + w[1][j] + IF(state[12 + j], state[0 + j], state[4 + j]);
                state[8 + j] = RotateLeft(state[8 + j], t) + R[j ^ ((8 * i + 1) % 3 + 1)];
                state[12 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[8 + j], t);
            for (int j = 0; j < 4; j++)
            {
                state[4 + j] = state[4 + j] + w[2][j] + IF(state[8 + j], state[12 + j], state[0 + j]);
                state[4 + j] = RotateLeft(state[4 + j], u) + R[j ^ ((8 * i + 2) % 3 + 1)];
                state[8 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[4 + j], u);
            for (int j = 0; j < 4; j++)
            {
                state[0 + j] = state[0 + j] + w[3][j] + IF(state[4 + j], state[8 + j], state[12 + j]);
                state[0 + j] = RotateLeft(state[0 + j], r) + R[j ^ ((8 * i + 3) % 3 + 1)];
                state[4 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[0 + j], r);
            for (int j = 0; j < 4; j++)
            {
                state[12 + j] = state[12 + j] + w[4][j] + MAJ(state[0 + j], state[4 + j], state[8 + j]);
                state[12 + j] = RotateLeft(state[12 + j], s) + R[j ^ ((8 * i + 4) % 3 + 1)];
                state[0 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[12 + j], s);
            for (int j = 0; j < 4; j++)
            {
                state[8 + j] = state[8 + j] + w[5][j] + MAJ(state[12 + j], state[0 + j], state[4 + j]);
                state[8 + j] = RotateLeft(state[8 + j], t) + R[j ^ ((8 * i + 5) % 3 + 1)];
                state[12 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[8 + j], t);
            for (int j = 0; j < 4; j++)
            {
                state[4 + j] = state[4 + j] + w[6][j] + MAJ(state[8 + j], state[12 + j], state[0 + j]);
                state[4 + j] = RotateLeft(state[4 + j], u) + R[j ^ ((8 * i + 6) % 3 + 1)];
                state[8 + j] = R[j];
            }

            for (int j = 0; j < 4; j++)
                R[j] = RotateLeft(state[4 + j], u);
            for (int j = 0; j < 4; j++)
            {
                state[0 + j] = state[0 + j] + w[7][j] + MAJ(state[4 + j], state[8 + j], state[12 + j]);
                state[0 + j] = RotateLeft(state[0 + j], r) + R[j ^ ((8 * i + 7) % 3 + 1)];
                state[4 + j] = R[j];
            }
        }
    }
}