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
            do
            {
                PrivateKey = new ECPPrivateKey(BigInteger.Random(curve.n, Random));
            } while (PrivateKey.Value.IsZero);
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
            var k = BigInteger.Random(q, Random);
            if (k.IsZero)
                goto CalculateK;

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

        public void SignHashWithRecovery(byte[] hash, out byte recId, out BigInteger r, out BigInteger s)
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
            var k = BigInteger.Random(q, Random);
            if (k.IsZero)
                goto CalculateK;

            var point = k * curve.g;
            r = point.x % q;
            if (r.IsZero)
                goto CalculateK;

            var kInv = BigInteger.ModularInverse(k, q);
            s = kInv * (h + PrivateKey.Value * r) % q;
            if (s.IsZero)
                goto CalculateK;

            if (Parameters.LowS)
            {
                var half = q >> 1;
                if (s > half)
                    s = q - s;
            }

            recId = CalculateRecId(r, s, h);
        }

        public ECPoint RecoverPublicKey(byte[] hash, byte recId, byte[] r, byte[] s)
        {
            return RecoverPublicKey(
                TruncateH(new BigInteger(hash, ByteOrder.BigEndian), hash.Length),
                recId,
                new BigInteger(r, ByteOrder.BigEndian),
                new BigInteger(s, ByteOrder.BigEndian));
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

        public bool VerifyHash(byte[] hash, byte[] r, byte[] s)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");
            if (PublicKey == null)
                throw new InvalidOperationException("Public key is required for verification");

            var h = new BigInteger(hash, ByteOrder.BigEndian);
            h = TruncateH(h, hash.Length);

            var curve = Parameters.Curve;
            var R = new BigInteger(r, ByteOrder.BigEndian);
            var S = new BigInteger(s, ByteOrder.BigEndian);
            var q = curve.n;

            var u1 = BigInteger.ModularInverse(S, q) * h % q;
            var u2 = BigInteger.ModularInverse(S, q) * R % q;

            var point = u1 * curve.g + u2 * PublicKey.Point;
            var v = point.x % q;

            return v == R;
        }

        private BigInteger TruncateH(BigInteger h, int length)
        {
            if (Parameters.Curve.n.BitSize < length * 8)
            {
                h = h >> (length * 8 - Parameters.Curve.n.BitSize);
            }
            return h;
        }

        private ECPoint RecoverPublicKey(BigInteger hash, byte recId, BigInteger r, BigInteger s)
        {
            if (recId >= 4)
                throw new ArgumentOutOfRangeException();

            var curve = Parameters.Curve;

            BigInteger x = r + (recId / 2) * curve.n;
            if (x >= curve.p)
                throw new InvalidOperationException();

            ECPoint R = new ECPoint();
            R.Curve = curve;
            R.x = x;
            R.CalculateY((recId & 0x01) == 1);

            if ((curve.n * R) != ECPoint.Infinity)
                throw new InvalidOperationException();

            BigInteger e = hash;
            BigInteger eInv = (curve.n - e) % curve.n;
            BigInteger rInv = BigInteger.ModularInverse(r, curve.n);
            BigInteger sr = rInv * s % curve.n;
            BigInteger er = rInv * eInv % curve.n;
            ECPoint Q = er * curve.g + sr * R;

            return Q;
        }

        private byte CalculateRecId(BigInteger r, BigInteger s, BigInteger h)
        {
            var publicKey = GetPublicKeyPoint();
            for (byte i = 0; i < 4; i++)
            {
                var recPoint = RecoverPublicKey(h, i, r, s);
                if (recPoint == publicKey)
                    return i;
            }

            throw new InvalidOperationException();
        }

        private ECPoint GetPublicKeyPoint()
        {
            if (PublicKey == null)
            {
                if (PrivateKey != null)
                    PublicKey = new ECPPublicKey(PrivateKey.Value * Parameters.Curve.g);
                else
                    return null;
            }

            return PublicKey.Point;
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