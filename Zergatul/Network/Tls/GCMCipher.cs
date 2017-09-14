using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Symmetric.CipherMode;

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