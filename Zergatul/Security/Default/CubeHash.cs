using System;

namespace Zergatul.Security.Default
{
    class CubeHash : MessageDigest
    {
        public override int BlockLength => _cubeHash?.BlockSize ?? 0;
        public override int DigestLength => _cubeHash?.HashSize ?? 0;

        Cryptography.Hash.Base.CubeHash _cubeHash;

        public override void Init(MDParameters parameters)
        {
            var p = parameters as CubeHashParameters;
            if (p == null)
                throw new ArgumentException(nameof(parameters));

            _cubeHash = new Cryptography.Hash.Base.CubeHash(
                p.InitializationRounds,
                p.RoundsPerBlock,
                p.BytesPerBlock,
                p.FinalizationRounds,
                p.HashSizeBits);
        }

        public override void Update(byte[] data, int offset, int length)
        {
            if (_cubeHash == null)
                throw new InvalidOperationException("Call Init before");
            _cubeHash.Update(data, offset, length);
        }

        public override byte[] Digest()
        {
            if (_cubeHash == null)
                throw new InvalidOperationException("Call Init before");
            return _cubeHash.ComputeHash();
        }

        public override void Reset()
        {
            if (_cubeHash == null)
                throw new InvalidOperationException("Call Init before");
            _cubeHash.Reset();
        }
    }

    class CubeHash224 : AbstractMessageDigest
    {
        public CubeHash224()
            : base(new Cryptography.Hash.CubeHash224())
        {

        }
    }

    class CubeHash256 : AbstractMessageDigest
    {
        public CubeHash256()
            : base(new Cryptography.Hash.CubeHash256())
        {

        }
    }

    class CubeHash384 : AbstractMessageDigest
    {
        public CubeHash384()
            : base(new Cryptography.Hash.CubeHash384())
        {

        }
    }

    class CubeHash512 : AbstractMessageDigest
    {
        public CubeHash512()
            : base(new Cryptography.Hash.CubeHash512())
        {

        }
    }
}