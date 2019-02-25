using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Symmetric.CipherMode;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class AEADCipher : AbstractTlsSymmetricCipher
    {
        private AbstractBlockCipher _cipher;
        private AbstractAEADCipherMode _mode;
        private int _tagLength;

        private AEADEncryptor _encryptor;
        private AEADDecryptor _decryptor;

        private byte[] _encImplicitNonce;
        private byte[] _decImplicitNonce;

        private ulong _explicitNonce;

        public AEADCipher(AbstractBlockCipher cipher, AbstractAEADCipherMode mode, int tagLength)
        {
            this._cipher = cipher;
            this._mode = mode;
            this._tagLength = tagLength;
        }

        public override void ApplySecurityParameters()
        {
            SecurityParameters.CipherType = CipherType.AEAD;

            SecurityParameters.BlockLength = 16;
            SecurityParameters.EncKeyLength = (byte)_cipher.KeySize;
            SecurityParameters.MACLength = 0;
            SecurityParameters.RecordIVLength = 8;
            SecurityParameters.FixedIVLength = 4;
        }

        public override void Init(TlsConnectionKeys keys, Role role)
        {
            byte[] encKey = role == Role.Client ? keys.ClientEncKey : keys.ServerEncKey;
            byte[] decKey = role == Role.Server ? keys.ClientEncKey : keys.ServerEncKey;

            _encryptor = _mode.CreateEncryptor(_cipher, encKey);
            _decryptor = _mode.CreateDecryptor(_cipher, decKey);

            _encImplicitNonce = role == Role.Client ? keys.ClientIV : keys.ServerIV;
            _decImplicitNonce = role == Role.Server ? keys.ClientIV : keys.ServerIV;

            byte[] nonce = new byte[8];
            Random.GetNextBytes(nonce);
            _explicitNonce = BitHelper.ToUInt64(nonce, 0, ByteOrder.BigEndian);
        }

        protected override byte[] Encrypt(TLSCompressed compressed, ulong seqnum)
        {
            byte[] IV = new byte[12];
            Array.Copy(_encImplicitNonce, 0, IV, 0, 4);
            BitHelper.GetBytes(_explicitNonce, ByteOrder.BigEndian, IV, 4);
            _explicitNonce++;

            byte[] ad = GetAdditionalData(compressed.Type, compressed.Version, compressed.Length, seqnum);
            var data = _encryptor.Encrypt(IV, compressed.Fragment, ad);

            byte[] result = new byte[8 + data.CipherText.Length + _tagLength];
            Array.Copy(IV, 4, result, 0, 8);
            Array.Copy(data.CipherText, 0, result, 8, data.CipherText.Length);
            Array.Copy(data.Tag, 0, result, 8 + data.CipherText.Length, _tagLength);

            return result;
        }

        protected override byte[] Decrypt(TLSCiphertext ciphertext, ulong seqnum)
        {
            byte[] IV = new byte[12];
            Array.Copy(_decImplicitNonce, 0, IV, 0, 4);
            Array.Copy(ciphertext.Fragment, 0, IV, 4, 8);

            var data = new AEADCipherData
            {
                CipherText = ByteArray.SubArray(ciphertext.Fragment, 8, ciphertext.Fragment.Length - 8 - _tagLength),
                Tag = ByteArray.SubArray(ciphertext.Fragment, ciphertext.Fragment.Length - _tagLength, _tagLength),
            };

            byte[] ad = GetAdditionalData(ciphertext.Type, ciphertext.Version, (ushort)(data.CipherText.Length), seqnum);
            byte[] plain = _decryptor.Decrypt(IV, data, ad);

            return plain;
        }

        protected byte[] GetAdditionalData(ContentType type, ProtocolVersion version, ushort length, ulong seqnum)
        {
            // https://tools.ietf.org/html/rfc5246#section-6.2.3.3
            // additional_data = seq_num + TLSCompressed.type + TLSCompressed.version + TLSCompressed.length;
            byte[] ad = new byte[8 + 1 + 2 + 2];
            BitHelper.GetBytes(seqnum, ByteOrder.BigEndian, ad, 0);
            ad[8] = (byte)type;
            BitHelper.GetBytes((ushort)version, ByteOrder.BigEndian, ad, 9);
            BitHelper.GetBytes(length, ByteOrder.BigEndian, ad, 11);
            return ad;
        }
    }
}