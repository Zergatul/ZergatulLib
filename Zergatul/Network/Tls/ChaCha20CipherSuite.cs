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
    // TODO: AES-CCM??? WTF! refactor
    internal class ChaCha20CipherSuite<KeyExchange, Signature, PRFHashFunction> : AEADCipherSuite<KeyExchange, Signature, AES128, CCM, PRFHashFunction>
        where KeyExchange : AbstractTlsKeyExchange, new()
        where Signature : AbstractSignature, new()
        where PRFHashFunction : AbstractHash, new()
    {
        private ChaCha20 _chacha20 = new ChaCha20();
        private Poly1305KeyGenerator _generator = new Poly1305KeyGenerator();
        private Poly1305 _poly1305 = new Poly1305();

        public override void Init(SecurityParameters secParams, Role role, ISecureRandom random)
        {
            base.Init(secParams, role, random);

            secParams.EncKeyLength = 32;

            secParams.RecordIVLength = 0;
            secParams.FixedIVLength = 12;
        }

        private byte[] GenerateNonce(byte[] IV, ulong sequenceNum)
        {
            // The 64-bit record sequence number is serialized as an 8-byte,
            // big - endian value and padded on the left with four 0x00 bytes.
            byte[] nonce = new byte[12];
            BitHelper.GetBytes(sequenceNum, ByteOrder.BigEndian, nonce, 4);

            // The padded sequence number is XORed with the client_write_IV
            // (when the client is sending) or server_write_IV (when the server
            // is sending).
            for (int i = 0; i < 12; i++)
                nonce[i] ^= IV[i];

            return nonce;
        }

        protected override TLSCiphertext Encode(TLSCompressed data, ulong sequenceNum)
        {
            byte[] IV = (_role == Role.Server ? _keys.ServerIV : _keys.ClientIV).Array;
            byte[] nonce = GenerateNonce(IV, sequenceNum);

            byte[] encKey = (_role == Role.Server ? _keys.ServerEncKey : _keys.ClientEncKey).Array;
            var enc = _poly1305.CreateEncryptor(_chacha20, encKey, _generator.GenerateKey(encKey, nonce));
            var encrypted = enc.Encrypt(nonce, data.Fragment.Array, GetAdditionalData(data, sequenceNum));

            return new TLSCiphertext
            {
                Type = data.Type,
                Version = data.Version,
                Content = new ByteArray(encrypted.CipherText) + new ByteArray(encrypted.Tag),
                Fragment = new GenericAEADCiphertext
                {
                    NonceExplicit = new byte[0]
                }
            };
        }

        protected override TLSCompressed Decode(TLSCiphertext data, ContentType type, ProtocolVersion version, ulong sequenceNum)
        {
            byte[] IV = (_role == Role.Server ? _keys.ClientIV : _keys.ServerIV).Array;
            byte[] nonce = GenerateNonce(IV, sequenceNum);

            var fragment = data.Fragment as GenericAEADCiphertext;

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

            byte[] decKey = (_role == Role.Server ? _keys.ClientEncKey : _keys.ServerEncKey).Array;
            var dec = _poly1305.CreateDecryptor(_chacha20, decKey, _generator.GenerateKey(decKey, nonce));
            var decrypted = dec.Decrypt(nonce, aeadData, GetAdditionalData(result, sequenceNum));

            result.Fragment = new ByteArray(decrypted);
            return result;
        }
    }
}