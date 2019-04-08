using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class BLAKE2b : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_blake2b512();
    }
}