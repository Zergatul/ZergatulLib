using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public class Camellia128 : Camellia
    {
        public Camellia128()
            : base(128, 18)
        {

        }
    }
}