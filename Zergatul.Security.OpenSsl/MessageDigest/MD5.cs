using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class MD5 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_md5();
    }
}