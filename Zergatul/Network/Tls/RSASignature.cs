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

namespace Zergatul.Network.Tls
{
    internal class RSASignature : AbstractSignature
    {
        private RSA _rsa;

        public override void SetAlgorithm(object algorithm)
        {
            _rsa = algorithm as RSA;
            if (_rsa == null)
                throw new ArgumentException(nameof(algorithm));
        }

        public override byte[] Sign(AbstractHash hash)
        {
            var ai = new AlgorithmIdentifier(hash.OID, new Null());
            var pkcs = new EMSA_PKCS1_v1_5(ai, hash.ComputeHash(), (_rsa.KeySize + 7) / 8);

            var signature = _rsa.Signature.SignHash(pkcs.ToBytes());
            return signature.ToBytes(ByteOrder.BigEndian);
        }

        public override bool Verify(AbstractHash hash, byte[] signature)
        {
            // TODO: REFACTOR!!!
            // why rsa computations here???
            BigInteger m = BigInteger.ModularExponentiation(new BigInteger(signature, ByteOrder.BigEndian), _rsa.PublicKey.e, _rsa.PublicKey.n);
            var pkcs = EMSA_PKCS1_v1_5.Parse(m.ToBytes(ByteOrder.BigEndian, (_rsa.KeySize + 7) / 8));

            return pkcs.DigestAlgorithm.Algorithm == hash.OID && pkcs.Digest.SequenceEqual(hash.ComputeHash());
        }
    }
}