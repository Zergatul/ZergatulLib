using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class RIPEMD160 : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_ripemd160();
    }
}