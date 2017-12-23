using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractSignature : AbstractAsymmetricAlgorithm
    {
        public abstract byte[] Sign(byte[] data);
        public abstract byte[] SignHash(byte[] hash);
        public abstract bool Verify(byte[] data, byte[] signature);
        public abstract bool VerifyHash(byte[] hash, byte[] signature);
    }

    public abstract class AbstractSignature<PrivateKeyClass, PublicKeyClass, ParametersClass> : AbstractSignature
        where PrivateKeyClass : AbstractPrivateKey
        where PublicKeyClass : AbstractPublicKey
        where ParametersClass : AbstractParameters
    {
        public virtual PrivateKeyClass PrivateKey { get; set; }
        public virtual PublicKeyClass PublicKey { get; set; }

        public ParametersClass Parameters { get; set; }
        public override void SetParameters(AbstractParameters parameters) => Parameters = (ParametersClass)parameters;

        public override AbstractPrivateKey GetPrivateKey() => PrivateKey;
        public override AbstractPublicKey GetPublicKey() => PublicKey;
        public override void SetPrivateKey(AbstractPrivateKey key) => PrivateKey = (PrivateKeyClass)key;
        public override void SetPublicKey(AbstractPublicKey key) => PublicKey = (PublicKeyClass)key;
    }
}