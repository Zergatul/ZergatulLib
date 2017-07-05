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

        protected static Encryptor ResolveEncryptor(BlockCipherMode mode)
        {
            switch (mode)
            {
                case BlockCipherMode.ECB: return new ECBEncryptor();
                case BlockCipherMode.CBC: return new CBCEncryptor();
                default:
                    throw new NotImplementedException();
            }
        }

        protected static Decryptor ResolveDecryptor(BlockCipherMode mode)
        {
            switch (mode)
            {
                case BlockCipherMode.ECB: return new ECBDecryptor();
                case BlockCipherMode.CBC: return new CBCDecryptor();
                default:
                    throw new NotImplementedException();
            }
        }

        public abstract Encryptor CreateEncryptor(byte[] key, BlockCipherMode mode);
        public abstract Decryptor CreateDecryptor(byte[] key, BlockCipherMode mode);
    }
}