using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal enum ConnectionState
    {
        Start,
        ClientHello,
        ServerHello,
        ServerCertificate,
        ServerKeyExchange,
        ServerHelloDone,
        ClientCertificate,
        ClientKeyExchange,
        ClientChangeCipherSpec,
        ClientFinished,
        ServerChangeCipherSpec,
        ServerFinished,
    }
}