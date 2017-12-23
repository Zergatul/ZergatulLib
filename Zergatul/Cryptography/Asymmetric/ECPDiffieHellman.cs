using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// Diffie-Hellman over Elliptic Curve Prime Field
    /// </summary>
    public class ECPDiffieHellman : AbstractKeyExchange<ECPPrivateKey, ECPPublicKey, ECPParameters>
    {
        public override void GenerateKeyPair(int keySize)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");

            var curve = Parameters.Curve;
            PrivateKey = new ECPPrivateKey(BigInteger.Random(curve.p, Random));
            PublicKey = new ECPPublicKey(PrivateKey.Value * curve.g);
        }

        public override byte[] CalculateSharedSecret(ECPPublicKey key)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");
            if (PrivateKey == null)
                throw new InvalidOperationException("Private key is required");

            var curve = Parameters.Curve;
            var point = PrivateKey.Value * key.Point;

            return point.ToCompressed();
        }

        #region Converters

        public override AbstractSignature ToSignature()
        {
            return new ECPDSA
            {
                Parameters = new ECPDSAParameters(Parameters.Curve),
                Random = Random,
                PrivateKey = PrivateKey,
                PublicKey = PublicKey
            };
        }

        public override AbstractKeyExchange ToKeyExchange()
        {
            return this;
        }

        #endregion
    }
}