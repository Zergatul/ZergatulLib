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

        private PrivateKeyInfo _keyInfo;

        internal PrivateKey(PrivateKeyInfo keyInfo)
        {
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
                return rsa;
            }
            else
                throw new NotImplementedException();
        }
    }
}