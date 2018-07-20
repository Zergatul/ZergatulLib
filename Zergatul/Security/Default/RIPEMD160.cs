namespace Zergatul.Security.Default
{
    class RIPEMD160 : AbstractMessageDigest
    {
        public RIPEMD160()
            : base(new Cryptography.Hash.RIPEMD160())
        {

        }
    }
}