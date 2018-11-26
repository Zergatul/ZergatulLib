using Zergatul.Security.Default;

namespace Zergatul.Security
{
    public class DefaultProvider : Provider
    {
        public override string Name => "Default";

        public DefaultProvider()
        {
            RegisterKeyDerivationFunction(KeyDerivationFunctions.PBKDF2, () => new PBKDF2());
            RegisterKeyDerivationFunction(KeyDerivationFunctions.Scrypt, () => new Scrypt());

            RegisterKeyPairGenerator(KeyPairGenerators.EC, () => new ECKeyPairGenerator());

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
            RegisterMessageDigest(MessageDigests.Groestl384, () => new Groestl384());
            RegisterMessageDigest(MessageDigests.Groestl512, () => new Groestl512());
            RegisterMessageDigest(MessageDigests.BMW224, () => new BMW224());
            RegisterMessageDigest(MessageDigests.BMW256, () => new BMW256());
            RegisterMessageDigest(MessageDigests.BMW384, () => new BMW384());
            RegisterMessageDigest(MessageDigests.BMW512, () => new BMW512());
            RegisterMessageDigest(MessageDigests.Skein512_224, () => new Skein512_224());
            RegisterMessageDigest(MessageDigests.Skein512_256, () => new Skein512_256());
            RegisterMessageDigest(MessageDigests.Skein512_384, () => new Skein512_384());
            RegisterMessageDigest(MessageDigests.Skein512_512, () => new Skein512_512());
            RegisterMessageDigest(MessageDigests.Luffa512, () => new Luffa512());
            RegisterMessageDigest(MessageDigests.CubeHash, () => new CubeHash());
            RegisterMessageDigest(MessageDigests.CubeHash224, () => new CubeHash224());
            RegisterMessageDigest(MessageDigests.CubeHash256, () => new CubeHash256());
            RegisterMessageDigest(MessageDigests.CubeHash384, () => new CubeHash384());
            RegisterMessageDigest(MessageDigests.CubeHash512, () => new CubeHash512());

            RegisterSignature(Signatures.ECDSA, () => new ECDSASignature());
        }
    }
}