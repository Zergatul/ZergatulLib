namespace Zergatul.Security
{
    public abstract class KeyDerivationFunction
    {
        public abstract void Init(KDFParameters parameters);
        public abstract byte[] GetKeyBytes();
    }
}