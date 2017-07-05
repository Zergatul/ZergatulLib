using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public class AES192 : AES
    {
        public AES192()
            : base(4, 6, 12)
        {

        }
    }
}