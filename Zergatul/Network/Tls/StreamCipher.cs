using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Symmetric;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls
{
    internal class StreamCipher : AbstractTlsSymmetricCipher
    {
        private AbstractStreamCipher _cipher;
        private AbstractHash _hash;

        private KeyStream _encKeyStream;
        private KeyStream _decKeyStream;
        private HMAC _encHMAC;
        private HMAC _decHMAC;

        public StreamCipher(AbstractStreamCipher cipher, AbstractHash hash)
        {
            this._cipher = cipher;
            this._hash = hash;
        }

        public override void ApplySecurityParameters()
        {
            SecurityParameters.CipherType = CipherType.Stream;

            SecurityParameters.BlockLength = (byte)_cipher.BlockSize;
            SecurityParameters.EncKeyLength = (byte)_cipher.KeySize;
            SecurityParameters.MACLength = (byte)_hash.HashSize;
            SecurityParameters.RecordIVLength = 0;
            SecurityParameters.FixedIVLength = 0;
        }

        public override void Init(TlsConnectionKeys keys, Role role)
        {
            byte[] encKey = role == Role.Client ? keys.ClientEncKey : keys.ServerEncKey;
            byte[] decKey = role == Role.Server ? keys.ClientEncKey : keys.ServerEncKey;
            byte[] encMacKey = role == Role.Client ? keys.ClientMACkey : keys.ServerMACkey;
            byte[] decMacKey = role == Role.Server ? keys.ClientMACkey : keys.ServerMACkey;

            _encKeyStream = _cipher.InitKeyStream(encKey, null, 0);
            _decKeyStream = _cipher.InitKeyStream(decKey, null, 0);

            _encHMAC = new HMAC(_hash, encMacKey);
            _decHMAC = new HMAC(_hash, decMacKey);
        }

        protected override byte[] Encrypt(TLSCompressed compressed, ulong seqnum)
        {
            // compute MAC
            byte[] mac = HMACCBCCipher.ComputeMAC(_encHMAC, seqnum, compressed.Type, compressed.Version, compressed.Fragment);

            // data to be encrypted
            byte[] data = new byte[compressed.Fragment.Length + SecurityParameters.MACLength];
            Array.Copy(compressed.Fragment, 0, data, 0, compressed.Fragment.Length);
            Array.Copy(mac, 0, data, compressed.Fragment.Length, SecurityParameters.MACLength);

            // encrypt
            byte[] keys = new byte[data.Length];
            _encKeyStream.Read(keys, 0, keys.Length);
            ByteArray.Xor(data, keys);

            return data;
        }

        protected override byte[] Decrypt(TLSCiphertext ciphertext, ulong seqnum)
        {
            // decrypt
            byte[] decrypted = ciphertext.Fragment;
            byte[] keys = new byte[decrypted.Length];
            _decKeyStream.Read(keys, 0, keys.Length);
            ByteArray.Xor(decrypted, keys);

            byte[] plain = ByteArray.SubArray(decrypted, 0, decrypted.Length - SecurityParameters.MACLength);
            byte[] mac = ByteArray.SubArray(decrypted, decrypted.Length - SecurityParameters.MACLength, SecurityParameters.MACLength);

            // validate MAC
            var calculatedMAC = HMACCBCCipher.ComputeMAC(_decHMAC, seqnum, ciphertext.Type, ciphertext.Version, plain);
            if (!ByteArray.Equals(mac, calculatedMAC))
                throw new TlsStreamException("Invalid MAC");

            return plain;
        }
    }
}