namespace Zergatul.Security.Default
{
    class SHA512 : AbstractMessageDigest
    {
        public SHA512()
            : base(new Cryptography.Hash.SHA512())
        {

        }
    }
}