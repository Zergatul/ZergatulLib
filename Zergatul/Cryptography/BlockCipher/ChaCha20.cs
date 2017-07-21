using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public class ChaCha20 : AbstractBlockCipher
    {
        public override int BlockSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int KeySize => 16;

        public override Decryptor CreateDecryptor(byte[] key, BlockCipherMode mode)
        {
            throw new NotImplementedException();
        }

        public override Encryptor CreateEncryptor(byte[] key, BlockCipherMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
