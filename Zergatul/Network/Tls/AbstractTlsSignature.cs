using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using Zergatul.Network.Tls.Extensions;
using Zergatul.Security;

namespace Zergatul.Network.Tls
{
    internal abstract class AbstractTlsSignature
    {
        public SecureRandom Random;
        public abstract SignatureAlgorithm Algorithm { get; }

        public abstract byte[] CreateSignature(AbstractSignature algo, AbstractHash hash, byte[] data);
        public abstract bool VerifySignature(AbstractSignature algo, AbstractHash hash, byte[] data, byte[] signature);
    }
}