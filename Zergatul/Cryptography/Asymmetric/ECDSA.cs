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
    public class ECDSA : AbstractAsymmetricAlgorithm<ECDSAParameters, ECPointGeneric, ECPrivateKey, ECPointGeneric>
    {
        public override ECPrivateKey PrivateKey { get; set; }
        public override ECPointGeneric PublicKey { get; set; }
        public override ECDSAParameters Parameters { get; set; }

        public override int KeySize => Parameters.Curve.BitSize;

        private ECDSASignature _signature;

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

        public override AbstractKeyExchangeAlgorithm<ECPointGeneric, ECPointGeneric> KeyExchange
        {
            get
            {
                throw new NotSupportedException("ECDSA doesn't support key exchange");
            }
        }

        public override AbstractSignatureAlgorithm Signature
        {
            get
            {
                if (_signature == null)
                {
                    if (PublicKey == null)
                        throw new InvalidOperationException("PublicKey is null");
                    _signature = new ECDSASignature(this);
                }
                return _signature;
            }
        }

        private class ECDSASignature : AbstractSignatureAlgorithm
        {
            private ECDSA _ecdsa;

            public ECDSASignature(ECDSA ecdsa)
            {
                this._ecdsa = ecdsa;
            }

            public override byte[] SignHash(AbstractHash hashAlgorithm)
            {
                byte[] hb = hashAlgorithm.ComputeHash();

                if (_ecdsa.PublicKey.PFECPoint != null)
                {
                    var curve = _ecdsa.Parameters.Curve as Math.EllipticCurves.PrimeField.EllipticCurve;
                    var q = curve.n;
                    var h = new BigInteger(hb, ByteOrder.BigEndian);

                CalculateK:
                    // k from [1..q - 1]
                    var k = BigInteger.Random(BigInteger.One, q, _ecdsa.Parameters.Random);

                    var point = k * curve.g;
                    var r = point.x % q;
                    if (r.IsZero)
                        goto CalculateK;

                    var kInv = BigInteger.ModularInverse(k, q);
                    var s = kInv * (h + _ecdsa.PrivateKey.BigInteger * r) % q;
                    if (s.IsZero)
                        goto CalculateK;

                    int byteLen = (q.BitSize + 7) / 8;
                    byte[] signature = new byte[byteLen * 2];
                    Array.Copy(r.ToBytes(ByteOrder.BigEndian, byteLen), signature, byteLen);
                    Array.Copy(s.ToBytes(ByteOrder.BigEndian, byteLen), 0, signature, byteLen, byteLen);

                    return signature;
                }
                if (_ecdsa.PublicKey.BFECPoint != null)
                {

                }

                throw new InvalidOperationException();
            }

            public override byte[] SignHash(byte[] data)
            {
                throw new NotImplementedException();
            }

            public override bool VerifyHash(AbstractHash hashAlgorithm, byte[] signature)
            {
                byte[] hb = hashAlgorithm.ComputeHash();

                byte[] rb = new byte[signature.Length / 2];
                byte[] sb = new byte[signature.Length / 2];
                Array.Copy(signature, 0, rb, 0, rb.Length);
                Array.Copy(signature, rb.Length, sb, 0, sb.Length);

                if (_ecdsa.PublicKey.PFECPoint != null)
                {
                    var curve = _ecdsa.Parameters.Curve as Math.EllipticCurves.PrimeField.EllipticCurve;
                    var r = new BigInteger(rb, ByteOrder.BigEndian);
                    var s = new BigInteger(sb, ByteOrder.BigEndian);
                    var q = curve.n;
                    var h = new BigInteger(hb, ByteOrder.BigEndian);

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

            public override bool VerifyHash(byte[] data, byte[] signature)
            {
                throw new NotImplementedException();
            }
        }
    }
}