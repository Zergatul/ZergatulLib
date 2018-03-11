using System;
using Zergatul.Math;
using Zergatul.Network.Asn1;
using Zergatul.Network.Asn1.Structures;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// ECDSA over Binary Field curves
    /// </summary>
    public class ECBDSA : AbstractSignature<ECBPrivateKey, ECBPublicKey, ECBDSAParameters>
    {
        public override void GenerateKeyPair(int keySize)
        {
            if (Parameters?.Curve == null)
                throw new InvalidOperationException("Parameters.Curve is null");

            var curve = Parameters.Curve;
            do
            {
                PrivateKey = new ECBPrivateKey(BinaryPolynomial.Random(curve.f.Degree - 1, Random));
            } while (PrivateKey.Value.IsZero);
            PublicKey = new ECBPublicKey(PrivateKey.Value * curve.g);
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
            var h = new BigInteger(digest, ByteOrder.BigEndian);

            var curve = Parameters.Curve;
            var q = curve.n;

            CalculateK:
            // k from [1..q - 1]
            var k = BinaryPolynomial.Random(curve.f.Degree - 1, Random);

            var point = k * curve.g;
            var r = point.x.ToBigInteger() % q;
            if (r.IsZero)
                goto CalculateK;

            var kInv = BigInteger.ModularInverse(k.ToBigInteger(), q);
            var s = kInv * (h + PrivateKey.Value.ToBigInteger() * r) % q;
            if (s.IsZero)
                goto CalculateK;

            return new ECDSASignatureValue(r, s).ToBytes();
        }

        public override byte[] SignHash(byte[] hash)
        {
            throw new NotImplementedException();
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
            BigInteger h = new BigInteger(digest, ByteOrder.BigEndian);

            var ed = ECDSASignatureValue.Parse(Asn1Element.ReadFrom(signature));

            var curve = Parameters.Curve;
            var r = ed.r;
            var s = ed.s;
            var q = curve.n;

            var u1 = BigInteger.ModularInverse(s, q) * h % q;
            var u2 = BigInteger.ModularInverse(s, q) * r % q;

            var point = new BinaryPolynomial(u1) * curve.g + new BinaryPolynomial(u2) * PublicKey.Point;
            var v = point.x.ToBigInteger() % q;

            return v == r;
        }

        public override bool VerifyHash(byte[] hash, byte[] signature)
        {
            throw new NotImplementedException();
        }

        #region Converters

        public override AbstractSignature ToSignature()
        {
            return this;
        }

        public override AbstractKeyExchange ToKeyExchange()
        {
            throw new NotImplementedException();
            /*return new ECBDiffieHellman
            {
                Parameters = new ECPParameters(Parameters.Curve),
                Random = Random,
                PrivateKey = PrivateKey,
                PublicKey = PublicKey
            };*/
        }

        #endregion
    }
}