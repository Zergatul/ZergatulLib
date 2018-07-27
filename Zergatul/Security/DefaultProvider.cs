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
        }
    }
}