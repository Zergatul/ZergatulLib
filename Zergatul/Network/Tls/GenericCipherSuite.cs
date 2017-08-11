using System;
using System.Text;
using Zergatul.Cryptography;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.BlockCipher.CipherMode;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls
{
    internal abstract class GenericCipherSuite<KeyExchange, Signature, BlockCipher, PRFHashFunction> : AbstractCipherSuite
        where KeyExchange : AbstractKeyExchange, new()
        where Signature : AbstractSignature, new()
        where BlockCipher : AbstractBlockCipher, new()
        where PRFHashFunction : AbstractHash, new()
    {
        protected TlsConnectionKeys _keys;

        protected KeyExchange _keyExchange;
        protected Signature _signature;
        protected BlockCipher _blockCipher;

        protected ByteArray _preMasterSecret;

        public override void Init(SecurityParameters secParams, Role role, ISecureRandom random)
        {
            base.Init(secParams, role, random);

            _keyExchange = new KeyExchange();
            _keyExchange.Random = random;
            _signature = new Signature();

            _blockCipher = new BlockCipher();

            secParams.EncKeyLength = (byte)_blockCipher.KeySize;
            secParams.BlockLength = (byte)_blockCipher.BlockSize;
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

        public override byte[] GetKeyExchangeDataToSign(ServerKeyExchange message)
        {
            return _keyExchange.GetDataToSign(message);
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

        public override byte[] CreateSignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash)
        {
            _signature.SetAlgorithm(algo);
            return _signature.Sign(hash);
        }

        public override bool VerifySignature(AbstractAsymmetricAlgorithm algo, AbstractHash hash, byte[] signature)
        {
            _signature.SetAlgorithm(algo);
            return _signature.Verify(hash, signature);
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
            var hash = new PRFHashFunction();
            hash.Update(data.Array);
            return new ByteArray(hash.ComputeHash());
        }

        protected ByteArray HMACHash(ByteArray secret, ByteArray seed)
        {
            return new ByteArray(new HMAC<PRFHashFunction>(secret.Array).ComputeHash(seed.Array));
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
                case CipherType.AEAD:
                    using (reader.SetReadLimit(data.Length))
                    {
                        ciphertext.Fragment = new GenericAEADCiphertext
                        {
                            NonceExplicit = reader.ReadBytes(_secParams.RecordIVLength)
                        };
                        ciphertext.Content = new ByteArray(reader.ReadToEnd());
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return Decompress(Decode(ciphertext, type, version, sequenceNum)).Fragment;
        }

        protected abstract TLSCompressed Decode(TLSCiphertext data, ContentType type, ProtocolVersion version, ulong sequenceNum);

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

        protected abstract TLSCiphertext Encode(TLSCompressed data, ulong sequenceNum);
    }
}