using Zergatul.Network;

namespace Zergatul.Cryptography.Hash.Base
{
    public class CubeHash : AbstractHash
    {
        public override int BlockSize => _blockSize;
        public override int HashSize => _hashSize;
        public override OID OID => null;

        private int _blockSize;
        private int _hashSize;
        private int _initRounds, _blockRounds, _finRounds;

        protected uint[] state = new uint[32];

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="i">Parameter i in {1,2,3,...}, the number of initialization rounds, typically 16</param>
        /// <param name="r">Parameter r in {1,2,3,...}, the number of rounds per message block, typically 16</param>
        /// <param name="b">Parameter b in {1,2,3,...,128}, the number of bytes per message block, typically 32</param>
        /// <param name="f">Parameter f in {1,2,3,...}, the number of finalization rounds, typically 32</param>
        /// <param name="h">Parameter h in {8,16,24,...,512}, the number of output bits, typically 512</param>
        public CubeHash(int i, int r, int b, int f, int h)
            : base(true)
        {
            _initRounds = i;
            _blockRounds = r;
            _blockSize = b;
            _finRounds = f;
            _hashSize = h >> 3;

            _block = new byte[BlockSize];
            Init();
        }

        protected override void Init()
        {
            state[0] = (uint)_hashSize;
            state[1] = (uint)_blockSize;
            state[2] = (uint)_blockRounds;
            for (int i = 3; i < 32; i++)
                state[i] = 0;

            for (int i = 0; i < _initRounds; i++)
                Round();
        }

        protected override void ProcessBlock()
        {
            // xor input block into state
            for (int i = 0; i < _blockSize; i++)
                state[i >> 2] ^= (uint)_block[i] << ((i & 0x03) << 3);

            for (int i = 0; i < _blockRounds; i++)
                Round();
        }

        protected override void AddPadding()
        {
            _buffer.Add(0x80);
            while (_buffer.Count % _blockSize != 0)
                _buffer.Add(0);
        }

        protected override byte[] InternalStateToBytes()
        {
            state[31] ^= 1;
            for (int i = 0; i < _finRounds; i++)
                Round();

            byte[] bytes = BitHelper.ToByteArray(state, ByteOrder.LittleEndian);
            return ByteArray.SubArray(bytes, 0, _hashSize);
        }

        private void Round()
        {
            // Add x[0jklm] into x[1jklm] modulo 2^32, for each (j,k,l,m)
            for (int i = 0; i < 16; i++)
                state[16 + i] += state[i];

            // Rotate x[0jklm] upwards by 7 bits, for each (j,k,l,m)
            for (int i = 0; i < 16; i++)
                state[i] = BitHelper.RotateLeft(state[i], 7);

            // Swap x[00klm] with x[01klm], for each (k,l,m)
            for (int i = 0; i < 8; i++)
            {
                int j = i + 8;
                uint buf = state[i];
                state[i] = state[j];
                state[j] = buf;
            }

            // Xor x[1jklm] into x[0jklm], for each (j,k,l,m)
            for (int i = 0; i < 16; i++)
                state[i] ^= state[i + 16];

            // Swap x[1jk0m] with x[1jk1m], for each (j,k,m)
            for (int i = 0; i < 8; i++)
            {
                int i1 = 0x10 | ((i & 6) << 1) | (i & 1);
                int i2 = i1 | 2;
                uint buf = state[i1];
                state[i1] = state[i2];
                state[i2] = buf;
            }

            // Add x[0jklm] into x[1jklm] modulo 2^32, for each (j,k,l,m)
            for (int i = 0; i < 16; i++)
                state[i + 16] += state[i];

            // Rotate x[0jklm] upwards by 11 bits, for each (j,k,l,m)
            for (int i = 0; i < 16; i++)
                state[i] = BitHelper.RotateLeft(state[i], 11);

            // Swap x[0j0lm] with x[0j1lm], for each (j,l,m)
            for (int i = 0; i < 8; i++)
            {
                int i1 = ((i & 4) << 1) | (i & 3);
                int i2 = i1 | 4;
                uint buf = state[i1];
                state[i1] = state[i2];
                state[i2] = buf;
            }

            // Xor x[1jklm] into x[0jklm], for each (j,k,l,m)
            for (int i = 0; i < 16; i++)
                state[i] ^= state[i + 16];

            // Swap x[1jkl0] with x[1jkl1], for each (j,k,l)
            for (int i = 0; i < 8; i++)
            {
                int i1 = 0x10 | (i << 1);
                int i2 = i1 | 1;
                uint buf = state[i1];
                state[i1] = state[i2];
                state[i2] = buf;
            }
        }
    }
}