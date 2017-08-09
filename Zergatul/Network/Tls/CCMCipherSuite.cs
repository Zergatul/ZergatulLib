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
    internal class CCMCipherSuite<KeyExchange, BlockCipher> : AEADCipherSuite<KeyExchange, BlockCipher, CCM, SHA256>
        where KeyExchange : AbstractKeyExchange, new()
        where BlockCipher : AbstractBlockCipher, new()
    {
        public CCMCipherSuite(int tagLength = 16)
        {
            this._tagLength = tagLength;
        }

        public override void Init(SecurityParameters secParams, Role role, ISecureRandom random)
        {
            base.Init(secParams, role, random);

            _aeadCipherMode.TagLength = _tagLength;
            _aeadCipherMode.OctetLength = 3;
        }
    }
}