using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls
{
    internal class HMACCipherSuite<KeyExchange, BlockCipher, CipherMode, HashFunction, PRFHashFunction> : GenericCipherSuite<KeyExchange, BlockCipher, CipherMode, PRFHashFunction>
        where KeyExchange : AbstractKeyExchange, new()
        where BlockCipher : AbstractBlockCipher, new()
        where CipherMode : AbstractBlockCipherMode, new()
        where HashFunction : AbstractHash, new()
        where PRFHashFunction : AbstractHash, new()
    {
        protected HMAC<HashFunction> _encryptHMAC;
        protected HMAC<HashFunction> _decryptHMAC;

        public override void Init(SecurityParameters secParams, Role role, ISecureRandom random)
        {
            base.Init(secParams, role, random);

            secParams.CipherType = CipherType.Block;

            secParams.MACLength = (byte)new HashFunction().HashSize;
            secParams.RecordIVLength = secParams.BlockLength;
            secParams.FixedIVLength = 0;
        }

        public override void GenerateKeyMaterial()
        {
            base.GenerateKeyMaterial();

            if (_role == Role.Client)
            {
                _encryptor = _blockCipher.CreateEncryptor(_keys.ClientEncKey.Array, _blockCipherMode);
                _decryptor = _blockCipher.CreateDecryptor(_keys.ServerEncKey.Array, _blockCipherMode);

                _encryptHMAC = new HMAC<HashFunction>(_keys.ClientMACkey.Array);
                _decryptHMAC = new HMAC<HashFunction>(_keys.ServerMACkey.Array);
            }

            if (_role == Role.Server)
            {
                _encryptor = _blockCipher.CreateEncryptor(_keys.ServerEncKey.Array, _blockCipherMode);
                _decryptor = _blockCipher.CreateDecryptor(_keys.ClientEncKey.Array, _blockCipherMode);

                _encryptHMAC = new HMAC<HashFunction>(_keys.ServerMACkey.Array);
                _decryptHMAC = new HMAC<HashFunction>(_keys.ClientMACkey.Array);
            }
        }

        protected override TLSCiphertext Encode(TLSCompressed data, ulong sequenceNum)
        {
            var result = new TLSCiphertext
            {
                Type = data.Type,
                Version = data.Version
            };

            var blockCiphertext = new GenericBlockCiphertext
            {
                IV = new ByteArray(_random, _secParams.RecordIVLength),
                MAC = ComputeMAC(sequenceNum, data)
            };
            ComputePadding(data, blockCiphertext);
            ComputeEncryptedContent(data, result, blockCiphertext);
            result.Fragment = blockCiphertext;

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

        protected override TLSCompressed Decode(TLSCiphertext data, ContentType type, ProtocolVersion version, ulong sequenceNum)
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
                (new ByteArray(sequenceNum) +
                new ByteArray((byte)type) +
                new ByteArray((ushort)version) +
                new ByteArray((ushort)result.Length) +
                result).Array);

            for (int i = 0; i < mac.Length; i++)
                if (mac[i] != calculatedMAC[i])
                    throw new TlsStreamException("Invalid MAC");

            return result;
        }

        private ByteArray ComputeMAC(ulong sequenceNum, TLSCompressed data)
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
            return new ByteArray(_encryptHMAC.ComputeHash(
                (new ByteArray(sequenceNum) +
                new ByteArray((byte)data.Type) +
                new ByteArray((ushort)data.Version) +
                new ByteArray(data.Length) +
                data.Fragment).Array));
        }
    }

    internal class HMACCipherSuiteDefaultPRF<KeyExchange, BlockCipher, CipherMode, HashFunction> : HMACCipherSuite<KeyExchange, BlockCipher, CipherMode, HashFunction, SHA256>
        where KeyExchange : AbstractKeyExchange, new()
        where BlockCipher : AbstractBlockCipher, new()
        where CipherMode : AbstractBlockCipherMode, new()
        where HashFunction : AbstractHash, new()
    {

    }
}