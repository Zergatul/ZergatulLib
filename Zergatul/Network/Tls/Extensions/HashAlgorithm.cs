using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls.Extensions
{
    internal enum HashAlgorithm : byte
    {
        None = 0,
        MD5 = 1,
        SHA1 = 2,
        SHA224 = 3,
        SHA256 = 4,
        SHA384 = 5,
        SHA512 = 6
    }

    internal static class HashAlgorithmHelper
    {
        public static AbstractHash Resolve(this HashAlgorithm algo)
        {
            switch (algo)
            {
                case HashAlgorithm.MD5: return new MD5();
                case HashAlgorithm.SHA1: return new SHA1();
                case HashAlgorithm.SHA224: return new SHA224();
                case HashAlgorithm.SHA256: return new SHA256();
                case HashAlgorithm.SHA384: return new SHA384();
                case HashAlgorithm.SHA512: return new SHA512();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}