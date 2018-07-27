namespace Zergatul.Security.Default
{
    class Groestl512 : AbstractMessageDigest
    {
        public Groestl512()
            : base(new Cryptography.Hash.GroestlBig())
        {

        }
    }
}