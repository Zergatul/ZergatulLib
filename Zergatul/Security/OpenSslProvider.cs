using Zergatul.Security.OpenSsl;

namespace Zergatul.Security
{
    public class OpenSslProvider : SecurityProvider
    {
        public override string Name => "OpenSSL";

        public OpenSslProvider()
        {
            RegisterKeyDerivationFunction(KeyDerivationFunctions.PBKDF2, () => new PBKDF2());
            RegisterKeyDerivationFunction(KeyDerivationFunctions.Scrypt, () => new Scrypt());

            RegisterKeyPairGenerator(KeyPairGenerators.EC, () => new ECKeyPairGenerator());

            RegisterMessageDigest(MessageDigests.MD4, () => new MD4());
            RegisterMessageDigest(MessageDigests.MD5, () => new MD5());
            RegisterMessageDigest(MessageDigests.SHA1, () => new SHA1());
            RegisterMessageDigest(MessageDigests.SHA224, () => new SHA224());
            RegisterMessageDigest(MessageDigests.SHA256, () => new SHA256());
            RegisterMessageDigest(MessageDigests.SHA384, () => new SHA384());
            RegisterMessageDigest(MessageDigests.SHA512, () => new SHA512());
            RegisterMessageDigest(MessageDigests.RIPEMD160, () => new RIPEMD160());

            RegisterSignature(Signatures.ECDSA, () => new ECDSASignature());

            RegisterSymmetricCipher(SymmetricCiphers.AES, () => new AES());
        }
    }
}