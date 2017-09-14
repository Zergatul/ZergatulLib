using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Symmetric.CipherMode;
using Zergatul.Cryptography.Hash;
using Zergatul.Network.Tls.Messages;

namespace Zergatul.Network.Tls
{
    internal class HMACCBCCipher : AbstractTlsSymmetricCipher
    {
        private AbstractBlockCipher _cipher;
        private AbstractHash _hash;

        private BlockCipherEncryptor _encryptor;
        private BlockCipherDecryptor _decryptor;
        private HMAC _encHMAC;
        private HMAC _decHMAC;

        public HMACCBCCipher(AbstractBlockCipher cipher, AbstractHash hash)
        {
            this._cipher = cipher;
            this._hash = hash;
        }

        public override void ApplySecurityParameters()
        {
            SecurityParameters.CipherType = CipherType.Block;

            SecurityParameters.BlockLength = (byte)_cipher.BlockSize;
            SecurityParameters.EncKeyLength = (byte)_cipher.KeySize;
            SecurityParameters.MACLength = (byte)_hash.HashSize;
            SecurityParameters.RecordIVLength = (byte)_cipher.BlockSize;
            SecurityParameters.FixedIVLength = 0;
        }

        public override void Init(TlsConnectionKeys keys, Role role)
        {
            byte[] encKey = role == Role.Client ? keys.ClientEncKey : keys.ServerEncKey;
            byte[] decKey = role == Role.Server ? keys.ClientEncKey : keys.ServerEncKey;
            byte[] encMacKey = role == Role.Client ? keys.ClientMACkey : keys.ServerMACkey;
            byte[] decMacKey = role == Role.Server ? keys.ClientMACkey : keys.ServerMACkey;

            var mode = new CBC();
            _encryptor = mode.CreateEncryptor(_cipher, encKey);
            _decryptor = mode.CreateDecryptor(_cipher, decKey);

            _encHMAC = new HMAC(_hash, encMacKey);
            _decHMAC = new HMAC(_hash, decMacKey);
        }

        protected override byte[] Encrypt(TLSCompressed compressed, ulong seqnum)
        {
            byte[] IV = new byte[SecurityParameters.RecordIVLength];
            Random.GetBytes(IV);

            // compute padding
            int modulo = (compressed.Fragment.Length + SecurityParameters.MACLength + 1) % SecurityParameters.BlockLength;
            byte paddingLength = (byte)(SecurityParameters.BlockLength - modulo);

            // compute MAC
            byte[] mac = ComputeMAC(_encHMAC, seqnum, compressed.Type, compressed.Version, compressed.Fragment);

            // data to be encrypted
            byte[] data = new byte[compressed.Fragment.Length + SecurityParameters.MACLength + 1 + paddingLength];
            int index = 0;
            Array.Copy(compressed.Fragment, 0, data, index, compressed.Fragment.Length);
            index += compressed.Fragment.Length;
            Array.Copy(mac, 0, data, index, SecurityParameters.MACLength);
            index += SecurityParameters.MACLength;
            for (; index < data.Length; index++)
                data[index] = paddingLength;

            return ByteArray.Concat(IV, _encryptor.Encrypt(IV, data));
        }

        protected override byte[] Decrypt(TLSCiphertext ciphertext, ulong seqnum)
        {
            // check length
            int minLength = (SecurityParameters.RecordIVLength + SecurityParameters.MACLength + SecurityParameters.BlockLength) / SecurityParameters.BlockLength * SecurityParameters.BlockLength;
            if (ciphertext.Fragment.Length < minLength)
                throw new TlsStreamException("Invalid data");

            byte[] IV = new byte[SecurityParameters.RecordIVLength];
            Array.Copy(ciphertext.Fragment, 0, IV, 0, SecurityParameters.RecordIVLength);

            byte[] encrypted = new byte[ciphertext.Fragment.Length - IV.Length];
            Array.Copy(ciphertext.Fragment, IV.Length, encrypted, 0, encrypted.Length);

            byte[] decrypted = _decryptor.Decrypt(IV, encrypted);

            // validate padding
            byte paddingLength = decrypted[decrypted.Length - 1];
            for (int i = 1; i <= paddingLength; i++)
                if (decrypted[decrypted.Length - i - 1] != paddingLength)
                    throw new TlsStreamException("Invalid padding");

            byte[] plaindata = new byte[decrypted.Length - 1 - paddingLength - SecurityParameters.MACLength];
            byte[] mac = new byte[SecurityParameters.MACLength];

            Array.Copy(decrypted, 0, plaindata, 0, plaindata.Length);
            Array.Copy(decrypted, plaindata.Length, mac, 0, mac.Length);

            // validate MAC
            var calculatedMAC = ComputeMAC(_decHMAC, seqnum, ciphertext.Type, ciphertext.Version, plaindata);
            if (!ByteArray.Equals(mac, calculatedMAC))
                throw new TlsStreamException("Invalid MAC");

            return plaindata;
        }

        public static byte[] ComputeMAC(HMAC hmac, ulong sequenceNum, ContentType type, ProtocolVersion version, byte[] data)
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
            byte[] concatenated = new byte[8 + 1 + 2 + 2 + data.Length];
            BitHelper.GetBytes(sequenceNum, ByteOrder.BigEndian, concatenated, 0);
            concatenated[8] = (byte)type;
            BitHelper.GetBytes((ushort)version, ByteOrder.BigEndian, concatenated, 9);
            BitHelper.GetBytes((ushort)data.Length, ByteOrder.BigEndian, concatenated, 11);
            Array.Copy(data, 0, concatenated, 13, data.Length);

            return hmac.ComputeHash(concatenated);
        }
    }
}