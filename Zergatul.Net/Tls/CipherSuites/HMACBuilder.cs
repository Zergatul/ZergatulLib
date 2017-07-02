using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal abstract class HMACBuilder
    {
        public abstract AbstractHMAC Create(ByteArray secretKey);
    }

    internal class HMACSHA1Builder : HMACBuilder
    {
        public override AbstractHMAC Create(ByteArray secretKey)
        {
            return new HMACSHA1(secretKey);
        }
    }

    internal class HMACSHA256Builder : HMACBuilder
    {
        public override AbstractHMAC Create(ByteArray secretKey)
        {
            return new HMACSHA256(secretKey);
        }
    }
}