using System;

namespace Zergatul.Security.Default
{
    class BLAKE2b : MessageDigest
    {
        public override int DigestLength => _blake.HashSize;

        Cryptography.Hash.BLAKE2b _blake;

        public BLAKE2b()
        {
            _blake = new Cryptography.Hash.BLAKE2b();
        }

        public override void Init(MDParameters parameters)
        {
            var p = parameters as BLAKE2Parameters;
            if (p == null)
                throw new ArgumentException(nameof(parameters));

            _blake = new Cryptography.Hash.BLAKE2b(p.HashSizeBytes ?? 64, p.Key);
        }

        public override void Update(byte[] data, int offset, int length) => _blake.Update(data, offset, length);

        public override byte[] Digest() => _blake.ComputeHash();

        public override void Reset() => _blake.Reset();
    }
}