using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.BlockCipher.CipherMode;

namespace Zergatul.Network.Tls
{
    internal class GCMCipher : AEADCipher
    {
        public GCMCipher(AbstractBlockCipher cipher)
            : base(cipher, new GCM(), 16)
        {

        }
    }
}