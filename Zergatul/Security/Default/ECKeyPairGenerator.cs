using System;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Security.Default
{
    class ECKeyPairGenerator : KeyPairGenerator
    {
        private EllipticCurve _curve;

        public override void Init(KeyPairGeneratorParameters parameters)
        {
            var @params = parameters as ECKeyPairGeneratorParameters;
            if (@params == null)
                throw new InvalidOperationException();

            switch (@params.Curve)
            {
                case Curves.secp256k1: _curve = EllipticCurve.secp256k1; break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override KeyPair GenerateKeyPair()
        {
            throw new NotImplementedException();
        }

        public override PublicKey GetPublicKey(PrivateKey privateKey)
        {
            if (privateKey == null)
                throw new ArgumentNullException(nameof(privateKey));
            if (_curve == null)
                throw new InvalidOperationException();

            switch (privateKey)
            {
                case RawPrivateKey raw:
                    var point = _curve.GeneratorMultiplication(new Math.BigInteger(raw.Data, ByteOrder.BigEndian));
                    return new ECPublicKey(point);

                default:
                    throw new NotImplementedException();
            }
        }

        public override byte[] Format(PublicKey publicKey, KeyFormat format)
        {
            if (publicKey == null)
                throw new ArgumentNullException(nameof(publicKey));
            if (format != KeyFormat.Uncompressed && format != KeyFormat.Compressed)
                throw new InvalidOperationException();

            switch (publicKey)
            {
                case ECPublicKey ec:
                    if (format == KeyFormat.Uncompressed)
                        return ec.Point.ToUncompressed();
                    else
                        return ec.Point.ToCompressed();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}