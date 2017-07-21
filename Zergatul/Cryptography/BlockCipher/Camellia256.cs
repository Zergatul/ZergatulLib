using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public class Camellia256 : Camellia
    {
        public Camellia256()
            : base(256, 24)
        {

        }
    }
}