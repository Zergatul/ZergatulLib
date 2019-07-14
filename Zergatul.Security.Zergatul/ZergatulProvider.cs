using System.IO;
using Zergatul.Security.Zergatul.MessageDigest;

namespace Zergatul.Security.Zergatul
{
    public class ZergatulProvider : SecurityProvider
    {
        public override string Name => "Zergatul";

        public ZergatulProvider()
        {
            RegisterMessageDigest(MessageDigests.MD2, () => new MD2());
            RegisterMessageDigest(MessageDigests.MD4, () => new MD4());
            RegisterMessageDigest(MessageDigests.MD5, () => new MD5());
            RegisterMessageDigest(MessageDigests.SHA1, () => new SHA1());
            RegisterMessageDigest(MessageDigests.SHA224, () => new SHA224());
            RegisterMessageDigest(MessageDigests.SHA256, () => new SHA256());
            RegisterMessageDigest(MessageDigests.SHA384, () => new SHA384());
            RegisterMessageDigest(MessageDigests.SHA512, () => new SHA512());
            RegisterMessageDigest(MessageDigests.RIPEMD128, () => new RIPEMD128());
            RegisterMessageDigest(MessageDigests.RIPEMD160, () => new RIPEMD160());
            RegisterMessageDigest(MessageDigests.RIPEMD256, () => new RIPEMD256());
            RegisterMessageDigest(MessageDigests.RIPEMD320, () => new RIPEMD320());
            RegisterMessageDigest(MessageDigests.BLAKE2s, () => new BLAKE2s());
            RegisterMessageDigest(MessageDigests.BLAKE2b, () => new BLAKE2b());
            RegisterMessageDigest(MessageDigests.BMW224, () => new BMW224());
            RegisterMessageDigest(MessageDigests.BMW256, () => new BMW256());
            RegisterMessageDigest(MessageDigests.BMW384, () => new BMW384());
            RegisterMessageDigest(MessageDigests.BMW512, () => new BMW512());
            RegisterMessageDigest(MessageDigests.CubeHash224, () => new CubeHash224());
            RegisterMessageDigest(MessageDigests.CubeHash256, () => new CubeHash256());
            RegisterMessageDigest(MessageDigests.CubeHash384, () => new CubeHash384());
            RegisterMessageDigest(MessageDigests.CubeHash512, () => new CubeHash512());
            RegisterMessageDigest(MessageDigests.Groestl224, () => new Groestl224());
            RegisterMessageDigest(MessageDigests.Groestl256, () => new Groestl256());
            RegisterMessageDigest(MessageDigests.Groestl384, () => new Groestl384());
            RegisterMessageDigest(MessageDigests.Groestl512, () => new Groestl512());
            RegisterMessageDigest(MessageDigests.JH224, () => new JH224());
            RegisterMessageDigest(MessageDigests.JH256, () => new JH256());
            RegisterMessageDigest(MessageDigests.JH384, () => new JH384());
            RegisterMessageDigest(MessageDigests.JH512, () => new JH512());
            RegisterMessageDigest(MessageDigests.BLAKE224, () => new BLAKE224());
            RegisterMessageDigest(MessageDigests.BLAKE256, () => new BLAKE256());
            RegisterMessageDigest(MessageDigests.BLAKE384, () => new BLAKE384());
            RegisterMessageDigest(MessageDigests.BLAKE512, () => new BLAKE512());
            RegisterMessageDigest(MessageDigests.Keccak224, () => new Keccak224());
            RegisterMessageDigest(MessageDigests.Keccak256, () => new Keccak256());
            RegisterMessageDigest(MessageDigests.Keccak384, () => new Keccak384());
            RegisterMessageDigest(MessageDigests.Keccak512, () => new Keccak512());
            RegisterMessageDigest(MessageDigests.SHA3x224, () => new SHA3x224());
            RegisterMessageDigest(MessageDigests.SHA3x256, () => new SHA3x256());
            RegisterMessageDigest(MessageDigests.SHA3x384, () => new SHA3x384());
            RegisterMessageDigest(MessageDigests.SHA3x512, () => new SHA3x512());
            RegisterMessageDigest(MessageDigests.SHAvite3x224, () => new SHAvite3x224());
            RegisterMessageDigest(MessageDigests.SHAvite3x256, () => new SHAvite3x256());
            RegisterMessageDigest(MessageDigests.SHAvite3x384, () => new SHAvite3x384());
            RegisterMessageDigest(MessageDigests.SHAvite3x512, () => new SHAvite3x512());
            RegisterMessageDigest(MessageDigests.ECHO224, () => new ECHO224());
            RegisterMessageDigest(MessageDigests.ECHO256, () => new ECHO256());
            RegisterMessageDigest(MessageDigests.ECHO384, () => new ECHO384());
            RegisterMessageDigest(MessageDigests.ECHO512, () => new ECHO512());
            RegisterMessageDigest(MessageDigests.Luffa224, () => new Luffa224());
            RegisterMessageDigest(MessageDigests.Luffa256, () => new Luffa256());
            RegisterMessageDigest(MessageDigests.Luffa384, () => new Luffa384());
            RegisterMessageDigest(MessageDigests.Luffa512, () => new Luffa512());
            RegisterMessageDigest(MessageDigests.SIMD224, () => new SIMD224());
            RegisterMessageDigest(MessageDigests.SIMD256, () => new SIMD256());
            RegisterMessageDigest(MessageDigests.SIMD384, () => new SIMD384());
            RegisterMessageDigest(MessageDigests.SIMD512, () => new SIMD512());
            RegisterMessageDigest(MessageDigests.Skein512x224, () => new Skein512x224());
            RegisterMessageDigest(MessageDigests.Skein512x256, () => new Skein512x256());
            RegisterMessageDigest(MessageDigests.Skein512x384, () => new Skein512x384());
            RegisterMessageDigest(MessageDigests.Skein512x512, () => new Skein512x512());
            RegisterMessageDigest(MessageDigests.Hamsi224, () => new Hamsi224());
            RegisterMessageDigest(MessageDigests.Hamsi256, () => new Hamsi256());
            RegisterMessageDigest(MessageDigests.Hamsi384, () => new Hamsi384());
            RegisterMessageDigest(MessageDigests.Hamsi512, () => new Hamsi512());
            RegisterMessageDigest(MessageDigests.Fugue512, () => new Fugue512());

            RegisterMessageDigest(MessageDigests.X11, () => new X11());
            RegisterMessageDigest(MessageDigests.X13, () => new X13());
        }

        public override TlsStream GetTlsStream(Stream innerStream) => new Tls.TlsStream(innerStream);
    }
}