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
            var signature = ecdsa.Signature.SignHash(hash);
            var ed = new ECDSASignatureValue(signature.r, signature.s);
            return ed.ToBytes();
        }

        public override bool VerifySignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash, byte[] signature)
        {
            var ecdsa = (ECDSA)algo;
            var ed = ECDSASignatureValue.Parse(ASN1.ASN1Element.ReadFrom(signature));
            return ecdsa.Signature.VerifyHash(hash, new Cryptography.Asymmetric.ECDSASignature
            {
                r = ed.r,
                s = ed.s
            });
        }
    }
}