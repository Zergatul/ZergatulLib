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
    public class ECDSA : AbstractAsymmetricAlgorithm<IEllipticCurve, ECPointGeneric, ECPrivateKey, ECPointGeneric>
    {
        public override ECPrivateKey PrivateKey { get; set; }
        public override ECPointGeneric PublicKey { get; set; }
        public override IEllipticCurve Parameters { get; set; }

        public override int KeySize => Parameters.BitSize;

        private ECDSASignature _signature;

        public override void GenerateKeys(ISecureRandom random)
        {
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
                throw new NotImplementedException();
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
                    var curve = _ecdsa.Parameters as Math.EllipticCurves.PrimeField.EllipticCurve;
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