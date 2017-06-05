using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal class ClientKeyExchangeResult
    {
        public ClientKeyExchange Message;
        public ByteArray PreMasterSecret;
    }
}