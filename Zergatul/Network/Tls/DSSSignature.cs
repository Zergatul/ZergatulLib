using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using Zergatul.Network.Tls.Extensions;

namespace Zergatul.Network.Tls
{
    internal class DSSSignature : AbstractTlsSignature
    {
        public override SignatureAlgorithm Algorithm => SignatureAlgorithm.DSA;

        public override byte[] CreateSignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash)
        {
            var dsa = (DSA)algo;
            dsa.Random = Random;
            return dsa.Signature.GetScheme("Default").Sign(hash.ComputeHash());
        }

        public override bool VerifySignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash, byte[] signature)
        {
            var dsa = (DSA)algo;
            return dsa.Signature.GetScheme("Default").Verify(signature, hash.ComputeHash());
        }
    }
}