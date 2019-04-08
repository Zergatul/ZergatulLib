using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class SHA1 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_sha1();
    }
}