using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal enum ConnectionState
    {
        NoConnection,
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
        ApplicationData,
        Closed
    }
}