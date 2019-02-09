using System;

namespace Zergatul.Security.OpenSsl
{
    class MD5 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_md5();
    }
}