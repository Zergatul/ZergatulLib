using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal class HMACSHA1 : AbstractHMAC
    {
        protected override ByteArray Hash(ByteArray data)
        {
            var sha1 = new SHA1Managed();
            return new ByteArray(sha1.ComputeHash(data.ToArray()));
        }
    }
}
