using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using Zergatul.Network.Asn1.Structures;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls
{
    internal class ECDSASignature : AbstractTlsSignature
    {
        public override SignatureAlgorithm Algorithm => SignatureAlgorithm.ECDSA;

        public override byte[] CreateSignature(AbstractSignature algo, AbstractHash hash, byte[] data)
        {
            var ecdsa = (ECPDSA)algo;
            ecdsa.Random = Random;
            ecdsa.Parameters.Hash = hash;
            return ecdsa.Sign(data);
        }

        public override bool VerifySignature(AbstractSignature algo, AbstractHash hash, byte[] data, byte[] signature)
        {
            var ecdsa = (ECPDSA)algo;
            ecdsa.Parameters.Hash = hash;
            return ecdsa.Verify(data, signature);
        }
    }
}