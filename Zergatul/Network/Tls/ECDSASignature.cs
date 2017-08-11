using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using Zergatul.Network.ASN1.Structures;

namespace Zergatul.Network.Tls
{
    internal class ECDSASignature : AbstractSignature
    {
        private ECDSA _ecdsa;

        public override void SetAlgorithm(object algorithm)
        {
            _ecdsa = algorithm as ECDSA;
            if (_ecdsa == null)
                throw new ArgumentException(nameof(algorithm));
        }

        public override byte[] Sign(AbstractHash hash)
        {
            var signature = _ecdsa.Signature.SignHash(hash);
            var ed = new ECDSASignatureValue(signature.r, signature.s);
            return ed.ToBytes();
        }

        public override bool Verify(AbstractHash hash, byte[] signature)
        {
            var ed = ECDSASignatureValue.Parse(ASN1.ASN1Element.ReadFrom(signature));
            return _ecdsa.Signature.VerifyHash(hash, new Cryptography.Asymmetric.ECDSASignature
            {
                r = ed.r,
                s = ed.s
            });
        }
    }
}