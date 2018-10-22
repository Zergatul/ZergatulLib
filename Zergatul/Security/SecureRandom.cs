namespace Zergatul.Security
{
    public abstract class SecureRandom
    {
        public abstract void GetNextBytes(byte[] bytes);
        public abstract void SetSeed(byte[] seed);

        public static SecureRandom GetInstance(string algorithm) => Provider.GetSecureRandomInstance(algorithm);
    }
}