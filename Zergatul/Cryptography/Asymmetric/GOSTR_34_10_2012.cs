using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// GOST R 34.10-2012
    /// https://tools.ietf.org/html/rfc7091
    /// </summary>
    public class GOSTR_34_10_2012 : AbstractAsymmetricAlgorithm<EllipticCurve, ECPoint, BigInteger, NullParam, BigInteger, GOSTRSignature>
    {
        public override BigInteger PrivateKey { get; set; }
        public override ECPoint PublicKey { get; set; }
        public override EllipticCurve Parameters { get; set; }

        public override int KeySize => Parameters.BitSize;

        private GOSTRSignatureImpl _signature;

        public override void GenerateKeys()
        {
            PrivateKey = BigInteger.Random(BigInteger.One, Parameters.n, Random);
            GeneratePublicKey();
        }

        public void GeneratePublicKey()
        {
            PublicKey = PrivateKey * Parameters.g;
        }

        public override AbstractAsymmetricEncryption Encryption
        {
            get
            {
                throw new NotSupportedException("GOST R 34.10-2012 doesn't support encryption");
            }
        }

        public override AbstractKeyExchangeAlgorithm<ECPoint, NullParam> KeyExchange
        {
            get
            {
                throw new NotSupportedException("GOST R 34.10-2012 doesn't key exchange");
            }
        }

        public override AbstractSignatureAlgorithm<BigInteger, GOSTRSignature> Signature
        {
            get
            {
                if (_signature == null)
                {
                    if (PublicKey == null)
                        throw new InvalidOperationException("PublicKey is null");
                    _signature = new GOSTRSignatureImpl(this);
                }
                return _signature;
            }
        }

        private class GOSTRSignatureImpl : AbstractSignatureAlgorithm<BigInteger, GOSTRSignature>
        {
            private GOSTR_34_10_2012 _gost;

            public GOSTRSignatureImpl(GOSTR_34_10_2012 gost)
            {
                this._gost = gost;
            }

            public override GOSTRSignature Sign(BigInteger data)
            {
                var curve = _gost.Parameters;
                var q = curve.n;
                var d = _gost.PrivateKey;

                var e = data % q;
                if (e.IsZero)
                    e = BigInteger.One;

                CalculateK:
                // k from [1..q - 1]
                var k = BigInteger.Random(BigInteger.One, q, _gost.Random);

                var point = k * curve.g;
                var r = point.x % q;
                if (r.IsZero)
                    goto CalculateK;

                var s = (r * d + k * e) % q;
                if (s.IsZero)
                    goto CalculateK;

                return new GOSTRSignature { r = r, s = s };
            }

            public override bool Verify(GOSTRSignature signature, BigInteger data)
            {
                var curve = _gost.Parameters;
                var q = curve.n;
                var r = signature.r;
                var s = signature.s;
                var Q = _gost.PublicKey;

                if (r < 0 || s < 0)
                    return false;

                if (r >= q || s >= q)
                    return false;

                var e = data % q;
                if (e.IsZero)
                    e = BigInteger.One;

                var v = BigInteger.ModularInverse(e, q);
                var z1 = s * v % q;
                var z2 = r * v % q;
                if (!z2.IsZero)
                    z2 = q - z2;
                var point = z1 * curve.g + z2 * Q;

                var R = point.x % q;

                return R == r;
            }


            public override SignatureScheme GetScheme(string name)
            {
                switch (name)
                {
                    case "Default":
                        return new GOSTRDefaultScheme(this);
                    default:
                        throw new InvalidOperationException("Unknown scheme");
                }
            }
        }

        private class GOSTRDefaultScheme : SignatureScheme
        {
            private GOSTRSignatureImpl _gost;

            public GOSTRDefaultScheme(GOSTRSignatureImpl gost)
            {
                this._gost = gost;
            }

            public override void SetParameter(object parameter)
            {
                
            }

            public override byte[] Sign(byte[] data)
            {
                var value = new BigInteger(data, ByteOrder.BigEndian);
                var signature = _gost.Sign(value);
                int length = (System.Math.Max(signature.r.BitSize, signature.s.BitSize) + 7) % 8;
                return ByteArray.Concat(signature.r.ToBytes(ByteOrder.BigEndian, length), signature.s.ToBytes(ByteOrder.BigEndian, length));
            }

            public override bool Verify(byte[] signature, byte[] data)
            {
                if (signature.Length % 2 == 1)
                    throw new InvalidOperationException();

                int length = signature.Length / 2;
                var r = new BigInteger(signature, 0, length, ByteOrder.BigEndian);
                var s = new BigInteger(signature, length, length, ByteOrder.BigEndian);

                return _gost.Verify(new GOSTRSignature
                {
                    r = r,
                    s = s,
                }, new BigInteger(data, ByteOrder.BigEndian));
            }
        }
    }
}