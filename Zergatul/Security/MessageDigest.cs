namespace Zergatul.Security
{
    public abstract class MessageDigest
    {
        public abstract int DigestLength { get; }

        public abstract byte[] Digest();
        public abstract byte[] Digest(byte[] data);
        public abstract byte[] Digest(byte[] data, int offset, int length);
        public abstract void Reset();
        public abstract void Update(byte[] data);
        public abstract void Update(byte[] data, int offset, int length);

        public static MessageDigest GetInstance(string algorithm)
        {
            return Provider.GetMessageDigestInstance(algorithm);
        }
    }
}