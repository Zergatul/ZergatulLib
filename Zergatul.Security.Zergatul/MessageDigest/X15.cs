﻿namespace Zergatul.Security.Zergatul.MessageDigest
{
    class X15 : ChainedMessageDigest
    {
        public override int DigestLength => 32;

        public X15()
            : base(
                  new BLAKE512(),
                  new BMW512(),
                  new Groestl512(),
                  new Skein512x512(),
                  new JH512(),
                  new Keccak512(),
                  new Luffa512(),
                  new CubeHash512(),
                  new SHAvite3x512(),
                  new SIMD512(),
                  new ECHO512(),
                  new Hamsi512(),
                  new Fugue512(),
                  new Shabal512(),
                  new Whirlpool())
        {

        }

        public override byte[] Digest()
        {
            byte[] digest = base.Digest();
            return ByteArray.SubArray(digest, 0, 32);
        }
    }
}