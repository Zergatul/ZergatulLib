using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures;

namespace Zergatul.Cryptography.Asymmetric
{
    public class ECDSA : AbstractAsymmetricAlgorithm<IEllipticCurve, ECPointGeneric, ECPrivateKey, NullParam, BigInteger, ECDSASignature>
    {
        public override ECPrivateKey PrivateKey { get; set; }
        public override ECPointGeneric PublicKey { get; set; }
        public override IEllipticCurve Parameters { get; set; }

        public override int KeySize => Parameters.BitSize;

        private ECDSASignatureImpl _signature;

        public override void GenerateKeys()
        {
            if (Parameters is Math.EllipticCurves.PrimeField.EllipticCurve)
            {
                var curve = (Math.EllipticCurves.PrimeField.EllipticCurve)Parameters;
                PrivateKey = new ECPrivateKey
                {
                    BigInteger = BigInteger.Random(BigInteger.One, curve.n, Random)
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

        public ECDiffieHellman ToECDH()
        {
            return new ECDiffieHellman
            {
                Parameters = this.Parameters,
                PrivateKey = this.PrivateKey,
                PublicKey = this.PublicKey,
                Random = this.Random
            };
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
                    var curve = _ecdsa.Parameters as Math.EllipticCurves.PrimeField.EllipticCurve;
                    var q = curve.n;

                    CalculateK:
                    // k from [1..q - 1]
                    var k = BigInteger.Random(BigInteger.One, q, _ecdsa.Random);

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

            public override bool Verify(ECDSASignature signature, BigInteger data)
            {
                if (_ecdsa.PublicKey.PFECPoint != null)
                {
                    var curve = _ecdsa.Parameters as Math.EllipticCurves.PrimeField.EllipticCurve;
                    var r = signature.r;
                    var s = signature.s;
                    var q = curve.n;
                    var h = data;

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


            public override SignatureScheme GetScheme(string name)
            {
                switch (name)
                {
                    case "Default":
                        return new ECDSADefaultScheme(this);
                    default:
                        throw new InvalidOperationException("Unknown scheme");
                }
            }
        }

        private class ECDSADefaultScheme : SignatureScheme
        {
            private ECDSASignatureImpl _ecdsa;

            public ECDSADefaultScheme(ECDSASignatureImpl ecdsa)
            {
                this._ecdsa = ecdsa;
            }

            public override void SetParameter(object parameter)
            {
                
            }

            public override byte[] Sign(byte[] data)
            {
                var value = new BigInteger(data, ByteOrder.BigEndian);
                var signature = _ecdsa.Sign(value);
                return new ECDSASignatureValue(signature.r, signature.s).ToBytes();
            }

            public override bool Verify(byte[] signature, byte[] data)
            {
                var ed = ECDSASignatureValue.Parse(ASN1Element.ReadFrom(signature));
                var value = new BigInteger(data, ByteOrder.BigEndian);
                return _ecdsa.Verify(new ECDSASignature { r = ed.r, s = ed.s }, value);
            }
        }
    }
}