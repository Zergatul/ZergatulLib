using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Hash
{
    public abstract class Keccak : AbstractHash
    {
        private static int[,] r = new int[5, 5]
        {
            {  0, 36,  3, 41, 18 },
            {  1, 44, 10, 45,  2 },
            { 62,  6, 43, 15, 61 },
            { 28, 55, 25, 21, 56 },
            { 27, 20, 39,  8, 14 }
        };

        private static ulong[] RC = new ulong[]
        {
            0x0000000000000001,
            0x0000000000008082,
            0x800000000000808A,
            0x8000000080008000,
            0x000000000000808B,
            0x0000000080000001,
            0x8000000080008081,
            0x8000000000008009,
            0x000000000000008A,
            0x0000000000000088,
            0x0000000080008009,
            0x000000008000000A,
            0x000000008000808B,
            0x800000000000008B,
            0x8000000000008089,
            0x8000000000008003,
            0x8000000000008002,
            0x8000000000000080,
            0x000000000000800A,
            0x800000008000000A,
            0x8000000080008081,
            0x8000000000008080,
            0x0000000080000001,
            0x8000000080008008
        };

        private static byte[,] f(KeccakParams kp, byte[,] A)
        {
            int rounds = 12 + 2 * kp.l;
            for (int i = 0; i < rounds; i++)
                A = Round(kp, A, RC[i]);
            return A;
        }

        private static byte[,] Round(KeccakParams kp, byte[,] A, ulong RC)
        {
            byte[,] B = new byte[5, 5];
            byte[] C = new byte[5];
            byte[] D = new byte[5];

            // θ step

            for (int x = 0; x < 5; x++)
                C[x] = (byte)(A[x, 0] ^ A[x, 1] ^ A[x, 2] ^ A[x, 3] ^ A[x, 4]);

            for (int x = 0; x < 5; x++)
                D[x] = (byte)(C[(x + 4) % 5] ^ BitHelper.RotateLeft(C[(x + 1) % 5], 1));

            for (int x = 0; x < 5; x++)
                for (int y = 0; y < 5; y++)
                    A[x, y] ^= D[x];

            // ρ and π steps

            for (int x = 0; x < 5; x++)
                for (int y = 0; y < 5; y++)
                    B[y, (2 * x + 3 * y) % 5] = BitHelper.RotateLeft(A[x, y], r[x, y]);

            // χ step

            for (int x = 0; x < 5; x++)
                for (int y = 0; y < 5; y++)
                    A[x, y] = (byte)(B[x, y] ^ (~B[(x + 1) % 5, y] & B[(x + 2) % 5, y]));

            // ι step

            A[0, 0] ^= (byte)RC;

            return A;
        }

        private byte[] Sponge(KeccakParams kp, int c, int r, byte[] M, byte d, int len)
        {
            // Initialization and padding
            byte[,] S = new byte[5, 5];
            byte[] P = new byte[(M.Length / 25 + 1) * 25];
            Array.Copy(M, P, M.Length);
            P[M.Length] = d;
            P[P.Length - 1] ^= 0x80;

            // Absorbing phase
            for (int b = 0; b < P.Length / 25; b++)
            {
                for (int x = 0; x < 5; x++)
                    for (int y = 0; y < 5; y++)
                        if (x + 5 * y < r / kp.w)
                            S[x, y] ^= P[b * 25 + x + 5 * y];
                S = f(new KeccakParams { b = r + c }, S);
            }

            // Squeezing phase
            List<byte> result = new List<byte>();
            while (result.Count < len)
            {
                for (int x = 0; x < 5; x++)
                    for (int y = 0; y < 5; y++)
                        if (x + 5 * y < r / kp.w)
                            result.Add(S[x, y]);

                S = f(new KeccakParams { b = r + c }, S);
            }

            return result.ToArray();
        }

        private struct KeccakParams
        {
            public int b;
            public int w => b / 25;
            public int l => (int)System.Math.Log(w, 2);
        }
    }
}