using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public abstract class Decryptor
    {
        internal AbstractBlockCipher Cipher;
        internal Func<byte[], byte[]> ProcessBlock;
        public abstract byte[] Decrypt(byte[] IV, byte[] data);

        public byte[] Decrypt(byte[] data) => Decrypt(null, data);
    }
}