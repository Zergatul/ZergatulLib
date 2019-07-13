using System;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    abstract class ChainedMessageDigest : Security.MessageDigest
    {
        public override int BlockLength => _chain[0].BlockLength;
        public override int DigestLength => _chain[_chain.Length - 1].DigestLength;

        private Security.MessageDigest[] _chain;

        protected ChainedMessageDigest(params Security.MessageDigest[] chain)
        {
            if (chain.Length == 0)
                throw new ArgumentException("chain is empty", nameof(chain));

            _chain = chain;
        }

        public override void Reset()
        {
            for (int i = 0; i < _chain.Length; i++)
                _chain[i].Reset();
        }

        public override void Update(byte[] data, int offset, int length)
        {
            _chain[0].Update(data, offset, length);
        }

        public override byte[] Digest()
        {
            byte[] digest = _chain[0].Digest();
            for (int i = 1; i < _chain.Length; i++)
                digest = _chain[i].Digest(digest);
            return digest;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                for (int i = 0; i < _chain.Length; i++)
                    _chain[i].Dispose();
            }
        }
    }
}