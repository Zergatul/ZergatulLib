using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Symmetric;

namespace Zergatul.Network.Tls
{
    internal class NullStreamCipher : AbstractStreamCipher
    {
        public override int BlockSize => 1;
        public override int KeySize => 0;
        public override int NonceSize => 0;

        public override KeyStream InitKeyStream(byte[] key, byte[] nonce, uint counter)
        {
            return new KeyStream(() => new byte[1]);
        }
    }
}