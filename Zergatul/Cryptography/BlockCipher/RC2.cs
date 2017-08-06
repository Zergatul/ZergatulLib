using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    // https://tools.ietf.org/html/rfc2268
    public class RC2 : AbstractBlockCipher
    {
        public override int BlockSize => 8;

        public override int KeySize => _keySize;

        private int _keySize;
        private int _keySizeBits;

        private static readonly byte[] Pitable = new byte[]
        {
            0xD9, 0x78, 0xF9, 0xC4, 0x19, 0xDD, 0xB5, 0xED, 0x28, 0xE9, 0xFD, 0x79, 0x4A, 0xA0, 0xD8, 0x9D,
            0xC6, 0x7E, 0x37, 0x83, 0x2B, 0x76, 0x53, 0x8E, 0x62, 0x4C, 0x64, 0x88, 0x44, 0x8B, 0xFB, 0xA2,
            0x17, 0x9A, 0x59, 0xF5, 0x87, 0xB3, 0x4F, 0x13, 0x61, 0x45, 0x6D, 0x8D, 0x09, 0x81, 0x7D, 0x32,
            0xBD, 0x8F, 0x40, 0xEB, 0x86, 0xB7, 0x7B, 0x0B, 0xF0, 0x95, 0x21, 0x22, 0x5C, 0x6B, 0x4E, 0x82,
            0x54, 0xD6, 0x65, 0x93, 0xCE, 0x60, 0xB2, 0x1C, 0x73, 0x56, 0xC0, 0x14, 0xA7, 0x8C, 0xF1, 0xDC,
            0x12, 0x75, 0xCA, 0x1F, 0x3B, 0xBE, 0xE4, 0xD1, 0x42, 0x3D, 0xD4, 0x30, 0xA3, 0x3C, 0xB6, 0x26,
            0x6F, 0xBF, 0x0E, 0xDA, 0x46, 0x69, 0x07, 0x57, 0x27, 0xF2, 0x1D, 0x9B, 0xBC, 0x94, 0x43, 0x03,
            0xF8, 0x11, 0xC7, 0xF6, 0x90, 0xEF, 0x3E, 0xE7, 0x06, 0xC3, 0xD5, 0x2F, 0xC8, 0x66, 0x1E, 0xD7,
            0x08, 0xE8, 0xEA, 0xDE, 0x80, 0x52, 0xEE, 0xF7, 0x84, 0xAA, 0x72, 0xAC, 0x35, 0x4D, 0x6A, 0x2A,
            0x96, 0x1A, 0xD2, 0x71, 0x5A, 0x15, 0x49, 0x74, 0x4B, 0x9F, 0xD0, 0x5E, 0x04, 0x18, 0xA4, 0xEC,
            0xC2, 0xE0, 0x41, 0x6E, 0x0F, 0x51, 0xCB, 0xCC, 0x24, 0x91, 0xAF, 0x50, 0xA1, 0xF4, 0x70, 0x39,
            0x99, 0x7C, 0x3A, 0x85, 0x23, 0xB8, 0xB4, 0x7A, 0xFC, 0x02, 0x36, 0x5B, 0x25, 0x55, 0x97, 0x31,
            0x2D, 0x5D, 0xFA, 0x98, 0xE3, 0x8A, 0x92, 0xAE, 0x05, 0xDF, 0x29, 0x10, 0x67, 0x6C, 0xBA, 0xC9,
            0xD3, 0x00, 0xE6, 0xCF, 0xE1, 0x9E, 0xA8, 0x2C, 0x63, 0x16, 0x01, 0x3F, 0x58, 0xE2, 0x89, 0xA9,
            0x0D, 0x38, 0x34, 0x1B, 0xAB, 0x33, 0xFF, 0xB0, 0xBB, 0x48, 0x0C, 0x5F, 0xB9, 0xB1, 0xCD, 0x2E,
            0xC5, 0xF3, 0xDB, 0x47, 0xE5, 0xA5, 0x9C, 0x77, 0x0A, 0xA6, 0x20, 0x68, 0xFE, 0x7F, 0xC1, 0xAD
        };

        private static readonly byte[] s = new byte[] { 1, 2, 3, 5 };

        public RC2(int keySizeBytes, int effectiveKeySizeBits)
        {
            this._keySize = keySizeBytes;
            this._keySizeBits = effectiveKeySizeBits;
        }

        private static ushort[] KeyExpansion(byte[] key, int bits)
        {
            int T = key.Length;

            byte[] L = new byte[128];
            Array.Copy(key, L, T);

            int T1 = bits;
            int T8 = (T1 + 7) / 8;
            int TM = 255 % (1 << (8 + T1 - 8 * T8));

            for (int i = T; i < 128; i++)
                L[i] = Pitable[(L[i - 1] + L[i - T]) & 0xFF];

            L[128 - T8] = Pitable[L[128 - T8] & TM];

            for (int i = 127 - T8; i >= 0; i--)
                L[i] = Pitable[L[i + 1] ^ L[i + T8]];

            ushort[] K = new ushort[64];
            for (int i = 0; i < 64; i++)
                K[i] = (ushort)(L[i << 1] | (L[(i << 1) | 1] << 8));

            return K;
        }

        private static void MixUp(ushort[] R, ushort[] K, int i, ref int j)
        {
            R[i] = (ushort)(R[i] + K[j] + (R[(i + 3) & 0x03] & R[(i + 2) & 0x03]) + (~R[(i + 3) & 0x03] & R[(i + 1) & 0x03]));
            j++;
            R[i] = BitHelper.RotateLeft(R[i], s[i]);
        }

        private static void MixUpRound(ushort[] R, ushort[] K, ref int j)
        {
            MixUp(R, K, 0, ref j);
            MixUp(R, K, 1, ref j);
            MixUp(R, K, 2, ref j);
            MixUp(R, K, 3, ref j);
        }

        private static void RMixUp(ushort[] R, ushort[] K, int i, ref int j)
        {
            R[i] = BitHelper.RotateRight(R[i], s[i]);
            R[i] = (ushort)(R[i] - K[j] - (R[(i + 3) & 0x03] & R[(i + 2) & 0x03]) - (~R[(i + 3) & 0x03] & R[(i + 1) & 0x03]));
            j--;
        }

        private static void RMixUpRound(ushort[] R, ushort[] K, ref int j)
        {
            RMixUp(R, K, 3, ref j);
            RMixUp(R, K, 2, ref j);
            RMixUp(R, K, 1, ref j);
            RMixUp(R, K, 0, ref j);
        }

        private static void Mash(ushort[] R, ushort[] K, int i)
        {
            R[i] += K[R[(i + 3) & 0x03] & 0x3F];
        }

        private static void MashRound(ushort[] R, ushort[] K)
        {
            Mash(R, K, 0);
            Mash(R, K, 1);
            Mash(R, K, 2);
            Mash(R, K, 3);
        }

        private static void RMash(ushort[] R, ushort[] K, int i)
        {
            R[i] -= K[R[(i + 3) & 0x03] & 0x3F];
        }

        private static void RMashRound(ushort[] R, ushort[] K)
        {
            RMash(R, K, 3);
            RMash(R, K, 2);
            RMash(R, K, 1);
            RMash(R, K, 0);
        }

        public override Encryptor CreateEncryptor(byte[] key, BlockCipherMode mode)
        {
            ushort[] K = KeyExpansion(key, _keySizeBits);

            var encryptor = ResolveEncryptor(mode);
            encryptor.Cipher = this;
            encryptor.ProcessBlock = (block) =>
            {
                ushort[] R = new ushort[4];
                for (int i = 0; i < 4; i++)
                    R[i] = BitHelper.ToUInt16(block, i << 1, ByteOrder.LittleEndian);

                int j = 0;

                for (int i = 0; i < 5; i++)
                    MixUpRound(R, K, ref j);

                MashRound(R, K);

                for (int i = 0; i < 6; i++)
                    MixUpRound(R, K, ref j);

                MashRound(R, K);

                for (int i = 0; i < 5; i++)
                    MixUpRound(R, K, ref j);

                byte[] result = new byte[8];
                for (int i = 0; i < 4; i++)
                    BitHelper.GetBytes(R[i], ByteOrder.LittleEndian, result, i << 1);

                return result;
            };

            return encryptor;
        }

        public override Decryptor CreateDecryptor(byte[] key, BlockCipherMode mode)
        {
            ushort[] K = KeyExpansion(key, _keySizeBits);

            var decryptor = ResolveDecryptor(mode);
            decryptor.Cipher = this;
            decryptor.ProcessBlock = (block) =>
            {
                ushort[] R = new ushort[4];
                for (int i = 0; i < 4; i++)
                    R[i] = BitHelper.ToUInt16(block, i << 1, ByteOrder.LittleEndian);

                int j = 63;

                for (int i = 0; i < 5; i++)
                    RMixUpRound(R, K, ref j);

                RMashRound(R, K);

                for (int i = 0; i < 6; i++)
                    RMixUpRound(R, K, ref j);

                RMashRound(R, K);

                for (int i = 0; i < 5; i++)
                    RMixUpRound(R, K, ref j);

                byte[] result = new byte[8];
                for (int i = 0; i < 4; i++)
                    BitHelper.GetBytes(R[i], ByteOrder.LittleEndian, result, i << 1);

                return result;
            };

            return decryptor;
        }
    }
}