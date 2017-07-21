using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    // https://info.isl.ntt.co.jp/crypt/eng/camellia/dl/01espec.pdf
    public abstract class Camellia : AbstractBlockCipher
    {
        public override int BlockSize => 16;
        public override int KeySize => Nk / 8;

        int Nk, Nr;

        public Camellia(int Nk, int Nr)
        {
            this.Nk = Nk;
            this.Nr = Nr;
        }

        /*private Key KeySchedule(byte[] key)
        {
            switch (Nk)
            {
                case 128:
                    return new Key
                    {
                        KLL = BitHelper.ToUInt64()
                    };
                case 192:
                    break;
                case 256:
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }*/

        public override Encryptor CreateEncryptor(byte[] key, BlockCipherMode mode)
        {
            throw new NotImplementedException();
        }

        public override Decryptor CreateDecryptor(byte[] key, BlockCipherMode mode)
        {
            throw new NotImplementedException();
        }

        private struct Key
        {
            public ulong KLL, KLR, KRL, KRR;
        }
    }
}