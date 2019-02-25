using Zergatul.Security;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractAsymmetricAlgorithm
    {
        public abstract void SetParameters(AbstractParameters parameters);
        public SecureRandom Random { get; set; }
        public abstract void GenerateKeyPair(int keySize);
        public abstract AbstractPrivateKey GetPrivateKey();
        public abstract AbstractPublicKey GetPublicKey();
        public abstract void SetPrivateKey(AbstractPrivateKey key);
        public abstract void SetPublicKey(AbstractPublicKey key);

        public abstract AbstractSignature ToSignature();
        public abstract AbstractKeyExchange ToKeyExchange();
    }
}