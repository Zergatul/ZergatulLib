using System;

namespace Zergatul.Security.OpenSsl
{
    class SHA512 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_sha512();
    }
}