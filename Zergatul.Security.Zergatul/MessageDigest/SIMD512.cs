using System;
using static Zergatul.BitHelper;
using static Zergatul.Security.Zergatul.MessageDigest.SIMDCommon;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    class SIMD512 : Security.MessageDigest
    {
        public override int BlockLength => 128;
        public override int DigestLength => 64;

        protected byte[] buffer;
        protected int bufOffset;
        protected uint[] state;
        protected ulong length;
        private readonly int[] y;
        private readonly uint[][] IV;
        private readonly uint[] R;
        private readonly uint[][] w;

        public SIMD512()
        {
            buffer = new byte[128];
            state = new uint[32];
            y = new int[256];
            IV = new uint[4][]
            {
                new uint[8],
                new uint[8],
                new uint[8],
                new uint[8],
            };
            R = new uint[8];
            w = new uint[8][]
            {
                new uint[8],
                new uint[8],
                new uint[8],
                new uint[8],
                new uint[8],
                new uint[8],
                new uint[8],
                new uint[8],
            };

            Reset();
        }

        public override void Reset()
        {
            Array.Copy(IV512, state, 32);

            bufOffset = 0;
            length = 0;
        }

        public override byte[] Digest()
        {
            if (bufOffset != 0)
            {
                while (bufOffset < 128)
                    buffer[bufOffset++] = 0;
                ProcessBlock(false);
            }

            GetBytes(length, ByteOrder.LittleEndian, buffer, 0);
            for (int i = 8; i < 128; i++)
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
            for (int i = 0; i < 0x80; i++)
                y[i] = buffer[i];

            FFT_256_halfzero(y, final);

            for (int i = 0; i < 8; i++)
            {
                IV[0][i] = state[i];
                IV[1][i] = state[i + 8];
                IV[2][i] = state[i + 16];
                IV[3][i] = state[i + 24];
            }

            for (int i = 0; i < 8; i++)
            {
                state[i] ^= ToUInt32(buffer, 4 * i, ByteOrder.LittleEndian);
                state[i + 8] ^= ToUInt32(buffer, 4 * (8 + i), ByteOrder.LittleEndian);
                state[i + 16] ^= ToUInt32(buffer, 4 * (16 + i), ByteOrder.LittleEndian);
                state[i + 24] ^= ToUInt32(buffer, 4 * (24 + i), ByteOrder.LittleEndian);
            }

            Round8(state, y, 0, 3, 23, 17, 27, R, w);
            Round8(state, y, 1, 28, 19, 22, 7, R, w);
            Round8(state, y, 2, 29, 9, 15, 5, R, w);
            Round8(state, y, 3, 4, 13, 10, 25, R, w);

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[0 + j], 4);
            for (int j = 0; j < 8; j++)
            {
                state[24 + j] = state[24 + j] + IV[0][j] + IF(state[0 + j], state[8 + j], state[16 + j]);
                state[24 + j] = RotateLeft(state[24 + j], 13) + R[j ^ P8Xor[32 % 7]];
                state[0 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[24 + j], 13);
            for (int j = 0; j < 8; j++)
            {
                state[16 + j] = state[16 + j] + IV[1][j] + IF(state[24 + j], state[0 + j], state[8 + j]);
                state[16 + j] = RotateLeft(state[16 + j], 10) + R[j ^ P8Xor[33 % 7]];
                state[24 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[16 + j], 10);
            for (int j = 0; j < 8; j++)
            {
                state[8 + j] = state[8 + j] + IV[2][j] + IF(state[16 + j], state[24 + j], state[0 + j]);
                state[8 + j] = RotateLeft(state[8 + j], 25) + R[j ^ P8Xor[34 % 7]];
                state[16 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[8 + j], 25);
            for (int j = 0; j < 8; j++)
            {
                state[0 + j] = state[0 + j] + IV[3][j] + IF(state[8 + j], state[16 + j], state[24 + j]);
                state[0 + j] = RotateLeft(state[0 + j], 4) + R[j ^ P8Xor[35 % 7]];
                state[8 + j] = R[j];
            }
        }

        protected virtual byte[] InternalStateToDigest()
        {
            byte[] digest = new byte[64];
            for (int i = 0; i < 16; i++)
                GetBytes(state[i], ByteOrder.LittleEndian, digest, i << 2);
            return digest;
        }

        private static void FFT_16(int[] y, int index, int stripe)
        {
            int u, v;
            u = y[index + stripe * 0];
            v = y[index + stripe * 8];
            y[index + stripe * 0] = u + v;
            y[index + stripe * 8] = (u - v) << 0;
            u = y[index + stripe * 1];
            v = y[index + stripe * 9];
            y[index + stripe * 1] = u + v;
            y[index + stripe * 9] = (u - v) << 1;
            u = y[index + stripe * 2];
            v = y[index + stripe * 10];
            y[index + stripe * 2] = u + v;
            y[index + stripe * 10] = (u - v) << 2;
            u = y[index + stripe * 3];
            v = y[index + stripe * 11];
            y[index + stripe * 3] = u + v;
            y[index + stripe * 11] = (u - v) << 3;
            u = y[index + stripe * 4];
            v = y[index + stripe * 12];
            y[index + stripe * 4] = u + v;
            y[index + stripe * 12] = (u - v) << 4;
            u = y[index + stripe * 5];
            v = y[index + stripe * 13];
            y[index + stripe * 5] = u + v;
            y[index + stripe * 13] = (u - v) << 5;
            u = y[index + stripe * 6];
            v = y[index + stripe * 14];
            y[index + stripe * 6] = u + v;
            y[index + stripe * 14] = (u - v) << 6;
            u = y[index + stripe * 7];
            v = y[index + stripe * 15];
            y[index + stripe * 7] = u + v;
            y[index + stripe * 15] = (u - v) << 7;
            y[index + stripe * 11] = Reduce(y[index + stripe * 11]);
            y[index + stripe * 12] = Reduce(y[index + stripe * 12]);
            y[index + stripe * 13] = Reduce(y[index + stripe * 13]);
            y[index + stripe * 14] = Reduce(y[index + stripe * 14]);
            y[index + stripe * 15] = Reduce(y[index + stripe * 15]);
            u = y[index + stripe * 0];
            v = y[index + stripe * 4];
            y[index + stripe * 0] = u + v;
            y[index + stripe * 4] = (u - v) << 0;
            u = y[index + stripe * 8];
            v = y[index + stripe * 12];
            y[index + stripe * 8] = u + v;
            y[index + stripe * 12] = (u - v) << 0;
            u = y[index + stripe * 1];
            v = y[index + stripe * 5];
            y[index + stripe * 1] = u + v;
            y[index + stripe * 5] = (u - v) << 2;
            u = y[index + stripe * 9];
            v = y[index + stripe * 13];
            y[index + stripe * 9] = u + v;
            y[index + stripe * 13] = (u - v) << 2;
            u = y[index + stripe * 2];
            v = y[index + stripe * 6];
            y[index + stripe * 2] = u + v;
            y[index + stripe * 6] = (u - v) << 4;
            u = y[index + stripe * 10];
            v = y[index + stripe * 14];
            y[index + stripe * 10] = u + v;
            y[index + stripe * 14] = (u - v) << 4;
            u = y[index + stripe * 3];
            v = y[index + stripe * 7];
            y[index + stripe * 3] = u + v;
            y[index + stripe * 7] = (u - v) << 6;
            u = y[index + stripe * 11];
            v = y[index + stripe * 15];
            y[index + stripe * 11] = u + v;
            y[index + stripe * 15] = (u - v) << 6;
            y[index + stripe * 5] = Reduce(y[index + stripe * 5]);
            y[index + stripe * 7] = Reduce(y[index + stripe * 7]);
            y[index + stripe * 13] = Reduce(y[index + stripe * 13]);
            y[index + stripe * 15] = Reduce(y[index + stripe * 15]);
            u = y[index + stripe * 0];
            v = y[index + stripe * 2];
            y[index + stripe * 0] = u + v;
            y[index + stripe * 2] = (u - v) << 0;
            u = y[index + stripe * 4];
            v = y[index + stripe * 6];
            y[index + stripe * 4] = u + v;
            y[index + stripe * 6] = (u - v) << 0;
            u = y[index + stripe * 8];
            v = y[index + stripe * 10];
            y[index + stripe * 8] = u + v;
            y[index + stripe * 10] = (u - v) << 0;
            u = y[index + stripe * 12];
            v = y[index + stripe * 14];
            y[index + stripe * 12] = u + v;
            y[index + stripe * 14] = (u - v) << 0;
            u = y[index + stripe * 1];
            v = y[index + stripe * 3];
            y[index + stripe * 1] = u + v;
            y[index + stripe * 3] = (u - v) << 4;
            u = y[index + stripe * 5];
            v = y[index + stripe * 7];
            y[index + stripe * 5] = u + v;
            y[index + stripe * 7] = (u - v) << 4;
            u = y[index + stripe * 9];
            v = y[index + stripe * 11];
            y[index + stripe * 9] = u + v;
            y[index + stripe * 11] = (u - v) << 4;
            u = y[index + stripe * 13];
            v = y[index + stripe * 15];
            y[index + stripe * 13] = u + v;
            y[index + stripe * 15] = (u - v) << 4;
            u = y[index + stripe * 0];
            v = y[index + stripe * 1];
            y[index + stripe * 0] = u + v;
            y[index + stripe * 1] = (u - v) << 0;
            u = y[index + stripe * 2];
            v = y[index + stripe * 3];
            y[index + stripe * 2] = u + v;
            y[index + stripe * 3] = (u - v) << 0;
            u = y[index + stripe * 4];
            v = y[index + stripe * 5];
            y[index + stripe * 4] = u + v;
            y[index + stripe * 5] = (u - v) << 0;
            u = y[index + stripe * 6];
            v = y[index + stripe * 7];
            y[index + stripe * 6] = u + v;
            y[index + stripe * 7] = (u - v) << 0;
            u = y[index + stripe * 8];
            v = y[index + stripe * 9];
            y[index + stripe * 8] = u + v;
            y[index + stripe * 9] = (u - v) << 0;
            u = y[index + stripe * 10];
            v = y[index + stripe * 11];
            y[index + stripe * 10] = u + v;
            y[index + stripe * 11] = (u - v) << 0;
            u = y[index + stripe * 12];
            v = y[index + stripe * 13];
            y[index + stripe * 12] = u + v;
            y[index + stripe * 13] = (u - v) << 0;
            u = y[index + stripe * 14];
            v = y[index + stripe * 15];
            y[index + stripe * 14] = u + v;
            y[index + stripe * 15] = (u - v) << 0;
            y[index + stripe * 0] = Reduce(y[index + stripe * 0]);
            y[index + stripe * 0] = y[index + stripe * 0] <= 128 ? y[index + stripe * 0] : y[index + stripe * 0] - 257;
            y[index + stripe * 1] = Reduce(y[index + stripe * 1]);
            y[index + stripe * 1] = y[index + stripe * 1] <= 128 ? y[index + stripe * 1] : y[index + stripe * 1] - 257;
            y[index + stripe * 2] = Reduce(y[index + stripe * 2]);
            y[index + stripe * 2] = y[index + stripe * 2] <= 128 ? y[index + stripe * 2] : y[index + stripe * 2] - 257;
            y[index + stripe * 3] = Reduce(y[index + stripe * 3]);
            y[index + stripe * 3] = y[index + stripe * 3] <= 128 ? y[index + stripe * 3] : y[index + stripe * 3] - 257;
            y[index + stripe * 4] = Reduce(y[index + stripe * 4]);
            y[index + stripe * 4] = y[index + stripe * 4] <= 128 ? y[index + stripe * 4] : y[index + stripe * 4] - 257;
            y[index + stripe * 5] = Reduce(y[index + stripe * 5]);
            y[index + stripe * 5] = y[index + stripe * 5] <= 128 ? y[index + stripe * 5] : y[index + stripe * 5] - 257;
            y[index + stripe * 6] = Reduce(y[index + stripe * 6]);
            y[index + stripe * 6] = y[index + stripe * 6] <= 128 ? y[index + stripe * 6] : y[index + stripe * 6] - 257;
            y[index + stripe * 7] = Reduce(y[index + stripe * 7]);
            y[index + stripe * 7] = y[index + stripe * 7] <= 128 ? y[index + stripe * 7] : y[index + stripe * 7] - 257;
            y[index + stripe * 8] = Reduce(y[index + stripe * 8]);
            y[index + stripe * 8] = y[index + stripe * 8] <= 128 ? y[index + stripe * 8] : y[index + stripe * 8] - 257;
            y[index + stripe * 9] = Reduce(y[index + stripe * 9]);
            y[index + stripe * 9] = y[index + stripe * 9] <= 128 ? y[index + stripe * 9] : y[index + stripe * 9] - 257;
            y[index + stripe * 10] = Reduce(y[index + stripe * 10]);
            y[index + stripe * 10] = y[index + stripe * 10] <= 128 ? y[index + stripe * 10] : y[index + stripe * 10] - 257;
            y[index + stripe * 11] = Reduce(y[index + stripe * 11]);
            y[index + stripe * 11] = y[index + stripe * 11] <= 128 ? y[index + stripe * 11] : y[index + stripe * 11] - 257;
            y[index + stripe * 12] = Reduce(y[index + stripe * 12]);
            y[index + stripe * 12] = y[index + stripe * 12] <= 128 ? y[index + stripe * 12] : y[index + stripe * 12] - 257;
            y[index + stripe * 13] = Reduce(y[index + stripe * 13]);
            y[index + stripe * 13] = y[index + stripe * 13] <= 128 ? y[index + stripe * 13] : y[index + stripe * 13] - 257;
            y[index + stripe * 14] = Reduce(y[index + stripe * 14]);
            y[index + stripe * 14] = y[index + stripe * 14] <= 128 ? y[index + stripe * 14] : y[index + stripe * 14] - 257;
            y[index + stripe * 15] = Reduce(y[index + stripe * 15]);
            y[index + stripe * 15] = y[index + stripe * 15] <= 128 ? y[index + stripe * 15] : y[index + stripe * 15] - 257;
        }

        private static void FFT_128_full(int[] y, int index)
        {
            for (int i = 0; i < 16; i++)
                FFT_8(y, index + i, 16);

            for (int i = 0; i < 0x80; i++)
                y[index + i] = Reduce(y[index + i] * FFT128_8_16_Twiddle[i]);

            for (int i = 0; i < 8; i++)
                FFT_16(y, index + 16 * i, 1);
        }

        private static void FFT_256_halfzero(int[] y, bool final)
        {
            int t0 = y[0x7F];

            for (int i = 0; i < 0x7F; i++)
                y[0x80 | i] = Reduce(y[i] * FFT256_2_128_Twiddle[i]);

            if (final)
            {
                int t1 = y[0x7D];
                y[0x7D] = Reduce(t1 + 1);
                y[0xFD] = Reduce((t1 - 1) * FFT256_2_128_Twiddle[0x7D]);
            }

            y[0x7F] = Reduce(t0 + 1);
            y[0xFF] = Reduce((t0 - 1) * FFT256_2_128_Twiddle[0x7F]);

            FFT_128_full(y, 0x00);
            FFT_128_full(y, 0x80);
        }

        private static void Round8(uint[] state, int[] y, int i, int r, int s, int t, int u, uint[] R, uint[][] w)
        {
            int code = i < 2 ? 185 : 233;

            for (int a = 0; a < 8; a++)
            for (int b = 0; b < 8; b++)
                w[a][b] = (uint)(((y[Q8[8 * i + a][b]] * code) << 16) | ((y[P8[8 * i + a][b]] * code) & 0xFFFF));

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[0 + j], r);
            for (int j = 0; j < 8; j++)
            {
                state[24 + j] = state[24 + j] + w[0][j] + IF(state[0 + j], state[8 + j], state[16 + j]);
                state[24 + j] = RotateLeft(state[24 + j], s) + R[j ^ P8Xor[(8 * i + 0) % 7]];
                state[0 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[24 + j], s);
            for (int j = 0; j < 8; j++)
            {
                state[16 + j] = state[16 + j] + w[1][j] + IF(state[24 + j], state[0 + j], state[8 + j]);
                state[16 + j] = RotateLeft(state[16 + j], t) + R[j ^ P8Xor[(8 * i + 1) % 7]];
                state[24 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[16 + j], t);
            for (int j = 0; j < 8; j++)
            {
                state[8 + j] = state[8 + j] + w[2][j] + IF(state[16 + j], state[24 + j], state[0 + j]);
                state[8 + j] = RotateLeft(state[8 + j], u) + R[j ^ P8Xor[(8 * i + 2) % 7]];
                state[16 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[8 + j], u);
            for (int j = 0; j < 8; j++)
            {
                state[0 + j] = state[0 + j] + w[3][j] + IF(state[8 + j], state[16 + j], state[24 + j]);
                state[0 + j] = RotateLeft(state[0 + j], r) + R[j ^ P8Xor[(8 * i + 3) % 7]];
                state[8 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[0 + j], r);
            for (int j = 0; j < 8; j++)
            {
                state[24 + j] = state[24 + j] + w[4][j] + MAJ(state[0 + j], state[8 + j], state[16 + j]);
                state[24 + j] = RotateLeft(state[24 + j], s) + R[j ^ P8Xor[(8 * i + 4) % 7]];
                state[0 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[24 + j], s);
            for (int j = 0; j < 8; j++)
            {
                state[16 + j] = state[16 + j] + w[5][j] + MAJ(state[24 + j], state[0 + j], state[8 + j]);
                state[16 + j] = RotateLeft(state[16 + j], t) + R[j ^ P8Xor[(8 * i + 5) % 7]];
                state[24 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[16 + j], t);
            for (int j = 0; j < 8; j++)
            {
                state[8 + j] = state[8 + j] + w[6][j] + MAJ(state[16 + j], state[24 + j], state[0 + j]);
                state[8 + j] = RotateLeft(state[8 + j], u) + R[j ^ P8Xor[(8 * i + 6) % 7]];
                state[16 + j] = R[j];
            }

            for (int j = 0; j < 8; j++)
                R[j] = RotateLeft(state[8 + j], u);
            for (int j = 0; j < 8; j++)
            {
                state[0 + j] = state[0 + j] + w[7][j] + MAJ(state[8 + j], state[16 + j], state[24 + j]);
                state[0 + j] = RotateLeft(state[0 + j], r) + R[j ^ P8Xor[(8 * i + 7) % 7]];
                state[8 + j] = R[j];
            }
        }
    }
}