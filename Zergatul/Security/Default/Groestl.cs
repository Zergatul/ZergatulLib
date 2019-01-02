namespace Zergatul.Security.Default
{
    class Groestl384 : AbstractMessageDigest
    {
        public Groestl384()
            : base(new Cryptography.Hash.Groestl384())
        {

        }
    }

    class Groestl512 : AbstractMessageDigest
    {
        public Groestl512()
            : base(new Cryptography.Hash.Groestl512())
        {

        }
    }
}