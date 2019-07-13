namespace Zergatul.Security.Zergatul.MessageDigest
{
    class X11 : ChainedMessageDigest
    {
        public override int DigestLength => 32;

        public X11()
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
                  new ECHO512())
        {

        }

        public override byte[] Digest()
        {
            byte[] digest = base.Digest();
            return ByteArray.SubArray(digest, 0, 32);
        }
    }
}