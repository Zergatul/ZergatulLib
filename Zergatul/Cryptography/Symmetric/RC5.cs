using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public class RC5 : AbstractBlockCipher
    {
        public override int BlockSize => _w >> 2;
        public override int KeySize => _k;

        private int _w, _r, _k;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w">Word size in bits. Allowable values: 32, 64</param>
        /// <param name="r">Number of rounds. Allowable values: 0..255</param>
        /// <param name="k">Key length in bytes. Allowable values: 0..255</param>
        public RC5(int w, int r, int k)
        {
            // 16 is not supported
            if (w != 32 && w != 64)
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
                case 64: return new _64Bit(_r, key).Encrypt;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override Func<byte[], byte[]> CreateDecryptor(byte[] key)
        {
            switch (_w)
            {
                case 32: return new _32Bit(_r, key).Decrypt;
                case 64: return new _64Bit(_r, key).Decrypt;
                default:
                    throw new InvalidOperationException();
            }
        }

        // Copied implementation
        // Please copy any changes between 3 classes: _16Bit, _32Bit, _64Bit
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
                int t = (rounds + 1) << 1;
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
                a += s[0];
                b += s[1];
                for (int round = 1; round <= rounds; round++)
                {
                    a = BitHelper.RotateLeft(a ^ b, b & 0x1F) + s[round << 1];
                    b = BitHelper.RotateLeft(b ^ a, a & 0x1F) + s[round << 1 | 1];
                }
                byte[] ciphertext = new byte[8];
                BitHelper.GetBytes(a, ByteOrder.LittleEndian, ciphertext, 0);
                BitHelper.GetBytes(b, ByteOrder.LittleEndian, ciphertext, 4);
                return ciphertext;
            }

            public byte[] Decrypt(byte[] block)
            {
                int a = BitHelper.ToInt32(block, 0, ByteOrder.LittleEndian);
                int b = BitHelper.ToInt32(block, 4, ByteOrder.LittleEndian);
                for (int round = rounds; round > 0; round--)
                {
                    b = BitHelper.RotateRight(b - s[round << 1 | 1], a & 0x1F) ^ a;
                    a = BitHelper.RotateRight(a - s[round << 1], b & 0x1F) ^ b;
                }
                a -= s[0];
                b -= s[1];
                byte[] plaintext = new byte[8];
                BitHelper.GetBytes(a, ByteOrder.LittleEndian, plaintext, 0);
                BitHelper.GetBytes(b, ByteOrder.LittleEndian, plaintext, 4);
                return plaintext;
            }
        }

        // Copied implementation
        // Please copy any changes between 3 classes: _16Bit, _32Bit, _64Bit
        private class _64Bit
        {
            const ulong P = 0xB7E151628AED2A6B;
            const ulong Q = 0x9E3779B97F4A7C15;

            private int rounds;
            private ulong[] s;

            public _64Bit(int rounds, byte[] key)
            {
                this.rounds = rounds;

                // Converting the secret key from bytes to words
                int c = (System.Math.Max(key.Length, 1) + 7) / 8;
                ulong[] L = new ulong[c];
                for (int k = 0; k < key.Length; k++)
                    L[k >> 3] |= (ulong)key[k] << ((k & 0x07) << 3);

                // Initializing the array S
                int t = (rounds + 1) << 1;
                s = new ulong[t];
                s[0] = P;
                for (int k = 1; k < t; k++)
                    s[k] = s[k - 1] + Q;

                // Mixing in the secret key
                int cycles = 3 * System.Math.Max(t, c);
                int i = 0, j = 0;
                ulong a = 0, b = 0;
                for (int k = 0; k < cycles; k++)
                {
                    a = s[i] = BitHelper.RotateLeft(s[i] + a + b, 3);
                    b = L[j] = BitHelper.RotateLeft(L[j] + a + b, (int)((a + b) & 0x3F));
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
                ulong a = BitHelper.ToUInt64(block, 0, ByteOrder.LittleEndian);
                ulong b = BitHelper.ToUInt64(block, 8, ByteOrder.LittleEndian);
                a += s[0];
                b += s[1];
                for (int round = 1; round <= rounds; round++)
                {
                    a = BitHelper.RotateLeft(a ^ b, (int)(b & 0x3F)) + s[round << 1];
                    b = BitHelper.RotateLeft(b ^ a, (int)(a & 0x3F)) + s[round << 1 | 1];
                }
                byte[] ciphertext = new byte[16];
                BitHelper.GetBytes(a, ByteOrder.LittleEndian, ciphertext, 0);
                BitHelper.GetBytes(b, ByteOrder.LittleEndian, ciphertext, 8);
                return ciphertext;
            }

            public byte[] Decrypt(byte[] block)
            {
                ulong a = BitHelper.ToUInt64(block, 0, ByteOrder.LittleEndian);
                ulong b = BitHelper.ToUInt64(block, 8, ByteOrder.LittleEndian);
                for (int round = rounds; round > 0; round--)
                {
                    b = BitHelper.RotateRight(b - s[round << 1 | 1], (int)(a & 0x3F)) ^ a;
                    a = BitHelper.RotateRight(a - s[round << 1], (int)(b & 0x3F)) ^ b;
                }
                a -= s[0];
                b -= s[1];
                byte[] plaintext = new byte[16];
                BitHelper.GetBytes(a, ByteOrder.LittleEndian, plaintext, 0);
                BitHelper.GetBytes(b, ByteOrder.LittleEndian, plaintext, 8);
                return plaintext;
            }
        }
    }
}