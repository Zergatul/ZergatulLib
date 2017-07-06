using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractAsymmetricAlgorithm<PublicKeyClass, PrivateKeyClass>
    {
        public abstract PublicKeyClass PublicKey { get; set; }
        public abstract PrivateKeyClass PrivateKey { get; set; }

        public abstract AbstractSignatureAlgorithm Signature { get; }
        public abstract AbstractKeyExchangeAlgorithm KeyExchange { get; }
    }
}