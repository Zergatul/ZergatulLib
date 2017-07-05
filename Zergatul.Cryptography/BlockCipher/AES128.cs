using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public class AES128 : AES
    {
        public AES128()
            : base(4, 4, 10)
        {

        }
    }
}