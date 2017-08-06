using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Generator
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc7292#appendix-B.2
    /// </summary>
    public class PKCS12v11
    {
        public const byte IDKeyMaterial = 1;
        public const byte IDInitialValue = 2;
        public const byte IDIntegrityKey = 3;

        public byte[] FormatPassword(string pwd)
        {
            if (pwd == null)
                return new byte[0];

            byte[] result = new byte[pwd.Length * 2 + 2];
            System.Text.Encoding.BigEndianUnicode.GetBytes(pwd, 0, pwd.Length, result, 0);
            return result;
        }

        public byte[] GenerateParameter(AbstractHash hash, byte id, byte[] password, byte[] salt, int iterations, int length)
        {
            int ub = hash.HashSize; //u in bytes
            int u = ub * 8;
            int vb = hash.BlockSize; // v in bytes
            int v = vb * 8;
            int pb = password.Length; // p in bytes
            int p = pb * 8;
            int sb = salt.Length; // s in bytes
            int s = sb * 8;

            byte[] D = new byte[vb];
            for (int i = 0; i < D.Length; i++)
                D[i] = id;

            // Length = v * Math.Ceil(s / v)
            byte[] S = new byte[(sb + vb - 1) / vb * vb];
            for (int i = 0; i < S.Length; i++)
                S[i] = salt[i % sb];

            // Length = v * Math.Ceil(p / v)
            byte[] P = new byte[(pb + vb - 1) / vb * vb];
            for (int i = 0; i < P.Length; i++)
                P[i] = password[i % pb];

            // I = S || P
            byte[] I = new byte[S.Length + P.Length];
            Array.Copy(S, 0, I, 0, S.Length);
            Array.Copy(P, 0, I, S.Length, P.Length);
            int k = I.Length / vb;

            // c = Math.Ceil(n / u)
            int c = (length + ub - 1) / ub;

            byte[][] A = new byte[c][];

            for (int i = 0; i < c; i++)
            {
                // H(D || I)
                hash.Reset();
                hash.Update(D);
                hash.Update(I);
                byte[] a = hash.ComputeHash();

                // H(H(H...H(D || I)))
                for (int r = 1; r < iterations; r++)
                {
                    hash.Reset();
                    hash.Update(a);
                    a = hash.ComputeHash();
                }

                A[i] = a;

                // Ij = (Ij + B + 1) mod 2^v
                // B = A0 || A1 || ... (length v)
                for (int j = 0; j < k; j++)
                {
                    int carry = 1;
                    for (int digit = vb - 1; digit >= 0; digit--)
                    {
                        carry = carry + I[j * vb + digit] + a[digit % ub];
                        I[j * vb + digit] = (byte)carry;
                        carry >>= 8;
                    }
                }
            }

            byte[] result = new byte[length];
            for (int i = 0; i < result.Length; i++)
                result[i] = A[i / ub][i % ub];

            return result;
        }
    }
}