using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    internal enum SignatureAlgorithm
    {
        Anonymous = 0,
        RSA = 1,
        DSA = 2,
        ECDSA = 3
    }
}
