using System;

namespace Zergatul.Security
{
    public abstract class KeyAgreement
    {
        protected bool _disposed;

        public abstract void Init(PrivateKey key, KeyAgreementParameters parameters);
        public abstract void DoPhase(PublicKey key, bool isLast);
        public virtual void DoPhase(PublicKey key) => DoPhase(key, true);
        public abstract byte[] GenerateSecret();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        ~KeyAgreement()
        {
            Dispose(false);
        }

        public static KeyAgreement GetInstance(string algorithm) => SecurityProvider.GetKeyAgreementInstance(algorithm);
    }
}