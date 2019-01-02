namespace Zergatul.Security.Default
{
    class SHA1 : AbstractMessageDigest
    {
        public SHA1()
            : base(new Cryptography.Hash.SHA1())
        {

        }
    }
}