﻿using Zergatul.Security.OpenSsl;

namespace Zergatul.Security
{
    public class OpenSslProvider : Provider
    {
        public override string Name => "OpenSSL";

        public OpenSslProvider()
        {
            RegisterMessageDigest(MessageDigests.MD4, () => new MD4());
            RegisterMessageDigest(MessageDigests.MD5, () => new MD5());
            RegisterMessageDigest(MessageDigests.SHA1, () => new SHA1());
            RegisterMessageDigest(MessageDigests.SHA224, () => new SHA224());
            RegisterMessageDigest(MessageDigests.SHA256, () => new SHA256());
            RegisterMessageDigest(MessageDigests.SHA384, () => new SHA384());
            RegisterMessageDigest(MessageDigests.SHA512, () => new SHA512());
            RegisterMessageDigest(MessageDigests.RIPEMD160, () => new RIPEMD160());
        }
    }
}