using Zergatul.Cryptography.Hash;

namespace Zergatul.Security.Default
{
    abstract class AbstractMessageDigest : MessageDigest
    {
        public override int BlockLength => _hash.BlockSize;
        public override int DigestLength => _hash.HashSize;

        private AbstractHash _hash;

        protected AbstractMessageDigest(AbstractHash hash)
        {
            this._hash = hash;
        }

        public override byte[] Digest() => _hash.ComputeHash();

        public override void Reset() => _hash.Reset();

        public override void Update(byte[] data) => _hash.Update(data);

        public override void Update(byte[] data, int offset, int length) => _hash.Update(data, offset, length);
    }
}