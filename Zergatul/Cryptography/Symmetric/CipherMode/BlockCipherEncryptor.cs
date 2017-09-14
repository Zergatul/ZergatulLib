using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric.CipherMode
{
    public abstract class BlockCipherEncryptor
    {
        protected AbstractBlockCipher _cipher;
        protected Func<byte[], byte[]> _processBlock;

        protected BlockCipherEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
        {
            this._cipher = cipher;
            this._processBlock = processBlock;
        }

        public abstract byte[] Encrypt(byte[] IV, byte[] data);
    }
}