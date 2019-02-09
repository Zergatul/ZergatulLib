using System;

namespace Zergatul.Security.OpenSsl
{
    class BLAKE2s : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_blake2s256();
    }
}