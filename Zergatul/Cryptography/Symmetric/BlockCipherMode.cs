using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric
{
    public enum BlockCipherMode
    {
        ECB,
        CBC,

        GCM,
        CCM
    }
}