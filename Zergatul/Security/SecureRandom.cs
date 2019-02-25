using System;

namespace Zergatul.Security
{
    public abstract class SecureRandom : IDisposable
    {
        protected bool _disposed;

        public virtual void GetNextBytes(byte[] bytes) => GetNextBytes(bytes, 0, bytes.Length);
        public abstract void GetNextBytes(byte[] bytes, int offset, int count);
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

        public static SecureRandom GetInstance(string algorithm) => SecurityProvider.GetSecureRandomInstance(algorithm);
    }
}