using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    public enum ECPointFormat : byte
    {
        Uncompressed = 0,
        ANSIX962CompressedPrime = 1,
        ANSIX962CompressedChar2 = 2
    }
}