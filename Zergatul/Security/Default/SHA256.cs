namespace Zergatul.Security.Default
{
    class SHA256 : AbstractMessageDigest
    {
        public SHA256()
            : base(new Cryptography.Hash.SHA256())
        {

        }
    }
}