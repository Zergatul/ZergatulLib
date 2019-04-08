using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class SHA256 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_sha256();
    }
}