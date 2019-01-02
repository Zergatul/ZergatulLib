namespace Zergatul.Security.Default
{
    class RIPEMD128 : AbstractMessageDigest
    {
        public RIPEMD128()
            : base(new Cryptography.Hash.RIPEMD128())
        {

        }
    }

    class RIPEMD160 : AbstractMessageDigest
    {
        public RIPEMD160()
            : base(new Cryptography.Hash.RIPEMD160())
        {

        }
    }

    class RIPEMD256 : AbstractMessageDigest
    {
        public RIPEMD256()
            : base(new Cryptography.Hash.RIPEMD256())
        {

        }
    }

    class RIPEMD320 : AbstractMessageDigest
    {
        public RIPEMD320()
            : base(new Cryptography.Hash.RIPEMD320())
        {

        }
    }
}