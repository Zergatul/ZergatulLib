using System;

namespace Zergatul.Security
{
    public abstract class MessageDigest
    {
        public abstract int DigestLength { get; }

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

        public static MessageDigest GetInstance(string algorithm)
        {
            return Provider.GetMessageDigestInstance(algorithm);
        }
    }
}