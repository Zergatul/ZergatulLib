using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric.CipherMode
{
    public abstract class AbstractAEADCipherMode
    {
        public int TagLength { get; set; } = 16;

        protected abstract AEADEncryptor CreateEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock);
        protected abstract AEADDecryptor CreateDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock);

        public AEADEncryptor CreateEncryptor(AbstractBlockCipher cipher, byte[] key)
        {
            var processBlock = cipher.CreateEncryptor(key);
            return CreateEncryptor(cipher, processBlock);
        }

        public AEADDecryptor CreateDecryptor(AbstractBlockCipher cipher, byte[] key)
        {
            var processBlock = cipher.CreateEncryptor(key);
            return CreateDecryptor(cipher, processBlock);
        }
    }
}