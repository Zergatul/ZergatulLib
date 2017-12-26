using System;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Math.EdwardsCurves;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// Edwards-Curve Digital Signature Algorithm
    /// <para>https://tools.ietf.org/html/rfc8032</para>
    /// </summary>
    public class EdDSA : AbstractSignature<EdDSAPrivateKey, ECPPublicKey, EdDSAParameters>
    {
        public override void GenerateKeyPair(int keySize)
        {
            throw new NotImplementedException();
        }

        public override byte[] Sign(byte[] data)
        {
            var tuple = SecretExpand(PrivateKey.Value);
            BigInteger a = tuple.Item1;
            byte[] prefix = tuple.Item2;

            byte[] A = PointCompress(a * Parameters.Curve.G);
            BigInteger r = SHA512modQ(ByteArray.Concat(prefix, data));
            EdPoint R = r * Parameters.Curve.G;
            byte[] Rs = PointCompress(R);
            BigInteger h = SHA512modQ(ByteArray.Concat(Rs, A, data));
            BigInteger s = (r + h * a) % Parameters.Curve.q;

            return ByteArray.Concat(Rs, s.ToBytes(ByteOrder.LittleEndian, 32));
        }

        public override byte[] SignHash(byte[] hash)
        {
            throw new NotImplementedException();
        }

        public override bool Verify(byte[] data, byte[] signature)
        {
            throw new NotImplementedException();
        }

        public override bool VerifyHash(byte[] hash, byte[] signature)
        {
            throw new NotImplementedException();
        }

        private Tuple<BigInteger, byte[]> SecretExpand(byte[] secret)
        {
            if (secret.Length != 32)
                throw new ArgumentException();

            var sha512 = new SHA512();
            sha512.Update(secret);
            byte[] h = sha512.ComputeHash();

            h[0] &= 0xF8;
            h[31] &= 0x3F;
            h[31] |= 0x40;
            BigInteger a = new BigInteger(h, 0, 32, ByteOrder.LittleEndian);

            return new Tuple<BigInteger, byte[]>(a, ByteArray.SubArray(h, 32, 32));
        }

        private BigInteger SHA512modQ(byte[] data)
        {
            var sha512 = new SHA512();
            sha512.Update(data);
            byte[] h = sha512.ComputeHash();

            return new BigInteger(h, ByteOrder.LittleEndian) % Parameters.Curve.q;
        }

        private byte[] PointCompress(EdPoint point)
        {
            BigInteger zInv = BigInteger.ModularInverse(point.z, Parameters.Curve.p);
            BigInteger x = point.x * zInv % Parameters.Curve.p;
            BigInteger y = point.y * zInv % Parameters.Curve.p;

            byte[] data = y.ToBytes(ByteOrder.LittleEndian, 32);
            if (x.IsOdd)
                data[31] |= 0x80;

            return data;
        }

        #region Converters

        public override AbstractKeyExchange ToKeyExchange()
        {
            throw new NotSupportedException();
        }

        public override AbstractSignature ToSignature()
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public class EdDSAPrivateKey : AbstractPrivateKey
    {
        public override int KeySize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public byte[] Value { get; set; }

        public override AbstractEncryption ResolveEncryption()
        {
            throw new NotImplementedException();
        }

        public override AbstractKeyExchange ResolveKeyExchange()
        {
            throw new NotImplementedException();
        }

        public override AbstractSignature ResolveSignature()
        {
            throw new NotImplementedException();
        }
    }
}