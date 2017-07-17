using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractKeyExchangeAlgorithm<PublicKeyClass, SharedSecretClass>
    {
        public virtual SharedSecretClass SharedSecret { get; protected set; }

        public abstract void CalculateSharedSecret(PublicKeyClass publicKey);
    }
}