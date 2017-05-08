using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.Extensions
{
    internal enum HashAlgorithm : byte
    {
        None = 0,
        MD5 = 1,
        SHA1 = 2,
        SHA224 = 3,
        SHA256 = 4,
        SHA384 = 5,
        SHA512 = 6
    }
}