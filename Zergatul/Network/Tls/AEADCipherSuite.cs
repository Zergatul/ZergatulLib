using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.BlockCipher.CipherMode;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls
{
    internal class AEADCipherSuite<KeyExchange, Signature, BlockCipher, CipherMode, PRFHashFunction> : GenericCipherSuite<KeyExchange, Signature, BlockCipher, PRFHashFunction>
        where KeyExchange : AbstractTlsKeyExchange, new()
        where Signature : AbstractSignature, new()
        where BlockCipher : AbstractBlockCipher, new()
        where CipherMode : AbstractAEADCipherMode, new()
        where PRFHashFunction : AbstractHash, new()
    {
        protected CipherMode _aeadCipherMode;
        protected int _tagLength = 16;

        protected AEADEncryptor _encryptor;
        protected AEADDecryptor _decryptor;

        private ulong _explicitNonce;

        public override void Init(SecurityParameters secParams, Role role, ISecureRandom random)
        {
            base.Init(secParams, role, random);

            _aeadCipherMode = new CipherMode();

            secParams.CipherType = CipherType.AEAD;

            secParams.MACLength = 0;
            secParams.RecordIVLength = 8;
            secParams.FixedIVLength = 4;

            _explicitNonce = _random.GetUInt64();
        }

        public override void GenerateKeyMaterial()
        {
            base.GenerateKeyMaterial();

            if (_role == Role.Client)
            {
                _encryptor = _aeadCipherMode.CreateEncryptor(_blockCipher, _keys.ClientEncKey.Array);
                _decryptor = _aeadCipherMode.CreateDecryptor(_blockCipher, _keys.ServerEncKey.Array);
            }

            if (_role == Role.Server)
            {
                _encryptor = _aeadCipherMode.CreateEncryptor(_blockCipher, _keys.ServerEncKey.Array);
                _decryptor = _aeadCipherMode.CreateDecryptor(_blockCipher, _keys.ClientEncKey.Array);
            }
        }

        protected override TLSCiphertext Encode(TLSCompressed data, ulong sequenceNum)
        {
            byte[] IV = new byte[12];
            Array.Copy((_role == Role.Server ? _keys.ServerIV : _keys.ClientIV).Array, IV, 4);
            byte[] explicitNonceBytes = BitHelper.GetBytes(_explicitNonce, ByteOrder.BigEndian);
            Array.Copy(explicitNonceBytes, 0, IV, 4, 8);
            _explicitNonce++;

            var encrypted = _encryptor.Encrypt(IV, data.Fragment.Array, GetAdditionalData(data, sequenceNum));

            return new TLSCiphertext
            {
                Type = data.Type,
                Version = data.Version,
                Content = new ByteArray(encrypted.CipherText) + new ByteArray(encrypted.Tag),
                Fragment = new GenericAEADCiphertext
                {
                    NonceExplicit = explicitNonceBytes
                }
            };
        }

        protected override TLSCompressed Decode(TLSCiphertext data, ContentType type, ProtocolVersion version, ulong sequenceNum)
        {
            var fragment = data.Fragment as GenericAEADCiphertext;

            byte[] IV = new byte[12];
            Array.Copy((_role == Role.Server ? _keys.ClientIV : _keys.ServerIV).Array, IV, 4);
            Array.Copy(fragment.NonceExplicit, 0, IV, 4, 8);

            var aeadData = new AEADCipherData
            {
                CipherText = new byte[data.Content.Length - _tagLength],
                Tag = new byte[_tagLength]
            };

            Array.Copy(data.Content.Array, 0, aeadData.CipherText, 0, aeadData.CipherText.Length);
            Array.Copy(data.Content.Array, aeadData.CipherText.Length, aeadData.Tag, 0, _tagLength);

            var result = new TLSCompressed
            {
                Version = version,
                Type = type,
                Length = (ushort)(data.Content.Length - _tagLength)
            };

            var decrypted = _decryptor.Decrypt(IV, aeadData, GetAdditionalData(result, sequenceNum));

            result.Fragment = new ByteArray(decrypted);
            return result;
        }

        protected byte[] GetAdditionalData(TLSCompressed data, ulong sequenceNum)
        {
            // https://tools.ietf.org/html/rfc5246#section-6.2.3.3
            // additional_data = seq_num + TLSCompressed.type + TLSCompressed.version + TLSCompressed.length;
            return
                (new ByteArray(sequenceNum) +
                new ByteArray((byte)data.Type) +
                new ByteArray((ushort)data.Version) +
                new ByteArray((ushort)data.Length)).Array;
        }
    }

    internal class AEADCipherSuiteDefaultPRF<KeyExchange, Signature, BlockCipher, CipherMode> : AEADCipherSuite<KeyExchange, Signature, BlockCipher, CipherMode, SHA256>
        where KeyExchange : AbstractTlsKeyExchange, new()
        where Signature : AbstractSignature, new()
        where BlockCipher : AbstractBlockCipher, new()
        where CipherMode : AbstractAEADCipherMode, new()
    {

    }
}