using System;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;
using Zergatul.Network.Asn1;
using Zergatul.Network.Asn1.Structures;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// ECDSA over Prime Field curves
    /// </summary>
    public class ECPDSA : AbstractSignature<ECPPrivateKey, ECPPublicKey, ECPDSAParameters>
    {
        public override void GenerateKeyPair(int keySize)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");

            var curve = Parameters.Curve;
            PrivateKey = new ECPPrivateKey(BigInteger.Random(BigInteger.One, curve.n, Random));
            PublicKey = new ECPPublicKey(PrivateKey.Value * curve.g);
        }

        public override byte[] Sign(byte[] data)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");
            if (PrivateKey == null)
                throw new InvalidOperationException("Private key is required for signing");
            if (Parameters?.Hash == null)
                throw new InvalidOperationException("Parameters.Hash is null");

            Parameters.Hash.Reset();
            Parameters.Hash.Update(data);
            byte[] digest = Parameters.Hash.ComputeHash();

            return SignHash(digest);
        }

        public override byte[] SignHash(byte[] hash)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");
            if (PrivateKey == null)
                throw new InvalidOperationException("Private key is required for signing");

            BigInteger h = new BigInteger(hash, ByteOrder.BigEndian);
            h = TruncateH(h, hash.Length);

            var curve = Parameters.Curve;
            var q = curve.n;

            CalculateK:
            // k from [1..q - 1]
            var k = BigInteger.Random(BigInteger.One, q, Random);

            var point = k * curve.g;
            var r = point.x % q;
            if (r.IsZero)
                goto CalculateK;

            var kInv = BigInteger.ModularInverse(k, q);
            var s = kInv * (h + PrivateKey.Value * r) % q;
            if (s.IsZero)
                goto CalculateK;

            return new ECDSASignatureValue(r, s).ToBytes();
        }

        public Tuple<byte, BigInteger, BigInteger> SignHashBase(byte[] hash)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");
            if (PrivateKey == null)
                throw new InvalidOperationException("Private key is required for signing");

            BigInteger h = new BigInteger(hash, ByteOrder.BigEndian);
            h = TruncateH(h, hash.Length);

            var curve = Parameters.Curve;
            var q = curve.n;

            CalculateK:
            // k from [1..q - 1]
            var k = BigInteger.Random(BigInteger.One, q, Random);

            var point = k * curve.g;
            var r = point.x % q;
            if (r.IsZero)
                goto CalculateK;

            var kInv = BigInteger.ModularInverse(k, q);
            var s = kInv * (h + PrivateKey.Value * r) % q;
            if (s.IsZero)
                goto CalculateK;

            return new Tuple<byte, BigInteger, BigInteger>(0, r, s);
        }

        public byte[] RecoverPublicKey(byte[] hash, byte v, byte[] r, byte[] s)
        {
            var curve = Parameters.Curve;
            BigInteger ri = new BigInteger(r, ByteOrder.BigEndian);
            BigInteger si = new BigInteger(s, ByteOrder.BigEndian);
            for (int j = 0; j < curve.h; j++)
            {
                BigInteger x = ri + j * curve.n;
                byte prefix;
                switch (v)
                {
                    case 0x1B:
                        prefix = 0x02;
                        break;
                    case 0x1C:
                        prefix = 0x03;
                        break;
                    case 0x1D:
                        x += curve.n;
                        prefix = 0x02;
                        break;
                    case 0x1E:
                        x += curve.n;
                        prefix = 0x03;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                byte[] publicKey = ByteArray.Concat(new byte[] { 0x02 }, x.ToBytes(ByteOrder.BigEndian, (curve.BitSize + 7) / 8));
                ECPoint R = ECPoint.FromBytes(publicKey, curve);
                //var zero = curve.n * point;
                //check zero
                BigInteger e = TruncateH(new BigInteger(hash, ByteOrder.BigEndian), hash.Length);
                ECPoint q = BigInteger.ModularInverse(ri, curve.n) * (si * R - curve.GeneratorMultiplication(e));
                return publicKey;
            }

            throw new InvalidOperationException();
        }

        public override bool Verify(byte[] data, byte[] signature)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");
            if (PublicKey == null)
                throw new InvalidOperationException("Public key is required for verification");
            if (Parameters?.Hash == null)
                throw new InvalidOperationException("Parameters.Hash is null");

            Parameters.Hash.Reset();
            Parameters.Hash.Update(data);
            byte[] digest = Parameters.Hash.ComputeHash();

            return VerifyHash(digest, signature);
        }

        public override bool VerifyHash(byte[] hash, byte[] signature)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");
            if (PublicKey == null)
                throw new InvalidOperationException("Public key is required for verification");

            var h = new BigInteger(hash, ByteOrder.BigEndian);
            h = TruncateH(h, hash.Length);

            var ed = ECDSASignatureValue.Parse(Asn1Element.ReadFrom(signature));

            var curve = Parameters.Curve;
            var r = ed.r;
            var s = ed.s;
            var q = curve.n;

            var u1 = BigInteger.ModularInverse(s, q) * h % q;
            var u2 = BigInteger.ModularInverse(s, q) * r % q;

            var point = u1 * curve.g + u2 * PublicKey.Point;
            var v = point.x % q;

            return v == r;
        }

        private BigInteger TruncateH(BigInteger h, int length)
        {
            if (Parameters.Curve.n.BitSize < length * 8)
            {
                h = h >> (length * 8 - Parameters.Curve.n.BitSize);
            }
            return h;
        }

        #region Converters

        public override AbstractSignature ToSignature()
        {
            return this;
        }

        public override AbstractKeyExchange ToKeyExchange()
        {
            return new ECPDiffieHellman
            {
                Parameters = new ECPParameters(Parameters.Curve),
                Random = Random,
                PrivateKey = PrivateKey,
                PublicKey = PublicKey
            };
        }

        #endregion
    }
}