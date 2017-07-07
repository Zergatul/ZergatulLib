using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractKeyExchangeAlgorithm<PublicKeyClass, SharedSecretClass>
    {
        public abstract byte[] GenerateSharedSecretBytes(PublicKeyClass publicKey);
        public abstract SharedSecretClass GenerateSharedSecret(PublicKeyClass publicKey);
    }
}