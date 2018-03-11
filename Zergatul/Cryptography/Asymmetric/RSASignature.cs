using System;
using Zergatul.Math;
using Zergatul.Network.Asn1;
using Zergatul.Network.Asn1.Structures;

namespace Zergatul.Cryptography.Asymmetric
{
    public class RSASignature : AbstractSignature<RSAPrivateKey, RSAPublicKey, RSASignatureParameters>
    {
        public RSASignature()
        {
            Parameters = new RSASignatureParameters();
        }

        public override void GenerateKeyPair(int keySize)
        {
            throw new NotImplementedException();
        }

        public override byte[] Sign(byte[] data)
        {
            if (PrivateKey == null)
                throw new InvalidOperationException("Private key is required for signing");
            if (Parameters.Hash == null)
                throw new InvalidOperationException("Parameters.Hash is null");

            switch (Parameters.Scheme)
            {
                case RSASignatureScheme.RSASSA_PKCS1_v1_5:
                    return PKCS1Sign(data);
                default:
                    throw new NotImplementedException();
            }
        }

        public override byte[] SignHash(byte[] hash)
        {
            if (PrivateKey == null)
                throw new InvalidOperationException("Private key is required for signing");
            if (Parameters.Hash == null)
                throw new InvalidOperationException("Parameters.Hash is null");

            switch (Parameters.Scheme)
            {
                case RSASignatureScheme.RSASSA_PKCS1_v1_5:
                    return PKCS1SignHash(hash);
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool Verify(byte[] data, byte[] signature)
        {
            if (PublicKey == null)
                throw new InvalidOperationException("Public key is required for signature verification");
            if (Parameters.Hash == null)
                throw new InvalidOperationException("Parameters.Hash is null");

            switch (Parameters.Scheme)
            {
                case RSASignatureScheme.RSASSA_PKCS1_v1_5:
                    return PKCS1Verify(data, signature);
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool VerifyHash(byte[] hash, byte[] signature)
        {
            if (PublicKey == null)
                throw new InvalidOperationException("Public key is required for signature verification");
            if (Parameters.Hash == null)
                throw new InvalidOperationException("Parameters.Hash is null");

            switch (Parameters.Scheme)
            {
                case RSASignatureScheme.RSASSA_PKCS1_v1_5:
                    return PKCS1VerifyHash(hash, signature);
                default:
                    throw new NotImplementedException();
            }
        }

        private byte[] PKCS1Sign(byte[] data)
        {
            Parameters.Hash.Reset();
            Parameters.Hash.Update(data);
            byte[] digest = Parameters.Hash.ComputeHash();

            return PKCS1SignHash(digest);
        }

        private byte[] PKCS1SignHash(byte[] hash)
        {
            var ai = new AlgorithmIdentifier(Parameters.Hash.OID, new Null());
            var pkcs = new EMSA_PKCS1_v1_5(ai, hash, PrivateKey.KeySizeBytes);
            var value = new BigInteger(pkcs.ToBytes(), ByteOrder.BigEndian);
            var result = BigInteger.ModularExponentiation(value, PrivateKey.d, PrivateKey.n);

            return result.ToBytes(ByteOrder.BigEndian, PrivateKey.KeySizeBytes);
        }

        private bool PKCS1Verify(byte[] data, byte[] signature)
        {
            Parameters.Hash.Reset();
            Parameters.Hash.Update(data);
            byte[] digest = Parameters.Hash.ComputeHash();

            return PKCS1VerifyHash(digest, signature);
        }

        private bool PKCS1VerifyHash(byte[] hash, byte[] signature)
        {
            var signValue = new BigInteger(signature, ByteOrder.BigEndian);
            var value = BigInteger.ModularExponentiation(signValue, PublicKey.e, PublicKey.n);
            EMSA_PKCS1_v1_5 pkcs;
            try
            {
                pkcs = EMSA_PKCS1_v1_5.Parse(value.ToBytes(ByteOrder.BigEndian, PublicKey.KeySizeBytes));
            }
            catch (ParseException)
            {
                return false;
            }

            if (pkcs.DigestAlgorithm.Algorithm != Parameters.Hash.OID)
                return false;

            return ByteArray.Equals(pkcs.Digest, hash);
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