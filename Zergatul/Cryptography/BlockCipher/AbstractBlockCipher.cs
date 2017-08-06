using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public abstract class AbstractBlockCipher
    {
        public abstract int BlockSize { get; }
        public abstract int KeySize { get; }

        public abstract Func<byte[], byte[]> CreateEncryptor(byte[] key);
        public abstract Func<byte[], byte[]> CreateDecryptor(byte[] key);

        public Encryptor CreateEncryptor(byte[] key, AbstractBlockCipherMode mode)
        {
            var enc = mode.CreateEncryptor();
            enc.Cipher = this;
            enc.ProcessBlock = CreateEncryptor(key);
            return enc;
        }

        public Decryptor CreateDecryptor(byte[] key, AbstractBlockCipherMode mode)
        {
            var dec = mode.CreateDecryptor();
            dec.Cipher = this;
            dec.ProcessBlock = CreateDecryptor(key);
            return dec;
        }

        public Encryptor CreateEncryptor(byte[] key, BlockCipherMode mode) => CreateEncryptor(key, AbstractBlockCipherMode.Resolve(mode));
        public Decryptor CreateDecryptor(byte[] key, BlockCipherMode mode) => CreateDecryptor(key, AbstractBlockCipherMode.Resolve(mode));
    }
}