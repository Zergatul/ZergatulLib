using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal enum KeyExchangeAlgorithm
    {
        DH_anon,
        DHE_DSS,
        DHE_RSA,
        RSA,
        DH_DSS,
        DH_RSA,

        ECDH_ECDSA,
        ECDHE_ECDSA,
        ECDH_RSA,
        ECDHE_RSA,
        ECDH_anon
    }
}