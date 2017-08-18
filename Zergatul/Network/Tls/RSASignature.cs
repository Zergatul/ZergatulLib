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

            var scheme = new EMSAPKCS1v15Scheme();
            scheme.KeySize = rsa.KeySize;
            scheme.HashAlgorithmOID = hash.OID;

            return rsa.Signature.SignHash(hash, scheme);
        }

        public override bool VerifySignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash, byte[] signature)
        {
            var rsa = (RSA)algo;

            var scheme = new EMSAPKCS1v15Scheme();
            scheme.KeySize = rsa.KeySize;
            scheme.HashAlgorithmOID = hash.OID;

            return rsa.Signature.VerifyHash(hash, signature, scheme);
        }
    }
}