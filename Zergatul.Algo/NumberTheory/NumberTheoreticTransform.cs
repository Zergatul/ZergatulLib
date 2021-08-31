using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Algo.NumberTheory
{
    public struct NumberTheoreticTransform
    {
        public int P; // prime
        public int M; // size
        long[] A, B, rev;
        long[][] w;

        public NumberTheoreticTransform(int p, int n)
        {
            P = p;
            for (M = 2; M < n; M <<= 1);
            M <<= 1;

            A = new long[M];
            B = new long[M];
            w = new long[2][];
            w[0] = new long[M];
            w[1] = new long[M];
            rev = new long[M];

            for (int i = 0; i < M; i++)
            {
                long x = i;
                rev[i] = 0;
                for (long k = 1; k < M; k <<= 1, x >>= 1)
                {
                    rev[i] <<= 1;
                    rev[i] |= x & 1;
                }
            }

            {
                long G = 10;
                long x = ModPow.Calculate(G, (P - 1) / M, P);
                long y = ModPow.Calculate(x, P - 2, P);

                w[0][0] = w[1][0] = 1;

                for (int i = 1; i < M; i++)
                {
                    w[0][i] = (w[0][i - 1] * x) % P;
                    w[1][i] = (w[1][i - 1] * y) % P;
                }
            }
        }

        public void Transform(long[] a, int f)
        {
            for (int i = 0; i < M; i++)
            {
                if (i < rev[i])
                    (a[i], a[rev[i]]) = (a[rev[i]], a[i]);
            }

            for (int i = 1; i < M; i <<= 1)
            {
                for (int j = 0, t = M / (i << 1); j < M; j += (i << 1))
                {
                    for (int k = 0, l = 0; k < i; k++, l += t)
                    {
                        long x = a[j + k + i] * w[f][l] % P;
                        long y = a[j + k];
                        a[j + k + i] = y - x < 0 ? y - x + P : y - x;
                        a[j + k] = y + x >= P ? y + x - P : y + x;
                    }
                }
            }

            if (f == 1)
            {
                long x = ModPow.Calculate(M, P - 2, P);
                for (int i = 0; i < M; i++)
                    a[i] = a[i] * x % P;
            }
        }

        public void Multiply(long[] X, long[] Y, long[] res)
        {
            //init(max(X.size(), Y.size()));

            Array.Clear(A, 0, M);
            Array.Clear(B, 0, M);
            Array.Copy(X, 0, A, 0, X.Length);
            Array.Copy(Y, 0, B, 0, Y.Length);

            Transform(A, 0);
            Transform(B, 0);
            //res.clear();
            //res.resize(M);
            for (int i = 0; i < M; i++)
                res[i] = A[i] * B[i] % P;

            Transform(res, 1);
        }
    }
}