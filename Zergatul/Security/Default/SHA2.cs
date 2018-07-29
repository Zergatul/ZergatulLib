namespace Zergatul.Security.Default
{
    class SHA224 : AbstractMessageDigest
    {
        public SHA224()
            : base(new Cryptography.Hash.SHA224())
        {

        }
    }

    class SHA256 : AbstractMessageDigest
    {
        public SHA256()
            : base(new Cryptography.Hash.SHA256())
        {

        }
    }

    class SHA384 : AbstractMessageDigest
    {
        public SHA384()
            : base(new Cryptography.Hash.SHA384())
        {

        }
    }

    class SHA512 : AbstractMessageDigest
    {
        public SHA512()
            : base(new Cryptography.Hash.SHA512())
        {

        }
    }
}