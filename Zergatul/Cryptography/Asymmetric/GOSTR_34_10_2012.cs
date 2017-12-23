using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// GOST R 34.10-2012
    /// https://tools.ietf.org/html/rfc7091
    /// </summary>
    public class GOSTR_34_10_2012 : AbstractSignature<ECPPrivateKey, ECPPublicKey, ECPDSAParameters>
    {
        public override void GenerateKeyPair(int keySize)
        {
            var curve = Parameters.Curve;
            PrivateKey = new ECPPrivateKey(BigInteger.Random(BigInteger.One, curve.n, Random));
            GeneratePublicKey();
        }

        public void GeneratePublicKey()
        {
            PublicKey = new ECPPublicKey(PrivateKey.Value * Parameters.Curve.g);
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

            var curve = Parameters.Curve;
            var q = curve.n;
            var d = PrivateKey.Value;

            var e = h % q;
            if (e.IsZero)
                e = BigInteger.One;

            CalculateK:
            // k from [1..q - 1]
            var k = BigInteger.Random(BigInteger.One, q, Random);

            var point = k * curve.g;
            var r = point.x % q;
            if (r.IsZero)
                goto CalculateK;

            var s = (r * d + k * e) % q;
            if (s.IsZero)
                goto CalculateK;

            int length = (curve.BitSize + 7) / 8;
            return ByteArray.Concat(r.ToBytes(ByteOrder.BigEndian, length), s.ToBytes(ByteOrder.BigEndian, length));
        }

        public override bool Verify(byte[] data, byte[] signature)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");
            if (PublicKey == null)
                throw new InvalidOperationException("Public key is required for verification");
            if (Parameters?.Hash == null)
                throw new InvalidOperationException("Parameters.Hash is null");
            if (signature.Length % 2 != 0)
                throw new InvalidOperationException();

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

            BigInteger h = new BigInteger(hash, ByteOrder.BigEndian);

            var curve = Parameters.Curve;
            var q = curve.n;
            var r = new BigInteger(signature, 0, signature.Length / 2, ByteOrder.BigEndian);
            var s = new BigInteger(signature, signature.Length / 2, signature.Length / 2, ByteOrder.BigEndian);
            var Q = PublicKey.Point;

            if (r < 0 || s < 0)
                return false;

            if (r >= q || s >= q)
                return false;

            var e = h % q;
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

        #region Converters

        public override AbstractSignature ToSignature()
        {
            return this;
        }

        public override AbstractKeyExchange ToKeyExchange()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}