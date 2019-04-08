using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class SHA224 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_sha224();
    }
}