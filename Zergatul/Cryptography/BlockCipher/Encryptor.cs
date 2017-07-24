using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public abstract class Encryptor
    {
        internal AbstractBlockCipher Cipher;
        internal Func<byte[], byte[]> ProcessBlock;
        public abstract byte[] Encrypt(byte[] IV, byte[] data);

        public byte[] Encrypt(byte[] data) => Encrypt(null, data);
    }
}