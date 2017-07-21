using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public class Camellia192 : Camellia
    {
        public Camellia192()
            : base(192, 24)
        {

        }
    }
}