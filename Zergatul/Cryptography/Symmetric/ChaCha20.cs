using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    // https://tools.ietf.org/html/rfc7539
    public class ChaCha20 : AbstractStreamCipher
    {
        public override int BlockSize => 64;
        public override int KeySize => 32;
        public override int NonceSize => 12;

        private static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d)
        {
            a += b; d ^= a; d = BitHelper.RotateLeft(d, 16);
            c += d; b ^= c; b = BitHelper.RotateLeft(b, 12);
            a += b; d ^= a; d = BitHelper.RotateLeft(d, 8);
            c += d; b ^= c; b = BitHelper.RotateLeft(b, 7);
        }

        private static void QuarterRound(uint[] state, int i1, int i2, int i3, int i4)
        {
            QuarterRound(ref state[i1], ref state[i2], ref state[i3], ref state[i4]);
        }

        private static uint[] InitState(byte[] key, uint counter, byte[] nonce)
        {
            uint[] state = new uint[16];
            state[0] = 0x61707865;
            state[1] = 0x3320646e;
            state[2] = 0x79622d32;
            state[3] = 0x6b206574;
            for (int i = 0; i < 8; i++)
                state[4 + i] = BitHelper.ToUInt32(key, i * 4, ByteOrder.LittleEndian);
            state[12] = counter;
            for (int i = 0; i < 3; i++)
                state[13 + i] = BitHelper.ToUInt32(nonce, i * 4, ByteOrder.LittleEndian);
            return state;
        }

        public override KeyStream InitKeyStream(byte[] key, byte[] nonce, uint counter)
        {
            return new KeyStream(() =>
            {
                uint[] state = InitState(key, counter, nonce);
                uint[] wstate = (uint[])state.Clone();
                counter++;

                for (int round = 0; round < 10; round++)
                {
                    // column rounds
                    QuarterRound(state, 0, 4, 8, 12);
                    QuarterRound(state, 1, 5, 9, 13);
                    QuarterRound(state, 2, 6, 10, 14);
                    QuarterRound(state, 3, 7, 11, 15);

                    // diagonal rounds
                    QuarterRound(state, 0, 5, 10, 15);
                    QuarterRound(state, 1, 6, 11, 12);
                    QuarterRound(state, 2, 7, 8, 13);
                    QuarterRound(state, 3, 4, 9, 14);
                }

                for (int i = 0; i < 16; i++)
                    state[i] += wstate[i];

                byte[] result = new byte[64];
                for (int i = 0; i < 16; i++)
                    BitHelper.GetBytes(state[i], ByteOrder.LittleEndian, result, i * 4);

                return result;
            });
        }
    }
}
