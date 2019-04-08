using System;

namespace Zergatul.Security.OpenSsl.MessageDigest
{
    class BLAKE2s : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => Native.EVP_blake2s256();
    }
}