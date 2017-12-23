using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractKeyExchange : AbstractAsymmetricAlgorithm
    {
        public abstract byte[] CalculateSharedSecret(AbstractPublicKey key);
    }

    public abstract class AbstractKeyExchange<PrivateKeyClass, PublicKeyClass, ParametersClass> : AbstractKeyExchange
        where PrivateKeyClass : AbstractPrivateKey
        where PublicKeyClass : AbstractPublicKey
        where ParametersClass : AbstractParameters
    {
        public virtual PrivateKeyClass PrivateKey { get; set; }
        public virtual PublicKeyClass PublicKey { get; set; }

        public virtual ParametersClass Parameters { get; set; }
        public override void SetParameters(AbstractParameters parameters) => Parameters = (ParametersClass)parameters;

        public override AbstractPrivateKey GetPrivateKey() => PrivateKey;
        public override AbstractPublicKey GetPublicKey() => PublicKey;
        public override void SetPrivateKey(AbstractPrivateKey key) => PrivateKey = (PrivateKeyClass)key;
        public override void SetPublicKey(AbstractPublicKey key) => PublicKey = (PublicKeyClass)key;

        public abstract byte[] CalculateSharedSecret(PublicKeyClass key);
        public override byte[] CalculateSharedSecret(AbstractPublicKey key) => CalculateSharedSecret((PublicKeyClass)key);
    }
}