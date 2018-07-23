namespace Zergatul.Security.OpenSsl
{
    class Scrypt : ScryptBase
    {
        public override byte[] GetKeyBytes()
        {
            Validate();

            return null;
        }
    }
}