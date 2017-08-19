using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class SignatureOnlyAssymetricAlgorithm<AlgorithmParameters, PublicKeyClass, PrivateKeyClass, SignatureInputClass, SignatureClass> : AbstractAsymmetricAlgorithm<AlgorithmParameters, PublicKeyClass, PrivateKeyClass, NullParam, SignatureInputClass, SignatureClass>
    {
        public override AbstractAsymmetricEncryption Encryption
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override AbstractKeyExchangeAlgorithm<PublicKeyClass, NullParam> KeyExchange
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }

    public abstract class AbstractAsymmetricAlgorithm<AlgorithmParameters, PublicKeyClass, PrivateKeyClass, SharedSecretClass, SignatureInputClass, SignatureClass> : AbstractAsymmetricAlgorithm
    {
        public abstract PublicKeyClass PublicKey { get; set; }
        public abstract PrivateKeyClass PrivateKey { get; set; }
        public abstract AlgorithmParameters Parameters { get; set; }

        public abstract void GenerateKeys();
        public abstract AbstractKeyExchangeAlgorithm<PublicKeyClass, SharedSecretClass> KeyExchange { get; }
        public abstract AbstractSignatureAlgorithm<SignatureInputClass, SignatureClass> Signature { get; }
    }

    public abstract class AbstractAsymmetricAlgorithm
    {
        /// <summary>
        /// Key size in bits
        /// </summary>
        public abstract int KeySize { get; }

        public ISecureRandom Random { get; set; }

        public abstract AbstractAsymmetricEncryption Encryption { get; }
    }
}