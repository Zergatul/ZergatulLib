using System;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash.Base
{
    public abstract class BMW_32Bit : AbstractHash
    {
        public override int BlockSize => 64;
        public override OID OID => null;

        private static readonly uint[] FinalBlock = new uint[]
        {
            0xAAAAAAA0, 0xAAAAAAA1, 0xAAAAAAA2, 0xAAAAAAA3,
            0xAAAAAAA4, 0xAAAAAAA5, 0xAAAAAAA6, 0xAAAAAAA7,
            0xAAAAAAA8, 0xAAAAAAA9, 0xAAAAAAAA, 0xAAAAAAAB,
            0xAAAAAAAC, 0xAAAAAAAD, 0xAAAAAAAE, 0xAAAAAAAF
        };

        private uint[] m = new uint[16];
        private uint[] q = new uint[32];
        protected uint[] h = new uint[16];

        protected override void ProcessBlock()
        {
            BitHelper.ToUInt32Array(_block, ByteOrder.LittleEndian, m);
            F0(h, m, q);
            F1(h, m, q);
            F2(h, m, q);
        }

        protected override void AddPadding()
        {
            _buffer.Add(0x80);
            while ((_buffer.Count + 8) % BlockSize != 0)
                _buffer.Add(0);
            _buffer.AddRange(BitHelper.GetBytes(_totalBytes * 8, ByteOrder.LittleEndian));
        }

        protected void DoFinal()
        {
            Array.Copy(h, 0, m, 0, 16);
            Array.Copy(FinalBlock, 0, h, 0, 16);
            F0(h, m, q);
            F1(h, m, q);
            F2(h, m, q);
        }

        private static void F0(uint[] h, uint[] m, uint[] q)
        {
            q[0x0] = s0((m[0x5] ^ h[0x5]) - (m[0x7] ^ h[0x7]) + (m[0xA] ^ h[0xA]) + (m[0xD] ^ h[0xD]) + (m[0xE] ^ h[0xE])) + h[0x1];
            q[0x1] = s1((m[0x6] ^ h[0x6]) - (m[0x8] ^ h[0x8]) + (m[0xB] ^ h[0xB]) + (m[0xE] ^ h[0xE]) - (m[0xF] ^ h[0xF])) + h[0x2];
            q[0x2] = s2((m[0x0] ^ h[0x0]) + (m[0x7] ^ h[0x7]) + (m[0x9] ^ h[0x9]) - (m[0xC] ^ h[0xC]) + (m[0xF] ^ h[0xF])) + h[0x3];
            q[0x3] = s3((m[0x0] ^ h[0x0]) - (m[0x1] ^ h[0x1]) + (m[0x8] ^ h[0x8]) - (m[0xA] ^ h[0xA]) + (m[0xD] ^ h[0xD])) + h[0x4];
            q[0x4] = s4((m[0x1] ^ h[0x1]) + (m[0x2] ^ h[0x2]) + (m[0x9] ^ h[0x9]) - (m[0xB] ^ h[0xB]) - (m[0xE] ^ h[0xE])) + h[0x5];
            q[0x5] = s0((m[0x3] ^ h[0x3]) - (m[0x2] ^ h[0x2]) + (m[0xA] ^ h[0xA]) - (m[0xC] ^ h[0xC]) + (m[0xF] ^ h[0xF])) + h[0x6];
            q[0x6] = s1((m[0x4] ^ h[0x4]) - (m[0x0] ^ h[0x0]) - (m[0x3] ^ h[0x3]) - (m[0xB] ^ h[0xB]) + (m[0xD] ^ h[0xD])) + h[0x7];
            q[0x7] = s2((m[0x1] ^ h[0x1]) - (m[0x4] ^ h[0x4]) - (m[0x5] ^ h[0x5]) - (m[0xC] ^ h[0xC]) - (m[0xE] ^ h[0xE])) + h[0x8];
            q[0x8] = s3((m[0x2] ^ h[0x2]) - (m[0x5] ^ h[0x5]) - (m[0x6] ^ h[0x6]) + (m[0xD] ^ h[0xD]) - (m[0xF] ^ h[0xF])) + h[0x9];
            q[0x9] = s4((m[0x0] ^ h[0x0]) - (m[0x3] ^ h[0x3]) + (m[0x6] ^ h[0x6]) - (m[0x7] ^ h[0x7]) + (m[0xE] ^ h[0xE])) + h[0xA];
            q[0xA] = s0((m[0x8] ^ h[0x8]) - (m[0x1] ^ h[0x1]) - (m[0x4] ^ h[0x4]) - (m[0x7] ^ h[0x7]) + (m[0xF] ^ h[0xF])) + h[0xB];
            q[0xB] = s1((m[0x8] ^ h[0x8]) - (m[0x0] ^ h[0x0]) - (m[0x2] ^ h[0x2]) - (m[0x5] ^ h[0x5]) + (m[0x9] ^ h[0x9])) + h[0xC];
            q[0xC] = s2((m[0x1] ^ h[0x1]) + (m[0x3] ^ h[0x3]) - (m[0x6] ^ h[0x6]) - (m[0x9] ^ h[0x9]) + (m[0xA] ^ h[0xA])) + h[0xD];
            q[0xD] = s3((m[0x2] ^ h[0x2]) + (m[0x4] ^ h[0x4]) + (m[0x7] ^ h[0x7]) + (m[0xA] ^ h[0xA]) + (m[0xB] ^ h[0xB])) + h[0xE];
            q[0xE] = s4((m[0x3] ^ h[0x3]) - (m[0x5] ^ h[0x5]) + (m[0x8] ^ h[0x8]) - (m[0xB] ^ h[0xB]) - (m[0xC] ^ h[0xC])) + h[0xF];
            q[0xF] = s0((m[0xC] ^ h[0xC]) - (m[0x4] ^ h[0x4]) - (m[0x6] ^ h[0x6]) - (m[0x9] ^ h[0x9]) + (m[0xD] ^ h[0xD])) + h[0x0];
        }

        private static void F1(uint[] h, uint[] m, uint[] q)
        {
            q[0x10] = Expand1(h, m, q, 16);
            q[0x11] = Expand1(h, m, q, 17);
            q[0x12] = Expand2(h, m, q, 18);
            q[0x13] = Expand2(h, m, q, 19);
            q[0x14] = Expand2(h, m, q, 20);
            q[0x15] = Expand2(h, m, q, 21);
            q[0x16] = Expand2(h, m, q, 22);
            q[0x17] = Expand2(h, m, q, 23);
            q[0x18] = Expand2(h, m, q, 24);
            q[0x19] = Expand2(h, m, q, 25);
            q[0x1A] = Expand2(h, m, q, 26);
            q[0x1B] = Expand2(h, m, q, 27);
            q[0x1C] = Expand2(h, m, q, 28);
            q[0x1D] = Expand2(h, m, q, 29);
            q[0x1E] = Expand2(h, m, q, 30);
            q[0x1F] = Expand2(h, m, q, 31);
        }

        private static void F2(uint[] h, uint[] m, uint[] q)
        {
            uint xl = q[16] ^ q[17] ^ q[18] ^ q[19] ^ q[20] ^ q[21] ^ q[22] ^ q[23];
            uint xh = xl ^ q[24] ^ q[25] ^ q[26] ^ q[27] ^ q[28] ^ q[29] ^ q[30] ^ q[31];
            h[0x0] = ((xh <<  5) ^ (q[16] >>  5) ^ m[0]) + (xl ^ q[24] ^ q[0]);
            h[0x1] = ((xh >>  7) ^ (q[17] <<  8) ^ m[1]) + (xl ^ q[25] ^ q[1]);
            h[0x2] = ((xh >>  5) ^ (q[18] <<  5) ^ m[2]) + (xl ^ q[26] ^ q[2]);
            h[0x3] = ((xh >>  1) ^ (q[19] <<  5) ^ m[3]) + (xl ^ q[27] ^ q[3]);
            h[0x4] = ((xh >>  3) ^ (q[20] <<  0) ^ m[4]) + (xl ^ q[28] ^ q[4]);
            h[0x5] = ((xh <<  6) ^ (q[21] >>  6) ^ m[5]) + (xl ^ q[29] ^ q[5]);
            h[0x6] = ((xh >>  4) ^ (q[22] <<  6) ^ m[6]) + (xl ^ q[30] ^ q[6]);
            h[0x7] = ((xh >> 11) ^ (q[23] <<  2) ^ m[7]) + (xl ^ q[31] ^ q[7]);
            h[0x8] = BitHelper.RotateLeft(h[4],  9) + (xh ^ q[24] ^ m[ 8]) + ((xl << 8) ^ q[23] ^ q[ 8]);
            h[0x9] = BitHelper.RotateLeft(h[5], 10) + (xh ^ q[25] ^ m[ 9]) + ((xl >> 6) ^ q[16] ^ q[ 9]);
            h[0xA] = BitHelper.RotateLeft(h[6], 11) + (xh ^ q[26] ^ m[10]) + ((xl << 6) ^ q[17] ^ q[10]);
            h[0xB] = BitHelper.RotateLeft(h[7], 12) + (xh ^ q[27] ^ m[11]) + ((xl << 4) ^ q[18] ^ q[11]);
            h[0xC] = BitHelper.RotateLeft(h[0], 13) + (xh ^ q[28] ^ m[12]) + ((xl >> 3) ^ q[19] ^ q[12]);
            h[0xD] = BitHelper.RotateLeft(h[1], 14) + (xh ^ q[29] ^ m[13]) + ((xl >> 4) ^ q[20] ^ q[13]);
            h[0xE] = BitHelper.RotateLeft(h[2], 15) + (xh ^ q[30] ^ m[14]) + ((xl >> 7) ^ q[21] ^ q[14]);
            h[0xF] = BitHelper.RotateLeft(h[3], 16) + (xh ^ q[31] ^ m[15]) + ((xl >> 2) ^ q[22] ^ q[15]);
        }

        private static uint Expand1(uint[] h, uint[]m, uint[] q, int j)
        {
            uint mp =
                BitHelper.RotateLeft(m[(j + 0x00) & 0x0F], (j +  1 - 16)) +
                BitHelper.RotateLeft(m[(j + 0x03) & 0x0F], (j +  4 - 16)) -
                BitHelper.RotateLeft(m[(j + 0x0A) & 0x0F], (j + 11 - 16)) +
                (uint)j * 0x05555555;
            return
                s1(q[j - 0x10]) + s2(q[j - 0x0F]) + s3(q[j - 0x0E]) + s0(q[j - 0x0D]) +
                s1(q[j - 0x0C]) + s2(q[j - 0x0B]) + s3(q[j - 0x0A]) + s0(q[j - 0x09]) +
                s1(q[j - 0x08]) + s2(q[j - 0x07]) + s3(q[j - 0x06]) + s0(q[j - 0x05]) +
                s1(q[j - 0x04]) + s2(q[j - 0x03]) + s3(q[j - 0x02]) + s0(q[j - 0x01]) +
                (h[j - 0x09] ^ mp);
        }

        private static uint Expand2(uint[] h, uint[] m, uint[] q, int j)
        {
            uint mp =
                BitHelper.RotateLeft(m[(j + 0x00) & 0x0F], 1 + ((j +  2 - 18) & 0x0F)) +
                BitHelper.RotateLeft(m[(j + 0x03) & 0x0F], 1 + ((j +  5 - 18) & 0x0F)) -
                BitHelper.RotateLeft(m[(j + 0x0A) & 0x0F], 1 + ((j + 12 - 18) & 0x0F)) +
                (uint)j * 0x05555555;
            return
                q[j - 0x10] + r1(q[j - 0x0F]) + q[j - 0x0E] + r2(q[j - 0x0D]) +
                q[j - 0x0C] + r3(q[j - 0x0B]) + q[j - 0x0A] + r4(q[j - 0x09]) +
                q[j - 0x08] + r5(q[j - 0x07]) + q[j - 0x06] + r6(q[j - 0x05]) +
                q[j - 0x04] + r7(q[j - 0x03]) + s4(q[j - 0x02]) + s5(q[j - 0x01]) +
                (h[(j - 0x09) & 0x0F] ^ mp);
        }

        private static uint s0(uint x) => (x >> 1) ^ (x << 3) ^ BitHelper.RotateLeft(x, 4) ^ BitHelper.RotateLeft(x, 19);
        private static uint s1(uint x) => (x >> 1) ^ (x << 2) ^ BitHelper.RotateLeft(x, 8) ^ BitHelper.RotateLeft(x, 23);
        private static uint s2(uint x) => (x >> 2) ^ (x << 1) ^ BitHelper.RotateLeft(x, 12) ^ BitHelper.RotateLeft(x, 25);
        private static uint s3(uint x) => (x >> 2) ^ (x << 2) ^ BitHelper.RotateLeft(x, 15) ^ BitHelper.RotateLeft(x, 29);
        private static uint s4(uint x) => (x >> 1) ^ x;
        private static uint s5(uint x) => (x >> 2) ^ x;
        private static uint r1(uint x) => BitHelper.RotateLeft(x, 3);
        private static uint r2(uint x) => BitHelper.RotateLeft(x, 7);
        private static uint r3(uint x) => BitHelper.RotateLeft(x, 13);
        private static uint r4(uint x) => BitHelper.RotateLeft(x, 16);
        private static uint r5(uint x) => BitHelper.RotateLeft(x, 19);
        private static uint r6(uint x) => BitHelper.RotateLeft(x, 23);
        private static uint r7(uint x) => BitHelper.RotateLeft(x, 27);
    }
}