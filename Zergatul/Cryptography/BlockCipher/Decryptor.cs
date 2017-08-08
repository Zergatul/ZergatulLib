using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public abstract class Decryptor
    {
        protected AbstractBlockCipher _cipher;
        protected Func<byte[], byte[]> _processBlock;

        public Decryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
        {
            this._cipher = cipher;
            this._processBlock = processBlock;
        }

        public abstract byte[] Decrypt(byte[] data);
        public abstract byte[] Decrypt(byte[] IV, byte[] data);
        public abstract byte[] Decrypt(byte[] IV, byte[] data, byte[] authenticatedData);
    }
}