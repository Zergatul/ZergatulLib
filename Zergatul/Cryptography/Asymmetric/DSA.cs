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
    // Deterministic Usage of the Digital Signature Algorithm (DSA) and
    // Elliptic Curve Digital Signature Algorithm (ECDSA)
    // https://tools.ietf.org/html/rfc6979
    public class DSA : SignatureOnlyAssymetricAlgorithm<DSAParameters, BigInteger, BigInteger, BigInteger, DSASignature>
    {
        public override BigInteger PrivateKey { get; set; }
        public override BigInteger PublicKey { get; set; }
        public override DSAParameters Parameters { get; set; }

        public override int KeySize => Parameters.g.BitSize;

        private DSASignatureImpl _signature;

        public override void GenerateKeys()
        {
            PrivateKey = BigInteger.Random(BigInteger.One, Parameters.q, Random);
            PublicKey = BigInteger.ModularExponentiation(Parameters.g, PrivateKey, Parameters.p);
        }

        public override AbstractSignatureAlgorithm<BigInteger, DSASignature> Signature
        {
            get
            {
                if (_signature == null)
                {
                    if (PublicKey == null)
                        throw new InvalidOperationException("PublicKey is null");
                    _signature = new DSASignatureImpl(this);
                }
                return _signature;
            }
        }

        private class DSASignatureImpl : AbstractSignatureAlgorithm<BigInteger, DSASignature>
        {
            private DSA _dsa;

            public DSASignatureImpl(DSA dsa)
            {
                this._dsa = dsa;
            }

            public override DSASignature Sign(BigInteger data)
            {
                if (_dsa.PrivateKey == null)
                    throw new InvalidOperationException("PrivateKey is null");

                BigInteger p = _dsa.Parameters.p;
                BigInteger q = _dsa.Parameters.q;
                BigInteger g = _dsa.Parameters.g;

                CalculateK:
                BigInteger k = BigInteger.Random(BigInteger.One, q, _dsa.Random);
                BigInteger r = BigInteger.ModularExponentiation(g, k, p) % q;
                if (r == 0)
                    goto CalculateK;

                BigInteger s = BigInteger.ModularInverse(k, q) * (data + _dsa.PrivateKey * r) % q;
                if (s == 0)
                    goto CalculateK;

                return new DSASignature { r = r, s = s };
            }

            public override bool Verify(DSASignature signature, BigInteger data)
            {
                if (_dsa.PublicKey == null)
                    throw new InvalidOperationException("PublicKey is null");

                BigInteger p = _dsa.Parameters.p;
                BigInteger q = _dsa.Parameters.q;
                BigInteger g = _dsa.Parameters.g;

                BigInteger r = signature.r;
                BigInteger s = signature.s;

                BigInteger w = BigInteger.ModularInverse(s, q);
                BigInteger u1 = data * w % q;
                BigInteger u2 = r * w % q;
                BigInteger v = BigInteger.ModularExponentiation(g, u1, p) * BigInteger.ModularExponentiation(_dsa.PublicKey, u2, p) % p % q;

                return v == r;
            }

            public override SignatureScheme GetScheme(string name)
            {
                switch (name)
                {
                    case "Default":
                        return new DSSScheme(_dsa);
                    default:
                        throw new InvalidOperationException("Unknown scheme");
                }
            }
        }

        private class DSSScheme : SignatureScheme
        {
            private DSA _dsa;
            private int _length => (_dsa.Parameters.q.BitSize + 7) / 8;

            public DSSScheme(DSA dsa)
            {
                this._dsa = dsa;
            }

            public override void SetParameter(object parameter)
            {
                throw new InvalidOperationException();
            }

            public override byte[] Sign(byte[] data)
            {
                BigInteger value = new BigInteger(data.Take(_length).ToArray(), ByteOrder.BigEndian);
                var signature = _dsa.Signature.Sign(value);
                return new ECDSASignatureValue(signature.r, signature.s).ToBytes();
            }

            public override bool Verify(byte[] signature, byte[] data)
            {
                var ed = ECDSASignatureValue.Parse(ASN1Element.ReadFrom(signature));
                var value = new BigInteger(data.Take(_length).ToArray(), ByteOrder.BigEndian);
                return _dsa.Signature.Verify(new DSASignature { r = ed.r, s = ed.s }, value);
            }
        }
    }
}