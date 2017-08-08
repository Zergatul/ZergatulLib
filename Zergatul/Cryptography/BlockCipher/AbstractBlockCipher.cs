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
            return mode.CreateEncryptor(this, CreateEncryptor(key));
        }

        public Decryptor CreateDecryptor(byte[] key, AbstractBlockCipherMode mode)
        {
            return mode.CreateDecryptor(this, CreateDecryptor(key));
        }

        public Encryptor CreateEncryptor(byte[] key, BlockCipherMode mode) => CreateEncryptor(key, AbstractBlockCipherMode.Resolve(mode));
        public Decryptor CreateDecryptor(byte[] key, BlockCipherMode mode) => CreateDecryptor(key, AbstractBlockCipherMode.Resolve(mode));
    }
}