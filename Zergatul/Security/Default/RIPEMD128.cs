namespace Zergatul.Security.Default
{
    class RIPEMD128 : AbstractMessageDigest
    {
        public RIPEMD128()
            : base(new Cryptography.Hash.RIPEMD128())
        {

        }
    }
}