using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public class AES256 : AES
    {
        public AES256()
            : base(4, 8, 14)
        {

        }
    }
}