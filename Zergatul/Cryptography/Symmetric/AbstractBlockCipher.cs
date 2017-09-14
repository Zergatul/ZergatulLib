using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Symmetric.CipherMode;

namespace Zergatul.Cryptography.Symmetric
{
    public abstract class AbstractBlockCipher
    {
        public abstract int BlockSize { get; }
        public abstract int KeySize { get; }

        public abstract Func<byte[], byte[]> CreateEncryptor(byte[] key);
        public abstract Func<byte[], byte[]> CreateDecryptor(byte[] key);
    }
}