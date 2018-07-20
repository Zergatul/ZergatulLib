namespace Zergatul.Security.Default
{
    class RIPEMD256 : AbstractMessageDigest
    {
        public RIPEMD256()
            : base(new Cryptography.Hash.RIPEMD256())
        {

        }
    }
}