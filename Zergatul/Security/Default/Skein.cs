namespace Zergatul.Security.Default
{
    class Skein512_224 : AbstractMessageDigest
    {
        public Skein512_224()
            : base(new Cryptography.Hash.Skein512(28))
        {

        }
    }

    class Skein512_256 : AbstractMessageDigest
    {
        public Skein512_256()
            : base(new Cryptography.Hash.Skein512(32))
        {

        }
    }

    class Skein512_384 : AbstractMessageDigest
    {
        public Skein512_384()
            : base(new Cryptography.Hash.Skein512(48))
        {

        }
    }

    class Skein512_512 : AbstractMessageDigest
    {
        public Skein512_512()
            : base(new Cryptography.Hash.Skein512(64))
        {

        }
    }
}