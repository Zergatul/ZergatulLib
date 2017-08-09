using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.BlockCipher.CipherMode;

namespace Zergatul.Cryptography.BlockCipher
{
    public abstract class AbstractBlockCipherMode
    {
        protected abstract BlockCipherEncryptor CreateEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock);
        protected abstract BlockCipherDecryptor CreateDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock);

        public BlockCipherEncryptor CreateEncryptor(AbstractBlockCipher cipher, byte[] key)
        {
            var processBlock = cipher.CreateEncryptor(key);
            return CreateEncryptor(cipher, processBlock);
        }

        public BlockCipherDecryptor CreateDecryptor(AbstractBlockCipher cipher, byte[] key)
        {
            var processBlock = cipher.CreateDecryptor(key);
            return CreateDecryptor(cipher, processBlock);
        }
    }
}