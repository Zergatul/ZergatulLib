using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Network.ASN1;
using Zergatul.Network.ASN1.Structures;

namespace Zergatul.Cryptography.Asymmetric
{
    // https://tools.ietf.org/html/rfc8017
    public class RSA : AbstractAsymmetricAlgorithm<RSAParameters, RSAPublicKey, RSAPrivateKey, NullParam, BigInteger, BigInteger>
    {
        public override RSAPrivateKey PrivateKey { get; set; }
        public override RSAPublicKey PublicKey { get; set; }
        public override RSAParameters Parameters { get; set; }

        public override int KeySize => PublicKey.n.BitSize;

        private RSASignature _signature;
        private RSAEncryption _encryption;

        public override void GenerateKeys(ISecureRandom random)
        {
            throw new NotImplementedException();
        }

        public override AbstractKeyExchangeAlgorithm<RSAPublicKey, NullParam> KeyExchange
        {
            get
            {
                throw new NotSupportedException("RSA doesn't support key exchange");
            }
        }

        public override AbstractAsymmetricEncryption Encryption
        {
            get
            {
                if (_encryption == null)
                    _encryption = new RSAEncryption(this);
                return _encryption;
            }
        }

        public override AbstractSignatureAlgorithm<BigInteger, BigInteger> Signature
        {
            get
            {
                if (_signature == null)
                {
                    if (PublicKey == null)
                        throw new InvalidOperationException("PublicKey is null");
                    _signature = new RSASignature(this);
                }
                return _signature;
            }
        }

        private class RSASignature : AbstractSignatureAlgorithm<BigInteger, BigInteger>
        {
            private RSA _rsa;

            public RSASignature(RSA rsa)
            {
                this._rsa = rsa;
            }

            public override BigInteger Sign(BigInteger data)
            {
                if (_rsa.PrivateKey == null)
                    throw new InvalidOperationException("PrivateKey is required for signing");

                if (data >= _rsa.PrivateKey.n)
                    throw new InvalidOperationException("Data too large for signing");

                return BigInteger.ModularExponentiation(data, _rsa.PrivateKey.d, _rsa.PrivateKey.n);
            }

            public override BigInteger Verify(BigInteger signature)
            {
                return BigInteger.ModularExponentiation(signature, _rsa.PublicKey.e, _rsa.PublicKey.n);
            }
        }

        // TODO: https://tools.ietf.org/html/rfc2313
        private class RSAEncryption : AbstractAsymmetricEncryption
        {
            private RSA _rsa;

            public RSAEncryption(RSA rsa)
            {
                this._rsa = rsa;
            }

            public override byte[] Encrypt(byte[] data)
            {
                if (_rsa.PublicKey == null)
                    throw new InvalidOperationException("Public key is null");
                BigInteger m = new BigInteger(data, ByteOrder.BigEndian);
                if (m >= _rsa.PublicKey.n)
                    throw new InvalidOperationException("Data too large for encryption");
                BigInteger c = BigInteger.ModularExponentiation(m, _rsa.PublicKey.e, _rsa.PublicKey.n);
                return c.ToBytes(ByteOrder.BigEndian);
            }

            public override byte[] Decrypt(byte[] data)
            {
                if (_rsa.PrivateKey == null)
                    throw new InvalidOperationException("Private key is null");
                BigInteger c = new BigInteger(data, ByteOrder.BigEndian);
                if (c >= _rsa.PrivateKey.n)
                    throw new InvalidOperationException("Ciphertext too large for decryption");
                BigInteger m = BigInteger.ModularExponentiation(c, _rsa.PrivateKey.d, _rsa.PrivateKey.n);
                return m.ToBytes(ByteOrder.BigEndian);
            }
        }
    }
}