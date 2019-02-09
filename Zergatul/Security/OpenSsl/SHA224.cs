using System;

namespace Zergatul.Security.OpenSsl
{
    class SHA224 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_sha224();
    }
}