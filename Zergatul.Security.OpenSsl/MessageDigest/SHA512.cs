using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class SHA512 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_sha512();
    }
}