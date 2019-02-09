using System;

namespace Zergatul.Security.OpenSsl
{
    class SHA256 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_sha256();
    }
}