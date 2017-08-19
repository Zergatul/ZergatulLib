using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls
{
    internal class RSASignature : AbstractTlsSignature
    {
        public override SignatureAlgorithm Algorithm => SignatureAlgorithm.RSA;

        public override byte[] CreateSignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash)
        {
            var rsa = (RSA)algo;
            var scheme = rsa.Signature.GetScheme("EMSA-PKCS1-v1.5");
            scheme.SetParameter(hash.OID);
            return scheme.Sign(hash.ComputeHash());
        }

        public override bool VerifySignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash, byte[] signature)
        {
            var rsa = (RSA)algo;
            var scheme = rsa.Signature.GetScheme("EMSA-PKCS1-v1.5");
            scheme.SetParameter(hash.OID);
            return scheme.Verify(signature, hash.ComputeHash());
        }
    }
}