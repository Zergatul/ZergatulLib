using System.IO;
using Zergatul.Security.OpenSsl.KeyDerivationFunction;
using Zergatul.Security.OpenSsl.MessageDigest;
using Zergatul.Security.OpenSsl.Signature;
using Zergatul.Security.OpenSsl.SymmetricCipher;

namespace Zergatul.Security.OpenSsl
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
            RegisterMessageDigest(MessageDigests.BLAKE2s, () => new BLAKE2s());
            RegisterMessageDigest(MessageDigests.BLAKE2b, () => new BLAKE2b());

            RegisterSignature(Signatures.ECDSA, () => new ECDSASignature());

            RegisterSymmetricCipher(SymmetricCiphers.AES, () => new AES());
        }

        public override TlsStream GetTlsStream(Stream innerStream) => new Tls.TlsStream(innerStream);
    }
}