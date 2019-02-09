using System;

namespace Zergatul.Security.OpenSsl
{
    class RIPEMD160 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_ripemd160();
    }
}