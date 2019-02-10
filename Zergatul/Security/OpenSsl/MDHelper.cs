using System;

using static Zergatul.Security.OpenSsl.OpenSsl;

namespace Zergatul.Security.OpenSsl
{
    internal static class MDHelper
    {
        public static IntPtr GetMdByName(string algorithm)
        {
            switch (algorithm)
            {
                case MessageDigests.MD4: return EVP_md4();
                case MessageDigests.MD5: return EVP_md5();
                case MessageDigests.SHA1: return EVP_sha1();
                case MessageDigests.SHA224: return EVP_sha224();
                case MessageDigests.SHA256: return EVP_sha256();
                case MessageDigests.SHA384: return EVP_sha384();
                case MessageDigests.SHA512: return EVP_sha512();
                case MessageDigests.RIPEMD160: return EVP_ripemd160();
                case MessageDigests.BLAKE2s: return EVP_blake2s256();
                case MessageDigests.BLAKE2b: return EVP_blake2b512();
                default:
                    throw new NotSupportedException();
            }
        }
    }
}