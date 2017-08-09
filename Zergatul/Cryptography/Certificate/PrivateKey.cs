using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Network;
using Zergatul.Network.ASN1.Structures;

namespace Zergatul.Cryptography.Certificate
{
    public class PrivateKey
    {
        public OID Algorithm { get; private set; }

        private X509Certificate _cert;
        private PrivateKeyInfo _keyInfo;

        internal PrivateKey(X509Certificate cert, PrivateKeyInfo keyInfo)
        {
            this._cert = cert;
            this._keyInfo = keyInfo;
            Algorithm = keyInfo.Algorithm.Algorithm;
        }

        public AbstractAsymmetricAlgorithm ResolveAlgorithm()
        {
            if (_keyInfo.RSA != null)
            {
                var rsa = new RSA();
                rsa.PrivateKey = new Asymmetric.RSAPrivateKey
                {
                    n = _keyInfo.RSA.Modulus,
                    d = _keyInfo.RSA.PrivateExponent
                };
                rsa.PublicKey = new RSAPublicKey
                {
                    n = _keyInfo.RSA.Modulus,
                    e = _keyInfo.RSA.PublicExponent
                };
                rsa.Parameters = new RSAParameters { BitSize = rsa.PublicKey.n.BitSize };
                return rsa;
            }
            else if (_keyInfo.EC != null)
            {
                if (_cert.PublicKey == null)
                    throw new InvalidOperationException();

                var curve = _cert.PublicKey.ResolveCurve();
                if (curve == null)
                    throw new InvalidOperationException();

                var ecdsa = new ECDSA();
                ecdsa.Parameters = new ECDSAParameters { Curve = curve };
                // TODO: move random to AbstractAsymmetricAlgorithm
                ecdsa.Parameters.Random = new DefaultSecureRandom();

                if (curve is Math.EllipticCurves.PrimeField.EllipticCurve)
                {
                    ecdsa.PrivateKey = new Asymmetric.ECPrivateKey
                    {
                        BigInteger = new Math.BigInteger(_keyInfo.EC.PrivateKey, ByteOrder.BigEndian)
                    };
                    if (_keyInfo.EC.PublicKey != null)
                        ecdsa.PublicKey = ECPointGeneric.Parse(_keyInfo.EC.PublicKey, curve);
                    else
                        ecdsa.PublicKey = ECPointGeneric.Parse(_cert.PublicKey.Key, curve);
                }
                else
                    throw new NotImplementedException();

                return ecdsa;
            }
            else
                throw new NotImplementedException();
        }
    }
}