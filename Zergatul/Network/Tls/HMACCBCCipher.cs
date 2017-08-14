using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.BlockCipher.CipherMode;
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

        public override byte[] Decrypt(ContentType type, ProtocolVersion version, ulong seqnum, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override byte[] Encrypt(ContentType type, ProtocolVersion version, ulong seqnum, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}