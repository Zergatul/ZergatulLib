using System;

namespace Zergatul.Security.OpenSsl
{
    class MD4 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_md4();
    }
}