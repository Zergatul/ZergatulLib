using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal abstract class AbstractHMAC
    {
        public ByteArray Compute(ByteArray secret, ByteArray seed)
        {
            // RFC 2104 // Page 2
            return Hash((secret ^ 0x5C) + Hash((secret ^ 0x36) + seed));
        }

        protected abstract ByteArray Hash(ByteArray data);
    }
}
