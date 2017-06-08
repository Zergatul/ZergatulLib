using System;
using System.Text;
using Zergatul.Cryptography;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal abstract class CipherSuite
    {
        protected ISecureRandom _random;

        protected SecurityParameters _secParams;
        protected Role _role;
        protected TlsConnectionKeys _keys;

        protected AbstractKeyExchange _keyExchange;
        protected AbstractHMAC _hmac;
        protected AbstractBlockCipher _blockCipher;

        protected ByteArray _preMasterSecret;

        private HMACSHA256 _hmacSHA256;

        public CipherSuite(SecurityParameters secParams, Role role, ISecureRandom random)
        {
            this._secParams = secParams;
            this._role = role;
            this._random = random;

            this._hmacSHA256 = new HMACSHA256();
        }

        protected ByteArray MACEncryptKey
        {
            get
            {
                if (_role == Role.Client)
                    return _keys.ClientMACkey;
                if (_role == Role.Server)
                    return _keys.ServerMACkey;
                throw new TlsStreamException("Invalid role");
            }
        }

        protected ByteArray BlockCipherEncryptKey
        {
            get
            {
                if (_role == Role.Client)
                    return _keys.ClientEncKey;
                if (_role == Role.Server)
                    return _keys.ServerEncKey;
                throw new TlsStreamException("Invalid role");
            }
        }

        public virtual void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader)
        {
            _keyExchange.ReadServerKeyExchange(message, reader);
        }

        public virtual ClientKeyExchange GetClientKeyExchange()
        {
            var result = _keyExchange.GenerateClientKeyExchange();
            _preMasterSecret = result.PreMasterSecret;
            return result.Message;
        }

        public virtual void CalculateMasterSecret()
        {
            // RFC 5246
            // https://tools.ietf.org/html/rfc5246#section-8.1
            // The master secret is always exactly 48 bytes in length.
            _secParams.MasterSecret = PseudoRandomFunction(_preMasterSecret, "master secret", _secParams.ClientRandom + _secParams.ServerRandom, 48);

            // RFC 5246
            // https://tools.ietf.org/html/rfc5246#section-8.1
            // The pre_master_secret should be deleted from memory once the master_secret has been computed.
            _preMasterSecret.ClearMemory();
        }

        public virtual Finished GetFinished(ByteArray data)
        {
            // RFC 5246
            // https://tools.ietf.org/html/rfc5246#section-7.4.9
            /*
                In previous versions of TLS, the verify_data was always 12 octets
                long.  In the current version of TLS, it depends on the cipher
                suite.  Any cipher suite which does not explicitly specify
                verify_data_length has a verify_data_length equal to 12.  This
                includes all existing cipher suites.  Note that this
                representation has the same encoding as with previous versions.
                Future cipher suites MAY specify other lengths but such length
                MUST be at least 12 bytes.
            */
            string label = _role == Role.Client ? "client finished" : "server finished";
            var verifyData = PseudoRandomFunction(_secParams.MasterSecret, label, Hash(data), 12);
            return new Finished
            {
                Data = verifyData
            };
        }

        protected virtual ByteArray Hash(ByteArray data)
        {
            var sha256 = new System.Security.Cryptography.SHA256Managed();
            return new ByteArray(data.ToArray());
        }

        protected ByteArray HMACHash(ByteArray secret, ByteArray seed)
        {
            return _hmacSHA256.Compute(secret, seed);
        }

        protected ByteArray PHash(ByteArray secret, ByteArray seed, int length)
        {
            // RFC 5246 // Page 14
            /*
                P_hash(secret, seed) = HMAC_hash(secret, A(1) + seed) +
                                       HMAC_hash(secret, A(2) + seed) +
                                       HMAC_hash(secret, A(3) + seed) + ...
                A() is defined as:
                     A(0) = seed
                     A(i) = HMAC_hash(secret, A(i - 1))
            */
            var a = seed;
            ByteArray result = new ByteArray();
            while (result.Length < length)
            {
                a = HMACHash(secret, a);
                result = result + HMACHash(secret, a + seed);
            }
            return result.Truncate(length);
        }

        public virtual ByteArray PseudoRandomFunction(ByteArray secret, string label, ByteArray seed, int length)
        {
            // RFC 5246 // Page 14
            // PRF(secret, label, seed) = P_<hash>(secret, label + seed)
            return PHash(secret, Encoding.ASCII.GetBytes(label) + seed, length);
        }

        public void GenerateKeyMaterial(SecurityParameters secParams)
        {
            // RFC 5426 // Page 25
            /*
                To generate the key material, compute

                key_block = PRF(SecurityParameters.master_secret,
                      "key expansion",
                      SecurityParameters.server_random +
                      SecurityParameters.client_random);
            */
            var keyBlock = PseudoRandomFunction(
                secParams.MasterSecret,
                "key expansion",
                secParams.ServerRandom + secParams.ClientRandom,
                2 * (secParams.MACLength + secParams.EncKeyLength + secParams.FixedIVLength));

            // RFC 5426 // Page 25
            /*
                Then, the key_block is partitioned as follows:

                    client_write_MAC_key[SecurityParameters.mac_key_length]
                    server_write_MAC_key[SecurityParameters.mac_key_length]
                    client_write_key[SecurityParameters.enc_key_length]
                    server_write_key[SecurityParameters.enc_key_length]
                    client_write_IV[SecurityParameters.fixed_iv_length]
                    server_write_IV[SecurityParameters.fixed_iv_length]
             */

            int position = 0;
            _keys = new TlsConnectionKeys();

            _keys.ClientMACkey = keyBlock.SubArray(position, secParams.MACLength);
            position += secParams.MACLength;
            _keys.ServerMACkey = keyBlock.SubArray(position, secParams.MACLength);
            position += secParams.MACLength;
            _keys.ClientEncKey = keyBlock.SubArray(position, secParams.EncKeyLength);
            position += secParams.EncKeyLength;
            _keys.ServerEncKey = keyBlock.SubArray(position, secParams.EncKeyLength);
            position += secParams.EncKeyLength;
            _keys.ClientIV = keyBlock.SubArray(position, secParams.FixedIVLength);
            position += secParams.FixedIVLength;
            _keys.ServerIV = keyBlock.SubArray(position, secParams.FixedIVLength);
            position += secParams.FixedIVLength;
        }

        public ByteArray ProcessPlaintext(ByteArray data, ulong sequenceNum)
        {
            var plaintext = new TLSPlaintext
            {
                Type = ContentType.Handshake,
                Version = ProtocolVersion.Tls12,
                Length = (ushort)data.Length,
                Fragment = data
            };
            var ciphertext = Encode(Compress(plaintext), sequenceNum);
            return ciphertext.ToBytes();
        }

        protected TLSCompressed Compress(TLSPlaintext data)
        {
            return new TLSCompressed
            {
                Type = data.Type,
                Version = data.Version,
                Length = data.Length,
                Fragment = data.Fragment
            };
        }

        protected TLSCiphertext Encode(TLSCompressed data, ulong sequenceNum)
        {
            var result = new TLSCiphertext
            {
                Type = data.Type,
                Version = data.Version
            };

            switch (_secParams.CipherType)
            {
                case CipherType.Block:
                    var blockCiphertext = new GenericBlockCiphertext
                    {
                        IV = new ByteArray(_random, _secParams.RecordIVLength),
                        MAC = ComputeMAC(sequenceNum, data)
                    };
                    ComputePadding(data, blockCiphertext);
                    ComputeEncryptedContent(data, result, blockCiphertext);
                    result.Fragment = blockCiphertext;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        private void ComputePadding(TLSCompressed data, GenericBlockCiphertext fragment)
        {
            int modulo = (data.Length + _secParams.MACLength + 1) % _secParams.BlockLength;
            fragment.PaddingLength = (byte)(_secParams.BlockLength - modulo);
            var padding = new byte[fragment.PaddingLength];
            for (int i = 0; i < padding.Length; i++)
                padding[i] = fragment.PaddingLength;
            fragment.Padding = new ByteArray(padding);
        }

        private void ComputeEncryptedContent(TLSCompressed dataCompressed, TLSCiphertext dataCiphertext, GenericBlockCiphertext fragment)
        {
            dataCiphertext.Content = _blockCipher.Encrypt(
                fragment.IV,
                dataCompressed.Fragment + fragment.MAC + fragment.Padding + new ByteArray(fragment.PaddingLength),
                BlockCipherEncryptKey);
            dataCiphertext.Length = (ushort)(fragment.IV.Length + dataCiphertext.Content.Length);
        }

        public ByteArray ComputeMAC(ulong sequenceNum, TLSCompressed data)
        {
            // RFC 5246 // Page 21
            /*
                The MAC is generated as:

                MAC(MAC_write_key, seq_num +
                                   TLSCompressed.type +
                                   TLSCompressed.version +
                                   TLSCompressed.length +
                                   TLSCompressed.fragment);

                where "+" denotes concatenation.

                seq_num
                    The sequence number for this record.

                MAC
                    The MAC algorithm specified by SecurityParameters.mac_algorithm.
            */
            return _hmac.Compute(MACEncryptKey,
                new ByteArray(sequenceNum) +
                new ByteArray((byte)data.Type) +
                new ByteArray((ushort)data.Version) +
                new ByteArray(data.Length) +
                data.Fragment);
        }

        public static CipherSuite Resolve(CipherSuiteType type, SecurityParameters secParams, Role role, ISecureRandom random)
        {
            switch (type)
            {
                /*case CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
                    return new TLS_DHE_RSA_WITH_AES_256_GCM_SHA384();*/
                case CipherSuiteType.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
                    return new TLS_DHE_RSA_WITH_AES_256_CBC_SHA(secParams, role, random);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}