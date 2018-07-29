using System;

namespace Zergatul.Cryptography.Hash.Base
{
    public class Skein_512BitState
    {
        private ulong[] h = new ulong[9];
        private ulong[] p = new ulong[8];
        private ulong[] t = new ulong[3];
        private ulong[] m = new ulong[8];
        private ulong totalBytesHi, totalBytesLo;
        private bool first, final;
        private SkeinParameters parameters;
        private byte[] block = new byte[64];
        private bool blockFilled;
        private int blockIndex;
        private int bitIndex;

        public Skein_512BitState(SkeinParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            this.parameters = parameters;
            Reset();
        }

        public void Reset()
        {
            Array.Copy(parameters.IV, 0, h, 0, 8);

            totalBytesHi = 0;
            totalBytesLo = 0;
            first = true;
            final = false;
            blockFilled = false;

            blockIndex = 0;
            bitIndex = 0;
        }

        public void Update(byte[] data, int index, int length)
        {
            if (bitIndex == 0)
            {
                while (length > 0)
                {
                    if (blockIndex + length >= 64)
                    {
                        if (blockFilled && blockIndex == 0)
                            ProcessBlock();

                        int copy = 64 - blockIndex;
                        Array.Copy(data, index, block, blockIndex, copy);
                        IncTotalBytes((ulong)copy);
                        index += copy;
                        length -= copy;
                        blockIndex = 0;
                        blockFilled = true;
                    }
                    else
                    {
                        if (blockFilled && blockIndex == 0)
                            ProcessBlock();

                        Array.Copy(data, index, block, blockIndex, length);
                        IncTotalBytes((ulong)length);
                        index += length;
                        blockIndex += length;
                        length = 0;
                    }
                }
            }
            else
                throw new NotImplementedException();
        }

        public byte[] GetResult()
        {
            if (blockIndex == 0 && !blockFilled) // zero bytes input
            {
                for (int i = 0; i < 64; i++)
                    block[i] = 0;
                blockIndex = 64;
            }
            if (0 < blockIndex && blockIndex < 64) // padding
            {
                for (; blockIndex < 64; blockIndex++)
                    block[blockIndex] = 0;
            }

            final = true;
            ProcessBlock();

            for (int i = 0; i < 8; i++)
                m[i] = 0;
            BuildTweak(true, true, SkeinTweak.Output, 0, 8);
            ProcessBlockInternal();

            byte[] hash = new byte[64];
            BitHelper.GetBytes(h[0], ByteOrder.LittleEndian, hash, 0x00);
            BitHelper.GetBytes(h[1], ByteOrder.LittleEndian, hash, 0x08);
            BitHelper.GetBytes(h[2], ByteOrder.LittleEndian, hash, 0x10);
            BitHelper.GetBytes(h[3], ByteOrder.LittleEndian, hash, 0x18);
            BitHelper.GetBytes(h[4], ByteOrder.LittleEndian, hash, 0x20);
            BitHelper.GetBytes(h[5], ByteOrder.LittleEndian, hash, 0x28);
            BitHelper.GetBytes(h[6], ByteOrder.LittleEndian, hash, 0x30);
            BitHelper.GetBytes(h[7], ByteOrder.LittleEndian, hash, 0x38);
            return hash;
        }

        private void ProcessBlock()
        {
            BitHelper.ToUInt64Array(block, ByteOrder.LittleEndian, m);

            BuildTweak(first, final, SkeinTweak.Message, totalBytesHi, totalBytesLo);
            first = false;

            ProcessBlockInternal();
        }

        private void ProcessBlockInternal()
        {
            Array.Copy(m, p, 8);

            t[2] = t[0] ^ t[1];
            h[8] = h[0] ^ h[1] ^ h[2] ^ h[3] ^ h[4] ^ h[5] ^ h[6] ^ h[7] ^ 0x1BD11BDAA9FC1A22;

            Treefish4e(0);
            Treefish4o(1);
            Treefish4e(2);
            Treefish4o(3);
            Treefish4e(4);
            Treefish4o(5);
            Treefish4e(6);
            Treefish4o(7);
            Treefish4e(8);
            Treefish4o(9);
            Treefish4e(10);
            Treefish4o(11);
            Treefish4e(12);
            Treefish4o(13);
            Treefish4e(14);
            Treefish4o(15);
            Treefish4e(16);
            Treefish4o(17);
            AddKey(18);

            h[0] = m[0] ^ p[0];
            h[1] = m[1] ^ p[1];
            h[2] = m[2] ^ p[2];
            h[3] = m[3] ^ p[3];
            h[4] = m[4] ^ p[4];
            h[5] = m[5] ^ p[5];
            h[6] = m[6] ^ p[6];
            h[7] = m[7] ^ p[7];
        }

        private void IncTotalBytes(ulong value)
        {
            if (totalBytesLo + value < totalBytesLo)
                totalBytesHi++;
            totalBytesLo = totalBytesLo + value;
        }

        private void BuildTweak(bool first, bool final, ulong type, ulong positionHi, ulong positionLo)
        {
            t[0] = positionLo;
            t[1] =
                (final ? 0x8000000000000000UL : 0) |
                (first ? 0x4000000000000000UL : 0) |
                ((type & 0x3F) << 56) |
                (positionHi & 0xFFFFFFFF);
        }

        private void Treefish4e(int index)
        {
            AddKey(index);
            Mix8(0, 1, 2, 3, 4, 5, 6, 7, 46, 36, 19, 37);
            Mix8(2, 1, 4, 7, 6, 5, 0, 3, 33, 27, 14, 42);
            Mix8(4, 1, 6, 3, 0, 5, 2, 7, 17, 49, 36, 39);
            Mix8(6, 1, 0, 7, 2, 5, 4, 3, 44,  9, 54, 56);
        }

        private void Treefish4o(int index)
        {
            AddKey(index);
            Mix8(0, 1, 2, 3, 4, 5, 6, 7, 39, 30, 34, 24);
            Mix8(2, 1, 4, 7, 6, 5, 0, 3, 13, 50, 10, 17);
            Mix8(4, 1, 6, 3, 0, 5, 2, 7, 25, 29, 39, 43);
            Mix8(6, 1, 0, 7, 2, 5, 4, 3,  8, 35, 56, 22);
        }

        private void AddKey(int index)
        {
            p[0] += h[(0 + index) % 9];
            p[1] += h[(1 + index) % 9];
            p[2] += h[(2 + index) % 9];
            p[3] += h[(3 + index) % 9];
            p[4] += h[(4 + index) % 9];
            p[5] += h[(5 + index) % 9] + t[(0 + index) % 3];
            p[6] += h[(6 + index) % 9] + t[(1 + index) % 3];
            p[7] += h[(7 + index) % 9] + (ulong)index;
        }

        private void Mix8(int i0, int i1, int i2, int i3, int i4, int i5, int i6, int i7, int r0, int r1, int r2, int r3)
        {
            Mix(i0, i1, r0);
            Mix(i2, i3, r1);
            Mix(i4, i5, r2);
            Mix(i6, i7, r3);
        }

        private void Mix(int i0, int i1, int r)
        {
            p[i0] += p[i1];
            p[i1] = BitHelper.RotateLeft(p[i1], r) ^ p[i0];
        }
    }
}