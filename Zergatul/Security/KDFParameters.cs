namespace Zergatul.Security
{
    public abstract class KDFParameters
    {
        public byte[] Password;
        public byte[] Salt;
        public int KeyLength;
    }
}