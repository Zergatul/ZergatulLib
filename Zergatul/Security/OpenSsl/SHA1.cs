using System;

namespace Zergatul.Security.OpenSsl
{
    class SHA1 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_sha1();
    }
}