using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public class ARIA256 : ARIA
    {
        public ARIA256()
            : base(256, 16)
        {

        }
    }
}