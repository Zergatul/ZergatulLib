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

        public override byte[] CreateSignature(AbstractSignature algo, AbstractHash hash, byte[] data)
        {
            var dsa = (DSA)algo;
            dsa.Random = Random;
            dsa.Parameters.Hash = hash;
            return dsa.Sign(data);
        }

        public override bool VerifySignature(AbstractSignature algo, AbstractHash hash, byte[] data, byte[] signature)
        {
            var dsa = (DSA)algo;
            dsa.Parameters.Hash = hash;
            return dsa.Verify(data, signature);
        }
    }
}