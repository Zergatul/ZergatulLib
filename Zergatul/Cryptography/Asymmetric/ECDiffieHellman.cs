using System;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECDiffieHellman : AbstractAsymmetricAlgorithm<IEllipticCurve, ECPointGeneric, ECPrivateKey, ECPointGeneric, NullParam, NullParam>
    {
        public override ECPrivateKey PrivateKey { get; set; }
        public override ECPointGeneric PublicKey { get; set; }
        public override IEllipticCurve Parameters { get; set; }

        public override int KeySize => Parameters.BitSize;

        private ECDHKeyExchange _keyExchange;

        public override void GenerateKeys()
        {
            if (Parameters is Math.EllipticCurves.PrimeField.EllipticCurve)
            {
                var curve = (Math.EllipticCurves.PrimeField.EllipticCurve)Parameters;
                PrivateKey = new ECPrivateKey
                {
                    BigInteger = BigInteger.Random(curve.p, Random)
                };
                PublicKey = new ECPointGeneric
                {
                    PFECPoint = PrivateKey.BigInteger * curve.g
                };
                return;
            }
            if (Parameters is Math.EllipticCurves.BinaryField.EllipticCurve)
            {
                var curve = (Math.EllipticCurves.BinaryField.EllipticCurve)Parameters;
                PrivateKey = new ECPrivateKey
                {
                    BinaryPolynomial = BinaryPolynomial.Random(curve.m - 1, Random)
                };
                PublicKey = new ECPointGeneric
                {
                    BFECPoint = PrivateKey.BinaryPolynomial * curve.g
                };
                return;
            }

            throw new InvalidOperationException();
        }

        public override AbstractSignatureAlgorithm<NullParam, NullParam> Signature
        {
            get
            {
                throw new NotSupportedException("EC Diffie Hellman doesn't support signing");
            }
        }

        public override AbstractAsymmetricEncryption Encryption
        {
            get
            {
                throw new NotSupportedException("EC Diffie Hellman doesn't support ecryption");
            }
        }


        public override AbstractKeyExchangeAlgorithm<ECPointGeneric, ECPointGeneric> KeyExchange
        {
            get
            {
                if (_keyExchange == null)
                {
                    if (PrivateKey == null || PublicKey == null)
                        throw new InvalidOperationException("You should fill PrivateKey/PublicKey before using KeyExchange");
                    _keyExchange = new ECDHKeyExchange(this);
                }
                return _keyExchange;
            }
        }

        private class ECDHKeyExchange : AbstractKeyExchangeAlgorithm<ECPointGeneric, ECPointGeneric>
        {
            private ECDiffieHellman ecdh;

            public ECDHKeyExchange(ECDiffieHellman ecdh)
            {
                this.ecdh = ecdh;
            }

            public override void CalculateSharedSecret(ECPointGeneric publicKey)
            {
                if (ecdh.Parameters is Math.EllipticCurves.PrimeField.EllipticCurve)
                {
                    var curve = (Math.EllipticCurves.PrimeField.EllipticCurve)ecdh.Parameters;
                    SharedSecret = new ECPointGeneric { PFECPoint = ecdh.PrivateKey.BigInteger * publicKey.PFECPoint };
                    return;
                }
                if (ecdh.Parameters is Math.EllipticCurves.BinaryField.EllipticCurve)
                {
                    var curve = (Math.EllipticCurves.BinaryField.EllipticCurve)ecdh.Parameters;
                    SharedSecret = new ECPointGeneric { BFECPoint = ecdh.PrivateKey.BinaryPolynomial * publicKey.BFECPoint };
                    return;
                }

                throw new InvalidOperationException();
            }
        }
    }
}
