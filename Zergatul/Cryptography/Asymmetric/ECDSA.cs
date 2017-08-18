using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECDSA : AbstractAsymmetricAlgorithm<ECDSAParameters, ECPointGeneric, ECPrivateKey, NullParam, BigInteger, ECDSASignature>
    {
        public override ECPrivateKey PrivateKey { get; set; }
        public override ECPointGeneric PublicKey { get; set; }
        public override ECDSAParameters Parameters { get; set; }

        public override int KeySize => Parameters.Curve.BitSize;

        private ECDSASignatureImpl _signature;

        public override void GenerateKeys(ISecureRandom random)
        {
            if (Parameters.Curve is Math.EllipticCurves.PrimeField.EllipticCurve)
            {
                var curve = (Math.EllipticCurves.PrimeField.EllipticCurve)Parameters.Curve;
                PrivateKey = new ECPrivateKey
                {
                    BigInteger = BigInteger.Random(BigInteger.One, curve.n, random)
                };
                PublicKey = new ECPointGeneric
                {
                    PFECPoint = PrivateKey.BigInteger * curve.g
                };
                return;
            }

            throw new NotImplementedException();
        }

        public override AbstractAsymmetricEncryption Encryption
        {
            get
            {
                throw new NotSupportedException("ECDSA doesn't support key encryption");
            }
        }

        public override AbstractKeyExchangeAlgorithm<ECPointGeneric, NullParam> KeyExchange
        {
            get
            {
                throw new NotSupportedException("ECDSA doesn't support key exchange");
            }
        }

        public override AbstractSignatureAlgorithm<BigInteger, ECDSASignature> Signature
        {
            get
            {
                if (_signature == null)
                {
                    if (PublicKey == null)
                        throw new InvalidOperationException("PublicKey is null");
                    _signature = new ECDSASignatureImpl(this);
                }
                return _signature;
            }
        }

        private class ECDSASignatureImpl : AbstractSignatureAlgorithm<BigInteger, ECDSASignature>
        {
            private ECDSA _ecdsa;

            public ECDSASignatureImpl(ECDSA ecdsa)
            {
                this._ecdsa = ecdsa;
            }

            public override ECDSASignature Sign(BigInteger data)
            {
                if (_ecdsa.PublicKey.PFECPoint != null)
                {
                    var curve = _ecdsa.Parameters.Curve as Math.EllipticCurves.PrimeField.EllipticCurve;
                    var q = curve.n;

                    CalculateK:
                    // k from [1..q - 1]
                    var k = BigInteger.Random(BigInteger.One, q, _ecdsa.Parameters.Random);

                    var point = k * curve.g;
                    var r = point.x % q;
                    if (r.IsZero)
                        goto CalculateK;

                    var kInv = BigInteger.ModularInverse(k, q);
                    var s = kInv * (data + _ecdsa.PrivateKey.BigInteger * r) % q;
                    if (s.IsZero)
                        goto CalculateK;

                    return new ECDSASignature { r = r, s = s };
                }
                if (_ecdsa.PublicKey.BFECPoint != null)
                {

                }

                throw new InvalidOperationException();
            }

            public override BigInteger Verify(ECDSASignature signature)
            {
                if (_ecdsa.PublicKey.PFECPoint != null)
                {
                    var curve = _ecdsa.Parameters.Curve as Math.EllipticCurves.PrimeField.EllipticCurve;
                    var r = signature.r;
                    var s = signature.s;
                    var q = curve.n;
                    var h = new BigInteger(hash.Take((curve.BitSize + 7) / 8).ToArray(), ByteOrder.BigEndian);

                    var u1 = BigInteger.ModularInverse(s, q) * h % q;
                    var u2 = BigInteger.ModularInverse(s, q) * r % q;

                    var point = u1 * curve.g + u2 * _ecdsa.PublicKey.PFECPoint;
                    var v = point.x % q;

                    return v == r;
                }
                if (_ecdsa.PublicKey.BFECPoint != null)
                {

                }

                throw new InvalidOperationException();
            }

            //public override ECDSASignature SignHash(byte[] hash)
            //{
            //    if (_ecdsa.PublicKey.PFECPoint != null)
            //    {
            //        var curve = _ecdsa.Parameters.Curve as Math.EllipticCurves.PrimeField.EllipticCurve;
            //        var q = curve.n;
            //        var h = new BigInteger(hash, ByteOrder.BigEndian);

            //        CalculateK:
            //        // k from [1..q - 1]
            //        var k = BigInteger.Random(BigInteger.One, q, _ecdsa.Parameters.Random);

            //        var point = k * curve.g;
            //        var r = point.x % q;
            //        if (r.IsZero)
            //            goto CalculateK;

            //        var kInv = BigInteger.ModularInverse(k, q);
            //        var s = kInv * (h + _ecdsa.PrivateKey.BigInteger * r) % q;
            //        if (s.IsZero)
            //            goto CalculateK;

            //        return new ECDSASignature { r = r, s = s };
            //    }
            //    if (_ecdsa.PublicKey.BFECPoint != null)
            //    {

            //    }

            //    throw new InvalidOperationException();
            //}

            //public override bool VerifyHash(byte[] hash, ECDSASignature signature)
            //{
            //    if (_ecdsa.PublicKey.PFECPoint != null)
            //    {
            //        var curve = _ecdsa.Parameters.Curve as Math.EllipticCurves.PrimeField.EllipticCurve;
            //        var r = signature.r;
            //        var s = signature.s;
            //        var q = curve.n;
            //        var h = new BigInteger(hash.Take((curve.BitSize + 7) / 8).ToArray(), ByteOrder.BigEndian);

            //        var u1 = BigInteger.ModularInverse(s, q) * h % q;
            //        var u2 = BigInteger.ModularInverse(s, q) * r % q;

            //        var point = u1 * curve.g + u2 * _ecdsa.PublicKey.PFECPoint;
            //        var v = point.x % q;

            //        return v == r;
            //    }
            //    if (_ecdsa.PublicKey.BFECPoint != null)
            //    {

            //    }

            //    throw new InvalidOperationException();
            //}
        }
    }
}