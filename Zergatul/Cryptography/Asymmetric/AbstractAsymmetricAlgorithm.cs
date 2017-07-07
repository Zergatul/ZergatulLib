using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractAsymmetricAlgorithm<AlgorithmParameters, PublicKeyClass, PrivateKeyClass, SharedSecretClass>
    {
        public abstract PublicKeyClass PublicKey { get; set; }
        public abstract PrivateKeyClass PrivateKey { get; set; }

        /// <summary>
        /// Key size in bits
        /// </summary>
        public abstract int KeySize { get; }

        public abstract void SetParameters(AlgorithmParameters parameters);
        public abstract void GenerateKeys(ISecureRandom random);

        public abstract AbstractSignatureAlgorithm Signature { get; }
        public abstract AbstractKeyExchangeAlgorithm<PublicKeyClass, SharedSecretClass> KeyExchange { get; }
    }
}