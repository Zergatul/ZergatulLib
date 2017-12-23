using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractEncryption : AbstractAsymmetricAlgorithm
    {
        public abstract byte[] Encrypt(byte[] data);
        public abstract byte[] Decrypt(byte[] data);
    }

    public abstract class AbstractEncryption<PrivateKeyClass, PublicKeyClass, ParametersClass> : AbstractEncryption
        where PrivateKeyClass : AbstractPrivateKey
        where PublicKeyClass : AbstractPublicKey
        where ParametersClass : AbstractParameters, new()
    {
        public virtual PrivateKeyClass PrivateKey { get; set; }
        public virtual PublicKeyClass PublicKey { get; set; }

        public ParametersClass Parameters { get; set; } = new ParametersClass();
        public override void SetParameters(AbstractParameters parameters) => Parameters = (ParametersClass)parameters;

        public override AbstractPrivateKey GetPrivateKey() => PrivateKey;
        public override AbstractPublicKey GetPublicKey() => PublicKey;
        public override void SetPrivateKey(AbstractPrivateKey key) => PrivateKey = (PrivateKeyClass)key;
        public override void SetPublicKey(AbstractPublicKey key) => PublicKey = (PublicKeyClass)key;
    }
}