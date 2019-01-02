using System;

namespace Zergatul.Security
{
    public abstract class SymmetricCipher : IDisposable
    {
        public abstract int BlockSize { get; }

        public abstract void InitForEncryption(byte[] key, SymmetricCipherParameters parameters);
        public abstract void InitForDecryption(byte[] key, SymmetricCipherParameters parameters);

        public abstract int Update(byte[] input, int inputLength, byte[] output);
        public abstract int DoFinal(byte[] output);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        public static SymmetricCipher GetInstance(string algorithm) => SecurityProvider.GetSymmetricCipherInstance(algorithm);
    }
}