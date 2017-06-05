using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal class HMACSHA256 : AbstractHMAC
    {
        protected override ByteArray Hash(ByteArray data)
        {
            var sha256 = new SHA256Managed();
            return new ByteArray(sha256.ComputeHash(data.ToArray()));
        }
    }
}
