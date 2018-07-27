namespace Zergatul.Security.Default
{
    class Groestl384 : AbstractMessageDigest
    {
        public Groestl384()
            : base(new Cryptography.Hash.GroestlBig(48))
        {

        }
    }
}