namespace Zergatul.Security.Zergatul.MessageDigest
{
    class X17 : ChainedMessageDigest
    {
        public X17()
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
                  new Whirlpool(),
                  new SHA512(),
                  new Haval256())
        {

        }
    }
}