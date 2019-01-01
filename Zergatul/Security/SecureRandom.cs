using System;

namespace Zergatul.Security
{
    public abstract class SecureRandom : IDisposable
    {
        protected bool _disposed;

        public abstract void GetNextBytes(byte[] bytes);
        public abstract void SetSeed(byte[] seed);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        ~SecureRandom()
        {
            Dispose(false);
        }

        public static SecureRandom GetInstance(string algorithm) => Provider.GetSecureRandomInstance(algorithm);
    }
}