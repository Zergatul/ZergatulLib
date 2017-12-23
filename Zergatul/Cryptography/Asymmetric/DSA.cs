using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// Digital Signature Algorithm
    /// <para>https://tools.ietf.org/html/rfc6979</para>
    /// </summary>
    public class DSA : AbstractSignature<DSAPrivateKey, DSAPublicKey, DSAParameters>
    {
        public override void GenerateKeyPair(int keySize)
        {
            PrivateKey = new DSAPrivateKey(BigInteger.Random(BigInteger.One, Parameters.q, Random));
            PublicKey = new DSAPublicKey(BigInteger.ModularExponentiation(Parameters.g, PrivateKey.Value, Parameters.p));
        }

        public override byte[] Sign(byte[] data)
        {
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
            if (PrivateKey == null)
                throw new InvalidOperationException("Private key is required for signing");

            BigInteger h = new BigInteger(hash, 0, System.Math.Min(hash.Length, Parameters.q.BitSize / 8), ByteOrder.BigEndian);

            BigInteger p = Parameters.p;
            BigInteger q = Parameters.q;
            BigInteger g = Parameters.g;

            CalculateK:
            BigInteger k = BigInteger.Random(BigInteger.One, q, Random);
            BigInteger r = BigInteger.ModularExponentiation(g, k, p) % q;
            if (r == 0)
                goto CalculateK;

            BigInteger s = BigInteger.ModularInverse(k, q) * (h + PrivateKey.Value * r) % q;
            if (s == 0)
                goto CalculateK;

            return new ECDSASignatureValue(r, s).ToBytes();
        }

        public override bool Verify(byte[] data, byte[] signature)
        {
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
            if (PublicKey == null)
                throw new InvalidOperationException("Public key is required for verification");

            BigInteger h = new BigInteger(hash, 0, System.Math.Min(hash.Length, Parameters.q.BitSize / 8), ByteOrder.BigEndian);

            var ed = ECDSASignatureValue.Parse(ASN1Element.ReadFrom(signature));

            BigInteger p = Parameters.p;
            BigInteger q = Parameters.q;
            BigInteger g = Parameters.g;

            BigInteger r = ed.r;
            BigInteger s = ed.s;

            BigInteger w = BigInteger.ModularInverse(s, q);
            BigInteger u1 = h * w % q;
            BigInteger u2 = r * w % q;
            BigInteger v = BigInteger.ModularExponentiation(g, u1, p) * BigInteger.ModularExponentiation(PublicKey.Value, u2, p) % p % q;

            return v == r;
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