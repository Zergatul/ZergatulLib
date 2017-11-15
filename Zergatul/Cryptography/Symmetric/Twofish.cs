using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Symmetric
{
    // https://www.schneier.com/academic/paperfiles/paper-twofish-paper.pdf
    // http://www.stillhq.com/gpg/source-1.0.3/cipher/twofish.html
    public class Twofish : AbstractBlockCipher
    {
        public override int BlockSize => 16;
        public override int KeySize => _keySize;

        private static readonly byte[,] RS = new byte[,]
        {
            { 0x01, 0xA4, 0x55, 0x87, 0x5A, 0x58, 0xDB, 0x9E },
            { 0xA4, 0x56, 0x82, 0xF3, 0x1E, 0xC6, 0x68, 0xE5 },
            { 0x02, 0xA1, 0xFC, 0xC1, 0x47, 0xAE, 0x3D, 0x19 },
            { 0xA4, 0x55, 0x87, 0x5A, 0x58, 0xDB, 0x9E, 0x03 },
        };
        private static readonly BinaryPolynomial RSpoly = BinaryPolynomial.FromPowers(8, 6, 3, 2, 0);

        private static readonly byte[,] MDS = new byte[,]
        {
            { 0x01, 0xEF, 0x5B, 0x5B },
            { 0x5B, 0xEF, 0xEF, 0x01 },
            { 0xEF, 0x5B, 0x01, 0xEF },
            { 0xEF, 0x01, 0xEF, 0x5B },
        };
        private static readonly BinaryPolynomial MDSpoly = BinaryPolynomial.FromPowers(8, 6, 5, 3, 0);

        private static readonly byte[] q0 = new byte[]
        {
            0xA9, 0x67, 0xB3, 0xE8, 0x04, 0xFD, 0xA3, 0x76, 0x9A, 0x92, 0x80, 0x78,
            0xE4, 0xDD, 0xD1, 0x38, 0x0D, 0xC6, 0x35, 0x98, 0x18, 0xF7, 0xEC, 0x6C,
            0x43, 0x75, 0x37, 0x26, 0xFA, 0x13, 0x94, 0x48, 0xF2, 0xD0, 0x8B, 0x30,
            0x84, 0x54, 0xDF, 0x23, 0x19, 0x5B, 0x3D, 0x59, 0xF3, 0xAE, 0xA2, 0x82,
            0x63, 0x01, 0x83, 0x2E, 0xD9, 0x51, 0x9B, 0x7C, 0xA6, 0xEB, 0xA5, 0xBE,
            0x16, 0x0C, 0xE3, 0x61, 0xC0, 0x8C, 0x3A, 0xF5, 0x73, 0x2C, 0x25, 0x0B,
            0xBB, 0x4E, 0x89, 0x6B, 0x53, 0x6A, 0xB4, 0xF1, 0xE1, 0xE6, 0xBD, 0x45,
            0xE2, 0xF4, 0xB6, 0x66, 0xCC, 0x95, 0x03, 0x56, 0xD4, 0x1C, 0x1E, 0xD7,
            0xFB, 0xC3, 0x8E, 0xB5, 0xE9, 0xCF, 0xBF, 0xBA, 0xEA, 0x77, 0x39, 0xAF,
            0x33, 0xC9, 0x62, 0x71, 0x81, 0x79, 0x09, 0xAD, 0x24, 0xCD, 0xF9, 0xD8,
            0xE5, 0xC5, 0xB9, 0x4D, 0x44, 0x08, 0x86, 0xE7, 0xA1, 0x1D, 0xAA, 0xED,
            0x06, 0x70, 0xB2, 0xD2, 0x41, 0x7B, 0xA0, 0x11, 0x31, 0xC2, 0x27, 0x90,
            0x20, 0xF6, 0x60, 0xFF, 0x96, 0x5C, 0xB1, 0xAB, 0x9E, 0x9C, 0x52, 0x1B,
            0x5F, 0x93, 0x0A, 0xEF, 0x91, 0x85, 0x49, 0xEE, 0x2D, 0x4F, 0x8F, 0x3B,
            0x47, 0x87, 0x6D, 0x46, 0xD6, 0x3E, 0x69, 0x64, 0x2A, 0xCE, 0xCB, 0x2F,
            0xFC, 0x97, 0x05, 0x7A, 0xAC, 0x7F, 0xD5, 0x1A, 0x4B, 0x0E, 0xA7, 0x5A,
            0x28, 0x14, 0x3F, 0x29, 0x88, 0x3C, 0x4C, 0x02, 0xB8, 0xDA, 0xB0, 0x17,
            0x55, 0x1F, 0x8A, 0x7D, 0x57, 0xC7, 0x8D, 0x74, 0xB7, 0xC4, 0x9F, 0x72,
            0x7E, 0x15, 0x22, 0x12, 0x58, 0x07, 0x99, 0x34, 0x6E, 0x50, 0xDE, 0x68,
            0x65, 0xBC, 0xDB, 0xF8, 0xC8, 0xA8, 0x2B, 0x40, 0xDC, 0xFE, 0x32, 0xA4,
            0xCA, 0x10, 0x21, 0xF0, 0xD3, 0x5D, 0x0F, 0x00, 0x6F, 0x9D, 0x36, 0x42,
            0x4A, 0x5E, 0xC1, 0xE0
        };
        private static readonly byte[] q1 = new byte[]
        {
            0x75, 0xF3, 0xC6, 0xF4, 0xDB, 0x7B, 0xFB, 0xC8, 0x4A, 0xD3, 0xE6, 0x6B,
            0x45, 0x7D, 0xE8, 0x4B, 0xD6, 0x32, 0xD8, 0xFD, 0x37, 0x71, 0xF1, 0xE1,
            0x30, 0x0F, 0xF8, 0x1B, 0x87, 0xFA, 0x06, 0x3F, 0x5E, 0xBA, 0xAE, 0x5B,
            0x8A, 0x00, 0xBC, 0x9D, 0x6D, 0xC1, 0xB1, 0x0E, 0x80, 0x5D, 0xD2, 0xD5,
            0xA0, 0x84, 0x07, 0x14, 0xB5, 0x90, 0x2C, 0xA3, 0xB2, 0x73, 0x4C, 0x54,
            0x92, 0x74, 0x36, 0x51, 0x38, 0xB0, 0xBD, 0x5A, 0xFC, 0x60, 0x62, 0x96,
            0x6C, 0x42, 0xF7, 0x10, 0x7C, 0x28, 0x27, 0x8C, 0x13, 0x95, 0x9C, 0xC7,
            0x24, 0x46, 0x3B, 0x70, 0xCA, 0xE3, 0x85, 0xCB, 0x11, 0xD0, 0x93, 0xB8,
            0xA6, 0x83, 0x20, 0xFF, 0x9F, 0x77, 0xC3, 0xCC, 0x03, 0x6F, 0x08, 0xBF,
            0x40, 0xE7, 0x2B, 0xE2, 0x79, 0x0C, 0xAA, 0x82, 0x41, 0x3A, 0xEA, 0xB9,
            0xE4, 0x9A, 0xA4, 0x97, 0x7E, 0xDA, 0x7A, 0x17, 0x66, 0x94, 0xA1, 0x1D,
            0x3D, 0xF0, 0xDE, 0xB3, 0x0B, 0x72, 0xA7, 0x1C, 0xEF, 0xD1, 0x53, 0x3E,
            0x8F, 0x33, 0x26, 0x5F, 0xEC, 0x76, 0x2A, 0x49, 0x81, 0x88, 0xEE, 0x21,
            0xC4, 0x1A, 0xEB, 0xD9, 0xC5, 0x39, 0x99, 0xCD, 0xAD, 0x31, 0x8B, 0x01,
            0x18, 0x23, 0xDD, 0x1F, 0x4E, 0x2D, 0xF9, 0x48, 0x4F, 0xF2, 0x65, 0x8E,
            0x78, 0x5C, 0x58, 0x19, 0x8D, 0xE5, 0x98, 0x57, 0x67, 0x7F, 0x05, 0x64,
            0xAF, 0x63, 0xB6, 0xFE, 0xF5, 0xB7, 0x3C, 0xA5, 0xCE, 0xE9, 0x68, 0x44,
            0xE0, 0x4D, 0x43, 0x69, 0x29, 0x2E, 0xAC, 0x15, 0x59, 0xA8, 0x0A, 0x9E,
            0x6E, 0x47, 0xDF, 0x34, 0x35, 0x6A, 0xCF, 0xDC, 0x22, 0xC9, 0xC0, 0x9B,
            0x89, 0xD4, 0xED, 0xAB, 0x12, 0xA2, 0x0D, 0x52, 0xBB, 0x02, 0x2F, 0xA9,
            0xD7, 0x61, 0x1E, 0xB4, 0x50, 0x04, 0xF6, 0xC2, 0x16, 0x25, 0x86, 0x56,
            0x55, 0x09, 0xBE, 0x91
        };

        private int _keySize;

        public Twofish(int keySize)
        {
            switch (keySize)
            {
                case 16:
                case 24:
                case 32:
                    break;
                default:
                    throw new InvalidOperationException("Only 16/24/32 byte length keys are supported in Twofish");
            }

            this._keySize = keySize;
        }

        private static uint h(uint X, uint[] L)
        {
            byte[] x;
            for (int i = L.Length - 1; i >= 0; i--)
            {
                x = BitHelper.GetBytes(X, ByteOrder.LittleEndian);
                switch (i)
                {
                    case 3:
                        x[0] = q1[x[0]];
                        x[1] = q0[x[1]];
                        x[2] = q0[x[2]];
                        x[3] = q1[x[3]];
                        break;
                    case 2:
                        x[0] = q0[x[0]];
                        x[1] = q0[x[1]];
                        x[2] = q1[x[2]];
                        x[3] = q1[x[3]];
                        break;
                    case 1:
                        x[0] = q1[x[0]];
                        x[1] = q0[x[1]];
                        x[2] = q1[x[2]];
                        x[3] = q0[x[3]];
                        break;
                    case 0:
                        x[0] = q1[x[0]];
                        x[1] = q1[x[1]];
                        x[2] = q0[x[2]];
                        x[3] = q0[x[3]];
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                X = BitHelper.ToUInt32(x, ByteOrder.LittleEndian);
                X ^= L[i];
            }

            x = BitHelper.GetBytes(X, ByteOrder.LittleEndian);
            x[0] = q0[x[0]];
            x[1] = q1[x[1]];
            x[2] = q0[x[2]];
            x[3] = q1[x[3]];

            var z = new BinaryPolynomial[]
            {
                BinaryPolynomial.Zero,
                BinaryPolynomial.Zero,
                BinaryPolynomial.Zero,
                BinaryPolynomial.Zero
            };

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    z[i] += BinaryPolynomial.ModularMultiplication(
                        new BinaryPolynomial(new byte[] { x[j] }, ByteOrder.LittleEndian),
                        new BinaryPolynomial(new byte[] { MDS[i, j] }, ByteOrder.LittleEndian),
                        MDSpoly);

            return BitHelper.ToUInt32(new[]
            {
                z[0].ToBytes(ByteOrder.LittleEndian, 1)[0],
                z[1].ToBytes(ByteOrder.LittleEndian, 1)[0],
                z[2].ToBytes(ByteOrder.LittleEndian, 1)[0],
                z[3].ToBytes(ByteOrder.LittleEndian, 1)[0]
            }, ByteOrder.LittleEndian);
        }

        private static void KeySchedule(byte[] key, out uint[] K, out byte[] sbox1, out byte[] sbox2, out byte[] sbox3, out byte[] sbox4)
        {
            if (key.Length < 16)
            {
                byte[] extendedKey = new byte[16];
                Array.Copy(key, 0, extendedKey, 0, key.Length);
                key = extendedKey;
            }
            if (key.Length > 16 && key.Length < 24)
            {
                byte[] extendedKey = new byte[24];
                Array.Copy(key, 0, extendedKey, 0, key.Length);
                key = extendedKey;
            }
            if (key.Length > 24 && key.Length < 32)
            {
                byte[] extendedKey = new byte[32];
                Array.Copy(key, 0, extendedKey, 0, key.Length);
                key = extendedKey;
            }

            K = new uint[40];
            sbox1 = new byte[256];
            sbox2 = new byte[256];
            sbox3 = new byte[256];
            sbox4 = new byte[256];

            int k = key.Length / 8;
            uint[] M = new uint[2 * k];
            for (int i = 0; i < M.Length; i++)
                BitHelper.ToUInt32(key, i * 4, ByteOrder.LittleEndian);

            uint[] Me = new uint[k];
            uint[] Mo = new uint[k];
            for (int i = 0; i < k; i++)
            {
                Me[i] = M[2 * i];
                Mo[i] = M[2 * i + 1];
            }

            uint[] S = new uint[k];
            for (int si = 0; si < k; si++)
            {
                var z = new BinaryPolynomial[]
                {
                    BinaryPolynomial.Zero,
                    BinaryPolynomial.Zero,
                    BinaryPolynomial.Zero,
                    BinaryPolynomial.Zero
                };
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 8; j++)
                        z[i] += BinaryPolynomial.ModularMultiplication(
                            new BinaryPolynomial(new byte[] { key[8 * si + j] }, ByteOrder.LittleEndian),
                            new BinaryPolynomial(new byte[] { RS[i, j] }, ByteOrder.LittleEndian),
                            RSpoly);
                S[si] = BitHelper.ToUInt32(new[]
                {
                    z[0].ToBytes(ByteOrder.LittleEndian, 1)[0],
                    z[1].ToBytes(ByteOrder.LittleEndian, 1)[0],
                    z[2].ToBytes(ByteOrder.LittleEndian, 1)[0],
                    z[3].ToBytes(ByteOrder.LittleEndian, 1)[0]
                }, ByteOrder.LittleEndian);
            }
            Array.Reverse(S);

            uint ρ = (1 << 24) | (1 << 16) | (1 << 8) | 1;
            for (uint i = 0; i < 20; i++)
            {
                uint A = h(2 * i * ρ, Me);
                uint B = BitHelper.RotateLeft(h((2 * i + 1) * ρ, Mo), 8);
                K[2 * i] = unchecked(A + B);
                K[2 * i + 1] = BitHelper.RotateLeft(unchecked(A + 2 * B), 9);
            }
        }

        delegate void FFunction(uint r0, uint r1, int r, out uint f0, out uint f1);

        public override Func<byte[], byte[]> CreateEncryptor(byte[] key)
        {
            uint[] k = null;
            byte[] sbox0 = null;
            byte[] sbox1 = null;
            byte[] sbox2 = null;
            byte[] sbox3 = null;

            KeySchedule(key, out k, out sbox0, out sbox1, out sbox2, out sbox3);

            var bf = BinaryPolynomial.FromPowers(8, 6, 5, 3, 0);

            Func<uint, uint> g = x =>
            {
                byte[] y = BitHelper.GetBytes(x, ByteOrder.LittleEndian);
                y[0] = sbox0[y[0]];
                y[1] = sbox1[y[1]];
                y[2] = sbox2[y[2]];
                y[3] = sbox3[y[3]];

                BinaryPolynomial[] z = new BinaryPolynomial[]
                {
                    BinaryPolynomial.Zero,
                    BinaryPolynomial.Zero,
                    BinaryPolynomial.Zero,
                    BinaryPolynomial.Zero
                };

                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        z[i] += BinaryPolynomial.ModularMultiplication(
                            new BinaryPolynomial(new byte[] { y[j] }, ByteOrder.LittleEndian),
                            new BinaryPolynomial(new byte[] { MDS[i, j] }, ByteOrder.LittleEndian),
                            bf);

                return BitHelper.ToUInt32(new byte[]
                {
                    z[0].ToBytes(ByteOrder.LittleEndian, 1)[0],
                    z[1].ToBytes(ByteOrder.LittleEndian, 1)[0],
                    z[2].ToBytes(ByteOrder.LittleEndian, 1)[0],
                    z[3].ToBytes(ByteOrder.LittleEndian, 1)[0],
                }, ByteOrder.LittleEndian);
            };

            FFunction F = (uint r0, uint r1, int r, out uint f0, out uint f1) =>
            {
                uint t0 = g(r0);
                uint t1 = g(BitHelper.RotateLeft(r1, 8));
                f0 = t0 + t1 + k[2 * r + 8];
                f1 = t0 + (t1 << 1) + k[2 * r + 9];
            };

            return (block) =>
            {
                uint x0 = BitHelper.ToUInt32(block, 0, ByteOrder.LittleEndian);
                uint x1 = BitHelper.ToUInt32(block, 4, ByteOrder.LittleEndian);
                uint x2 = BitHelper.ToUInt32(block, 8, ByteOrder.LittleEndian);
                uint x3 = BitHelper.ToUInt32(block, 12, ByteOrder.LittleEndian);

                x0 ^= k[0];
                x1 ^= k[1];
                x2 ^= k[2];
                x3 ^= k[3];

                for (int round = 0; round < 16; round++)
                {
                    uint f0, f1;
                    F(x0, x1, round, out f0, out f1);
                    uint y0 = BitHelper.RotateRight(x1 ^ f0, 1);
                    uint y1 = BitHelper.RotateLeft(x3, 1) ^ f1;

                    x0 = y0;
                    x1 = y1;
                    x2 = x0;
                    x3 = x1;
                }

                x0 ^= k[4];
                x1 ^= k[5];
                x2 ^= k[6];
                x3 ^= k[7];

                return null;
            };
        }

        public override Func<byte[], byte[]> CreateDecryptor(byte[] key)
        {
            throw new NotImplementedException();
        }
    }
}