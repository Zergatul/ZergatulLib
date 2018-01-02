using System;
using Zergatul.Network;

namespace Zergatul.Cryptography.Hash
{
    public class SHA3 : Keccak
    {
        public override OID OID => null;

        public SHA3(int bits)
            : base(bits << 1, 0x06, bits)
        {

        }
    }

    public class SHA3_224 : SHA3
    {
        public SHA3_224()
            : base(224)
        {

        }
    }

    public class SHA3_256 : SHA3
    {
        public SHA3_256()
            : base(256)
        {

        }
    }

    public class SHA3_384 : SHA3
    {
        public SHA3_384()
            : base(384)
        {

        }
    }

    public class SHA3_512 : SHA3
    {
        public SHA3_512()
            : base(512)
        {

        }
    }
}