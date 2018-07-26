using System;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
    // https://tools.ietf.org/html/rfc7693
    public class BLAKE2s : AbstractHash
    {
        public override int BlockSize => 64;
        public override int HashSize => _hashSizeBytes;
        public override OID OID => null;

        private static readonly uint[] IV = new uint[]
        {
            0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A,
            0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19
        };

        private static int[] Sigma = new[]
        {
             0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15,
            14, 10,  4,  8,  9, 15, 13,  6,  1, 12,  0,  2, 11,  7,  5,  3,
            11,  8, 12,  0,  5,  2, 15, 13, 10, 14,  3,  6,  7,  1,  9,  4,
             7,  9,  3,  1, 13, 12, 11, 14,  2,  6,  5, 10,  4,  0, 15,  8,
             9,  0,  5,  7,  2,  4, 10, 15, 14,  1, 11, 12,  6,  8,  3, 13,
             2, 12,  6, 10,  0, 11,  8,  3,  4, 13,  7,  5, 15, 14,  1,  9,
            12,  5,  1, 15, 14, 13,  4, 10,  0,  7,  6,  3,  9,  2,  8, 11,
            13, 11,  7, 14, 12,  1,  3,  9,  5,  0, 15,  4,  8,  6,  2, 10,
             6, 15, 14,  9, 11,  3,  0,  8, 12,  2, 13,  7,  1,  4, 10,  5,
            10,  2,  8,  4,  7,  6,  1,  5, 15, 11,  9, 14,  3, 12, 13,  0,
        };

        private int _hashSizeBytes;
        private byte[] _key;
        private uint[] h;
        private uint counter;
        private uint[] m;

        public BLAKE2s(int hashSizeBytes = 32, byte[] key = null)
            : base(true)
        {
            if (hashSizeBytes <= 0 || hashSizeBytes > 32)
                throw new ArgumentOutOfRangeException(nameof(hashSizeBytes));

            this._hashSizeBytes = hashSizeBytes;
            this._key = key;

            this._block = new byte[BlockSize];
            this.m = new uint[16];

            Init();
        }

        protected override void Init()
        {
            counter = 0;
            h = (uint[])IV.Clone();
            h[0] = h[0] ^ 0x01010000 ^ ((uint)(_key?.Length ?? 0) << 8) ^ (uint)_hashSizeBytes;

            if (_key?.Length > 0)
            {
                _buffer.AddRange(_key);
                int padding = BlockSize - (_key.Length % BlockSize);
                for (int i = 0; i < padding; i++)
                    _buffer.Add(0);
            }
        }

        protected override void ProcessBlock()
        {
            counter += 64;
            BitHelper.ToUInt32Array(_block, ByteOrder.LittleEndian, m);
            F(h, m, counter, false);
        }

        protected override void ProcessBuffer()
        {
            while (_buffer.Count > BlockSize)
            {
                _buffer.CopyTo(0, _block, 0, BlockSize);
                ProcessBlock();
                _buffer.RemoveRange(0, BlockSize);
            }
        }

        protected override void AddPadding()
        {
            // pad only in key+data=0, or when buffer is not filled
            ulong length = (ulong)(_key?.Length ?? 0) + _totalBytes;
            if (length == 0 || _buffer.Count != BlockSize)
            {
                int padding = BlockSize - _buffer.Count % BlockSize;
                for (int i = 0; i < padding; i++)
                    _buffer.Add(0);
            }

            ProcessBuffer();
            ProcessFinalBlock();
        }

        private void ProcessFinalBlock()
        {
            if (_buffer.Count != BlockSize)
                throw new InvalidOperationException();

            _buffer.CopyTo(0, _block, 0, BlockSize);

            counter = (uint)_totalBytes;
            if (_key?.Length > 0)
                counter += 64;
            BitHelper.ToUInt32Array(_block, ByteOrder.LittleEndian, m);
            F(h, m, counter, true);

            _buffer.RemoveRange(0, BlockSize);
        }

        protected override byte[] InternalStateToBytes()
        {
            byte[] hash = BitHelper.ToByteArray(h, ByteOrder.LittleEndian);
            if (HashSize == 64)
                return hash;
            else
                return ByteArray.SubArray(hash, 0, HashSize);
        }

        private void G(uint[] v, int a, int b, int c, int d, uint x, uint y)
        {
            v[a] = v[a] + v[b] + x;
            v[d] = BitHelper.RotateRight(v[d] ^ v[a], 16);
            v[c] = v[c] + v[d];
            v[b] = BitHelper.RotateRight(v[b] ^ v[c], 12);
            v[a] = v[a] + v[b] + y;
            v[d] = BitHelper.RotateRight(v[d] ^ v[a], 8);
            v[c] = v[c] + v[d];
            v[b] = BitHelper.RotateRight(v[b] ^ v[c], 7);
        }

        private void F(uint[] h, uint[] m, uint t, bool f)
        {
            uint[] v = new uint[16];
            Array.Copy(h, 0, v, 0, 8);
            Array.Copy(IV, 0, v, 8, 8);

            v[12] ^= t;
            v[13] ^= 0;

            if (f)
                v[14] = ~v[14];

            for (int i = 0; i < 10; i++)
            {
                int index = i * 16;

                G(v, 0, 4,  8, 12, m[Sigma[index +  0]], m[Sigma[index +  1]]);
                G(v, 1, 5,  9, 13, m[Sigma[index +  2]], m[Sigma[index +  3]]);
                G(v, 2, 6, 10, 14, m[Sigma[index +  4]], m[Sigma[index +  5]]);
                G(v, 3, 7, 11, 15, m[Sigma[index +  6]], m[Sigma[index +  7]]);

                G(v, 0, 5, 10, 15, m[Sigma[index +  8]], m[Sigma[index +  9]]);
                G(v, 1, 6, 11, 12, m[Sigma[index + 10]], m[Sigma[index + 11]]);
                G(v, 2, 7,  8, 13, m[Sigma[index + 12]], m[Sigma[index + 13]]);
                G(v, 3, 4,  9, 14, m[Sigma[index + 14]], m[Sigma[index + 15]]);
            }

            for (int i = 0; i < 8; i++)
                h[i] = h[i] ^ v[i] ^ v[i + 8];
        }
    }
}