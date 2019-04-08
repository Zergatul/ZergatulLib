using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// <para>https://tools.ietf.org/html/rfc8017</para>
    /// </summary>
    public class RSAEncryption : AbstractEncryption<RSAPrivateKey, RSAPublicKey, RSAEncryptionParameters>
    {
        public override void GenerateKeyPair(int keySize)
        {
            throw new NotImplementedException();
        }

        public override byte[] Encrypt(byte[] data)
        {
            if (PublicKey == null)
                throw new InvalidOperationException("Public key is required for encryption");

            switch (Parameters.Scheme)
            {
                case RSAEncryptionScheme.Raw:
                    return RawEncrypt(data);
                case RSAEncryptionScheme.RSAES_PKCS1_v1_5:
                    return PKCS1Encrypt(data);
                case RSAEncryptionScheme.RSAES_OAEP:
                    return OAEPEncrypt(data);
                default:
                    throw new NotImplementedException();
            }
        }

        public override byte[] Decrypt(byte[] data)
        {
            if (PrivateKey == null)
                throw new InvalidOperationException("Public key is required for encryption");

            int length = PrivateKey.KeySizeBytes;

            if (data.Length > length)
                throw new InvalidOperationException("Ciphertext too large for decryption");

            switch (Parameters.Scheme)
            {
                case RSAEncryptionScheme.Raw:
                    return RawDecrypt(data);
                case RSAEncryptionScheme.RSAES_PKCS1_v1_5:
                    return PKCS1Decrypt(data);
                default:
                    throw new NotImplementedException();
            }
        }

        private byte[] RawEncrypt(byte[] data)
        {
            if (data.Length > PublicKey.KeySizeBytes)
                throw new InvalidOperationException("Data too large for encryption");

            // RSA computation
            BigInteger m = new BigInteger(data, ByteOrder.BigEndian);
            BigInteger c = BigInteger.ModularExponentiation(m, PublicKey.e, PublicKey.n);
            return c.ToBytes(ByteOrder.BigEndian, PublicKey.KeySizeBytes);
        }

        private byte[] RawDecrypt(byte[] data)
        {
            BigInteger c = new BigInteger(data, ByteOrder.BigEndian);
            BigInteger m = BigInteger.ModularExponentiation(c, PrivateKey.d, PrivateKey.n);
            return m.ToBytes(ByteOrder.BigEndian);
        }

        private byte[] PKCS1Encrypt(byte[] data)
        {
            int length = PublicKey.KeySizeBytes;

            /*
                The length of the data D shall not be more than k-11 octets, which is
                positive since the length k of the modulus is at least 12 octets.
                This limitation guarantees that the length of the padding string PS
                is at least eight octets, which is a security condition.
            */
            if (data.Length > length - 11)
                throw new InvalidOperationException("Data too large for encryption");

            // EB = 00 || BT || PS || 00 || D
            byte[] EB = new byte[length];
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
            for (int i = 2; i < length - data.Length - 1; i++)
            {
                do
                {
                    Random.GetNextBytes(buffer, 0, 1);
                }
                while (buffer[0] == 0);
                EB[i] = buffer[0];
            }
            Array.Copy(data, 0, EB, length - data.Length, data.Length);

            // RSA computation
            BigInteger m = new BigInteger(EB, ByteOrder.BigEndian);
            BigInteger c = BigInteger.ModularExponentiation(m, PublicKey.e, PublicKey.n);
            return c.ToBytes(ByteOrder.BigEndian, length);
        }

        private byte[] PKCS1Decrypt(byte[] data)
        {
            // RSA computation
            BigInteger c = new BigInteger(data, ByteOrder.BigEndian);
            BigInteger m = BigInteger.ModularExponentiation(c, PrivateKey.d, PrivateKey.n);

            byte[] EB = m.ToBytes(ByteOrder.BigEndian, PrivateKey.KeySizeBytes);
            if (EB[0] != 0 || EB[1] != 2)
                throw new InvalidOperationException("Decryption error");
            int i = 2;
            while (EB[i] != 0 && i < EB.Length)
                i++;
            if (i >= EB.Length - 1)
                throw new InvalidOperationException("Decryption error");
            return ByteArray.SubArray(EB, i + 1, EB.Length - i - 1);
        }

        private byte[] OAEPEncrypt(byte[] M)
        {
            if (Parameters.Hash == null)
                throw new InvalidOperationException("Parameters.Hash is not specified.");

            int k = PublicKey.KeySizeBytes;
            int hLen = Parameters.Hash.HashSize;

            if (M.Length > k - 2 * hLen - 2)
                throw new InvalidOperationException("Data too large for encryption");

            Parameters.Hash.Reset();
            byte[] lHash = Parameters.Hash.ComputeHash();

            byte[] DB = new byte[k - hLen - 1];
            Array.Copy(lHash, 0, DB, 0, hLen);
            DB[DB.Length - 1 - M.Length] = 0x01;
            Array.Copy(M, 0, DB, DB.Length - M.Length, M.Length);

            //TODO
            return null;
        }

        #region Converters

        public override AbstractSignature ToSignature()
        {
            return new RSASignature
            {
                Parameters = new RSASignatureParameters(),
                Random = Random,
                PrivateKey = PrivateKey,
                PublicKey = PublicKey
            };
        }

        public override AbstractKeyExchange ToKeyExchange()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}