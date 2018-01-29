using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Network;

namespace Zergatul.Cryptography.KDF
{
#if !UseOpenSSL

    /// <summary>
    /// The scrypt Password-Based Key Derivation Function
    /// <para>https://tools.ietf.org/html/rfc7914</para>
    /// </summary>
    public class Scrypt
    {
        public Scrypt()
        {

        }

        /// <summary>
        /// Derives key bytes
        /// </summary>
        /// <param name="password">Human-chosen password</param>
        /// <param name="salt">Salt</param>
        /// <param name="blockSize">Block size</param>
        /// <param name="costParameter">CPU/Memory cost parameter, must be larger than 1, a power of 2 and less than 2^(128 * blockSize / 8)</param>
        /// <param name="parallelizationParameter">Less than or equal to ((2^32-1) * 32) / (128 * blockSize)</param>
        /// <param name="keyLength">Less than or equal to (2^32 - 1) * 32</param>
        public byte[] DeriveKeyBytes(byte[] password, byte[] salt, int blockSize, ulong costParameter, int parallelizationParameter, long keyLength)
        {
            ulong N = costParameter;
            int r = blockSize;
            int p = parallelizationParameter;

            var hkdf = new PBKDF2(new SHA256());

            uint[] B = BitHelper.ToUInt32Array(hkdf.DeriveKeyBytes(password, salt, 1, (ulong)(128L * p * r)), ByteOrder.LittleEndian);

            uint[] Bp = new uint[32 * r];
            for (int i = 0; i < p; i++)
            {
                Array.Copy(B, i * 32 * r, Bp, 0, 32 * r);
                ScryptROMix(r, Bp, N, out Bp);
                Array.Copy(Bp, 0, B, i * 32 * r, 32 * r);
            }

            return hkdf.DeriveKeyBytes(password, BitHelper.ToByteArray(B, ByteOrder.LittleEndian), 1, (ulong)keyLength);
        }

        internal static void Salsa(uint[] input, uint[] output)
        {
            uint[] x = new uint[16];
            Array.Copy(input, 0, x, 0, 16);

            for (int i = 8; i > 0; i -= 2)
            {
                x[ 4] ^= BitHelper.RotateLeft(x[ 0] + x[12],  7);
                x[ 8] ^= BitHelper.RotateLeft(x[ 4] + x[ 0],  9);
                x[12] ^= BitHelper.RotateLeft(x[ 8] + x[ 4], 13);
                x[ 0] ^= BitHelper.RotateLeft(x[12] + x[ 8], 18);
                x[ 9] ^= BitHelper.RotateLeft(x[ 5] + x[ 1],  7);
                x[13] ^= BitHelper.RotateLeft(x[ 9] + x[ 5],  9);
                x[ 1] ^= BitHelper.RotateLeft(x[13] + x[ 9], 13);
                x[ 5] ^= BitHelper.RotateLeft(x[ 1] + x[13], 18);
                x[14] ^= BitHelper.RotateLeft(x[10] + x[ 6],  7);
                x[ 2] ^= BitHelper.RotateLeft(x[14] + x[10],  9);
                x[ 6] ^= BitHelper.RotateLeft(x[ 2] + x[14], 13);
                x[10] ^= BitHelper.RotateLeft(x[ 6] + x[ 2], 18);
                x[ 3] ^= BitHelper.RotateLeft(x[15] + x[11],  7);
                x[ 7] ^= BitHelper.RotateLeft(x[ 3] + x[15],  9);
                x[11] ^= BitHelper.RotateLeft(x[ 7] + x[ 3], 13);
                x[15] ^= BitHelper.RotateLeft(x[11] + x[ 7], 18);
                x[ 1] ^= BitHelper.RotateLeft(x[ 0] + x[ 3],  7);
                x[ 2] ^= BitHelper.RotateLeft(x[ 1] + x[ 0],  9);
                x[ 3] ^= BitHelper.RotateLeft(x[ 2] + x[ 1], 13);
                x[ 0] ^= BitHelper.RotateLeft(x[ 3] + x[ 2], 18);
                x[ 6] ^= BitHelper.RotateLeft(x[ 5] + x[ 4],  7);
                x[ 7] ^= BitHelper.RotateLeft(x[ 6] + x[ 5],  9);
                x[ 4] ^= BitHelper.RotateLeft(x[ 7] + x[ 6], 13);
                x[ 5] ^= BitHelper.RotateLeft(x[ 4] + x[ 7], 18);
                x[11] ^= BitHelper.RotateLeft(x[10] + x[ 9],  7);
                x[ 8] ^= BitHelper.RotateLeft(x[11] + x[10],  9);
                x[ 9] ^= BitHelper.RotateLeft(x[ 8] + x[11], 13);
                x[10] ^= BitHelper.RotateLeft(x[ 9] + x[ 8], 18);
                x[12] ^= BitHelper.RotateLeft(x[15] + x[14],  7);
                x[13] ^= BitHelper.RotateLeft(x[12] + x[15],  9);
                x[14] ^= BitHelper.RotateLeft(x[13] + x[12], 13);
                x[15] ^= BitHelper.RotateLeft(x[14] + x[13], 18);
            }

            for (int i = 0; i < 16; i++)
                output[i] = unchecked(x[i] + input[i]);
        }

