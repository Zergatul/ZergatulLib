namespace Zergatul.Security.Default
{
    class BMW224 : AbstractMessageDigest
    {
        public BMW224()
            : base(new Cryptography.Hash.BMW224())
        {

        }
    }

    class BMW256 : AbstractMessageDigest
    {
        public BMW256()
            : base(new Cryptography.Hash.BMW256())
        {

        }
    }

    class BMW384 : AbstractMessageDigest
    {
        public BMW384()
            : base(new Cryptography.Hash.BMW384())
        {

        }
    }

    class BMW512 : AbstractMessageDigest
    {
        public BMW512()
            : base(new Cryptography.Hash.BMW512())
        {

        }
    }
}