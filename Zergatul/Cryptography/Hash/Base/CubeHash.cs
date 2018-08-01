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

        private uint[] state = new uint[32];

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
            //state[0] = (uint)_hashSize;
            //state[1] = (uint)_blockSize;
            //state[2] = (uint)_blockRounds;
            //for (int i = 3; i < 32; i++)
            //    state[i] = 0;

            //for (int i = 0; i < _initRounds; i++)
            //    Round();

            state[0x00] = 0x2AEA2A61;
            state[0x01] = 0x50F494D4;
            state[0x02] = 0x2D538B8B;
            state[0x03] = 0x4167D83E;
            state[0x04] = 0x3FEE2313;
            state[0x05] = 0xC701CF8C;
            state[0x06] = 0xCC39968E;
            state[0x07] = 0x50AC5695;
            state[0x08] = 0x4D42C787;
            state[0x09] = 0xA647A8B3;
            state[0x0A] = 0x97CF0BEF;
            state[0x0B] = 0x825B4537;
            state[0x0C] = 0xEEF864D2;
            state[0x0D] = 0xF22090C4;
            state[0x0E] = 0xD0E5CD33;
            state[0x0F] = 0xA23911AE;
            state[0x10] = 0xFCD398D9;
            state[0x11] = 0x148FE485;
            state[0x12] = 0x1B017BEF;
            state[0x13] = 0xB6444532;
            state[0x14] = 0x6A536159;
            state[0x15] = 0x2FF5781C;
            state[0x16] = 0x91FA7934;
            state[0x17] = 0x0DBADEA9;
            state[0x18] = 0xD65C8A2B;
            state[0x19] = 0xA5A70E75;
            state[0x1A] = 0xB1C62456;
            state[0x1B] = 0xBC796576;
            state[0x1C] = 0x1921C8F7;
            state[0x1D] = 0xE7989AF1;
            state[0x1E] = 0x7795D246;
            state[0x1F] = 0xD43E3B44;
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
                int i1 = 0x10 | ((i & 6) << 2) | (i & 1);
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