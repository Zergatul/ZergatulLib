namespace Zergatul.Security.Default
{
    class SHA384 : AbstractMessageDigest
    {
        public SHA384()
            : base(new Cryptography.Hash.SHA384())
        {

        }
    }
}