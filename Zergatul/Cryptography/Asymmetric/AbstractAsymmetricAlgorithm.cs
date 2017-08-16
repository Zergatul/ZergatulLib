using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractAsymmetricAlgorithm<AlgorithmParameters, PublicKeyClass, PrivateKeyClass, SharedSecretClass, SignatureClass> : AbstractAsymmetricAlgorithm
    {
        public abstract PublicKeyClass PublicKey { get; set; }
        public abstract PrivateKeyClass PrivateKey { get; set; }
        public abstract AlgorithmParameters Parameters { get; set; }

        public abstract void GenerateKeys(ISecureRandom random);
        public abstract AbstractKeyExchangeAlgorithm<PublicKeyClass, SharedSecretClass> KeyExchange { get; }
        public abstract AbstractSignatureAlgorithm<SignatureClass> Signature { get; }
    }

    public abstract class AbstractAsymmetricAlgorithm
    {
        /// <summary>
        /// Key size in bits
        /// </summary>
        public abstract int KeySize { get; }

        public abstract AbstractAsymmetricEncryption Encryption { get; }
    }
}