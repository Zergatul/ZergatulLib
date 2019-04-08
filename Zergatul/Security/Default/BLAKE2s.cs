using System;

namespace Zergatul.Security.Default
{
    class BLAKE2s : MessageDigest
    {
        public override int BlockLength => _blake.BlockSize;
        public override int DigestLength => _blake.HashSize;

        Cryptography.Hash.BLAKE2s _blake;

        public BLAKE2s()
        {
            _blake = new Cryptography.Hash.BLAKE2s();
        }

        public override void Init(MDParameters parameters)
        {
            var p = parameters as BLAKE2Parameters;
            if (p == null)
                throw new ArgumentException(nameof(parameters));

            _blake = new Cryptography.Hash.BLAKE2s(p.DigestLength ?? 32, p.Key);
        }

        public override void Update(byte[] data, int offset, int length) => _blake.Update(data, offset, length);

        public override byte[] Digest() => _blake.ComputeHash();

        public override void Reset() => _blake.Reset();
    }
}