﻿using Zergatul.Security.Default;

namespace Zergatul.Security
{
    public class DefaultProvider : Provider
    {
        public override string Name => "Default";

        public DefaultProvider()
        {
            RegisterMessageDigest(MessageDigests.MD5, () => new MD5());
            RegisterMessageDigest(MessageDigests.SHA1, () => new SHA1());
            RegisterMessageDigest(MessageDigests.SHA224, () => new SHA224());
            RegisterMessageDigest(MessageDigests.SHA256, () => new SHA256());
            RegisterMessageDigest(MessageDigests.SHA384, () => new SHA384());
            RegisterMessageDigest(MessageDigests.SHA512, () => new SHA512());
            RegisterMessageDigest(MessageDigests.RIPEMD128, () => new RIPEMD128());
            RegisterMessageDigest(MessageDigests.RIPEMD160, () => new RIPEMD160());
            RegisterMessageDigest(MessageDigests.RIPEMD256, () => new RIPEMD256());
            RegisterMessageDigest(MessageDigests.RIPEMD320, () => new RIPEMD320());
        }
    }
}