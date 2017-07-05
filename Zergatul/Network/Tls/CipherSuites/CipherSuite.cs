using System;
using System.Text;
using Zergatul.Cryptography;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls.CipherSuites
{
    internal class CipherSuite<KeyExchange, BlockCipher, HashFunction> : AbstractCipherSuite
        where KeyExchange : AbstractKeyExchange, new()
        where BlockCipher : AbstractBlockCipher, new()
        where HashFunction : AbstractHash, new()
    {
        protected ISecureRandom _random;

        protected SecurityParameters _secParams;
        protected Role _role;
        protected TlsConnectionKeys _keys;

        protected KeyExchange _keyExchange;
        protected BlockCipher _blockCipher;
        protected BlockCipherMode _blockCipherMode;
        protected Encryptor _encryptor;
        protected Decryptor _decryptor;
        protected HMAC<HashFunction> _encryptHMAC;
        protected HMAC<HashFunction> _decryptHMAC;

        protected ByteArray _preMasterSecret;

        public CipherSuite(SecurityParameters secParams, Role role, BlockCipherMode mode, ISecureRandom random)
        {
            _keyExchange = new KeyExchange();
            _keyExchange.Random = random;

            _blockCipher = new BlockCipher();
            _blockCipherMode = mode;

            secParams.CipherType = CipherType.Block;
            secParams.EncKeyLength = (byte)_blockCipher.KeySize;
            secParams.BlockLength = (byte)_blockCipher.BlockSize;
            secParams.RecordIVLength = secParams.BlockLength;
            secParams.FixedIVLength = 0;
            secParams.MACLength = (byte)new HashFunction().HashSize;

            this._secParams = secParams;
            this._role = role;
            this._random = random;
        }

        public override ServerKeyExchange GetServerKeyExchange()
        {
            var message = new ServerKeyExchange(this);
            _keyExchange.GetServerKeyExchange(message);
            return message;
        }

        public override void ReadServerKeyExchange(ServerKeyExchange message, BinaryReader reader)
        {
            _keyExchange.ReadServerKeyExchange(message, reader);
        }

        public override void WriteServerKeyExchange(ServerKeyExchange message, BinaryWriter writer)
        {
            _keyExchange.WriteServerKeyExchange(message, writer);
        }

        public override ClientKeyExchange GetClientKeyExchange()
        {
            var message = new ClientKeyExchange(this);
            _keyExchange.GetClientKeyExchange(message);
            _preMasterSecret = _keyExchange.PreMasterSecret;
            return message;
        }

        public override void ReadClientKeyExchange(ClientKeyExchange message, BinaryReader reader)
        {
            _keyExchange.ReadClientKeyExchange(message, reader);
            _preMasterSecret = _keyExchange.PreMasterSecret;
        }

        public override void WriteClientKeyExchange(ClientKeyExchange message, BinaryWriter writer)
        {
            _keyExchange.WriteClientKeyExchange(message, writer);
        }

        public override void CalculateMasterSecret()
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

        public override ByteArray GetFinishedVerifyData(ByteArray data, Role role)
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
            string label = role == Role.Client ? "client finished" : "server finished";
            return PseudoRandomFunction(_secParams.MasterSecret, label, Hash(data), 12);
        }

        public override Finished GetFinished(ByteArray data)
        {
            return new Finished
            {
                Data = GetFinishedVerifyData(data, _role)
            };
        }

        protected virtual ByteArray Hash(ByteArray data)
        {
            var sha256 = new System.Security.Cryptography.SHA256Managed();
            return new ByteArray(sha256.ComputeHash(data.Array));
        }

        protected ByteArray HMACHash(ByteArray secret, ByteArray seed)
        {
            return new HMAC<SHA256>(secret).ComputeHash(seed);
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
            ByteArray result = new ByteArray(new byte[0]);
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

        public override void GenerateKeyMaterial()
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
                _secParams.MasterSecret,
                "key expansion",
                _secParams.ServerRandom + _secParams.ClientRandom,
                2 * (_secParams.MACLength + _secParams.EncKeyLength + _secParams.FixedIVLength));

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

            _keys.ClientMACkey = keyBlock.SubArray(position, _secParams.MACLength);
            position += _secParams.MACLength;
            _keys.ServerMACkey = keyBlock.SubArray(position, _secParams.MACLength);
            position += _secParams.MACLength;
            _keys.ClientEncKey = keyBlock.SubArray(position, _secParams.EncKeyLength);
            position += _secParams.EncKeyLength;
            _keys.ServerEncKey = keyBlock.SubArray(position, _secParams.EncKeyLength);
            position += _secParams.EncKeyLength;
            _keys.ClientIV = keyBlock.SubArray(position, _secParams.FixedIVLength);
            position += _secParams.FixedIVLength;
            _keys.ServerIV = keyBlock.SubArray(position, _secParams.FixedIVLength);
            position += _secParams.FixedIVLength;

            if (_role == Role.Client)
            {
                _encryptHMAC = new HMAC<HashFunction>(_keys.ClientMACkey);
                _decryptHMAC = new HMAC<HashFunction>(_keys.ServerMACkey);

                _encryptor = _blockCipher.CreateEncryptor(_keys.ClientEncKey.Array, _blockCipherMode);
                _decryptor = _blockCipher.CreateDecryptor(_keys.ServerEncKey.Array, _blockCipherMode);
            }

            if (_role == Role.Server)
            {
                _encryptHMAC = new HMAC<HashFunction>(_keys.ServerMACkey);
                _decryptHMAC = new HMAC<HashFunction>(_keys.ClientMACkey);

                _encryptor = _blockCipher.CreateEncryptor(_keys.ServerEncKey.Array, _blockCipherMode);
                _decryptor = _blockCipher.CreateDecryptor(_keys.ClientEncKey.Array, _blockCipherMode);
            }
        }

        public override ByteArray Decode(ByteArray data, ContentType type, ProtocolVersion version, ulong sequenceNum)
        {
            var reader = new BinaryReader(data.Array);
            var ciphertext = new TLSCiphertext();
            switch (_secParams.CipherType)
            {
                case CipherType.Block:
                    using (reader.SetReadLimit(data.Length))
                    {
                        ciphertext.Fragment = new GenericBlockCiphertext
                        {
                            IV = new ByteArray(reader.ReadBytes(_secParams.RecordIVLength))
                        };
                        ciphertext.Content = new ByteArray(reader.ReadToEnd());
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return Decompress(Decode(ciphertext, type, version, sequenceNum)).Fragment;
        }

        protected TLSPlaintext Decompress(TLSCompressed data)
        {
            return new TLSPlaintext
            {
                Type = data.Type,
                Version = data.Version,
                Length = data.Length,
                Fragment = data.Fragment
            };
        }

        protected TLSCompressed Decode(TLSCiphertext data, ContentType type, ProtocolVersion version, ulong sequenceNum)
        {
            var result = new TLSCompressed();
            ComputeDecryptedContent(data, result, type, version, sequenceNum, data.Fragment as GenericBlockCiphertext);
            return result;
        }

        private void ComputeDecryptedContent(TLSCiphertext dataCiphertext, TLSCompressed dataCompressed, ContentType type, ProtocolVersion version, ulong sequenceNum, GenericBlockCiphertext fragment)
        {
            var data = _decryptor.Decrypt(fragment.IV.Array, dataCiphertext.Content.Array);
            data = ValidateAndRemovePadding(data);
            data = ValidateAndRemoveMAC(data, type, version, sequenceNum);
            dataCompressed.Fragment = new ByteArray(data);
        }

        private byte[] ValidateAndRemovePadding(byte[] data)
        {
            byte last = data[data.Length - 1];
            if (last + 1 > data.Length)
                throw new TlsStreamException("Invalid padding length");

            for (int i = data.Length - 1 - last; i < data.Length; i++)
                if (data[i] != last)
                    throw new TlsStreamException("Invalid padding");

            byte[] result = new byte[data.Length - last - 1];
            Array.Copy(data, result, result.Length);
            return result;
        }

        private byte[] ValidateAndRemoveMAC(byte[] data, ContentType type, ProtocolVersion version, ulong sequenceNum)
        {
            if (data.Length < _secParams.MACLength)
                throw new TlsStreamException("Invalid MAC length");

            var mac = new byte[_secParams.MACLength];
            Array.Copy(data, data.Length - mac.Length, mac, 0, mac.Length);

            var result = new byte[data.Length - mac.Length];
            Array.Copy(data, result, result.Length);

            var calculatedMAC = _decryptHMAC.ComputeHash(
                new ByteArray(sequenceNum) +
                new ByteArray((byte)type) +
                new ByteArray((ushort)version) +
                new ByteArray((ushort)result.Length) +
                result);

            for (int i = 0; i < mac.Length; i++)
                if (mac[i] != calculatedMAC[i])
                    throw new TlsStreamException("Invalid MAC");

            return result;
        }

        public override ByteArray Encode(ByteArray data, ContentType type, ProtocolVersion version, ulong sequenceNum)
        {
            var plaintext = new TLSPlaintext
            {
                Type = type,
                Version = version,
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
            dataCiphertext.Content = new ByteArray(_encryptor.Encrypt(
                fragment.IV.Array,
                (dataCompressed.Fragment + fragment.MAC + fragment.Padding + new ByteArray(fragment.PaddingLength)).Array));
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
            return _encryptHMAC.ComputeHash(
                new ByteArray(sequenceNum) +
                new ByteArray((byte)data.Type) +
                new ByteArray((ushort)data.Version) +
                new ByteArray(data.Length) +
                data.Fragment);
        }
    }
}