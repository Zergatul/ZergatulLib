using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Network;
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

        public override void GenerateKeys()
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

            public override bool Verify(BigInteger signature, BigInteger data)
            {
                return data == BigInteger.ModularExponentiation(signature, _rsa.PublicKey.e, _rsa.PublicKey.n);
            }

            public override SignatureScheme GetScheme(string name)
            {
                switch (name)
                {
                    case "EMSA-PKCS1-v1.5":
                        return new EMSAPKCS1v15Scheme(_rsa);
                    default:
                        throw new InvalidOperationException("Unknown scheme");
                }
            }
        }

        private class EMSAPKCS1v15Scheme : SignatureScheme
        {
            private RSA _rsa;
            private OID _hashAlgorithmOID;
            private int _length => (_rsa.KeySize + 7) / 8;

            public EMSAPKCS1v15Scheme(RSA rsa)
            {
                this._rsa = rsa;
            }

            public override void SetParameter(object parameter)
            {
                if (parameter is OID)
                    _hashAlgorithmOID = (OID)parameter;
                else
                    throw new InvalidOperationException("Parameter should be OID");
            }

            public override byte[] Sign(byte[] data)
            {
                if (_rsa.PrivateKey == null)
                    throw new InvalidOperationException("PrivateKey is required for signing");

                var ai = new AlgorithmIdentifier(_hashAlgorithmOID, new Null());
                var pkcs = new EMSA_PKCS1_v1_5(ai, data, _length);
                var value = new BigInteger(pkcs.ToBytes(), ByteOrder.BigEndian);
                var result = BigInteger.ModularExponentiation(value, _rsa.PrivateKey.d, _rsa.PrivateKey.n);

                return result.ToBytes(ByteOrder.BigEndian);
            }

            public override bool Verify(byte[] signature, byte[] data)
            {
                var signValue = new BigInteger(signature, ByteOrder.BigEndian);
                var value = BigInteger.ModularExponentiation(signValue, _rsa.PublicKey.e, _rsa.PublicKey.n);
                var pkcs = EMSA_PKCS1_v1_5.Parse(value.ToBytes(ByteOrder.BigEndian, _length));

                if (pkcs.DigestAlgorithm.Algorithm != _hashAlgorithmOID)
                    return false;

                return ByteArray.Equals(pkcs.Digest, data);
            }
        }

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

            public override EncryptionScheme GetScheme(string name)
            {
                switch (name)
                {
                    case "RSAES-PKCS1-v1.5":
                        return new PKCS1RSAEncryption15Scheme(_rsa);
                    default:
                        throw new InvalidOperationException("Unknown scheme");
                }
            }
        }

        // https://tools.ietf.org/html/rfc2313#section-8.1
        private class PKCS1RSAEncryption15Scheme : EncryptionScheme
        {
            private RSA _rsa;
            private int _length => (_rsa.KeySize + 7) / 8;

            public PKCS1RSAEncryption15Scheme(RSA rsa)
            {
                this._rsa = rsa;
            }

            public override byte[] Encrypt(byte[] data)
            {
                /*
                    The length of the data D shall not be more than k-11 octets, which is
                    positive since the length k of the modulus is at least 12 octets.
                    This limitation guarantees that the length of the padding string PS
                    is at least eight octets, which is a security condition.
                 */
                if (data.Length > _length - 11)
                    throw new InvalidOperationException("Data too large for encryption");

                // EB = 00 || BT || PS || 00 || D
                byte[] EB = new byte[_length];
                /*
                    The block type BT shall be a single octet indicating the structure of
                    the encryption block. For this version of the document it shall have
                    value 00, 01, or 02. For a private- key operation, the block type
                    shall be 00 or 01. For a public-key operation, it shall be 02.
                */
                EB[1] = 2;
                /*
                    The padding string PS shall consist of k-3-||D|| octets. For block
                    type 00, the octets shall have value 00; for block type 01, they
                    shall have value FF; and for block type 02, they shall be
                    pseudorandomly generated and nonzero.
                */
                byte[] buffer = new byte[1];
                for (int i = 2; i < _length - data.Length - 1; i++)
                {
                    do
                    {
                        _rsa.Random.GetBytes(buffer, 0, 1);
                    }
                    while (buffer[0] == 0);
                    EB[i] = buffer[0];
                }
                Array.Copy(data, 0, EB, _length - data.Length, data.Length);

                return _rsa.Encryption.Encrypt(EB);
            }

            public override byte[] Decrypt(byte[] data)
            {
                byte[] EB = _rsa.Encryption.Decrypt(data);
                if (EB.Length + 1 != _length)
                    throw new InvalidOperationException("Decryption error");
                if (EB[0] != 2)
                    throw new InvalidOperationException("Decryption error");
                int i = 1;
                while (EB[i] != 0 && i < EB.Length)
                    i++;
                if (i >= EB.Length - 1)
                    throw new InvalidOperationException("Decryption error");
                return ByteArray.SubArray(EB, i + 1, EB.Length - i - 1);
            }
        }
    }
}