using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Symmetric.CipherMode;

namespace Zergatul.Network.Tls
{
    internal class CCMCipher : AEADCipher
    {
        public CCMCipher(AbstractBlockCipher cipher)
            : base(cipher, new CCM { OctetLength = 3, TagLength = 16 }, 16)
        {

        }
    }

    internal class CCM8Cipher : AEADCipher
    {
        public CCM8Cipher(AbstractBlockCipher cipher)
            : base(cipher, new CCM { OctetLength = 3, TagLength = 8 }, 8)
        {

        }
    }
}