using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public abstract class AbstractStreamCipher
    {
        public abstract int KeySize { get; }
        public abstract int BlockSize { get; }
        public abstract int NonceSize { get; }

        public abstract KeyStream InitKeyStream(byte[] key, byte[] nonce, uint counter);
    }
}