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

            var ai = new AlgorithmIdentifier(hash.OID, new Null());
            var pkcs = new EMSA_PKCS1_v1_5(ai, hash.ComputeHash(), (rsa.KeySize + 7) / 8);

            var signature = rsa.Signature.SignHash(pkcs.ToBytes());
            return signature.ToBytes(ByteOrder.BigEndian);
        }

        public override bool VerifySignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash, byte[] signature)
        {
            var rsa = (RSA)algo;
            // TODO: REFACTOR!!!
            // why rsa computations here???
            BigInteger m = BigInteger.ModularExponentiation(new BigInteger(signature, ByteOrder.BigEndian), rsa.PublicKey.e, rsa.PublicKey.n);
            var pkcs = EMSA_PKCS1_v1_5.Parse(m.ToBytes(ByteOrder.BigEndian, (rsa.KeySize + 7) / 8));

            return pkcs.DigestAlgorithm.Algorithm == hash.OID && pkcs.Digest.SequenceEqual(hash.ComputeHash());
        }
    }
}