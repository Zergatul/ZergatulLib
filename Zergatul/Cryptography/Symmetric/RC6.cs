using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public class RC6 : AbstractBlockCipher
    {
        public override int BlockSize => _w >> 1;
        public override int KeySize => _k;

        private int _w, _r, _k;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w">Word size in bits. Allowable values: 32</param>
        /// <param name="r">Number of rounds. Allowable values: 0..255</param>
        /// <param name="k">Key length in bytes. Allowable values: 0..255</param>
        public RC6(int w, int r, int k)
        {
            // 16/64 is not supported
            if (w != 32)
                throw new ArgumentException("Invalid w");
            if (r < 0 || 255 < r)
                throw new ArgumentException("Invalid r");
            if (k < 0 || 255 < k)
                throw new ArgumentException("Invalid k");

            this._w = w;
            this._r = r;
            this._k = k;
        }

        public override Func<byte[], byte[]> CreateEncryptor(byte[] key)
        {
            switch (_w)
            {
                case 32: return new _32Bit(_r, key).Encrypt;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override Func<byte[], byte[]> CreateDecryptor(byte[] key)
        {
            switch (_w)
            {
                case 32: return new _32Bit(_r, key).Decrypt;
                default:
                    throw new InvalidOperationException();
            }
        }

        private class _32Bit
        {
            const int P = unchecked((int)0xB7E15163);
            const int Q = unchecked((int)0x9E3779B9);

            private int rounds;
            private int[] s;

            public _32Bit(int rounds, byte[] key)
            {
                this.rounds = rounds;

                // Converting the secret key from bytes to words
                int c = (System.Math.Max(key.Length, 1) + 3) / 4;
                int[] L = new int[c];
                for (int k = 0; k < key.Length; k++)
                    L[k >> 2] |= key[k] << ((k & 0x03) << 3);

                // Initializing the array S
                int t = (rounds + 2) << 1;
                s = new int[t];
                s[0] = P;
                for (int k = 1; k < t; k++)
                    s[k] = s[k - 1] + Q;

                // Mixing in the secret key
                int cycles = 3 * System.Math.Max(t, c);
                int i = 0, j = 0, a = 0, b = 0;
                for (int k = 0; k < cycles; k++)
                {
                    a = s[i] = BitHelper.RotateLeft(s[i] + a + b, 3);
                    b = L[j] = BitHelper.RotateLeft(L[j] + a + b, (a + b) & 0x1F);
                    i++;
                    j++;
                    if (i == t)
                        i = 0;
                    if (j == c)
                        j = 0;
                }
            }

            public byte[] Encrypt(byte[] block)
            {
                int a = BitHelper.ToInt32(block, 0, ByteOrder.LittleEndian);
                int b = BitHelper.ToInt32(block, 4, ByteOrder.LittleEndian);
                int c = BitHelper.ToInt32(block, 8, ByteOrder.LittleEndian);
                int d = BitHelper.ToInt32(block, 12, ByteOrder.LittleEndian);
                b += s[0];
                d += s[1];
                for (int round = 1; round <= rounds; round++)
                {
                    int t = BitHelper.RotateLeft(b * (b << 1 | 1), 5);
                    int u = BitHelper.RotateLeft(d * (d << 1 | 1), 5);
                    a = BitHelper.RotateLeft(a ^ t, u & 0x1F) + s[round << 1];
                    c = BitHelper.RotateLeft(c ^ u, t & 0x1F) + s[round << 1 | 1];

                    int buf = a;
                    a = b;
                    b = c;
                    c = d;
                    d = buf;
                }
                a += s[rounds * 2 + 2];
                c += s[rounds * 2 + 3];
                byte[] ciphertext = new byte[16];
                BitHelper.GetBytes(a, ByteOrder.LittleEndian, ciphertext, 0);
                BitHelper.GetBytes(b, ByteOrder.LittleEndian, ciphertext, 4);
                BitHelper.GetBytes(c, ByteOrder.LittleEndian, ciphertext, 8);
                BitHelper.GetBytes(d, ByteOrder.LittleEndian, ciphertext, 12);
                return ciphertext;
            }

            public byte[] Decrypt(byte[] block)
            {
                int a = BitHelper.ToInt32(block, 0, ByteOrder.LittleEndian);
                int b = BitHelper.ToInt32(block, 4, ByteOrder.LittleEndian);
                int c = BitHelper.ToInt32(block, 8, ByteOrder.LittleEndian);
                int d = BitHelper.ToInt32(block, 12, ByteOrder.LittleEndian);
                a -= s[rounds * 2 + 2];
                c -= s[rounds * 2 + 3];
                for (int round = rounds; round > 0; round--)
                {
                    int buf = a;
                    a = d;
                    d = c;
                    c = b;
                    b = buf;

                    int t = BitHelper.RotateLeft(b * (b << 1 | 1), 5);
                    int u = BitHelper.RotateLeft(d * (d << 1 | 1), 5);
                    a = BitHelper.RotateRight(a - s[round << 1], u & 0x1F) ^ t;
                    c = BitHelper.RotateRight(c - s[round << 1 | 1], t & 0x1F) ^ u;
                }
                b -= s[0];
                d -= s[1];
                byte[] plaintext = new byte[16];
                BitHelper.GetBytes(a, ByteOrder.LittleEndian, plaintext, 0);
                BitHelper.GetBytes(b, ByteOrder.LittleEndian, plaintext, 4);
                BitHelper.GetBytes(c, ByteOrder.LittleEndian, plaintext, 8);
                BitHelper.GetBytes(d, ByteOrder.LittleEndian, plaintext, 12);
                return plaintext;
            }
        }
    }
}