using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public class ARIA192 : ARIA
    {
        public ARIA192()
            : base(192, 14)
        {

        }
    }
}