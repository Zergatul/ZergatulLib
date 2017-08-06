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

        public override Func<byte[], byte[]> CreateEncryptor(byte[] key)
        {
            throw new NotImplementedException();
        }

        public override Func<byte[], byte[]> CreateDecryptor(byte[] key)
        {
            throw new NotImplementedException();
        }
    }
}
