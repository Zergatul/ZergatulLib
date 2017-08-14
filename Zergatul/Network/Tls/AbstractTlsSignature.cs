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
    internal abstract class AbstractTlsSignature
    {
        public abstract SignatureAlgorithm Algorithm { get; }

        public abstract byte[] CreateSignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash);
        public abstract bool VerifySignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash, byte[] signature);
    }
}