using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using Zergatul.Network.ASN1.Structures;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls
{
    internal class ECDSASignature : AbstractTlsSignature
    {
        public override SignatureAlgorithm Algorithm => SignatureAlgorithm.ECDSA;

        public override byte[] CreateSignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash)
        {
            var ecdsa = (ECDSA)algo;
            return ecdsa.Signature.GetScheme("Default").Sign(hash.ComputeHash());
        }

        public override bool VerifySignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash, byte[] signature)
        {
            var ecdsa = (ECDSA)algo;
            return ecdsa.Signature.GetScheme("Default").Verify(signature, hash.ComputeHash());
        }
    }
}