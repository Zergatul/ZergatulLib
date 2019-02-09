using System;

namespace Zergatul.Security.OpenSsl
{
    class SHA384 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_sha384();
    }
}