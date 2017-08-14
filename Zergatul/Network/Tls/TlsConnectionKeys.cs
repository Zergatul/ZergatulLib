using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal class TlsConnectionKeys
    {
        public byte[] ClientMACkey, ServerMACkey, ClientEncKey, ServerEncKey, ClientIV, ServerIV;
    }
}