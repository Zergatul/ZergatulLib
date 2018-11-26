using System;

namespace Zergatul.Security
{
    public abstract class KeyPairGenerator : IDisposable
    {
        public abstract void Init(KeyPairGeneratorParameters parameters);
        public abstract KeyPair GenerateKeyPair();
        public abstract PublicKey GetPublicKey(PrivateKey privateKey);
        public abstract byte[] Format(PublicKey publicKey, KeyFormat format);

        protected bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        ~KeyPairGenerator()
        {
            Dispose(false);
        }

        public static KeyPairGenerator GetInstance(string algorithm) => Provider.GetKeyPairGeneratorInstance(algorithm);
    }
}