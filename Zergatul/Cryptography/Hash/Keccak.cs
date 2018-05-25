using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
    public abstract class Keccak : AbstractHash
    {
        public override int BlockSize => _blockSizeByte;
        public override int HashSize => _hashSizeByte;

        private static ulong[] RC = new ulong[]
        {
            0x0000000000000001,
            0x0000000000008082,
            0x800000000000808A,
            0x8000000080008000,
            0x000000000000808B,
            0x0000000080000001,
            0x8000000080008081,
            0x8000000000008009,
            0x000000000000008A,
            0x0000000000000088,
            0x0000000080008009,
            0x000000008000000A,
            0x000000008000808B,
            0x800000000000008B,
            0x8000000000008089,
            0x8000000000008003,
            0x8000000000008002,
            0x8000000000000080,
            0x000000000000800A,
            0x800000008000000A,
            0x8000000080008081,
            0x8000000000008080,
            0x0000000080000001,
            0x8000000080008008
        };

        private ulong[] S;
        private ulong[] B;
        private int _bits;
        private int _blockSizeULong;
        private int _blockSizeByte;
        private int _hashSizeByte;
        private byte _padding;

        public Keccak(int bits, byte padding, int hashBits)
            : base(true)
        {
            this._bits = bits;
            this._padding = padding;

            _blockSizeULong = (1600 - _bits) >> 6;
            _blockSizeByte = _blockSizeULong * 8;
            _hashSizeByte = hashBits >> 3;

            _block = new byte[_blockSizeByte];

            B = new ulong[25];
            S = new ulong[25];
        }

        protected override void Init()
        {
            for (int i = 0; i < S.Length; i++)
                S[i] = 0;
        }

        protected override void ProcessBlock()
        {
            BitHelper.ToUInt64Array(_block, ByteOrder.LittleEndian, B);
            for (int i = 0; i < _blockSizeULong; i++)
                S[i] ^= B[i];

            f(S);
        }

        protected override void AddPadding()
        {
            _buffer.Add(_padding);

            while (_buffer.Count < _blockSizeByte)
                _buffer.Add(0);

            _buffer[_blockSizeByte - 1] |= 0x80;
        }

        protected override byte[] InternalStateToBytes()
        {
            byte[] digest = new byte[HashSize];

            int di = 0;
            int si = 0;
            byte[] buffer = new byte[8];
            while (di < HashSize)
            {
                if (si == _blockSizeULong)
                {
                    f(S);
                    si = 0;
                }

                BitHelper.GetBytes(S[si++], ByteOrder.LittleEndian, buffer, 0);
                for (int i = 0; i < 8 && di < HashSize; i++)
                    digest[di++] = buffer[i];
            }

            return digest;
        }

        private void f(ulong[] A)
        {
            for (int i = 0; i < 24; i++)
            {
                // θ step

                ulong C0 = A[0] ^ A[5] ^ A[10] ^ A[15] ^ A[20];
                ulong C1 = A[1] ^ A[6] ^ A[11] ^ A[16] ^ A[21];
                ulong C2 = A[2] ^ A[7] ^ A[12] ^ A[17] ^ A[22];
                ulong C3 = A[3] ^ A[8] ^ A[13] ^ A[18] ^ A[23];
                ulong C4 = A[4] ^ A[9] ^ A[14] ^ A[19] ^ A[24];

                ulong D;
                D = C4 ^ BitHelper.RotateLeft(C1, 1);
                A[0] ^= D; A[5] ^= D; A[10] ^= D; A[15] ^= D; A[20] ^= D;
                D = C0 ^ BitHelper.RotateLeft(C2, 1);
                A[1] ^= D; A[6] ^= D; A[11] ^= D; A[16] ^= D; A[21] ^= D;
                D = C1 ^ BitHelper.RotateLeft(C3, 1);
                A[2] ^= D; A[7] ^= D; A[12] ^= D; A[17] ^= D; A[22] ^= D;
                D = C2 ^ BitHelper.RotateLeft(C4, 1);
                A[3] ^= D; A[8] ^= D; A[13] ^= D; A[18] ^= D; A[23] ^= D;
                D = C3 ^ BitHelper.RotateLeft(C0, 1);
                A[4] ^= D; A[9] ^= D; A[14] ^= D; A[19] ^= D; A[24] ^= D;

                // ρ and π steps

                B[00] = A[0];
                B[01] = BitHelper.RotateLeft(A[06], 44);
                B[02] = BitHelper.RotateLeft(A[12], 43);
                B[03] = BitHelper.RotateLeft(A[18], 21);
                B[04] = BitHelper.RotateLeft(A[24], 14);
                B[05] = BitHelper.RotateLeft(A[03], 28);
                B[06] = BitHelper.RotateLeft(A[09], 20);
                B[07] = BitHelper.RotateLeft(A[10], 03);
                B[08] = BitHelper.RotateLeft(A[16], 45);
                B[09] = BitHelper.RotateLeft(A[22], 61);
                B[10] = BitHelper.RotateLeft(A[01], 01);
                B[11] = BitHelper.RotateLeft(A[07], 06);
                B[12] = BitHelper.RotateLeft(A[13], 25);
                B[13] = BitHelper.RotateLeft(A[19], 08);
                B[14] = BitHelper.RotateLeft(A[20], 18);
                B[15] = BitHelper.RotateLeft(A[04], 27);
                B[16] = BitHelper.RotateLeft(A[05], 36);
                B[17] = BitHelper.RotateLeft(A[11], 10);
                B[18] = BitHelper.RotateLeft(A[17], 15);
                B[19] = BitHelper.RotateLeft(A[23], 56);
                B[20] = BitHelper.RotateLeft(A[02], 62);
                B[21] = BitHelper.RotateLeft(A[08], 55);
                B[22] = BitHelper.RotateLeft(A[14], 39);
                B[23] = BitHelper.RotateLeft(A[15], 41);
                B[24] = BitHelper.RotateLeft(A[21], 02);

                // χ step

                A[00] = B[00] ^ (~B[01] & B[02]);
                A[01] = B[01] ^ (~B[02] & B[03]);
                A[02] = B[02] ^ (~B[03] & B[04]);
                A[03] = B[03] ^ (~B[04] & B[00]);
                A[04] = B[04] ^ (~B[00] & B[01]);
                A[05] = B[05] ^ (~B[06] & B[07]);
                A[06] = B[06] ^ (~B[07] & B[08]);
                A[07] = B[07] ^ (~B[08] & B[09]);
                A[08] = B[08] ^ (~B[09] & B[05]);
                A[09] = B[09] ^ (~B[05] & B[06]);
                A[10] = B[10] ^ (~B[11] & B[12]);
                A[11] = B[11] ^ (~B[12] & B[13]);
                A[12] = B[12] ^ (~B[13] & B[14]);
                A[13] = B[13] ^ (~B[14] & B[10]);
                A[14] = B[14] ^ (~B[10] & B[11]);
                A[15] = B[15] ^ (~B[16] & B[17]);
                A[16] = B[16] ^ (~B[17] & B[18]);
                A[17] = B[17] ^ (~B[18] & B[19]);
                A[18] = B[18] ^ (~B[19] & B[15]);
                A[19] = B[19] ^ (~B[15] & B[16]);
                A[20] = B[20] ^ (~B[21] & B[22]);
                A[21] = B[21] ^ (~B[22] & B[23]);
                A[22] = B[22] ^ (~B[23] & B[24]);
                A[23] = B[23] ^ (~B[24] & B[20]);
                A[24] = B[24] ^ (~B[20] & B[21]);

                // ι step

                A[0] ^= RC[i];
            }
        }
    }

    public class Keccak256 : Keccak
    {
        public override OID OID
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Keccak256()
            : base(512, 1, 256)
        {

        }
    }
}