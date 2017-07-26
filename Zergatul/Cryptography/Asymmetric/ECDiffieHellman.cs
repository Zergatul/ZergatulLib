using System;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECDiffieHellman : AbstractAsymmetricAlgorithm<DiffieHellmanParameters, ECPoint, BigInteger, ECPoint>
    {
        public override BigInteger PrivateKey { get; set; }
        public override ECPoint PublicKey { get; set; }
        public override DiffieHellmanParameters Parameters { get; set; }

        public override AbstractKeyExchangeAlgorithm<ECPoint, ECPoint> KeyExchange
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int KeySize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override AbstractSignatureAlgorithm Signature
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void GenerateKeys(ISecureRandom random)
        {
            throw new NotImplementedException();
        }

        public override AbstractAsymmetricEncryption Encryption
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
