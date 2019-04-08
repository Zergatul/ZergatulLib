using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class SHA384 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_sha384();
    }
}