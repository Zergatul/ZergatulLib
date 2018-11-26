using System;

namespace Zergatul.Security
{
    public abstract class MessageDigest : IDisposable
    {
        public abstract int DigestLength { get; }

        protected bool _disposed;

        public virtual void Init(MDParameters parameters)
        {
            throw new NotSupportedException();
        }

        public abstract byte[] Digest();

        public virtual byte[] Digest(byte[] data)
        {
            Update(data);
            return Digest();
        }

        public virtual byte[] Digest(byte[] data, int offset, int length)
        {
            Update(data, offset, length);
            return Digest();
        }

        public abstract void Reset();

        public virtual void Update(byte[] data)
        {
            Update(data, 0, data.Length);
        }

        public abstract void Update(byte[] data, int offset, int length);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        ~MessageDigest()
        {
            Dispose(false);
        }

        public static MessageDigest GetInstance(string algorithm) => Provider.GetMessageDigestInstance(algorithm);
    }
}