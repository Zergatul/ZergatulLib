namespace Zergatul.Security.Default
{
    class CubeHash224 : AbstractMessageDigest
    {
        public CubeHash224()
            : base(new Cryptography.Hash.CubeHash224())
        {

        }
    }

    class CubeHash256 : AbstractMessageDigest
    {
        public CubeHash256()
            : base(new Cryptography.Hash.CubeHash256())
        {

        }
    }

    class CubeHash384 : AbstractMessageDigest
    {
        public CubeHash384()
            : base(new Cryptography.Hash.CubeHash384())
        {

        }
    }

    class CubeHash512 : AbstractMessageDigest
    {
        public CubeHash512()
            : base(new Cryptography.Hash.CubeHash512())
        {

        }
    }
}