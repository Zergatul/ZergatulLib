namespace Zergatul.Security.Default
{
    class RIPEMD320 : AbstractMessageDigest
    {
        public RIPEMD320()
            : base(new Cryptography.Hash.RIPEMD320())
        {

        }
    }
}