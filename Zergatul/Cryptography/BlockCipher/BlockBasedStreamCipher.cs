using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public class BlockBasedStreamCipher<BlockCipher> : AbstractStreamCipher
        where BlockCipher : AbstractBlockCipher, new()
    {
        public override int BlockSize => _blockCipher.BlockSize;
        public override int KeySize => _blockCipher.KeySize;
        public override int NonceSize => 12;

        private BlockCipher _blockCipher = new BlockCipher();

        public override KeyStream InitKeyStream(byte[] key, byte[] nonce, uint counter)
        {
            throw new NotImplementedException();
        }
    }
}