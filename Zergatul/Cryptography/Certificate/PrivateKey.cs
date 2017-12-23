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
                var rsa = new RSAEncryption();
                rsa.PrivateKey = new Asymmetric.RSAPrivateKey(_keyInfo.RSA.Modulus, _keyInfo.RSA.PrivateExponent);
                rsa.PublicKey = new Asymmetric.RSAPublicKey(_keyInfo.RSA.Modulus, _keyInfo.RSA.PublicExponent);
                return rsa;
            }
            else if (_keyInfo.EC != null)
            {
                if (_cert.PublicKey == null)
                    throw new InvalidOperationException();

                var curve = _cert.PublicKey.ResolveCurve();
                if (curve == null)
                    throw new InvalidOperationException();

                var ecdsa = new ECPDSA();
                ecdsa.Parameters = new ECPDSAParameters((Math.EllipticCurves.PrimeField.EllipticCurve)curve);

                if (curve is Math.EllipticCurves.PrimeField.EllipticCurve)
                {
                    ecdsa.PrivateKey = new Asymmetric.ECPPrivateKey(_keyInfo.EC.PrivateKey);
                    if (_keyInfo.EC.PublicKey != null)
                        ecdsa.PublicKey = new ECPPublicKey(ECPointGeneric.Parse(_keyInfo.EC.PublicKey, curve).PFECPoint);
                    else
                        ecdsa.PublicKey = new ECPPublicKey(ECPointGeneric.Parse(_cert.PublicKey.Key, curve).PFECPoint);
                }
                else
                    throw new NotImplementedException();

                return ecdsa;
            }
            else if (_keyInfo.DSA != null)
            {
                var dsa = (DSA)_cert.PublicKey.ResolveAlgorithm();
                dsa.PrivateKey = new Asymmetric.DSAPrivateKey(_keyInfo.DSA.x);
                return dsa;
            }
            else if (_keyInfo.DH != null)
            {
                var dh = (DiffieHellman)_cert.PublicKey.ResolveAlgorithm();
                dh.PrivateKey = new DiffieHellmanPrivateKey(_keyInfo.DH);
                return dh;
            }
            else
                throw new NotImplementedException();
        }
    }
}