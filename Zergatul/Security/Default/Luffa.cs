namespace Zergatul.Security.Default
{
    class Luffa512 : AbstractMessageDigest
    {
        public Luffa512()
            : base(new Cryptography.Hash.Luffa512())
        {

        }
    }
}