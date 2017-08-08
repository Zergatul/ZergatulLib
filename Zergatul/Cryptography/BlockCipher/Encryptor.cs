using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public abstract class Encryptor
    {
        protected AbstractBlockCipher _cipher;
        protected Func<byte[], byte[]> _processBlock;

        public Encryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
        {
            this._cipher = cipher;
            this._processBlock = processBlock;
        }

        public abstract byte[] Encrypt(byte[] data);
        public abstract byte[] Encrypt(byte[] IV, byte[] data);
        public abstract AEADCipherData Encrypt(byte[] IV, byte[] data, byte[] authenticatedData);
    }
}