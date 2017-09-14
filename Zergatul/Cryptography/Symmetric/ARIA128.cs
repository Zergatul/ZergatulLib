using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public class ARIA128 : ARIA
    {
        public ARIA128()
            : base(128, 12)
        {

        }
    }
}