        internal static void ScryptBlockMix(int r, uint[] B, uint[] Bp)
        {
            if (B.Length != 2 * r * 16)
                throw new InvalidOperationException();
            if (B.Length != Bp.Length)
                throw new InvalidOperationException();

            uint[] X = new uint[16];
            uint[] T = new uint[16];

            // X = B[2 * r - 1]
            Array.Copy(B, B.Length - 16, X, 0, 16);

            for (int i = 0; i < 2 * r; i++)
            {
                for (int j = 0; j < 16; j++)
                    T[j] = X[j] ^ B[i * 16 + j];
                Salsa(T, X);

                if ((i & 1) == 0)
                    Array.Copy(X, 0, Bp, i * 8, 16);
                else
                    Array.Copy(X, 0, Bp, B.Length / 2 + (i - 1) * 8, 16);
            }
        }

        internal static void ScryptROMix(int r, uint[] B, ulong N, out uint[] Bp)
        {
            if (B.Length != 32 * r)
                throw new InvalidOperationException();

            uint[] X = new uint[B.Length];
            uint[][] V = new uint[N][];

            Array.Copy(B, 0, X, 0, B.Length);
            for (ulong i = 0; i < N; i++)
            {
                V[i] = (uint[])X.Clone();
                ScryptBlockMix(r, V[i], X);
            }

            uint[] T = new uint[B.Length];
            for (ulong i = 0; i < N; i++)
            {
                ulong j = Integrity(X, r) % N;
                for (int k = 0; k < T.Length; k++)
                    T[k] = X[k] ^ V[j][k];
                ScryptBlockMix(r, T, X);
            }

            Bp = X;
        }

        private static ulong Integrity(uint[] B, int r)
        {
            int index = (2 * r - 1) * 16;
            return ((ulong)B[index - 1] << 32) | B[index];
        }
    }

#else

    /// <summary>
    /// The scrypt Password-Based Key Derivation Function
    /// <para>https://tools.ietf.org/html/rfc7914</para>
    /// </summary>
    public class Scrypt
    {
        public Scrypt()
        {

        }

        /// <summary>
        /// Derives key bytes
        /// </summary>
        /// <param name="password">Human-chosen password</param>
        /// <param name="salt">Salt</param>
        /// <param name="blockSize">Block size</param>
        /// <param name="costParameter">CPU/Memory cost parameter, must be larger than 1, a power of 2 and less than 2^(128 * blockSize / 8)</param>
        /// <param name="parallelizationParameter">Less than or equal to ((2^32-1) * 32) / (128 * blockSize)</param>
        /// <param name="keyLength">Less than or equal to (2^32 - 1) * 32</param>
        public byte[] DeriveKeyBytes(byte[] password, byte[] salt, int blockSize, ulong costParameter, int parallelizationParameter, long keyLength)
        {
            ulong N = costParameter;
            ulong r = (ulong)blockSize;
            ulong p = (ulong)parallelizationParameter;

            byte[] key = new byte[keyLength];

            int result = OpenSSL.EVP_PBE_scrypt(
                password, password.Length,
                salt, salt?.Length ?? 0,
                N, r, p,
                ulong.MaxValue,
                key, key.Length);
            if (result != 1)
                throw new OpenSSLException();

            return key;
        }
    }

#endif
}