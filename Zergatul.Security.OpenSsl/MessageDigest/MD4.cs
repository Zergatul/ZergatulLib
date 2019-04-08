using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class MD4 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_md4();
    }
}