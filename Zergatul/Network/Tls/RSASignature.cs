using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Network.Asn1;
using Zergatul.Network.Asn1.Structures;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls
{
    internal class RSASignature : AbstractTlsSignature
    {
        public override SignatureAlgorithm Algorithm => SignatureAlgorithm.RSA;

        public override byte[] CreateSignature(AbstractSignature algo, AbstractHash hash, byte[] data)
        {
            var rsa = (Cryptography.Asymmetric.RSASignature)algo;
            rsa.Parameters.Scheme = RSASignatureScheme.RSASSA_PKCS1_v1_5;
            rsa.Parameters.Hash = hash;
            return rsa.Sign(data);
        }

        public override bool VerifySignature(AbstractSignature algo, AbstractHash hash, byte[] data, byte[] signature)
        {
            var rsa = (Cryptography.Asymmetric.RSASignature)algo;
            rsa.Parameters.Scheme = RSASignatureScheme.RSASSA_PKCS1_v1_5;
            rsa.Parameters.Hash = hash;
            return rsa.Verify(data, signature);
        }
    }
}