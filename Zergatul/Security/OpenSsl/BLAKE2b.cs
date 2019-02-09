using System;

namespace Zergatul.Security.OpenSsl
{
    class BLAKE2b : AbstractMessageDigest
    {
        protected override IntPtr CreateMD() => OpenSsl.EVP_blake2b512();
    }
}