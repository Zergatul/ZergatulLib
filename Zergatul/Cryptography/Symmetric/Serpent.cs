using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public class Serpent : AbstractBlockCipher
    {
        public override int BlockSize => 16;
        public override int KeySize => _keySize;

        private int _keySize;

        public Serpent(int keySize)
        {
            this._keySize = KeySize;
        }

        private static void sb0(uint a, uint b, uint c, uint d, out uint x0, out uint x1, out uint x2, out uint x3)
        {
            uint t1 = a ^ d;
            uint t3 = c ^ t1;
            uint t4 = b ^ t3;
            x3 = (a & d) ^ t4;
            uint t7 = a ^ (b & t1);
            x2 = t4 ^ (c | t7);
            uint t12 = x3 & (t3 ^ t7);
            x1 = (~t3) ^ t12;
            x0 = t12 ^ (~t7);
        }

        private static void sb1(uint a, uint b, uint c, uint d, out uint x0, out uint x1, out uint x2, out uint x3)
        {
            uint t2 = b ^ (~a);
            uint t5 = c ^ (a | t2);
            x2 = d ^ t5;
            uint t7 = b ^ (d | t2);
            uint t8 = t2 ^ x2;
            x3 = t8 ^ (t5 & t7);
            uint t11 = t5 ^ t7;
            x1 = x3 ^ t11;
            x0 = t5 ^ (t8 & t11);
        }

        private static void sb2(uint a, uint b, uint c, uint d, out uint x0, out uint x1, out uint x2, out uint x3)
        {
            uint t1 = ~a;
            uint t2 = b ^ d;
            uint t3 = c & t1;
            x0 = t2 ^ t3;
            uint t5 = c ^ t1;
            uint t6 = c ^ x0;
            uint t7 = b & t6;
            x3 = t5 ^ t7;
            x2 = a ^ ((d | t7) & (x0 | t5));
            x1 = (t2 ^ x3) ^ (x2 ^ (d | t1));
        }

        private static void sb3(uint a, uint b, uint c, uint d, out uint x0, out uint x1, out uint x2, out uint x3)
        {
            uint t1 = a ^ b;
            uint t2 = a & c;
            uint t3 = a | d;
            uint t4 = c ^ d;
            uint t5 = t1 & t3;
            uint t6 = t2 | t5;
            x2 = t4 ^ t6;
            uint t8 = b ^ t3;
            uint t9 = t6 ^ t8;
            uint t10 = t4 & t9;
            x0 = t1 ^ t10;
            uint t12 = x2 & x0;
            x1 = t9 ^ t12;
            x3 = (b | d) ^ (t4 ^ t12);
        }

        private static void sb4(uint a, uint b, uint c, uint d, out uint x0, out uint x1, out uint x2, out uint x3)
        {
            uint t1 = a ^ d;
            uint t2 = d & t1;
            uint t3 = c ^ t2;
            uint t4 = b | t3;
            x3 = t1 ^ t4;
            uint t6 = ~b;
            uint t7 = t1 | t6;
            x0 = t3 ^ t7;
            uint t9 = a & x0;
            uint t10 = t1 ^ t6;
            uint t11 = t4 & t10;
            x2 = t9 ^ t11;
            x1 = (a ^ t3) ^ (t10 & x2);
        }

        private static void sb5(uint a, uint b, uint c, uint d, out uint x0, out uint x1, out uint x2, out uint x3)
        {
            uint t1 = ~a;
            uint t2 = a ^ b;
            uint t3 = a ^ d;
            uint t4 = c ^ t1;
            uint t5 = t2 | t3;
            x0 = t4 ^ t5;
            uint t7 = d & x0;
            uint t8 = t2 ^ x0;
            x1 = t7 ^ t8;
            uint t10 = t1 | x0;
            uint t11 = t2 | t7;
            uint t12 = t3 ^ t10;
            x2 = t11 ^ t12;
            x3 = (b ^ t7) ^ (x1 & t12);
        }

        private static void sb6(uint a, uint b, uint c, uint d, out uint x0, out uint x1, out uint x2, out uint x3)
        {
            uint t1 = ~a;
            uint t2 = a ^ d;
            uint t3 = b ^ t2;
            uint t4 = t1 | t2;
            uint t5 = c ^ t4;
            x1 = b ^ t5;
            uint t7 = t2 | x1;
            uint t8 = d ^ t7;
            uint t9 = t5 & t8;
            x2 = t3 ^ t9;
            uint t11 = t5 ^ t8;
            x0 = x2 ^ t11;
            x3 = (~t5) ^ (t3 & t11);
        }

        private static void sb7(uint a, uint b, uint c, uint d, out uint x0, out uint x1, out uint x2, out uint x3)
        {
            uint t1 = b ^ c;
            uint t2 = c & t1;
            uint t3 = d ^ t2;
            uint t4 = a ^ t3;
            uint t5 = d | t1;
            uint t6 = t4 & t5;
            x1 = b ^ t6;
            uint t8 = t3 | x1;
            uint t9 = a & t4;
            x3 = t1 ^ t9;
            uint t11 = t4 ^ t8;
            uint t12 = x3 & t11;
            x2 = t3 ^ t12;
            x0 = (~t11) ^ (x3 & x2);
        }

        private static void LT(ref uint x0, ref uint x1, ref uint x2, ref uint x3)
        {
            uint t0 = BitHelper.RotateLeft(x0, 13);
            uint t2 = BitHelper.RotateLeft(x2, 3);
            uint t1 = x1 ^ t0 ^ t2;
            uint t3 = x3 ^ t2 ^ (t0 << 3);

            x1 = BitHelper.RotateLeft(t1, 1);
            x3 = BitHelper.RotateLeft(t3, 7);
            x0 = BitHelper.RotateLeft(t0 ^ x1 ^ x3, 5);
            x2 = BitHelper.RotateLeft(t2 ^ x3 ^ (x1 << 7), 22);
        }

        private static uint[] KeySchedule(byte[] key)
        {
            // extend key to 256 bits
            byte[] extKey;
            if (key.Length < 32)
            {
                extKey = new byte[32];
                int len = key.Length;
                Array.Copy(key, extKey, len);
                extKey[len++] = 1;
            }
            else
                extKey = key;

            uint[] w = new uint[132];
            for (int i = 0; i < 8; i++)
                w[i] = BitHelper.ToUInt32(extKey, i * 4, ByteOrder.LittleEndian);
            for (uint i = 8; i < 16; i++)
                w[i] = BitHelper.RotateLeft(w[i - 8] ^ w[i - 5] ^ w[i - 3] ^ w[i - 1] ^ 0x9E3779B9 ^ (i - 8), 11);
            for (int i = 0; i < 8; i++)
                w[i] = w[8 + i];
            for (uint i = 8; i < 132; i++)
                w[i] = BitHelper.RotateLeft(w[i - 8] ^ w[i - 5] ^ w[i - 3] ^ w[i - 1] ^ 0x9E3779B9 ^ i, 11);

            for (int i = 0; i < 33; i++)
            {
                uint x0, x1, x2, x3;
                int i4 = i << 2;
                switch (7 - (i + 4) % 8)
                {
                    case 0: sb0(w[i4], w[i4 + 1], w[i4 + 2], w[i4 + 3], out x0, out x1, out x2, out x3); break;
                    case 1: sb1(w[i4], w[i4 + 1], w[i4 + 2], w[i4 + 3], out x0, out x1, out x2, out x3); break;
                    case 2: sb2(w[i4], w[i4 + 1], w[i4 + 2], w[i4 + 3], out x0, out x1, out x2, out x3); break;
                    case 3: sb3(w[i4], w[i4 + 1], w[i4 + 2], w[i4 + 3], out x0, out x1, out x2, out x3); break;
                    case 4: sb4(w[i4], w[i4 + 1], w[i4 + 2], w[i4 + 3], out x0, out x1, out x2, out x3); break;
                    case 5: sb5(w[i4], w[i4 + 1], w[i4 + 2], w[i4 + 3], out x0, out x1, out x2, out x3); break;
                    case 6: sb6(w[i4], w[i4 + 1], w[i4 + 2], w[i4 + 3], out x0, out x1, out x2, out x3); break;
                    case 7: sb7(w[i4], w[i4 + 1], w[i4 + 2], w[i4 + 3], out x0, out x1, out x2, out x3); break;
                    default:
                        throw new InvalidOperationException();
                }
                w[i4] = x0;
                w[i4 + 1] = x1;
                w[i4 + 2] = x2;
                w[i4 + 3] = x3;
            }

            return w;
        }

        public override Func<byte[], byte[]> CreateEncryptor(byte[] key)
        {
            uint[] w = KeySchedule(key);

            return (block) =>
            {
                uint x0 = BitHelper.ToUInt32(block, 0, ByteOrder.LittleEndian);
                uint x1 = BitHelper.ToUInt32(block, 4, ByteOrder.LittleEndian);
                uint x2 = BitHelper.ToUInt32(block, 8, ByteOrder.LittleEndian);
                uint x3 = BitHelper.ToUInt32(block, 12, ByteOrder.LittleEndian);
                
                for (int round = 0; round < 32; round++)
                {
                    int round4 = round << 2;
                    uint a = w[round4] ^ x0;
                    uint b = w[round4 + 1] ^ x1;
                    uint c = w[round4 + 2] ^ x2;
                    uint d = w[round4 + 3] ^ x3;
                    switch (round % 8)
                    {
                        case 0: sb0(a, b, c, d, out x0, out x1, out x2, out x3); break;
                        case 1: sb1(a, b, c, d, out x0, out x1, out x2, out x3); break;
                        case 2: sb2(a, b, c, d, out x0, out x1, out x2, out x3); break;
                        case 3: sb3(a, b, c, d, out x0, out x1, out x2, out x3); break;
                        case 4: sb4(a, b, c, d, out x0, out x1, out x2, out x3); break;
                        case 5: sb5(a, b, c, d, out x0, out x1, out x2, out x3); break;
                        case 6: sb6(a, b, c, d, out x0, out x1, out x2, out x3); break;
                        case 7: sb7(a, b, c, d, out x0, out x1, out x2, out x3); break;
                        default:
                            throw new InvalidOperationException();
                    }
                    if (round != 31)
                        LT(ref x0, ref x1, ref x2, ref x3);
                }

                byte[] ciphertext = new byte[16];
                BitHelper.GetBytes(x0 ^ w[128], ByteOrder.LittleEndian, ciphertext, 0);
                BitHelper.GetBytes(x1 ^ w[129], ByteOrder.LittleEndian, ciphertext, 4);
                BitHelper.GetBytes(x2 ^ w[130], ByteOrder.LittleEndian, ciphertext, 8);
                BitHelper.GetBytes(x3 ^ w[131], ByteOrder.LittleEndian, ciphertext, 12);
                return ciphertext;
            };
        }

        public override Func<byte[], byte[]> CreateDecryptor(byte[] key)
        {
            throw new NotImplementedException();
        }
    }
}