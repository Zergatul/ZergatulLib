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
    public class EdDSA : AbstractSignature<EdDSAPrivateKey, EdDSAPublicKey, EdDSAParameters>
    {
        public override void GenerateKeyPair(int keySize)
        {
            throw new NotImplementedException();
        }

        public void GeneratePublicKey()
        {
            if (PrivateKey == null)
                throw new InvalidOperationException();

            var tuple = SecretExpand(PrivateKey.Value);
            BigInteger a = tuple.Item1;
            PublicKey = new EdDSAPublicKey(PointCompress(a * Parameters.Curve.G));
        }

        public override byte[] Sign(byte[] data)
        {
            var tuple = SecretExpand(PrivateKey.Value);
            BigInteger a = tuple.Item1;
            byte[] prefix = tuple.Item2;

            byte[] A = PublicKey.Value;
            BigInteger r = SHA512modQ(Parameters.Dom(), prefix, Parameters.PH(data));
            EdPoint R = r * Parameters.Curve.G;
            byte[] Rs = PointCompress(R);
            BigInteger h = SHA512modQ(Parameters.Dom(), Rs, A, Parameters.PH(data));
            BigInteger s = (r + h * a) % Parameters.Curve.q;

            return ByteArray.Concat(Rs, s.ToBytes(ByteOrder.LittleEndian, 32));
        }

        public override byte[] SignHash(byte[] hash)
        {
            throw new NotSupportedException();
        }

        public override bool Verify(byte[] data, byte[] signature)
        {
            if (PublicKey == null)
                throw new InvalidOperationException();
            if (PublicKey.Value == null)
                throw new InvalidOperationException();
            if (PublicKey.Value.Length != 32)
                throw new InvalidOperationException();

            if (signature.Length != 64)
                throw new InvalidOperationException("Invalid signature length");

            EdPoint A = PointDecompress(PublicKey.Value);
            if (A == null)
                return false;

            byte[] Rs = ByteArray.SubArray(signature, 0, 32);
            EdPoint R = PointDecompress(Rs);
            if (R == null)
                return false;

            BigInteger q = Parameters.Curve.q;
            EdPoint G = Parameters.Curve.G;

            BigInteger s = new BigInteger(signature, 32, 32, ByteOrder.LittleEndian);
            if (s >= q)
                return false;

            BigInteger h = SHA512modQ(Parameters.Dom(), Rs, PublicKey.Value, Parameters.PH(data));
            EdPoint sB = s * G;
            EdPoint hA = h * A;

            return sB == R + hA;
        }

        public override bool VerifyHash(byte[] hash, byte[] signature)
        {
            throw new NotSupportedException();
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

        private BigInteger SHA512modQ(byte[] data1, byte[] data2, byte[] data3 = null, byte[] data4 = null)
        {
            var sha512 = new SHA512();
            sha512.Update(data1);
            sha512.Update(data2);
            sha512.Update(data3);
            sha512.Update(data4);
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

        private EdPoint PointDecompress(byte[] data)
        {
            if (data.Length != 32)
                throw new InvalidOperationException();

            data = (byte[])data.Clone();
            bool sign = (data[31] & 0x80) != 0;
            data[31] &= 0x7F;

            BigInteger y = new BigInteger(data, ByteOrder.LittleEndian);
            BigInteger x = RecoverX(y, sign);

            if (x == null)
                return null;
            else
                return new EdPoint
                {
                    x = x,
                    y = y,
                    z = BigInteger.One,
                    t = x * y % Parameters.Curve.p,

                    Curve = Parameters.Curve
                };
        }

        private BigInteger RecoverX(BigInteger y, bool sign)
        {
            BigInteger p = Parameters.Curve.p;
            BigInteger d = Parameters.Curve.d;
            BigInteger sqrtMinusOne = Parameters.Curve.SqrtMinusOne;

            if (y >= p)
                return null;

            BigInteger x2 = (y * y - 1) * BigInteger.ModularInverse(d * y * y + 1, p);
            if (x2 == 0)
                if (sign)
                    return null;
                else
                    return BigInteger.Zero;

            BigInteger x = BigInteger.ModularExponentiation(x2, (p + 3) / 8, p);
            if ((x * x - x2) % p != 0)
                x = x * sqrtMinusOne % p;
            if ((x * x - x2) % p != 0)
                return null;

            if (x.IsBitSet(0) != sign)
                x = p - x;

            return x;
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

        public EdDSAPrivateKey(byte[] value)
        {
            this.Value = value;
        }

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

    public class EdDSAPublicKey : AbstractPublicKey
    {
        public byte[] Value { get; set; }

        public EdDSAPublicKey(byte[] value)
        {
            this.Value = value;
        }

        public override int KeySize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        

        public override AbstractEncryption ResolveEncryption()
        {
            throw new NotImplementedException();
        }

        public override AbstractSignature ResolveSignature()
        {
            throw new NotImplementedException();
        }

        public override AbstractKeyExchange ResolveKeyExchange()
        {
            throw new NotImplementedException();
        }
    }
}