using System;

namespace Zergatul.Security
{
    public abstract class Signature : IDisposable
    {
        protected bool _disposed;

        public abstract void InitForSign(PrivateKey key, SignatureParameters parameters);
        public abstract void InitForVerify(PublicKey key, SignatureParameters parameters);
        public abstract void Update(byte[] data, int offset, int length);
        public abstract byte[] Sign();
        public abstract bool Verify(byte[] signature);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        ~Signature()
        {
            Dispose(false);
        }

        public static Signature GetInstance(string algorithm) => Provider.GetSignatureInstance(algorithm);
    }
}