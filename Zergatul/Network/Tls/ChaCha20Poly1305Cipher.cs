using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.BlockCipher.CipherMode;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class ChaCha20Poly1305Cipher : AbstractTlsSymmetricCipher
    {
        private ChaCha20 _chacha20;
        private Poly1305KeyGenerator _generator;
        private Poly1305 _poly1305;

        private byte[] _encKey;
        private byte[] _decKey;
        private byte[] _encIV;
        private byte[] _decIV;

        public ChaCha20Poly1305Cipher()
        {
            _chacha20 = new ChaCha20();
            _generator = new Poly1305KeyGenerator();
            _poly1305 = new Poly1305();
        }

        public override void ApplySecurityParameters()
        {
            SecurityParameters.CipherType = CipherType.AEAD;

            SecurityParameters.EncKeyLength = 32;
            SecurityParameters.MACLength = 0;
            SecurityParameters.RecordIVLength = 0;
            SecurityParameters.FixedIVLength = 12;
        }

        public override void Init(TlsConnectionKeys keys, Role role)
        {
            _encKey = role == Role.Client ? keys.ClientEncKey : keys.ServerEncKey;
            _decKey = role == Role.Server ? keys.ClientEncKey : keys.ServerEncKey;

            _encIV = role == Role.Client ? keys.ClientIV : keys.ServerIV;
            _decIV = role == Role.Server ? keys.ClientIV : keys.ServerIV;
        }

        protected override byte[] Encrypt(TLSCompressed compressed, ulong seqnum)
        {
            byte[] nonce = GenerateNonce(_encIV, seqnum);

            byte[] poly1305Key = _generator.GenerateKey(_encKey, nonce);
            var enc = _poly1305.CreateEncryptor(_chacha20, _encKey, poly1305Key);

            byte[] ad = GetAdditionalData(compressed.Type, compressed.Version, compressed.Length, seqnum);
            var data = enc.Encrypt(nonce, compressed.Fragment, ad);

            byte[] result = ByteArray.Concat(data.CipherText, data.Tag);

            return result;
        }

        protected override byte[] Decrypt(TLSCiphertext ciphertext, ulong seqnum)
        {
            byte[] nonce = GenerateNonce(_decIV, seqnum);

            byte[] poly1305Key = _generator.GenerateKey(_decKey, nonce);
            var dec = _poly1305.CreateDecryptor(_chacha20, _decKey, poly1305Key);

            var data = new AEADCipherData
            {
                CipherText = ByteArray.SubArray(ciphertext.Fragment, 0, ciphertext.Length - 16),
                Tag = ByteArray.SubArray(ciphertext.Fragment, ciphertext.Length - 16, 16)
            };

            byte[] ad = GetAdditionalData(ciphertext.Type, ciphertext.Version, (ushort)data.CipherText.Length, seqnum);
            byte[] plain = dec.Decrypt(nonce, data, ad);

            return plain;
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

        protected byte[] GetAdditionalData(ContentType type, ProtocolVersion version, ushort length, ulong seqnum)
        {
            byte[] ad = new byte[8 + 1 + 2 + 2];
            BitHelper.GetBytes(seqnum, ByteOrder.BigEndian, ad, 0);
            ad[8] = (byte)type;
            BitHelper.GetBytes((ushort)version, ByteOrder.BigEndian, ad, 9);
            BitHelper.GetBytes(length, ByteOrder.BigEndian, ad, 11);
            return ad;
        }
    }
}