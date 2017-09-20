using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Symmetric
{
    // https://www.schneier.com/academic/paperfiles/paper-twofish-paper.pdf
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

        private static readonly byte[,] MDS = new byte[,]
        {
            { 0x01, 0xEF, 0x5B, 0x5B },
            { 0x5B, 0xEF, 0xEF, 0x01 },
            { 0xEF, 0x5B, 0x01, 0xEF },
            { 0xEF, 0x01, 0xEF, 0x5B },
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

        private static void KeySchedule(byte[] key)
        {
            int k = key.Length / 8;
            uint[] M = new uint[2 * k];
            for (int i = 0; i < M.Length; i++)
                BitHelper.ToUInt32(key, i * 4, ByteOrder.LittleEndian);

            var bf = BinaryPolynomial.FromPowers(8, 6, 3, 2, 0);

            byte[] s = new byte[key.Length / 2];
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
                            new BinaryPolynomial(new byte[] { MDS[i, j] }, ByteOrder.LittleEndian),
                            bf);
                s[si * 4 + 0] = z[0].ToBytes(ByteOrder.LittleEndian, 1)[0];
                s[si * 4 + 1] = z[1].ToBytes(ByteOrder.LittleEndian, 1)[0];
                s[si * 4 + 2] = z[2].ToBytes(ByteOrder.LittleEndian, 1)[0];
                s[si * 4 + 3] = z[3].ToBytes(ByteOrder.LittleEndian, 1)[0];
            }
            uint[] S = new uint[k];
            for (int i = 0; i < k; i++)
                BitHelper.ToUInt32(s, (k - 1 - i) * 4, ByteOrder.LittleEndian);
        }

        delegate void FFunction(uint r0, uint r1, int r, out uint f0, out uint f1);

        public override Func<byte[], byte[]> CreateEncryptor(byte[] key)
        {
            uint[] k = null;
            byte[] sbox0 = null;
            byte[] sbox1 = null;
            byte[] sbox2 = null;
            byte[] sbox3 = null;

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