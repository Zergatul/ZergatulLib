namespace Zergatul.Security.Default
{
    class SHA224 : AbstractMessageDigest
    {
        public SHA224()
            : base(new Cryptography.Hash.SHA224())
        {

        }
    }
}