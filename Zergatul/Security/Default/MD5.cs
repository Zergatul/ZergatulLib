namespace Zergatul.Security.Default
{
    class MD5 : AbstractMessageDigest
    {
        public MD5()
            : base(new Cryptography.Hash.MD5())
        {

        }
    }
}